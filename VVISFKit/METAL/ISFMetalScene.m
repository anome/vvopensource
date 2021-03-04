#import "ISFMetalScene.h"
#import "ISFAttrib.h"
#import "ISFFileManager.h"
#import "ISFRenderPass.h"
#import "ISFStringAdditions.h"
#import "MISFErrorCodes.h"
#import "MISFModel.h"
#import "MISFRenderPass.h"
#import "MISFRenderer.h"
#import "MISFShaderConverter.h"
#import "MISFTargetBuffer.h"
#import "MISFTextureRenderer.h"
#import <DDMathParser/DDMathParser.h>
#import <MetalKit/MetalKit.h>

// Not quite the same bit-wise than GL implementation, but seems to work just fine
const MTLPixelFormat PIXEL_FORMAT_FOR_FLOAT_TARGET = MTLPixelFormatRGBA32Float;

@implementation ISFMetalScene
{
    NSMutableDictionary<NSString *, MISFTargetBuffer *> *persistentBuffers;
    // Note: temp buffers are not really temp, they are kept just like persistent ones in this implementtion
    NSMutableDictionary<NSString *, MISFTargetBuffer *> *tempBuffers;
    NSMutableArray<ISFAttrib *> *importedImages;
    NSMutableArray<MISFRenderPass *> *passes;
    NSMutableArray<MISFRenderer *> *renderers;
    // Contains inputs secretly added for rendering such as persistent and temp buffers. Adding those as inputs
    // simplifies ISFRenderer logic
    MutLockArray *privateInputs;
// !! This array is a concatenation of arrays 'inputs' and 'privateInputs'. It's created once the two arrays have all
// their values. After creation, inputs and privateInputs should not change ! (and have no reason to)
#warning mto-anomes: possibly dangerous workaround. Nothing prevents user/developer to modify inputs or privateInputs
    MutLockArray *publicAndPrivateInputs;
    MISFTextureRenderer *textureRenderer;
    MutLockArray *importedImageInputs;
    VVStopwatch *swatch;  //    used to pass time to shaders
    int renderFrameIndex; //    used to pass FRAMEINDEX to shaders
    double renderTime;
    double renderTimeDelta;
    BOOL bufferRequiresEval; //    NO by default, set to YES during file open if any of the buffers require evaluation
    id<MTLDevice> device;
    MTLPixelFormat pixelFormat;
    id<MTLTexture> outputTexture;
    MISFPreloadedMedia *preloadedMedia;
}
@synthesize inputs;

#pragma mark INIT

- (id)initWithDevice:(id<MTLDevice>)theDevice
         pixelFormat:(MTLPixelFormat)thePixelFormat
    fragmentFilePath:(NSString *)filePath
           withError:(NSError **)errorPtr
{
    MISFPreloadedMedia *aPreloadedMedia = [ISFMetalScene preloadFile:filePath onDevice:theDevice withError:errorPtr];
    if( aPreloadedMedia )
    {
        return [self initWithDevice:theDevice
                        pixelFormat:thePixelFormat
                     preloadedMedia:aPreloadedMedia
                          withError:errorPtr];
    }
    else
    {
        return nil;
    }
}

- (id)initWithDevice:(id<MTLDevice>)theDevice
         pixelFormat:(MTLPixelFormat)thePixelFormat
      preloadedMedia:(MISFPreloadedMedia *)thePreloadedMedia
           withError:(NSError **)errorPtr
{
    self = [super init];
    if( self )
    {
#warning mto-anomes: error case: if preloadedmedia MTLdevice and given MTLdevice here are different, it could turn bad
        preloadedMedia = [thePreloadedMedia retain];
        device = theDevice;
        pixelFormat = thePixelFormat;
        passes = [NSMutableArray<MISFRenderPass *> new];
        persistentBuffers = [NSMutableDictionary<NSString *, MISFTargetBuffer *> new];
        tempBuffers = [NSMutableDictionary<NSString *, MISFTargetBuffer *> new];
        importedImages = [NSMutableArray<ISFAttrib *> new];
        renderers = nil;
        inputs = [[MutLockArray alloc] init];
        privateInputs = [[MutLockArray alloc] init];
        publicAndPrivateInputs = [[MutLockArray alloc] init];
        importedImageInputs = [[MutLockArray alloc] init];
        swatch = [[VVStopwatch alloc] init];
        BOOL allocateSuccess = [self allocateGpuResourcesWithError:errorPtr];
        if( !allocateSuccess )
        {
            return nil;
        }
    }
    return self;
}

