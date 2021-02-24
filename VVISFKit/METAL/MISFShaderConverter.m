#import "MISFShaderConverter.h"
#import "FileTools.h"
#import "MISFErrorCodes.h"
#import "RegexTools.h"
#import <Metal/Metal.h>

typedef enum ProgramType
{
    ProgramTypeFragment = 0,
    ProgramTypeVertex = 1,
} ProgramType;

static NSString *const ERROR_STRING_OPERATION_KEY = @"String Operation Failed";
static NSString *const MISF_SPIRV_FILENAME_FRAGMENT_SUFFIX = @"_fs";
static NSString *const MISF_SPIRV_FILENAME_VERTEX_SUFFIX = @"_vs";

#pragma mark GL TOPPINGS

static NSString *const MISF_TOPPINGS_FILE_BEGINNING = @""
                                                      @"#version 400\n"
                                                       "precision mediump float;\n";

static NSString *const MISF_TOPPINGS_BUILTINS_UNIFORM_DEFINITIONS =
    @"\n"
     "uniform vec3        isf_VertNorm;\n" // originally varying in isf framework
     "uniform vec3        isf_VertPos;\n"  // originally varying in isf framework
     "uniform int        PASSINDEX;\n"
     "uniform vec2        RENDERSIZE;\n"
     "uniform float        TIME;\n"
     "uniform float        TIMEDELTA;\n"
     "uniform vec4        DATE;\n"
     "uniform int        FRAMEINDEX;\n"
     "uniform vec2 isf_FragNormCoord;\n"
     "uniform vec2 vv_FragNormCoord;\n";

static NSString *const MISF_TOPPINGS_VERTEX_FUNCTIONS = @""
                                                         ""
                                                         "void isf_vertShaderInit()\n"
                                                         "{\n"
                                                         "return;\n"
                                                         "}\n"
                                                         "void vv_vertShaderInit()\n"
                                                         "{\n"
                                                         "return;\n"
                                                         "}\n"
                                                         "";

static NSString *const MISF_TOPPINGS_SAMPLING_FUNCTIONS =
    @""
     "vec2 IMG_SIZE(sampler2D imageName)\n"
     "{\n"
     "    return vec2(0.);\n"
     "}\n"
     "vec4 IMG_THIS_PIXEL(sampler2D imageName)\n"
     "{\n"
     "// Convert FragNormCoord of output size to a FragNormCoord on the sampled image size\n"
     "vec2 imageSize = IMG_SIZE(imageName);\n"
     "vec2 fragPixelCoord = vec2(RENDERSIZE.x * isf_FragNormCoord.x, RENDERSIZE.y * isf_FragNormCoord.y);\n"
     "vec2 imageNormCoord = vec2( fragPixelCoord.x / imageSize.x, fragPixelCoord.y / imageSize.y);\n"
     "    return texture(imageName, imageNormCoord);\n"
     "}\n"
     "\n"
     // Doc is unclear about which one of those two methods should exist
     "vec4 IMG_THIS_NORM_PIXEL(sampler2D imageName)\n"
     "{\n"
     "   return IMG_THIS_PIXEL(imageName);\n"
     "}\n"
     // Doc says this one, but it's never implemented in VVISFKit. Maybe in the cpp version?
     "vec4 IMG_NORM_THIS_PIXEL(sampler2D imageName)\n"
     "{\n"
     "   return IMG_THIS_PIXEL(imageName);\n"
     "}\n"
     "\n"
     "vec4 IMG_PIXEL(sampler2D imageName, vec2 pixelCoord)\n"
     "{\n"
     "    return texture(imageName, vec2(pixelCoord.x/IMG_SIZE(imageName).x, pixelCoord.y/IMG_SIZE(imageName).y));\n"
     "}\n"
     "\n"
     "vec4 IMG_NORM_PIXEL(sampler2D imageName, vec2 normalizedPixelCoord)\n"
     "{\n"
     "    return texture(imageName, vec2(normalizedPixelCoord.x, normalizedPixelCoord.y));\n"
     "}\n"
     "\n";

static NSString *const MISF_TOPPINGS_FIRST_IN_MAIN = @"\n"
                                                      "vec2 isf_FragNormCoord = vec2(0,0);\n"
                                                      "vec2 vv_FragNormCoord = vec2(0,0);\n"
                                                      "vec2 isf_fragCoord = floor(isf_FragNormCoord * RENDERSIZE);";

#pragma mark REGEX PATTERNS

// https://regex101.com/r/nUXD8W/2
static NSString *const MISF_REGEX_GL_VOID_MAIN_WITH_BRACKET =
    @"void\\s*main(\\s*|)[(]\\s*(\\s*|\\s*void\\s*)[)]\\s*[{]";
