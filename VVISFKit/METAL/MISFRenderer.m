#import "MISFRenderer.h"
#import "ISFAttrib.h"
#import "MISFInputDataType.h"
#import "MISFInputsBuffer.h"
#import "MISFShaderConverter.h"
#import "RegexTools.h"

typedef enum MetalBitsVertexInputIndex
{
    MetalBitsVertexInputIndexVertices = 0,
    MetalBitsVertexInputIndexViewportSize = 1,
} MetalBitsVertexInputIndex;

typedef struct
{
    vector_float2 position;
    vector_float2 textureCoordinate;
} MetalBitsTextureVertex;

typedef struct
{
    int PASSINDEX;
    vector_float2 RENDERSIZE;
    float TIME;
    float TIMEDELTA;
    vector_float4 DATE;
    int FRAMEINDEX;
} MISFBuiltInVariablesBufferType;

typedef enum BufferIndex
{
    BufferIndexZero = 0,
    BufferIndexOne = 1,
    BufferIndexTwo = 2,
    BufferIndexThree = 3
} BufferIndex;

static NSString *const SHADER_TYPES = @""
                                       "#include <simd/simd.h>\n"
                                       "#include <metal_stdlib>\n"
                                       "using namespace metal;\n"
                                       "typedef enum BufferIndex\n"
                                       "{\n"
                                       "    BufferIndexZero = 0,\n"
                                       "    BufferIndexOne = 1,\n"
                                       "    BufferIndexTwo = 2,\n"
                                       "    BufferIndexThree = 3\n"
                                       "} BufferIndex;\n"
                                       "\n"
                                       "typedef enum MetalBitsVertexInputIndex\n"
                                       "{"
                                       "    MetalBitsVertexInputIndexVertices     = 0,\n"
                                       "    MetalBitsVertexInputIndexViewportSize =  1,\n"
                                       "} MetalBitsVertexInputIndex;\n"
                                       "typedef struct\n"
                                       "{"
                                       "    vector_float2 position;\n"
                                       "    vector_float2 textureCoordinate;\n"
                                       "} MetalBitsTextureVertex;\n"
                                       "\n"
                                       "typedef struct\n"
                                       "{\n"
                                       "    int PASSINDEX;\n"
                                       "    vector_float2 RENDERSIZE;\n"
                                       "    float TIME;\n"
                                       "    float TIMEDELTA;\n"
                                       "    vector_float4 DATE;\n"
                                       "    int FRAMEINDEX;\n"
                                       "} IsfBuiltInsBufferType;\n";

static NSString *const VERTEX_BUFFER_STRUCT_MARKER = @"/* {MARKER FOR INJECTION RASTERIZER DATA CONTENT} */";
static NSString *const VERTEX_BUFFER_STRUCT =
    @""
     "typedef struct\n"
     "{"
     "    float4 clipSpacePosition [[position]];\n"
     "    float4 color;\n"
     "    float2 textureCoordinate;\n"
     "    float4 gl_Position;\n" // TODO: this is just to help spirV conversion but should not be used
     "/* {MARKER FOR INJECTION RASTERIZER DATA CONTENT} */\n"
     "} RasterizerData;\n";

static NSString *const DEFAULT_VERTEX_PROGRAM =
    @""

     "vertex RasterizerData main0(uint vertexID [[ vertex_id ]], constant MetalBitsTextureVertex "
     "*vertexArray [[ buffer(MetalBitsVertexInputIndexVertices) ]], constant vector_uint2 *viewportSizePointer  [[ "
     "buffer(MetalBitsVertexInputIndexViewportSize) ]]) \n"
     "{\n"
     "RasterizerData out;\n"
     "float2 pixelSpacePosition = vertexArray[vertexID].position.xy;\n"
     "float2 viewportSize = float2(*viewportSizePointer);\n"
     "out.clipSpacePosition.xy = pixelSpacePosition / (viewportSize / 2.0);\n"
     "out.clipSpacePosition.z = 0.0;\n"
     "out.clipSpacePosition.w = 1.0;\n"
     "out.textureCoordinate = vertexArray[vertexID].textureCoordinate;\n"
     "return out;\n"
     "}\n";