#pragma mark SHADER PREP

+ (MISFModel *)parseFile:(NSString *)filePath withError:(NSError **)errorPtr
{
    MISFModel *isfModel = [[MISFModel alloc] initWithFilePath:filePath withError:errorPtr];
    return isfModel;
}

+ (MISFMetalModel *)convertModelToMetal:(MISFModel *)model withError:(NSError **)errorPtr
{
    MISFMetalModel *media = [MISFMetalModel new];
    media.parentModel = model;
    NSString *varDeclarations =
        [[[ISFMetalScene _assembleShaderSource_VarDeclarationsFromModel:model] copy] autorelease];
    NSString *fragmentCode = [MISFShaderConverter translateFragmentToMetal:model.fragShaderSource
                                                             inputUniforms:varDeclarations
                                                                 withError:errorPtr];
    if( fragmentCode == nil )
    {
        return nil;
    }
    media.convertedFragmentCode = fragmentCode;

    NSArray<MISFAttribBufferDefinition *> *fragmentBufferDefinitions =
        [MISFShaderConverter parseFragmentBuffers:fragmentCode withError:errorPtr];
    if( fragmentBufferDefinitions == nil )
    {
        return nil;
    }
    media.fragmentBufferDefinitions = fragmentBufferDefinitions;

    if( model.hasVertexShader )
    {
        NSString *vertexCode = [MISFShaderConverter translateVertexToMetal:model.vertShaderSource
                                                             inputUniforms:varDeclarations
                                                                 withError:errorPtr];
        if( vertexCode == nil )
        {
            return nil;
        }
        media.convertedVertexCode = vertexCode;
        NSArray<MISFAttribBufferDefinition *> *vertexBufferDefinitions =
            [MISFShaderConverter parseVertexBuffers:vertexCode withError:errorPtr];
        if( vertexBufferDefinitions == nil )
        {
            return nil;
        }
        media.vertexBufferDefinitions = vertexBufferDefinitions;
    }

    return media;
}

+ (MISFPreloadedMedia *)preloadModel:(MISFMetalModel *)model
                            onDevice:(id<MTLDevice>)device
                           withError:(NSError **)errorPtr
{
    MISFPreloadedMedia *preloadedModel = [MISFRenderer preloadModel:model onDevice:device withError:errorPtr];
    return preloadedModel;
}

+ (MISFPreloadedMedia *)preloadFile:(NSString *)filePath onDevice:(id<MTLDevice>)device withError:(NSError **)errorPtr
{
    MISFModel *isfModel = [ISFMetalScene parseFile:filePath withError:errorPtr];
    if( isfModel == nil )
    {
        return nil;
    }
    MISFMetalModel *metalModel = [ISFMetalScene convertModelToMetal:isfModel withError:errorPtr];
    if( metalModel == nil )
    {
        return nil;
    }
    MISFPreloadedMedia *preloadedModel = [ISFMetalScene preloadModel:metalModel onDevice:device withError:errorPtr];
    return preloadedModel;
}

+ (id<MTLTexture>)loadTextureUsingMetalKit:(NSURL *)url device:(id<MTLDevice>)device
{
    MTKTextureLoader *loader = [[MTKTextureLoader alloc] initWithDevice:device];
    NSDictionary<MTKTextureLoaderOption, id> *options =
        @{MTKTextureLoaderOptionSRGB : @NO, MTKTextureLoaderOptionOrigin : MTKTextureLoaderOriginBottomLeft};
    id<MTLTexture> texture = [loader newTextureWithContentsOfURL:url options:options error:nil];

    if( !texture )
    {
        NSLog(@"VVISfKit: Failed to create the texture from %@", url.absoluteString);
        return nil;
    }
    return texture;
}

#pragma mark LIFECYCLE

- (void)resetTimer
{
    renderTime = 0.;
    renderTimeDelta = 0.;
    [swatch start];
    renderFrameIndex = 0;
}

- (void)dealloc
{
    VVRELEASE(publicAndPrivateInputs);
    VVRELEASE(inputs);
    VVRELEASE(privateInputs);
    VVRELEASE(swatch);
    VVRELEASE(persistentBuffers);
    VVRELEASE(tempBuffers);
    VVRELEASE(importedImages);
    VVRELEASE(renderers);
    VVRELEASE(textureRenderer);
    VVRELEASE(importedImageInputs);
    VVRELEASE(preloadedMedia);

    [super dealloc];
}