#warning mto-anomes: TODO: no param = it's fragment void and might not work - create test shader to verify behaviour
static NSString *const MISF_REGEX_METAL_FRAGMENT_MAIN_WITH_BRACKET = @"fragment float4 main0\\s*[(](.*)\\s*[{]";
#warning mto-anomes: TODO: no param = it's vertex void and might not work - create test shader to verify behaviour
static NSString *const MISF_REGEX_METAL_VERTEX_MAIN_WITH_BRACKET = @"vertex RasterizerData main0\\s*[(](.*)\\s*[{]";
static NSString *const MISF_REGEX_METAL_FRAGMENT_MAIN = @"fragment float4 main0\\s*[(](.*)";
static NSString *const MISF_REGEX_METAL_VERTEX_MAIN = @"vertex RasterizerData main0\\s*[(](.*)";
static NSString *const MISF_REGEX_PARAMETER_LIST = @"(.*)";

static NSString *const ISF_MARKER_INSIDE_FRAGMENT_MAIN = @"\n/* ISF_MARKER_INSIDE_MAIN_FRAGMENT */\n";
static NSString *const ISF_MARKER_BEFORE_FRAGMENT_MAIN = @"\n/* ISF_MARKER_BEFORE_MAIN_FRAGMENT */\n";
static NSString *const ISF_MARKER_INSIDE_VERTEX_MAIN = @"\n/* ISF_MARKER_INSIDE_MAIN_VERTEX */\n";
static NSString *const ISF_MARKER_BEFORE_VERTEX_MAIN = @"\n/* ISF_MARKER_BEFORE_MAIN_VERTEX */\n";

#pragma mark MACROS

#define CHECK_PROGRAM_TYPE(programType, defaultReturn)                                                                 \
    if( programType != ProgramTypeVertex && programType != ProgramTypeFragment )                                       \
    {                                                                                                                  \
        NSLog(@"ERR: program type not handled");                                                                       \
        if( errorPtr )                                                                                                 \
        {                                                                                                              \
            NSDictionary *userInfo = @{                                                                                \
                @"Program Type Unknown" :                                                                              \
                    [NSString stringWithFormat:@"Program type (%i) does not exist. This is likely an internal error",  \
                                               programType]                                                            \
            };                                                                                                         \
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];       \
        }                                                                                                              \
        return defaultReturn;                                                                                          \
    }

#define IS_VERTEX(programType) (programType == ProgramTypeVertex)

@implementation MISFShaderConverter

#pragma mark Translation
+ (NSString *)translateFragmentToMetal:(NSString *)glCode
                         inputUniforms:(NSString *)isfInputUniforms
                             withError:(NSError **)errorPtr
{

    return [MISFShaderConverter translateCodeToMetal:glCode
                                       inputUniforms:isfInputUniforms
                                         programType:ProgramTypeFragment
                                           withError:errorPtr];
}

+ (NSString *)translateVertexToMetal:(NSString *)glCode
                       inputUniforms:(NSString *)isfInputUniforms
                           withError:(NSError **)errorPtr
{
    return [MISFShaderConverter translateCodeToMetal:glCode
                                       inputUniforms:isfInputUniforms
                                         programType:ProgramTypeVertex
                                           withError:errorPtr];
}

/*
Translation goes in three steps:
 1. injecting strings inside the glsl for spirV to transpile properly (glslWithToppings)
 2. Running Spir-V (intermediate)
 3. Making string replaces in the Spir-V built MSL to make it ISF-compliant
 */
+ (NSString *)translateCodeToMetal:(NSString *)glCode
                     inputUniforms:(NSString *)isfInputUniforms
                       programType:(ProgramType)programType
                         withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)
    NSString *glslWithToppings = [MISFShaderConverter injectToppingsInCode:glCode
                                                          isfInputUniforms:isfInputUniforms
                                                               programType:programType
                                                                 withError:errorPtr];
    if( glslWithToppings == nil )
    {
        return nil;
    }
    NSString *intermediate = [MISFShaderConverter spirVConvertToMetal:glslWithToppings
                                                          programType:programType
                                                            withError:errorPtr];
    if( intermediate == nil )
    {
        return nil;
    }
    NSString *metalIsf = [MISFShaderConverter finaliseIntermediate:intermediate
                                                       programType:programType
                                                         withError:errorPtr];
    return metalIsf;
}