static NSString *const MISF_BUILTINS_STRUCT_TO_VARIABLES = @"\n"
                                                            "/* convert buffers data to ISF inputs variables */\n"
                                                            "float PASSINDEX = isf_builtIns.PASSINDEX;\n"
                                                            "float2 RENDERSIZE = isf_builtIns.RENDERSIZE;\n"
                                                            "float TIME = isf_builtIns.TIME;\n"
                                                            "float TIMEDELTA = isf_builtIns.TIMEDELTA;\n"
                                                            "float4 DATE = isf_builtIns.DATE;\n"
                                                            "float FRAMEINDEX = isf_builtIns.FRAMEINDEX;\n";

@implementation MISFRenderer
{
    vector_uint2 _viewportSize;
    id<MTLRenderPipelineState> pipelineState;
    MISFBuiltInVariablesBufferType builtInVariablesDataPointer;
    id<MTLBuffer> builtInVariablesBuffer;
    MISFInputsBuffer *inputsBufferForFragment;
    MISFInputsBuffer *inputsBufferForVertex;
    BOOL customVertexCode;
}

- (instancetype)initWithDevice:(id<MTLDevice>)device
              colorPixelFormat:(MTLPixelFormat)colorPixelFormat
                      forModel:(MISFMetalModel *)model
                     withError:(NSError **)errorPtr
{
    MISFPreloadedMedia *modelForRenderer = [MISFRenderer preloadModel:model onDevice:device withError:nil];
    return [self initWithDevice:device
               colorPixelFormat:colorPixelFormat
              forPreloadedMedia:modelForRenderer
                      withError:errorPtr];
}

- (instancetype)initWithDevice:(id<MTLDevice>)device
              colorPixelFormat:(MTLPixelFormat)colorPixelFormat
             forPreloadedMedia:(MISFPreloadedMedia *)preloadedMedia
                     withError:(NSError **)errorPtr
{
    self = [super init];
    if( self )
    {
        customVertexCode = preloadedMedia.vertexCode != nil;

        // --- Create built-in variables buffer

        builtInVariablesBuffer = [device newBufferWithLength:sizeof(MISFBuiltInVariablesBufferType)
                                                     options:MTLResourceStorageModeShared];

        // --- Create Buffer for ISF Inputs
#warning mto-anomes: this is a bit dirty
        inputsBufferForFragment =
            [MISFRenderer createInputBufferWithDefinitions:preloadedMedia.model.fragmentBufferDefinitions];
        [inputsBufferForFragment createBufferOnDevice:device];
        inputsBufferForVertex =
            [MISFRenderer createInputBufferWithDefinitions:preloadedMedia.model.vertexBufferDefinitions];
        [inputsBufferForVertex createBufferOnDevice:device];

        // TODO: compilation Error example
        //        if( fragmentError )
        //        {
        //            switch( fragmentError.code )
        //            {
        //            case MTLLibraryErrorCompileWarning:
        //                NSLog(@"Fragment compiled successfully with warnings :%@", fragmentError);
        //                break;
        //            default:
        //                NSLog(@"ERR: ISF could not compile fragment: %@", fragmentError);
        //                break;
        //            }
        //        }

        // Load the vertex/fragment functions from the library
        id<MTLFunction> vertexFunction = [preloadedMedia.vertexLibrary newFunctionWithName:@"main0"];
        id<MTLFunction> fragmentFunction = [preloadedMedia.fragmentLibrary newFunctionWithName:@"main0"];

        // Set up a descriptor for creating a pipeline state object
        MTLRenderPipelineDescriptor *pipelineStateDescriptor = [[MTLRenderPipelineDescriptor alloc] init];
        pipelineStateDescriptor.vertexFunction = vertexFunction;
        pipelineStateDescriptor.fragmentFunction = fragmentFunction;
        pipelineStateDescriptor.colorAttachments[0].pixelFormat = colorPixelFormat;
        //        pipelineStateDescriptor.colorAttachments[0].blendingEnabled = YES;
        //        pipelineStateDescriptor.colorAttachments[0].rgbBlendOperation = MTLBlendFactorOne;
        //        pipelineStateDescriptor.colorAttachments[0].alphaBlendOperation = MTLBlendOperationAdd;
        //        pipelineStateDescriptor.colorAttachments[0].sourceRGBBlendFactor = MTLBlendFactorSourceAlpha;
        //        pipelineStateDescriptor.colorAttachments[0].sourceAlphaBlendFactor = MTLBlendFactorSourceAlpha;
        //        pipelineStateDescriptor.colorAttachments[0].destinationRGBBlendFactor =
        //        MTLBlendFactorOneMinusSourceAlpha;
        //        pipelineStateDescriptor.colorAttachments[0].destinationAlphaBlendFactor =
        //        MTLBlendFactorOneMinusSourceAlpha;
        pipelineState = [device newRenderPipelineStateWithDescriptor:pipelineStateDescriptor error:errorPtr];

        if( !pipelineState )
        {
            return nil;
        }
    }

    return self;
}