- (BOOL)allocateGpuResourcesWithError:(NSError **)errorPtr
{
    MISFModel *isfModel = preloadedMedia.model.parentModel;
    [inputs lockRemoveAllObjects];
    NSLog(@"description %@", isfModel.fileDescription);
    NSLog(@"credits %@", isfModel.credits);
    NSLog(@"cat names %@", isfModel.categoryNames);
    NSLog(@"name %@", isfModel.fileName);
    NSLog(@"buffers %@", isfModel.persistentBuffers);

    // IMPLEMENT PERSISTENT BUFFERS
    for( MISFModelBuffer *model in isfModel.persistentBuffers )
    {
        if( model.requiresEval )
        {
            bufferRequiresEval = YES;
        }
        MTLPixelFormat pixelFormatForBuffer = model.floatFlag ? PIXEL_FORMAT_FOR_FLOAT_TARGET : pixelFormat;
        MISFTargetBuffer *newBuffer = [MISFTargetBuffer createForDevice:device
                                                            pixelFormat:pixelFormatForBuffer
                                                              fromModel:model];
        [persistentBuffers setValue:newBuffer forKey:newBuffer.name];
    }

    // IMPLEMENT PASSES
    for( MISFModelPass *model in isfModel.passes )
    {
        MISFRenderPass *newPass = [MISFRenderPass create];
        newPass.targetName = model.targetBuffer.name;
        newPass.targetIsFloat = model.targetBuffer.floatFlag;
        MISFTargetBuffer *targetForPass = [persistentBuffers objectForKey:newPass.targetName];

        // Create one if needed
        if( targetForPass == nil )
        {
            if( model.targetBuffer.requiresEval )
            {
                bufferRequiresEval = YES;
            }
            MTLPixelFormat pixelFormatForBuffer =
                model.targetBuffer.floatFlag ? PIXEL_FORMAT_FOR_FLOAT_TARGET : pixelFormat;
            MISFTargetBuffer *newBuffer = [MISFTargetBuffer createForDevice:device
                                                                pixelFormat:pixelFormatForBuffer
                                                                  fromModel:model.targetBuffer];
            if( model.targetBuffer.persistent )
            {
                [persistentBuffers setValue:newBuffer forKey:newBuffer.name];
            }
            else
            {
                [tempBuffers setValue:newBuffer forKey:newBuffer.name];
            }
        }

        [passes addObject:newPass];
    }

    //    if at this point there aren't any passes, add an empty pass
    if( [passes count] < 1 )
    {
        NSString *MISF_SECRET_SINGLE_PASS_TARGET = @"misf_SinglePassTarget";
        MISFRenderPass *renderPass = [MISFRenderPass create];
        MISFTargetBuffer *targetBufferForPass = [MISFTargetBuffer createForDevice:device pixelFormat:pixelFormat];
        [targetBufferForPass setName:MISF_SECRET_SINGLE_PASS_TARGET];
        renderPass.targetName = MISF_SECRET_SINGLE_PASS_TARGET;
        [tempBuffers setObject:targetBufferForPass forKey:MISF_SECRET_SINGLE_PASS_TARGET];
        [passes addObject:renderPass];
    }

    // We copy inputs from the model one by one and create our own inputs (using from model would be messy)
    for( ISFAttrib *attrib in isfModel.inputs )
    {
        ISFAttrib *copyAttrib = [ISFAttrib createFromAttrib:attrib];
        [inputs lockAddObject:copyAttrib];
    }

    // parse imported images and add them to inputs
    // IMPLEMENT IMPORTED IMAGES
    for( MISFModelImportedImage *model in isfModel.importedImages )
    {
        id<MTLTexture> importedBuffer = nil;
        NSString *parentDirectory = [isfModel.filePath stringByDeletingLastPathComponent];
        NSFileManager *fm = [NSFileManager defaultManager];
        NSString *fullPath = [VVFMTSTRING(@"%@/%@", parentDirectory, model.path) stringByStandardizingPath];

        //    if the path doesn't describe a valid file, throw an error
        if( ![fm fileExistsAtPath:fullPath] )
        {
            if( errorPtr )
            {
                NSDictionary *userInfo = @{
                    @"Missing filter resource" :
                        [NSString stringWithFormat:@"can't load, file %@ is missing", model.path]
                };
                *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeParsing userInfo:userInfo];
            }
            return NO;
        }
        NSURL *url = [NSURL fileURLWithPath:fullPath];
        importedBuffer = [ISFMetalScene loadTextureUsingMetalKit:url device:device];

        //    throw an error if i can't load the image
        if( importedBuffer == nil )
        {
            if( errorPtr )
            {
                NSDictionary *userInfo = @{
                    @"filter resource can't be loaded" :
                        [NSString stringWithFormat:@"file %@ was found, but can't be loaded", model.path]
                };
                *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeParsing userInfo:userInfo];
            }
            return NO;
        }
        else
        {
            // keep track of imported images, because we need to destroy them ourselves at some point
            //    assuming i've imported or located the appropriate file, make an attrib for it and store it
            importedBuffer.label = model.name;
            ISFAttrib *newAttrib = nil;
            ISFAttribVal minVal;
            ISFAttribVal maxVal;
            ISFAttribVal defVal;
            ISFAttribVal idenVal;
            minVal.imageVal = 0;
            maxVal.imageVal = 0;
            defVal.imageVal = 0;
            idenVal.imageVal = 0;
            newAttrib = [ISFAttrib createWithName:importedBuffer.label
                                      description:fullPath
                                            label:nil
                                             type:ISFAT_Image
                                           values:minVal:maxVal:defVal:idenVal:nil:nil];
            [newAttrib setUserInfo:importedBuffer];
            ISFAttribVal currentVal;
            currentVal.metalImageVal = importedBuffer;
            [newAttrib setCurrentVal:currentVal];
            [inputs lockAddObject:newAttrib];
            [importedImages addObject:newAttrib];
        }
    }

    // Metal Resources allocation starts here
    textureRenderer = [[MISFTextureRenderer alloc] initWithDevice:device colorPixelFormat:pixelFormat];

    // Inject all buffers as private inputs...
    for( NSString *bufferKey in tempBuffers )
    {
        ISFAttrib *newAttrib = nil;
        ISFAttribVal minVal;
        ISFAttribVal maxVal;
        ISFAttribVal defVal;
        ISFAttribVal idenVal;
        minVal.imageVal = 0;
        maxVal.imageVal = 0;
        defVal.imageVal = 0;
        idenVal.imageVal = 0;
        newAttrib = [ISFAttrib createWithName:bufferKey
                                  description:@""
                                        label:bufferKey // used to change texture at lazy init
                                         type:ISFAT_Image
                                       values:minVal:maxVal:defVal:idenVal:nil:nil];
        ISFAttribVal currentVal;
        // This will be filled at render time (because texture sizes might change)
        currentVal.metalImageVal = nil;
        [newAttrib setCurrentVal:currentVal];
        [privateInputs lockAddObject:newAttrib];
    }

    for( NSString *bufferKey in persistentBuffers )
    {
        ISFAttrib *newAttrib = nil;
        ISFAttribVal minVal;
        ISFAttribVal maxVal;
        ISFAttribVal defVal;
        ISFAttribVal idenVal;
        minVal.imageVal = 0;
        maxVal.imageVal = 0;
        defVal.imageVal = 0;
        idenVal.imageVal = 0;
        newAttrib = [ISFAttrib createWithName:bufferKey
                                  description:@""
                                        label:bufferKey // used to change texture at lazy init
                                         type:ISFAT_Image
                                       values:minVal:maxVal:defVal:idenVal:nil:nil];
        ISFAttribVal currentVal;
        // This will be filled at render time (because texture sizes might change)
        currentVal.metalImageVal = nil;
        [newAttrib setCurrentVal:currentVal];
        [privateInputs lockAddObject:newAttrib];
    }

    renderers = [NSMutableArray new];
    for( int n = 0; n < passes.count; n++ )
    {
        MTLPixelFormat pixelFormatForRenderer = passes[n].targetIsFloat ? PIXEL_FORMAT_FOR_FLOAT_TARGET : pixelFormat;
        MISFRenderer *renderer = [[MISFRenderer alloc] initWithDevice:device
                                                     colorPixelFormat:pixelFormatForRenderer
                                                    forPreloadedMedia:preloadedMedia
                                                            withError:errorPtr];
        if( renderer == nil )
        {
            return NO;
        }
        [renderers addObject:renderer];
    }

    // at this point, inputs will no longer change so create an array containing everyone
    [publicAndPrivateInputs lockAddObjectsFromArray:inputs];
    [publicAndPrivateInputs lockAddObjectsFromArray:privateInputs];
    [self resetTimer];
    return YES;
}