+ (NSString *)injectToppingsInCode:(NSString *)glCode
                  isfInputUniforms:(NSString *)isfInputUniforms
                       programType:(ProgramType)programType
                         withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)
    // First, identify code parts
    NSError *regexError;
    NSRange rangeMainPrototype = [RegexTools getRangeInString:glCode
                                                      pattern:MISF_REGEX_GL_VOID_MAIN_WITH_BRACKET
                                                    withError:&regexError];
    if( rangeMainPrototype.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY : [NSString
                    stringWithFormat:@"failed to find rangeMainPrototype during inject toppings. Regex Tools Error: %@",
                                     regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSRange rangeBeforeMain = NSMakeRange(0, rangeMainPrototype.location);
    NSRange rangeInsideMainUntilEnd =
        NSMakeRange(rangeMainPrototype.location + rangeMainPrototype.length,
                    glCode.length - rangeMainPrototype.location - rangeMainPrototype.length);
    NSString *glBeforeMain = [glCode substringWithRange:rangeBeforeMain];
    NSString *glMainPrototypeUntilFirstBracket = [glCode substringWithRange:rangeMainPrototype];
    NSString *glMainAndAfter = [glCode substringWithRange:rangeInsideMainUntilEnd];

    // Inject toppings at top of file, but also inside main
    NSString *glWithToppings = @"";

    if( programType == ProgramTypeFragment )
    {
        glWithToppings =
            [NSString stringWithFormat:@" %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n",
                                       MISF_TOPPINGS_FILE_BEGINNING, MISF_TOPPINGS_BUILTINS_UNIFORM_DEFINITIONS,
                                       MISF_TOPPINGS_SAMPLING_FUNCTIONS, isfInputUniforms, glBeforeMain,
                                       glMainPrototypeUntilFirstBracket, MISF_TOPPINGS_FIRST_IN_MAIN, glMainAndAfter];
    }
    else if( programType == ProgramTypeVertex )
    {
        glWithToppings = [NSString stringWithFormat:@" %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n %@ \n",
                                                    MISF_TOPPINGS_FILE_BEGINNING, MISF_TOPPINGS_VERTEX_FUNCTIONS,
                                                    MISF_TOPPINGS_BUILTINS_UNIFORM_DEFINITIONS,

                                                    isfInputUniforms, glBeforeMain, glMainPrototypeUntilFirstBracket,
                                                    MISF_TOPPINGS_FIRST_IN_MAIN, glMainAndAfter];
    }

    return glWithToppings;
}

+ (NSString *)spirVConvertToMetal:(NSString *)glsl programType:(ProgramType)programType withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)
    // 1. Write file in temp folder
    NSString *glslPath = [FileTools pathForTemporaryFileWithPrefix:@"beforespirv_"];
    [glsl writeToFile:glslPath atomically:YES encoding:NSUTF8StringEncoding error:nil];

    // 2. Run SpirVCross CLI
    NSString *outputFilePath = [FileTools pathForTemporaryFileWithPrefix:@"transpiled_"];

    NSString *programTypeParameter = @"";
    if( programType == ProgramTypeFragment )
    {
        programTypeParameter = @"--frag";
    }
    else if( programType == ProgramTypeVertex )
    {
        programTypeParameter = @"--vert";
    }
    NSTask *transpileTask = [[NSTask alloc] init];
    NSBundle *frameworkBundle = [NSBundle bundleForClass:[self class]];
    NSString *binaryPath = [frameworkBundle pathForResource:@"glslcc" ofType:nil];
    NSString *bashPath = @"/bin/bash";
    transpileTask.launchPath = binaryPath;
    [@"" writeToFile:outputFilePath atomically:YES encoding:NSUTF8StringEncoding error:nil];
    NSArray *taskArguments =
        [NSArray arrayWithObjects:bashPath, // TODO: this seems useless.
                                  programTypeParameter, glslPath, @"--output", outputFilePath, @"--lang=msl",
                                  //                              @"--reflect", // Not used at this point
                                  nil];
    [transpileTask setArguments:taskArguments];

    NSPipe *outputPipe = [NSPipe pipe];
    [transpileTask setStandardOutput:outputPipe];

    NSPipe *errorPipe = [NSPipe pipe];
    [transpileTask setStandardError:errorPipe];

    [transpileTask launch];
    [transpileTask waitUntilExit];
    NSData *data = [[outputPipe fileHandleForReading] readDataToEndOfFile];
    NSString *result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];

    NSData *dataErr = [[errorPipe fileHandleForReading] readDataToEndOfFile];
    NSString *resultErr = [[NSString alloc] initWithData:dataErr encoding:NSUTF8StringEncoding];

    NSString *transpiled = nil;
    NSString *transpiledPath = nil;
    if( programType == ProgramTypeFragment )
    {
        transpiledPath = [outputFilePath stringByAppendingString:MISF_SPIRV_FILENAME_FRAGMENT_SUFFIX];
    }
    else if( programType == ProgramTypeVertex )
    {
        transpiledPath = [outputFilePath stringByAppendingString:MISF_SPIRV_FILENAME_VERTEX_SUFFIX];
    }
    NSError *error;
    transpiled = [NSString stringWithContentsOfFile:transpiledPath encoding:NSUTF8StringEncoding error:&error];
    if( transpiled == nil )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                @"SpirV-Cross Failed" : [NSString
                    stringWithFormat:@"Error reading file at %@\n%@. Maybe spirV failed, spirV output : %@ - %@",
                                     transpiledPath, [error localizedFailureReason], result, resultErr]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }

    return transpiled;
    //    NSString *reflectionFilePath = [NSString stringWithFormat:@"%@%@%@", outputFilePath, SPIRV_SUFFIX, @".json"];
    //    NSString *reflection = [NSString stringWithContentsOfFile:reflectionFilePath];
    //    NSLog(@"Reflection: %@ - %@", reflectionFilePath, reflection);
}