- (void)renderIsfOnTexture:(id<MTLTexture>)outputTexture
           onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
                withInputs:(MutLockArray *)inputs
{
    if( outputTexture == nil )
    {
        NSLog(@"ERR: nil texture to render into");
        return;
    }
    // --- Prep viewport
    vector_uint2 _viewportSize;
    _viewportSize.x = (int)outputTexture.width;
    _viewportSize.y = (int)outputTexture.height;

    const float w = outputTexture.width / 2;
    const float h = outputTexture.height / 2;

    const MetalBitsTextureVertex quadVertices[] = {
        // Pixel positions, Texture coordinates
        {{w, h}, {1.f, 1.f}}, {{-w, h}, {0.f, 1.f}},  {{-w, -h}, {0.f, 0.f}},
        {{w, h}, {1.f, 1.f}}, {{-w, -h}, {0.f, 0.f}}, {{w, -h}, {1.f, 0.f}},
    };
    NSUInteger numberOfVertices = sizeof(quadVertices) / sizeof(MetalBitsTextureVertex);

    // --- Prep Render Pass
    MTLRenderPassDescriptor *renderPassDescriptor = [MTLRenderPassDescriptor renderPassDescriptor];
    renderPassDescriptor.colorAttachments[0].texture = outputTexture;
    // always start fresh
    renderPassDescriptor.colorAttachments[0].loadAction = MTLLoadActionClear;
    renderPassDescriptor.colorAttachments[0].storeAction = MTLStoreActionStore;
    renderPassDescriptor.colorAttachments[0].clearColor = MTLClearColorMake(0, 0, 0, 0);

    // Create a render command encoder so we can render into something
    id<MTLRenderCommandEncoder> renderEncoder = [commandBuffer renderCommandEncoderWithDescriptor:renderPassDescriptor];
    renderEncoder.label = @"ISF Renderer Render Encoder";
    [renderEncoder setRenderPipelineState:pipelineState];
    [renderEncoder setFragmentTexture:outputTexture atIndex:0];

    // --- feeding buffer of Built in variables
    {
        MISFBuiltInVariablesBufferType *pointer = builtInVariablesBuffer.contents;
        builtInVariablesDataPointer.PASSINDEX = self.builtin_PASSINDEX;
        builtInVariablesDataPointer.RENDERSIZE =
            simd_make_float2(self.builtin_RENDERSIZE.width, self.builtin_RENDERSIZE.height);
        builtInVariablesDataPointer.TIME = self.builtin_TIME;
        builtInVariablesDataPointer.TIMEDELTA = self.builtin_TIMEDELTA;
        builtInVariablesDataPointer.DATE =
            simd_make_float4(self.builtin_DATE.x, self.builtin_DATE.y, self.builtin_DATE.z, self.builtin_DATE.w);
        builtInVariablesDataPointer.FRAMEINDEX = self.builtin_FRAMEINDEX;
        *pointer = builtInVariablesDataPointer;
    }
    [renderEncoder setFragmentBuffer:builtInVariablesBuffer offset:0 atIndex:BufferIndexZero];
    if( customVertexCode )
    {
        [renderEncoder setVertexBuffer:builtInVariablesBuffer offset:0 atIndex:BufferIndexTwo];
    }

    // Vertex Buffer
    [renderEncoder setVertexBytes:quadVertices length:sizeof(quadVertices) atIndex:MetalBitsVertexInputIndexVertices];
    [renderEncoder setVertexBytes:&_viewportSize
                           length:sizeof(_viewportSize)
                          atIndex:MetalBitsVertexInputIndexViewportSize];

    // Inputs Buffer feeding
    [inputsBufferForFragment feedInputs:inputs forRenderEncoder:renderEncoder];
    [renderEncoder setFragmentBuffer:inputsBufferForFragment.buffer offset:0 atIndex:BufferIndexOne];

    if( customVertexCode )
    {
        [inputsBufferForVertex feedInputs:inputs forRenderEncoder:renderEncoder];
        [renderEncoder setVertexBuffer:inputsBufferForVertex.buffer offset:0 atIndex:BufferIndexThree];
    }

    [renderEncoder drawPrimitives:MTLPrimitiveTypeTriangle vertexStart:0 vertexCount:numberOfVertices];
    [renderEncoder endEncoding];
}