#pragma mark - RENDERING

- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
              withError:(NSError **)errorPtr
{
    const double t = [swatch timeSinceStart];
    return [self renderOnTexture:outputTexture onCommandBuffer:commandBuffer renderTime:t withError:errorPtr];
}

- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
             renderTime:(double)t
              withError:(NSError **)errorPtr
{
    id<MTLCommandQueue> commandQueue = [commandBuffer commandQueue];

    const VVSIZE outputTextureSize = VVMAKESIZE(outputTexture.width, outputTexture.height);
    renderTimeDelta = (t <= 0.) ? 0. : fabs(t - renderTime);
    renderTime = t;

    /// The subdict substitutionDictionary serves as a container for all the parameters that can be evaluated at runtime
    NSMutableDictionary *subDict = (bufferRequiresEval) ? [self _assembleSubstitutionDict] : nil;
    if( subDict != nil )
    {
        [subDict retain];
        [subDict setObject:NUMINT(outputTextureSize.width) forKey:@"WIDTH"];
        [subDict setObject:NUMINT(outputTextureSize.height) forKey:@"HEIGHT"];
    }

    //    make sure that all the persistent buffers are sized appropriately
    // AND INITIALISE THEM DIRECTLY if needed
    for( NSString *bufferKey in persistentBuffers )
    {
        MISFTargetBuffer *tmpBuffer = persistentBuffers[bufferKey];

        if( [tmpBuffer targetSizeNeedsEval] )
        {
            [tmpBuffer evalTargetSizeWithSubstitutionsDict:subDict];
        }

        else
        {
            [tmpBuffer setTargetSize:outputTextureSize];
        }
    }
    //    make sure all the temp buffers are also sized appropriately
    for( NSString *bufferKey in tempBuffers )
    {
        MISFTargetBuffer *tmpBuffer = tempBuffers[bufferKey];
        if( [tmpBuffer targetSizeNeedsEval] )
        {
            [tmpBuffer evalTargetSizeWithSubstitutionsDict:subDict];
        }

        else
        {
            [tmpBuffer setTargetSize:outputTextureSize];
        }
    }

    // Workaround : connect buffers to inputs as an easy way to make them accessible for the renderer
    // Runned every frame, could probably be runned only once
    for( NSString *bufferKey in tempBuffers )
    {
        id<MTLTexture> texture = [tempBuffers[bufferKey] getBufferTexture];
        ISFAttribVal imageVal;
        imageVal.metalImageVal = texture;
        [self setValue:imageVal forPrivateInputKey:bufferKey];
    }
    for( NSString *bufferKey in persistentBuffers )
    {
        id<MTLTexture> texture = [persistentBuffers[bufferKey] getBufferTexture];
        ISFAttribVal imageVal;
        imageVal.metalImageVal = texture;
        [self setValue:imageVal forPrivateInputKey:bufferKey];
    }

    // --------- Set buffer for Built-in ISF values
    NSDate *nowDate = [NSDate date];
    NSDateComponents *dateComps = [[NSCalendar currentCalendar]
        components:NSCalendarUnitNanosecond | NSCalendarUnitSecond | NSCalendarUnitMinute | NSCalendarUnitHour |
                   NSCalendarUnitDay | NSCalendarUnitMonth | NSCalendarUnitYear
          fromDate:nowDate];
    double timeInSeconds = 0.;
    {
        timeInSeconds += (double)[dateComps nanosecond] * (0.000000001);
        timeInSeconds += (double)[dateComps second];
        timeInSeconds += (double)[dateComps minute] * 60.;
        timeInSeconds += (double)[dateComps hour] * 60. * 60.;
    }

    // HERE: multi-pass debug
    const unsigned long numberOfPasses = passes.count; // self.choosePassIndex;
    const BOOL isMultiPass = 1 < passes.count;
    for( int index = 0; index < numberOfPasses; index++ )
    {
        MISFRenderPass *renderPass = passes[index];
        MISFRenderer *renderer = renderers[index];
        renderer.builtin_FRAMEINDEX = (int)renderFrameIndex;
        renderer.builtin_TIMEDELTA = renderTimeDelta;
        renderer.builtin_PASSINDEX = index;

        renderer.builtin_TIME = renderTime;
        renderer.builtin_DATE = simd_make_float4([dateComps year], [dateComps month], [dateComps day], timeInSeconds);
        NSString *passOutputKey = renderPass.targetName;
        id<MTLTexture> passOutputTexture = nil; // nil;
        const BOOL isLastPass = (index + 1 == numberOfPasses);

        if( isMultiPass )
        {
            MISFTargetBuffer *targetBuffer = [self getBufferNamed:passOutputKey];
            if( targetBuffer == nil )
            {
                if( errorPtr )
                {
                    NSDictionary *userInfo = @{
                        @"Internal error " :
                            [NSString stringWithFormat:@"could not find a buffer to render the pass !"]
                    };
                    *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeRendering userInfo:userInfo];
                }
                return NO;
            }
            passOutputTexture = [targetBuffer getBufferTexture];

            renderer.builtin_RENDERSIZE =
                NSMakeSize(passOutputTexture.width, passOutputTexture.height); // outputTexture?
            id<MTLCommandBuffer> passCommandBuffer = [commandQueue commandBuffer];
            passCommandBuffer.label =
                [NSString stringWithFormat:@"Pass command buffer N %i [Frame %i]", index, renderFrameIndex];

            [renderer renderIsfOnTexture:passOutputTexture
                         onCommandBuffer:passCommandBuffer
                              withInputs:publicAndPrivateInputs];
            [passCommandBuffer commit];
            if( isLastPass )
            {
                //                NSLog(@"INFO: got last pass, render  to outputTexture !");
                [textureRenderer renderFromTexture:passOutputTexture
                                         inTexture:outputTexture
                                   onCommandBuffer:commandBuffer];
            }
        }
        else // Single Pass
        {
            MISFTargetBuffer *targetBuffer = [self getBufferNamed:passOutputKey];
            if( targetBuffer == nil )
            {
                // Supposed to render in a target we dont have !
                if( errorPtr )
                {
                    NSDictionary *userInfo = @{
                        @"Internal error " :
                            [NSString stringWithFormat:@"could not find render target %@ to render into", passOutputKey]
                    };
                    *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeRendering userInfo:userInfo];
                }
                return NO;
            }
            passOutputTexture = [targetBuffer getBufferTexture];
            renderer.builtin_RENDERSIZE = NSMakeSize(passOutputTexture.width, passOutputTexture.height);
            id<MTLCommandBuffer> passCommandBuffer = [commandQueue commandBuffer];
            passCommandBuffer.label = @"ISF Single pass command Buffer";
            [renderer renderIsfOnTexture:passOutputTexture
                         onCommandBuffer:passCommandBuffer
                              withInputs:publicAndPrivateInputs];
            [passCommandBuffer commit];
            [textureRenderer renderFromTexture:passOutputTexture inTexture:outputTexture onCommandBuffer:commandBuffer];
        }
    }

    renderFrameIndex++;
    if( INT32_MAX - 10 < renderFrameIndex )
    {
        renderFrameIndex = 0;
    }
    return YES;
}