+ (NSString *)finaliseIntermediate:(NSString *)intermediate
                       programType:(ProgramType)programType
                         withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)

    switch( programType )
    {
    case ProgramTypeFragment:
    {
        BOOL returnTypeIsVoid = NO;

        NSRange range = [intermediate rangeOfString:@"fragment main0_out main0("];
        if( range.location == NSNotFound )
        {
            // Try to find a void prototype
            NSRange voidRange = [intermediate rangeOfString:@"fragment void main0("];
            if( voidRange.location == NSNotFound )
            {
                if( errorPtr )
                {
                    NSDictionary *userInfo = @{
                        ERROR_STRING_OPERATION_KEY : [NSString
                            stringWithFormat:@"failed to find fragment main prototype during finaliseIntermediate"]
                    };
                    *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
                }
                return nil;
            }
            else
            {
                returnTypeIsVoid = YES;
            }
        }

        // Change prototype return type
        if( returnTypeIsVoid )
        {
            intermediate = [MISFShaderConverter replaceOccurences:@"fragment void main0("
                                                       withString:@"fragment float4 main0("
                                                         onString:intermediate
                                          numberOfMatchesExpected:1
                                                            error:errorPtr];
        }
        else
        {
            intermediate = [MISFShaderConverter replaceOccurences:@"fragment main0_out main0("
                                                       withString:@"fragment float4 main0("
                                                         onString:intermediate
                                          numberOfMatchesExpected:1
                                                            error:errorPtr];
        }

        if( intermediate == nil )
        {
            return nil;
        }

        // Insert common prototype parameters
        intermediate = [MISFShaderConverter replaceOccurences:@"fragment float4 main0("
                                                   withString:@"fragment float4 main0(RasterizerData in [[stage_in]],"
                                                     onString:intermediate
                                      numberOfMatchesExpected:1
                                                        error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }

        // Insert posisition markers asap
        intermediate = [MISFShaderConverter identifyCodePartsInIntermediate:intermediate
                                                                programType:programType
                                                                  withError:errorPtr];

        if( returnTypeIsVoid )
        {
            intermediate = [MISFShaderConverter
                injectCode:@"// ISF Default return for empty fragment\n return float4(0.0,0.0,0.0,0.0);"
                    inCode:intermediate
                  atMarker:ISF_MARKER_INSIDE_FRAGMENT_MAIN
                 withError:errorPtr];
            if( intermediate == nil )
            {
                return nil;
            }
            // main is empty, so ignore the rest, it's useless
            break;
        }

        // ISF Built-in function IMG_SIZE becomes a preprocessor macro in MSL. string replaced based on expected Spir-V
        // output Output Function could be absent because Spir-V removes it if not used
        intermediate = [intermediate
            stringByReplacingOccurrencesOfString:@"inline float2 IMG_SIZE(thread const texture2d<float> imageName, "
                                                 @"thread const sampler imageNameSmplr)\n"
                                                  "{\n"
                                                  "    return float2(0.0);\n"
                                                  "}"
                                      withString:@"#define IMG_SIZE(a,b) float2(a.get_width(), a.get_height())"];

        // based on ISF_PRE_PROCESS_CODE_TO_INJECT_IN_MAIN, detect the SpirV output and adapt it
        intermediate =
            [MISFShaderConverter replaceOccurences:@"float2 isf_FragNormCoord = float2(0.0);"
                                        withString:@"float2 isf_FragNormCoord = float2(in.textureCoordinate.x, "
                                                   @"1.0-in.textureCoordinate.y);"
                                          onString:intermediate
                           numberOfMatchesExpected:1
                                             error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }

        intermediate = [MISFShaderConverter
                  replaceOccurences:@"float2 vv_FragNormCoord = float2(0.0);"
                         withString:@""
                                     "float2 vv_FragNormCoord = isf_FragNormCoord;\n"
                                     // clipSpacePosition in intermediate doesnt have the same coordinates than
                                     // glsl, so flip it
                                     "float4 gl_FragCoord = "
                                     "float4(isf_outputTexture.get_width()*in.textureCoordinate.x, "
                                     "isf_outputTexture.get_height()*(1.0-in.textureCoordinate.y), 0, 0);\n"
                           onString:intermediate
            numberOfMatchesExpected:1
                              error:errorPtr];

        if( intermediate == nil )
        {
            return nil;
        }

        // Return color instead of struct
        intermediate = [MISFShaderConverter replaceOccurences:@"return out;"
                                                   withString:@"return out._gl_FragColor;"
                                                     onString:intermediate
                                      numberOfMatchesExpected:1
                                                        error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }

        break;
    }

    case ProgramTypeVertex:
    {
        BOOL returnTypeIsVoid = NO;

        NSRange range = [intermediate rangeOfString:@"vertex main0_out main0("];
        if( range.location == NSNotFound )
        {
            // Try to find a void prototype
            NSRange voidRange = [intermediate rangeOfString:@"vertex void main0("];
            if( voidRange.location == NSNotFound )
            {
                if( errorPtr )
                {
                    NSDictionary *userInfo = @{
                        ERROR_STRING_OPERATION_KEY : [NSString
                            stringWithFormat:@"failed to find vertex main prototype during finaliseIntermediate"]
                    };
                    *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
                }
                return nil;
            }
            else
            {
                returnTypeIsVoid = YES;
            }
        }

        if( returnTypeIsVoid )
        {
            intermediate = [MISFShaderConverter replaceOccurences:@"vertex void main0("
                                                       withString:@"vertex RasterizerData main0("
                                                         onString:intermediate
                                          numberOfMatchesExpected:1
                                                            error:errorPtr];
            if( intermediate == nil )
            {
                return nil;
            }
        }
        else
        {
            intermediate = [MISFShaderConverter replaceOccurences:@"vertex main0_out main0("
                                                       withString:@"vertex RasterizerData main0("
                                                         onString:intermediate
                                          numberOfMatchesExpected:1
                                                            error:errorPtr];
            if( intermediate == nil )
            {
                return nil;
            }
        }

        // Insert markers asap
        intermediate = [MISFShaderConverter identifyCodePartsInIntermediate:intermediate
                                                                programType:programType
                                                                  withError:errorPtr];
        // Insert common prototype parameters
        intermediate = [MISFShaderConverter
                  replaceOccurences:@"vertex RasterizerData main0("
                         withString:@"vertex RasterizerData main0(uint vertexID [[ vertex_id ]], constant "
                                    @"MetalBitsTextureVertex "
                                     "*vertexArray [[ buffer(MetalBitsVertexInputIndexVertices) ]], "
                                     "constant vector_uint2 *viewportSizePointer  [[ "
                                     "buffer(MetalBitsVertexInputIndexViewportSize) ]],"
                           onString:intermediate
            numberOfMatchesExpected:1
                              error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }

        // Replace return struct by RasterizerData struct (defined in MISFRenderer.m)
        if( returnTypeIsVoid )
        {
            intermediate = [MISFShaderConverter injectCode:@"main0_out out = {};"
                                                    inCode:intermediate
                                                  atMarker:ISF_MARKER_INSIDE_VERTEX_MAIN
                                                 withError:errorPtr];
            // We know an empty vertex shader ends with this, so add a return at this point
            intermediate = [MISFShaderConverter replaceOccurences:@"isf_vertShaderInit();"
                                                       withString:@"isf_vertShaderInit();\n return out;"
                                                         onString:intermediate
                                          numberOfMatchesExpected:1
                                                            error:errorPtr];
            if( intermediate == nil )
            {
                return nil;
            }
        }

        intermediate = [MISFShaderConverter
                  replaceOccurences:@"main0_out out = {};"
                         withString:@"RasterizerData out;\n"
                                     ""
                                     ""
                                     "float2 pixelSpacePosition = vertexArray[vertexID].position.xy;\n"
                                     "float2 viewportSize = float2(*viewportSizePointer);\n"
                                     "out.clipSpacePosition.xy = pixelSpacePosition / (viewportSize / 2.0);\n"
                                     "out.clipSpacePosition.z = 0.0;\n"
                                     "out.clipSpacePosition.w = 1.0;\n"
                                     "out.textureCoordinate = vertexArray[vertexID].textureCoordinate;\n"
                                     ""
                           onString:intermediate
            numberOfMatchesExpected:1
                              error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }
        // ISF_PRE_PROCESS_CODE_TO_INJECT_IN_MAIN finalise
        intermediate =
            [MISFShaderConverter replaceOccurences:@"float2 isf_FragNormCoord = float2(0.0);"
                                        withString:@"float2 isf_FragNormCoord = float2(out.textureCoordinate.x, "
                                                   @"1.0-out.textureCoordinate.y);"
                                          onString:intermediate
                           numberOfMatchesExpected:1
                                             error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }

        intermediate = [MISFShaderConverter replaceOccurences:@"float2 vv_FragNormCoord = float2(0.0);"
                                                   withString:@""
                                                               "float2 vv_FragNormCoord = isf_FragNormCoord;\n"
                                                     onString:intermediate
                                      numberOfMatchesExpected:1
                                                        error:errorPtr];
        if( intermediate == nil )
        {
            return nil;
        }
#warning mto-anomes: gl_FragCoord is not defined here (it is in fragment). maybe look for an example online that uses it? or add access to RENDERSIZE manually in the buffer to make sure we can access proper size
        intermediate = [intermediate stringByReplacingOccurrencesOfString:@"out.gl_Position"
                                                               withString:@"out.clipSpacePosition"];
        break;
    }
    }

    return intermediate;
}