#pragma mark Helpers

+ (id<MTLLibrary>)compileShader:(NSString *)shader onDevice:(id<MTLDevice>)device withError:(NSError **)errorPtr
{
    MTLCompileOptions *compileOptions = [MTLCompileOptions new];
    compileOptions.languageVersion = MTLLanguageVersion1_1;
    id<MTLLibrary> library = [device newLibraryWithSource:shader options:compileOptions error:errorPtr];
    return library;
}

+ (MISFInputsBuffer *)createInputBufferWithDefinitions:(NSArray<MISFAttribBufferDefinition *> *)bufferDefinitions
{
    MISFInputsBuffer *inputBuffer = [MISFInputsBuffer new];
    for( MISFAttribBufferDefinition *bufferDefinition in bufferDefinitions )
    {
        BOOL isBuiltInIsfVariable = [MISFShaderConverter isABuiltInIsfVariable:bufferDefinition.variableName];
        if( isBuiltInIsfVariable )
        {
            continue;
        }

        MISFInputDataType dataType = bufferDefinition.type;
        NSString *attribName = bufferDefinition.variableName;
        [inputBuffer addEntry:attribName dataType:dataType];
    }
    return inputBuffer;
}

+ (MISFPreloadedMedia *)preloadModel:(MISFMetalModel *)model
                            onDevice:(id<MTLDevice>)device
                           withError:(NSError **)errorPtr
{
    BOOL customVertexCode = model.convertedVertexCode != nil;
    NSString *vertexCode = customVertexCode ? model.convertedVertexCode
                                            : [NSString stringWithFormat:@"%@%@%@", SHADER_TYPES, VERTEX_BUFFER_STRUCT,
                                                                         DEFAULT_VERTEX_PROGRAM];
    NSString *fragmentCode = model.convertedFragmentCode;

#warning mto-anomes MISFInputBuffer has metal resources inside but we dont use the metal resources here. We're interested in the buffer struct definitions. Best thing would be to refactor MISFInputBuffer and separate model from resources
    MISFInputsBuffer *inputsBufferForFragment =
        [MISFRenderer createInputBufferWithDefinitions:model.fragmentBufferDefinitions];
    MISFInputsBuffer *inputsBufferForVertex = nil;
    if( customVertexCode )
    {
        inputsBufferForVertex = [MISFRenderer createInputBufferWithDefinitions:model.vertexBufferDefinitions];
    }

    NSString *rasteriserDataStruct = VERTEX_BUFFER_STRUCT;
    if( customVertexCode )
    {
        // Varyings are converted into in/out structures by SpirV transpilation process.
        // We look for those structures, parse their definitions and inject them in our main vertex struct,
        // RasteriserData.

#warning mto-anomes TODO: could use SpirV reflect data instead of a dirty regex?
        // https://regex101.com/r/AsoPaY/1
        NSString *REGEX_MAIN_OUT_STRUCT = @"struct\\s+main0_out\\s+[{]([^}])*[}]";
        NSRange structRange = [RegexTools getRangeInString:vertexCode pattern:REGEX_MAIN_OUT_STRUCT withError:nil];
        if( structRange.location == NSNotFound )
        {
            NSLog(@"ISF INFO: cant find main0_out in vertex code. Nothing to inject in RasterizerData. This could be "
                  @"bad. But may be normal.");
        }
        else
        {
            NSString *structString = [vertexCode substringWithRange:structRange];
            NSString *structContent = [[[[[structString stringByReplacingOccurrencesOfString:@"{" withString:@""]
                stringByReplacingOccurrencesOfString:@"}"
                                          withString:@""] stringByReplacingOccurrencesOfString:@"struct" withString:@""]
                stringByReplacingOccurrencesOfString:@"main0_out"
                                          withString:@""]
                stringByReplacingOccurrencesOfString:@"float4 gl_Position [[position]];"
                                          withString:@""]; // No need to add this one
            NSError *regexError;
            rasteriserDataStruct = [RegexTools injectString:structContent
                                                   inString:rasteriserDataStruct
                                                   atMarker:VERTEX_BUFFER_STRUCT_MARKER
                                                  withError:&regexError];
            if( rasteriserDataStruct == nil )
            {
                if( errorPtr )
                {
                }
                return nil;
            }
        }
    }

    // Add buffer structs in code
    {
        NSString *structDefinition = [inputsBufferForFragment structDefinition];
        fragmentCode = [[[SHADER_TYPES stringByAppendingString:structDefinition]
            stringByAppendingString:rasteriserDataStruct] stringByAppendingString:fragmentCode];
    }
    if( customVertexCode )
    {
        NSString *structDefinition = [inputsBufferForVertex structDefinition];
        vertexCode = [[[SHADER_TYPES stringByAppendingString:structDefinition]
            stringByAppendingString:rasteriserDataStruct] stringByAppendingString:vertexCode];
    }

    // --- Modify shader main prototype by adding buffer parameters
    // and create ISF variable Names accessing structBufferData at top of shader main()
    {
        NSString *inputsBufferParameters =
            [inputsBufferForFragment bufferParametersStringWithBufferIndex:@"buffer(BufferIndexOne)"];
        NSString *shaderFragmentBufferParameter = [NSString
            stringWithFormat:@"RasterizerData in [[stage_in]], texture2d<float> isf_outputTexture [[texture(0)]], "
                             @"const device IsfBuiltInsBufferType &isf_builtIns [[ buffer(BufferIndexZero) "
                             @"]], %@",
                             inputsBufferParameters];

        fragmentCode = [MISFShaderConverter replaceBuffers:shaderFragmentBufferParameter
                                                     inMsl:fragmentCode
                                                  isVertex:NO
                                                 withError:errorPtr];
        if( fragmentCode == nil )
        {
            return nil;
        }
        NSString *structToVariables = [inputsBufferForFragment structToVariables];
        structToVariables = [structToVariables stringByAppendingString:MISF_BUILTINS_STRUCT_TO_VARIABLES];
        fragmentCode = [MISFShaderConverter injectCode:structToVariables
                                      atTopOfMainInMsl:fragmentCode
                                              isVertex:NO
                                             withError:errorPtr];
        if( fragmentCode == nil )
        {
            return nil;
        }
    }

    // --- Same but for vertex
    if( customVertexCode )
    {
        NSString *inputsBufferParameters =
            [inputsBufferForVertex bufferParametersStringWithBufferIndex:@"buffer(BufferIndexThree)"];
        NSString *shaderVertexBufferParameter = [NSString
            stringWithFormat:@"uint vertexID [[ vertex_id ]], constant MetalBitsTextureVertex *vertexArray [[ "
                             @"buffer(MetalBitsVertexInputIndexVertices) ]], constant vector_uint2 "
                              "*viewportSizePointer  [[ buffer(MetalBitsVertexInputIndexViewportSize) ]], const "
                              "device IsfBuiltInsBufferType &isf_builtIns [[ buffer(BufferIndexTwo) "
                             @"]], %@",
                             inputsBufferParameters];

        // Fragment-related
        vertexCode = [MISFShaderConverter replaceBuffers:shaderVertexBufferParameter
                                                   inMsl:vertexCode
                                                isVertex:YES
                                               withError:errorPtr];
        if( vertexCode == nil )
        {
            return nil;
        }
        NSString *structToVariables = [inputsBufferForVertex structToVariables];
        structToVariables = [structToVariables stringByAppendingString:MISF_BUILTINS_STRUCT_TO_VARIABLES];
        vertexCode = [MISFShaderConverter injectCode:structToVariables
                                    atTopOfMainInMsl:vertexCode
                                            isVertex:YES
                                           withError:errorPtr];
        if( vertexCode == nil )
        {
            return nil;
        }
    }

    // Second step: compilation
    id<MTLLibrary> fragmentLibrary = [MISFRenderer compileShader:fragmentCode onDevice:device withError:errorPtr];
    if( fragmentLibrary == nil )
    {
        return nil;
    }
    id<MTLLibrary> vertexLibrary = [MISFRenderer compileShader:vertexCode onDevice:device withError:errorPtr];
    if( vertexLibrary == nil )
    {
        return nil;
    }

    MISFPreloadedMedia *media = [MISFPreloadedMedia new];
    media.fragmentCode = fragmentCode;
    media.vertexCode = vertexCode;
    media.fragmentLibrary = fragmentLibrary;
    media.vertexLibrary = vertexLibrary;
    media.model = model;
    return media;
}

@end