#pragma mark Inputs

- (void)setValue:(ISFAttribVal)n forPrivateInputKey:(NSString *)k
{
    if( k == nil )
    {
        return;
    }
    [privateInputs rdlock];
    for( ISFAttrib *attrib in [privateInputs array] )
    {
        if( [[attrib attribName] isEqualToString:k] )
        {
            [attrib setCurrentVal:n];
            break;
        }
    }
    [privateInputs unlock];
}

- (void)setValue:(ISFAttribVal)n forInputKey:(NSString *)k
{
    if( k == nil )
    {
        return;
    }
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        if( [[attrib attribName] isEqualToString:k] )
        {
            [attrib setCurrentVal:n];
            break;
        }
    }
    [inputs unlock];
}

- (void)setNSObjectVal:(id)objectVal forInputKey:(NSString *)inputKey
{
    if( objectVal == nil || inputKey == nil )
        return;
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        if( [[attrib attribName] isEqualToString:inputKey] )
        {
            ISFAttribValType type = [attrib attribType];
            ISFAttribVal newVal;
            switch( type )
            {
            case ISFAT_Event:
                newVal.eventVal = [objectVal boolValue];
                [attrib setCurrentVal:newVal];
                break;
            case ISFAT_Bool:
                newVal.boolVal = [objectVal boolValue];
                [attrib setCurrentVal:newVal];
                break;
            case ISFAT_Long:
                newVal.longVal = [objectVal longValue];
                [attrib setCurrentVal:newVal];
                break;
            case ISFAT_Float:
                newVal.floatVal = [objectVal floatValue];
                [attrib setCurrentVal:newVal];
                break;
            case ISFAT_Point2D:
            {
                VVPOINT tmpPoint = [objectVal pointValue];
                newVal.point2DVal[0] = tmpPoint.x;
                newVal.point2DVal[1] = tmpPoint.y;
                [attrib setCurrentVal:newVal];
            }
            break;
            case ISFAT_Color:
            {
                CGFloat tmpVals[4];
                [objectVal getComponents:tmpVals];
                for( int i = 0; i < 4; ++i )
                {
                    newVal.colorVal[i] = tmpVals[i];
                }
                [attrib setCurrentVal:newVal];
                break;
            }
            case ISFAT_Image:
                [attrib setUserInfo:objectVal];
                break;
            default:
                break;
#warning mto-anomes: metal ignore
            case ISFAT_Cube:
                //                    [attrib setUserInfo:objectVal];
                break;
            case ISFAT_Audio:
                //                    [attrib setUserInfo:objectVal];
                break;
            case ISFAT_AudioFFT:
                //                    [attrib setUserInfo:objectVal];
                break;
            }
            break;
        }
    }
    [inputs unlock];
}