+ (NSString *)identifyCodePartsInIntermediate:(NSString *)intermediate
                                  programType:(ProgramType)programType
                                    withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)
    // Identify code parts
    NSError *regexError;
    NSRange rangeMainPrototype =
        [RegexTools getRangeInString:intermediate
                             pattern:IS_VERTEX(programType) ? MISF_REGEX_METAL_VERTEX_MAIN_WITH_BRACKET
                                                            : MISF_REGEX_METAL_FRAGMENT_MAIN_WITH_BRACKET
                           withError:&regexError];

    if( rangeMainPrototype.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY : [NSString
                    stringWithFormat:@"failed to find void main Range during finalise Intermediate. Regex Error: %@",
                                     regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSRange rangeBeforeMain = NSMakeRange(0, rangeMainPrototype.location);
    NSRange rangeInsideMainUntilEnd =
        NSMakeRange(rangeMainPrototype.location + rangeMainPrototype.length,
                    intermediate.length - rangeMainPrototype.location - rangeMainPrototype.length);
    NSString *intermediateBeforeMain = [intermediate substringWithRange:rangeBeforeMain];
    NSString *intermediateMainPrototypeUntilFirstBracket = [intermediate substringWithRange:rangeMainPrototype];
    NSString *intermediateMainAndAfter = [intermediate substringWithRange:rangeInsideMainUntilEnd];

    intermediate = [NSString
        stringWithFormat:@"%@%@%@%@%@", intermediateBeforeMain,
                         IS_VERTEX(programType) ? ISF_MARKER_BEFORE_VERTEX_MAIN : ISF_MARKER_BEFORE_FRAGMENT_MAIN,
                         intermediateMainPrototypeUntilFirstBracket,
                         IS_VERTEX(programType) ? ISF_MARKER_INSIDE_VERTEX_MAIN : ISF_MARKER_INSIDE_FRAGMENT_MAIN,
                         intermediateMainAndAfter];
    return intermediate;
}
#pragma mark Operations on translated code

+ (NSArray<MISFAttribBufferDefinition *> *)parseVertexBuffers:(NSString *)msl withError:(NSError **)errorPtr
{
    return [MISFShaderConverter parseBuffers:msl programType:ProgramTypeVertex withError:errorPtr];
}

+ (NSArray<MISFAttribBufferDefinition *> *)parseFragmentBuffers:(NSString *)msl withError:(NSError **)errorPtr
{
    return [MISFShaderConverter parseBuffers:msl programType:ProgramTypeFragment withError:errorPtr];
}

+ (NSArray<MISFAttribBufferDefinition *> *)parseBuffers:(NSString *)msl
                                            programType:(ProgramType)programType
                                              withError:(NSError **)errorPtr
{
    CHECK_PROGRAM_TYPE(programType, nil)
    NSMutableArray<MISFAttribBufferDefinition *> *buffers = [NSMutableArray<MISFAttribBufferDefinition *> new];

    NSError *regexError;
    NSRange rangeMainPrototype = [RegexTools
        getRangeInString:msl
                 pattern:IS_VERTEX(programType) ? MISF_REGEX_METAL_VERTEX_MAIN : MISF_REGEX_METAL_FRAGMENT_MAIN
               withError:&regexError];

    if( rangeMainPrototype.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:
                                  @"failed to find main prototype Range during parseBufferDefinitions. Regex Error: %@",
                                  regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSString *mainPrototype = [msl substringWithRange:rangeMainPrototype];

    NSError *regexError2;
    NSRange rangeParameterList = [RegexTools getRangeInString:mainPrototype
                                                      pattern:MISF_REGEX_PARAMETER_LIST
                                                    withError:&regexError2];
    if( rangeParameterList.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:
                                  @"failed to find parameter list Range during parseBufferDefinitions. Regex Error: %@",
                                  regexError2]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }

    NSString *parameterListString = [mainPrototype substringWithRange:rangeParameterList];
    NSArray *parameterList = [parameterListString componentsSeparatedByString:@","];

    // Ignore three first ones, currently vertex data
    // TODO: probably a way to clean this
    const int initialIndex = IS_VERTEX(programType) ? 3 : 1;
    for( int index = initialIndex; index < parameterList.count; index++ )
    {

        NSString *parameter = parameterList[index];
        // closing parenthesis can be the last of the list if there was one too much coma character. Ignore it.
        if( [parameter isEqualToString:@")"] )
        {
            continue;
        }
        NSArray<NSString *> *tokens = [parameter componentsSeparatedByString:@" "];
        if( tokens.count < 3 )
        {
            NSLog(@"WARN: unexpected tokens in parseBufferDefinitions: %@. Will ignore", tokens);
            continue;
        }
        NSString *bufferIndexToken = tokens.lastObject;
        NSString *variableNameToken = tokens[tokens.count - 2];
        NSString *partialTypeToken = tokens[tokens.count - 3];
        // Remove adress character
        partialTypeToken = [partialTypeToken stringByReplacingOccurrencesOfString:@"&" withString:@""];
        int bufferIndex = [RegexTools extractNumberFromString:bufferIndexToken];
        // Ignore values without indexes
        if( bufferIndex == -1 )
        {
            continue;
        }
        MISFInputDataType typeForBuffer = [MISFShaderConverter typeOfPartialTypeToken:partialTypeToken];
        MISFAttribBufferDefinition *bufferData =
            [[MISFAttribBufferDefinition alloc] initWithVariableName:variableNameToken
                                                         bufferIndex:bufferIndex
                                                                type:typeForBuffer];
        [buffers addObject:bufferData];
    }

    return [buffers copy];
}

// FRAGMENT-RELATED
+ (NSString *)replaceBuffers:(NSString *)bufferForReplacement
                       inMsl:(NSString *)msl
                    isVertex:(BOOL)isVertex
                   withError:(NSError **)errorPtr
{
    NSError *regexToolsError;
    NSRange mainPrototypeRange =
        [RegexTools getRangeInString:msl
                             pattern:isVertex ? MISF_REGEX_METAL_VERTEX_MAIN : MISF_REGEX_METAL_FRAGMENT_MAIN
                           withError:&regexToolsError];
    if( mainPrototypeRange.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:
                                  @"failed to find mainPrototypeRange during replaceBuffers. Got RegexToolsError: %@",
                                  regexToolsError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSString *mainPrototype = [msl substringWithRange:mainPrototypeRange];

    //    NSLog(@"Got main?");
    //    NSLog(@"%@", mainPrototype);
    NSError *regexToolsError2;
    NSRange parameterListRange = [RegexTools getRangeInString:mainPrototype
                                                      pattern:MISF_REGEX_PARAMETER_LIST
                                                    withError:&regexToolsError2];
    if( parameterListRange.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:
                                  @"failed to find parameterListRange during replaceBuffers. Got RegexToolsError: %@",
                                  regexToolsError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSString *parameterListString = [mainPrototype substringWithRange:parameterListRange];
    NSArray<NSString *> *parameterList = [parameterListString componentsSeparatedByString:@","];
    //    NSLog(@"parameters %@", parameterList);
    NSRange rangeOfPrototypeDefinition = [parameterList[0] rangeOfString:@"("];

    if( rangeOfPrototypeDefinition.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:@"failed to find rangeOfPrototypeDefinition during replaceBuffers"]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }

    NSString *prototype = [parameterList[0] substringWithRange:NSMakeRange(0, rangeOfPrototypeDefinition.location)];
    // TODO: remove the first parameter properly !
    NSString *newParameterListString = prototype;

    NSString *bufferToInject = [NSString stringWithFormat:@"( %@ )", bufferForReplacement];
    newParameterListString = [newParameterListString stringByAppendingString:bufferToInject];

    NSRange parameterListRangeInMsl = [msl rangeOfString:parameterListString];
    if( parameterListRangeInMsl.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:@"failed to find parameterListRangeInMsl during replaceBuffers"]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSString *mslWithBuffersReplaced = [msl stringByReplacingCharactersInRange:parameterListRangeInMsl
                                                                    withString:newParameterListString];
    //    NSLog(@"new param list: %@", newParameterListString);
    //    NSLog(@"final result: %@", msl);

    return mslWithBuffersReplaced;
}

+ (NSString *)injectCode:(NSString *)codeToInject
         beforeMainInMsl:(NSString *)msl
                isVertex:(BOOL)isVertex
               withError:(NSError **)errorPtr
{
    NSString *marker = isVertex ? ISF_MARKER_BEFORE_VERTEX_MAIN : ISF_MARKER_BEFORE_FRAGMENT_MAIN;
    return [MISFShaderConverter injectCode:codeToInject inCode:msl atMarker:marker withError:errorPtr];
}

+ (NSString *)injectCode:(NSString *)codeToInject
        atTopOfMainInMsl:(NSString *)msl
                isVertex:(BOOL)isVertex
               withError:(NSError **)errorPtr
{
    NSString *marker = isVertex ? ISF_MARKER_INSIDE_VERTEX_MAIN : ISF_MARKER_INSIDE_FRAGMENT_MAIN;
    return [MISFShaderConverter injectCode:codeToInject inCode:msl atMarker:marker withError:errorPtr];
}

+ (NSString *)injectCode:(NSString *)codeToInject
                  inCode:(NSString *)msl
                atMarker:(NSString *)marker
               withError:(NSError **)errorPtr
{
    // Inject ISF Variables
    NSRange insideMainRange = [msl rangeOfString:marker];
    if( insideMainRange.location == NSNotFound )
    {
#warning mto-anomes TODO: this is not fired ?!
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:@"failed to find marker (%@) to inject code", marker]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        }
        return nil;
    }
    NSString *before = [msl substringWithRange:NSMakeRange(0, insideMainRange.location + insideMainRange.length)];
    NSString *after =
        [msl substringWithRange:NSMakeRange(insideMainRange.location + insideMainRange.length,
                                            msl.length - insideMainRange.location - insideMainRange.length)];

    NSString *codeWithInjection = [NSString stringWithFormat:@"%@%@%@", before, codeToInject, after];
    //    NSLog(@"INJECTION %@", codeWithInjection);
    return codeWithInjection;
}

#pragma mark Utils

#include <simd/simd.h>

+ (MISFInputDataType)typeOfPartialTypeToken:(NSString *)partialType
{
    NSString *stringToTest = [partialType stringByReplacingOccurrencesOfString:@"&" withString:@""];
    if( [stringToTest isEqualToString:@"float4"] )
    {
        return MisfDataTypeFloat4;
    }
    if( [stringToTest isEqualToString:@"texture2d<float>"] )
    {
        return MisfDataTypeTexture;
    }
    if( [stringToTest isEqualToString:@"sampler"] )
    {
        return MisfDataTypeSampler;
    }
    if( [stringToTest isEqualToString:@"float2"] )
    {
        return MisfDataTypeFloat2;
    }
    if( [stringToTest isEqualToString:@"float"] )
    {
        return MisfDataTypeFloat;
    }
    if( [stringToTest isEqualToString:@"int"] )
    {
        return MisfDataTypeInt;
    }
    // long is casted to int as metal v1 doesnt handle longs
    if( [stringToTest isEqualToString:@"long"] )
    {
        return MisfDataTypeInt;
    }
    if( [stringToTest isEqualToString:@"bool"] )
    {
        return MisfDataTypeBool;
    }
    // Default, expect crash
    NSLog(@"no correct type, expect crash.");
    return 0;
}