#pragma mark MISC

+ (NSMutableString *)_assembleShaderSource_VarDeclarationsFromModel:(MISFModel *)model
{
    NSMutableString *varDeclarations = [NSMutableString stringWithCapacity:0];
    //    first declare the variables for the various attributes
    for( ISFAttrib *attrib in model.inputs )
    {
        ISFAttribValType attribType = [attrib attribType];
        NSString *attribName = [attrib attribName];
        switch( attribType )
        {
        case ISFAT_Event:
            [varDeclarations appendString:VVFMTSTRING(@"uniform bool\t\t%@;\n", attribName)];
            break;
        case ISFAT_Bool:
            [varDeclarations appendString:VVFMTSTRING(@"uniform bool\t\t%@;\n", attribName)];
            break;
        case ISFAT_Long:
            [varDeclarations appendString:VVFMTSTRING(@"uniform int\t\t%@;\n", attribName)];
            break;
        case ISFAT_Float:
            [varDeclarations appendString:VVFMTSTRING(@"uniform float\t\t%@;\n", attribName)];
            break;
        case ISFAT_Point2D:
            [varDeclarations appendString:VVFMTSTRING(@"uniform vec2\t\t%@;\n", attribName)];
            break;
        case ISFAT_Color:
            [varDeclarations appendString:VVFMTSTRING(@"uniform vec4\t\t%@;\n", attribName)];
            break;
        case ISFAT_Audio:
        case ISFAT_AudioFFT:
        case ISFAT_Image: //    most of the voodoo happens here
            [varDeclarations
                appendString:VVFMTSTRING(@"uniform sampler2D\t\t%@;\n",
                                         attribName)]; // Was originally sampler2DRect, not handled by SpirV

#warning mto-anomes Metal ignore
            /*
            //    a vec4 describing the image rect IN NATIVE GL TEXTURE COORDS (2D is normalized, RECT is not)
            [varDeclarations appendString:VVFMTSTRING(@"uniform vec4\t\t_%@_imgRect;\n",attribName)];
            //    a vec2 describing the size in pixels of the image
            [varDeclarations appendString:VVFMTSTRING(@"uniform vec2\t\t_%@_imgSize;\n",attribName)];
            //    a bool describing whether the image in the texture should be flipped vertically
            [varDeclarations appendString:VVFMTSTRING(@"uniform bool\t\t_%@_flip;\n",attribName)];
             */
            break;
        case ISFAT_Cube:
#warning mto-anomes Metal ignore
            //    make a sampler for the cubemap texture
            // [varDeclarations appendString:VVFMTSTRING(@"uniform samplerCube\t\t%@;\n",attribName)];
            //    just pass in the imgSize
            // [varDeclarations appendString:VVFMTSTRING(@"uniform vec2\t\t_%@_imgSize;\n",attribName)];
            break;
        }
    }

    NSMutableArray<NSString *> *bufferNames = [NSMutableArray<NSString *> new];
    //    add the variables for the persistent buffers
    for( MISFModelBuffer *buffer in model.persistentBuffers )
    {
        [bufferNames addObject:buffer.name];
    }
    // Look out for duplicates !
    for( MISFModelPass *pass in model.passes )
    {
        if( [bufferNames indexOfObject:pass.targetBuffer.name] == NSNotFound )
        {
            [bufferNames addObject:pass.targetBuffer.name];
        }
    }
    for( NSString *bufferName in bufferNames )
    {
        [varDeclarations appendString:VVFMTSTRING(@"uniform sampler2D\t\t%@;\n", bufferName)];
    }
    for( MISFModelImportedImage *importedImage in model.importedImages )
    {
        [varDeclarations appendString:VVFMTSTRING(@"uniform sampler2D\t\t%@;\n", importedImage.name)];
    }
    //    [tempBufferArray unlock];
    // Built-ins are added in shaderConverter instead of here, so we can choose precisely where to put it in the code
    return varDeclarations;
}