+ (size_t)sizeOfDataType:(MISFInputDataType)dataType
{
    switch( dataType )
    {

    case MisfDataTypeFloat:
        return sizeof(float);
    case MisfDataTypeFloat2:
        return sizeof(vector_float2);
    case MisfDataTypeFloat3:
        return sizeof(vector_float3);
    case MisfDataTypeFloat4:
        return sizeof(vector_float4);
    case MisfDataTypeInt:
        return sizeof(int);
    case MisfDataTypeBool:
        return sizeof(bool);
    default:
        NSLog(@"ERR isf conversion: expect a compilation error/crash --  %lu", dataType);
        return sizeof(int);
    }
}

// Only handles types expected in ISF inputs
+ (NSString *)stringForMtlDataType:(MISFInputDataType)dataType
{
    switch( dataType )
    {

    case MisfDataTypeFloat:
        return @"float";
    case MisfDataTypeFloat2:
        return @"float2";
    case MisfDataTypeFloat3:
        return @"float3";
    case MisfDataTypeFloat4:
        return @"float4";
    case MisfDataTypeInt:
        return @"int";
    case MisfDataTypeBool:
        return @"bool";
    default:
        NSLog(@"ERR isf conversion: expect a compilation error for unknown type: %lu", dataType);
        return @"<UNHANDLED_DATA_TYPE>";
    }
}

+ (BOOL)isABuiltInIsfVariable:(NSString *)variableName
{
    // TODO: remove the two last ones?
    NSArray<NSString *> *isfBuiltInVariables = @[
        @"TIME", @"RENDERSIZE", @"PASSINDEX", @"TIMEDELTA", @"FRAMEINDEX", @"DATE", @"isf_FragNormCoord",
        @"vv_FragNormCoord"
    ];
    for( int index = 0; index < isfBuiltInVariables.count; index++ )
    {
        if( [isfBuiltInVariables[index] isEqualToString:variableName] )
        {
            return YES;
        }
    }
    return NO;
}

+ (NSString *)replaceOccurences:(NSString *)occurence
                     withString:(NSString *)replacement
                       onString:(NSString *)str
        numberOfMatchesExpected:(int)numberOfMatchesExpected
                          error:(NSError **)errorPtr
{
    NSUInteger count = 0, length = [str length];
    NSRange range = NSMakeRange(0, length);
    while( range.location != NSNotFound )
    {
        range = [str rangeOfString:occurence options:0 range:range];
        if( range.location != NSNotFound )
        {
            range = NSMakeRange(range.location + range.length, length - (range.location + range.length));
            count++;
        }
    }

    if( count != numberOfMatchesExpected )
    {

        // TODO: error
        NSDictionary *userInfo = @{
            @"Failed to replace string" :
                [NSString stringWithFormat:@"Occurrences mismatch (Expected %lu, got %i occurences of the string). "
                                           @"String to replace: (%@) \n String replacement: (%@)",
                                           count, numberOfMatchesExpected, occurence, replacement]
        };
        *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFMetalConversionError userInfo:userInfo];
        return nil;
    }
    else
    {
        NSString *s = [str stringByReplacingOccurrencesOfString:occurence withString:replacement];
        return s;
    }
}

@end