- (NSMutableDictionary *)_assembleSubstitutionDict
{
    if( !bufferRequiresEval )
        return nil;
    NSMutableDictionary *returnMe = MUTDICT;
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        ISFAttribValType attribType = [attrib attribType];
        ISFAttribVal attribVal = [attrib currentVal];

        switch( attribType )
        {
        case ISFAT_Event:
            [returnMe setObject:((attribVal.eventVal) ? NUMFLOAT(1.0) : NUMFLOAT(0.0)) forKey:[attrib attribName]];
            break;
        case ISFAT_Bool:
            [returnMe setObject:((attribVal.boolVal) ? NUMFLOAT(1.0) : NUMFLOAT(0.0)) forKey:[attrib attribName]];
            break;
        case ISFAT_Long:
            [returnMe setObject:NUMFLOAT(attribVal.longVal) forKey:[attrib attribName]];
            break;
        case ISFAT_Float:
            [returnMe setObject:NUMFLOAT(attribVal.floatVal) forKey:[attrib attribName]];
            break;
        case ISFAT_Point2D:
            break;
        case ISFAT_Color:
            break;
        case ISFAT_Image:
            break;
        case ISFAT_Cube:
            break;
        case ISFAT_Audio:
            break;
        case ISFAT_AudioFFT:
            break;
        }
    }
    [inputs unlock];
    return returnMe;
}

- (MISFTargetBuffer *)getBufferNamed:(NSString *)bufferName
{
    MISFTargetBuffer *targetBuffer = [persistentBuffers objectForKey:bufferName];
    // If unlucky, look in temp buffers
    if( targetBuffer == nil )
    {
        targetBuffer = [tempBuffers objectForKey:bufferName];
    }
    // Return might be nil
    return targetBuffer;
}

#pragma mark - Model getters

- (NSString *)filePath
{
    return preloadedMedia.model.parentModel.filePath;
}

- (NSString *)fileName
{
    return preloadedMedia.model.parentModel.fileName;
}

- (NSString *)fileDescription
{
    return preloadedMedia.model.parentModel.fileDescription;
}

- (NSString *)fileCredits
{
    return preloadedMedia.model.parentModel.credits;
}

- (NSArray<NSString *> *)categoryNames
{
    return preloadedMedia.model.parentModel.categoryNames;
}

- (NSString *)jsonString
{
    return preloadedMedia.model.parentModel.jsonString;
}

- (NSString *)vertShaderSource
{
    return preloadedMedia.model.parentModel.vertShaderSource;
}

- (NSString *)fragShaderSource
{
    return preloadedMedia.model.parentModel.fragShaderSource;
}

- (NSArray<NSString *> *)passTargetNames
{
    return [[preloadedMedia.model.parentModel.passes valueForKey:@"targetBuffer"] valueForKey:@"name"];
}

- (ISFFunctionality)fileFunctionality
{
    return preloadedMedia.model.parentModel.fileFunctionality;
}

- (int)passCount
{
    if( passes == nil )
    {
        return 0;
    }
    return (int)[passes count];
}

#warning mto-anomes: mocked
// This just doesnt make sense in Metal API, because user gives the texture in which to draw, and the scene adapts its
// size according to it
- (VVSIZE)renderSize
{
    return VVMAKESIZE(0, 0);
}

- (NSMutableArray *)inputsOfType:(ISFAttribValType)t
{
    NSMutableArray *returnMe = MUTARRAY;
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        if( [attrib attribType] == t )
            [returnMe addObject:attrib];
    }
    [inputs unlock];
    return returnMe;
}
- (ISFAttrib *)attribForInputWithKey:(NSString *)k
{
    if( k == nil )
        return nil;
    ISFAttrib *returnMe = nil;
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        if( [[attrib attribName] isEqualToString:k] )
        {
            returnMe = attrib;
            break;
        }
    }
    [inputs unlock];
    if( returnMe != nil )
        [returnMe retain];
    return [returnMe autorelease];
}

@end
