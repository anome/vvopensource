#import "AppDelegate.h"
#import "CheckboxView.h"
#import "SliderView.h"
#import <CoreImage/CoreImage.h>
#import <IOSurface/IOSurface.h>
#import <MetalKit/MetalKit.h>
#import <OpenGL/CGLMacro.h>

#define RENDER_RES_WIDTH 1280
#define RENDER_RES_HEIGHT 720

#define ISF_EXPORT_GL_PATH @"/tmp/gl.tiff"
#define ISF_EXPORT_METAL_PATH @"/tmp/metal.tiff"

@implementation AppDelegate
{
    // METAL
    id<MTLCommandQueue> commandQueue;
    id<MTLTexture> screenTexture;
    id<MTLTexture> inputImage;
    id<MTLTexture> inputImage2;
    ISFMetalScene *metalScene;
    int passIndex;

    // GL
    VVBuffer *glImageBuffer; //    this VVBuffer is created from a PNG file included with this application
    VVBuffer *glImageBuffer2;
    ISFGLScene *glScene;   //    tell this to load an ISF file and it renders buffers/textures
    VVStopwatch *glSwatch; //    used to animate the size of the checkerboard
    NSOpenGLContext *sharedContext;

    // COMMON
    NSString *shaderFileKeyToRender;
    NSMutableDictionary<NSString *, NSURL *> *shaderFiles;
    NSMutableArray<NSString *> *shaderKeys;
    CVDisplayLinkRef displayLink;

    // GL/METAL VISUAL COMPARISON STUFF
    IOSurfaceRef screenTextureSurfaceRef;
    BOOL shouldExportNextFrame;
    int frameExportCount;
}

- (id)init
{
    if( self = [super init] )
    {
        passIndex = 0;
        shaderFileKeyToRender = @"z_white.fs";

        /// GL INIT
        //    make a shared GL context.  other GL contexts created to share this one may share resources (textures,
        //    buffers, etc).
        NSLog(@"GL Pixel format: %@", [GLScene defaultPixelFormat]);
        sharedContext = [[NSOpenGLContext alloc] initWithFormat:[GLScene defaultPixelFormat] shareContext:nil];
        //    create the global buffer pool from the shared context
        [VVBufferPool createGlobalVVBufferPoolWithSharedContext:sharedContext];

        glSwatch = [[VVStopwatch alloc] init];
        [glSwatch start];

        shouldExportNextFrame = NO;
        frameExportCount = 0;
        return self;
    }
    [self release];
    return nil;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    shaderFiles = [NSMutableDictionary<NSString *, NSURL *> new];
    shaderKeys = [NSMutableArray<NSString *> new];
    NSBundle *bundle = [NSBundle mainBundle];

    NSArray<NSURL *> *someShaderUrls = [bundle URLsForResourcesWithExtension:@"fs" subdirectory:@"isfLibrary"];

    NSArray<NSURL *> *moreShaderUrls = [bundle URLsForResourcesWithExtension:@"fs" subdirectory:@"working"];

    NSArray<NSURL *> *shaderUrls = [someShaderUrls arrayByAddingObjectsFromArray:moreShaderUrls];
    NSArray<NSURL *> *shaderUrlsAlphabetically =
        [shaderUrls sortedArrayUsingComparator:^NSComparisonResult(NSURL *url1, NSURL *url2) {
          NSString *filename1 = [url1 lastPathComponent];
          NSString *filename2 = [url2 lastPathComponent];
          return [filename1 localizedCaseInsensitiveCompare:filename2];
        }];
    for( NSURL *fileUrl in shaderUrlsAlphabetically )
    {
        NSString *fileName = [fileUrl lastPathComponent];
        [shaderFiles setValue:fileUrl forKey:fileName];
        [shaderKeys addObject:fileName];
        [shaderSourceButton addItemWithTitle:fileName];
    }

    [shaderSourceButton selectItemWithTitle:shaderFileKeyToRender];

    //    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *imageUrl = [bundle URLForResource:@"inputImage" withExtension:@"jpg"];
    inputImage = [[self loadTextureUsingMetalKit:imageUrl device:metalImageView.device] retain];

    //    load the image buffer included with this app
    NSImage *tmpImg = [[NSImage alloc] initWithContentsOfURL:imageUrl];
    glImageBuffer = [[VVBufferPool globalVVBufferPool] allocBufferForNSImage:tmpImg];
    [tmpImg release];
    tmpImg = nil;

    [self loadIsfScene];

    //	make the displaylink, which will drive rendering
    CVReturn err = kCVReturnSuccess;
    CGOpenGLDisplayMask totalDisplayMask = 0;
    GLint virtualScreen = 0;
    GLint displayMask = 0;
    NSOpenGLPixelFormat *format = [GLScene defaultPixelFormat];

    for( virtualScreen = 0; virtualScreen < [format numberOfVirtualScreens]; ++virtualScreen )
    {
        [format getValues:&displayMask forAttribute:NSOpenGLPFAScreenMask forVirtualScreen:virtualScreen];
        totalDisplayMask |= displayMask;
    }

    err = CVDisplayLinkCreateWithOpenGLDisplayMask(totalDisplayMask, &displayLink);
    if( err )
    {
        NSLog(@"\t\terr %d creating display link in %s", err, __func__);
        displayLink = NULL;
    }
    else
    {
        CVDisplayLinkSetOutputCallback(displayLink, displayLinkCallback, self);
        CVDisplayLinkStart(displayLink);
    }

    controlsStackView.translatesAutoresizingMaskIntoConstraints = NO;
}

- (void)loadIsfScene
{
    NSURL *fileUrl = [shaderFiles objectForKey:shaderFileKeyToRender];
    NSString *filePath = fileUrl.path;

    // Preload API
    {
        NSError *preloadError;
        MISFPreloadedMedia *preloadedModel = [ISFMetalScene preloadFile:filePath
                                                               onDevice:metalImageView.device
                                                              withError:&preloadError];
        if( preloadedModel == nil )
        {
            NSLog(@"PRELOAD ERROR ! %@", preloadError);
            return;
        }
        NSError *error;
        metalScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                               pixelFormat:metalImageView.colorPixelFormat
                                            preloadedMedia:preloadedModel
                                                 withError:&error];
        if( metalScene == nil )
        {
            NSLog(@"ERROR ! %@", error);
            return;
        }
    }

    // Classic API
    /*
        NSError *error;
        metalScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                             pixelFormat:metalImageView.colorPixelFormat
                                        fragmentFilePath:filePath
                                               withError:&error];
        if( metalScene == nil )
        {
            NSLog(@"ERROR ! %@", error);
        }
     */

    //// GL
    glScene = [[ISFGLScene alloc] initWithSharedContext:sharedContext];
    glScene.throwExceptions = YES;
    [glScene setSize:NSMakeSize(RENDER_RES_WIDTH, RENDER_RES_HEIGHT)];
    @try
    {
        [glScene useFile:filePath];
    }
    @catch( NSException *e )
    {
        NSLog(@"GL useFile crashed: %@", e);
    }

    //// Generate Controls UI
    [[controlsStackView views] enumerateObjectsUsingBlock:^(NSView *obj, NSUInteger idx, BOOL *stop) {
      [controlsStackView removeView:obj];
    }];
    MutLockArray *inputs = metalScene.inputs;
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        NSString *attribName = attrib.attribName;
        ISFAttribValType attribType = attrib.attribType;

        const ISFAttribVal defaultVal = [attrib defaultVal];
        const ISFAttribVal minVal = [attrib minVal];
        const ISFAttribVal maxVal = [attrib maxVal];
        switch( attribType )
        {
        case ISFAT_Float:
        {
            NSRect frame = NSMakeRect(0, 0, 200, 50);
            SliderView *slider = [[SliderView alloc] initWithFrame:frame
                                                              name:attribName
                                                            minVal:minVal.floatVal
                                                        defaultVal:defaultVal.floatVal
                                                            maxVal:maxVal.floatVal];
            slider.onChange = ^(float value) {
              ISFAttribVal val;
              val.floatVal = value;
              [metalScene setValue:val forInputKey:attribName];
              [glScene setValue:val forInputKey:attribName];
            };
            [slider awakeFromNib];
            [controlsStackView addArrangedSubview:slider];
            break;
        }
        case ISFAT_Bool:
        {
            NSRect frame = NSMakeRect(0, 0, 200, 50);
            CheckboxView *checkbox = [[CheckboxView alloc] initWithFrame:frame
                                                                    name:attribName
                                                              defaultVal:defaultVal.boolVal];
            checkbox.onChange = ^(BOOL value) {
              ISFAttribVal val;
              val.boolVal = value;
              [metalScene setValue:val forInputKey:attribName];
              [glScene setValue:val forInputKey:attribName];
            };
            [checkbox awakeFromNib];
            [controlsStackView addArrangedSubview:checkbox];
            break;
        }
        case ISFAT_Long:
        {
            NSRect frame = NSMakeRect(0, 0, 200, 50);
            SliderView *slider = [[SliderView alloc] initWithFrame:frame
                                                              name:attribName
                                                            minVal:minVal.longVal
                                                        defaultVal:defaultVal.longVal
                                                            maxVal:maxVal.longVal];
            slider.onChange = ^(float value) {
              ISFAttribVal val;
              val.longVal = floor(value);
              [metalScene setValue:val forInputKey:attribName];
              [glScene setValue:val forInputKey:attribName];
            };
            [slider awakeFromNib];
            [controlsStackView addArrangedSubview:slider];
            break;
        }
        case ISFAT_Color:
        {
            NSArray *channelNames = @[ @"Red", @"Green", @"Blue", @"Alpha" ];
            for( int i = 0; i < 4; i++ )
            {
                NSString *sliderName = [[NSString stringWithFormat:@"%@ [%@]", attribName, channelNames[i]] retain];
                NSRect frame = NSMakeRect(0, 0, 200, 50);
                SliderView *slider = [[SliderView alloc] initWithFrame:frame
                                                                  name:sliderName
                                                                minVal:minVal.colorVal[i]
                                                            defaultVal:defaultVal.colorVal[i]
                                                                maxVal:maxVal.colorVal[i]];
                slider.onChange = ^(float value) {
                  ISFAttribVal val;
                  GLfloat *currentVal = [attrib currentVal].colorVal;
                  val.colorVal[0] = i == 0 ? value : currentVal[0];
                  val.colorVal[1] = i == 1 ? value : currentVal[1];
                  val.colorVal[2] = i == 2 ? value : currentVal[2];
                  val.colorVal[3] = i == 3 ? value : currentVal[3];
                  [metalScene setValue:val forInputKey:attribName];
                  [glScene setValue:val forInputKey:attribName];
                };
                [slider awakeFromNib];
                [controlsStackView addArrangedSubview:slider];
            }
            break;
        }
        case ISFAT_Point2D:
        {
            NSArray *channelNames = @[ @"X", @"Y" ];
            for( int i = 0; i < 2; i++ )
            {
                NSString *sliderName = [[NSString stringWithFormat:@"%@ [%@]", attribName, channelNames[i]] retain];
                NSRect frame = NSMakeRect(0, 0, 200, 50);
                SliderView *slider = [[SliderView alloc] initWithFrame:frame
                                                                  name:sliderName
                                                                minVal:-1.
                                                            defaultVal:0.
                                                                maxVal:1.];
                slider.onChange = ^(float value) {
                  ISFAttribVal val;
                  GLfloat *currentVal = [attrib currentVal].point2DVal;
                  val.point2DVal[0] = i == 0 ? value : currentVal[0];
                  val.point2DVal[1] = i == 1 ? value : currentVal[1];
                  [metalScene setValue:val forInputKey:attribName];
                  [glScene setValue:val forInputKey:attribName];
                };
                [slider awakeFromNib];
                [controlsStackView addArrangedSubview:slider];
            }
        }
        default:
        {
            NSLog(@"WARN: attrib type %lu not handled", attribType);
            break;
        }
        }
    }
    [inputs unlock];
}

- (IBAction)onShaderSourceButtonClicked:(id)sender
{
    NSString *fileKey = shaderSourceButton.selectedItem.title;
    shaderFileKeyToRender = fileKey;
    metalScene = nil;
    [self loadIsfScene];
}

- (IBAction)onButtonPreviousClicked:(id)sender
{
    NSString *fileKey = shaderSourceButton.selectedItem.title;
    NSUInteger currentIndex = [shaderKeys indexOfObject:fileKey];
    if( currentIndex == 0 )
    {
        return;
    }
    shaderFileKeyToRender = shaderKeys[currentIndex - 1];
    [shaderSourceButton selectItemAtIndex:currentIndex - 1];
    metalScene = nil;
    [self loadIsfScene];
}
- (IBAction)onButtonNextClicked:(id)sender
{
    NSString *fileKey = shaderSourceButton.selectedItem.title;
    NSUInteger currentIndex = [shaderKeys indexOfObject:fileKey];
    if( currentIndex + 1 == shaderKeys.count )
    {
        return;
    }
    shaderFileKeyToRender = shaderKeys[currentIndex + 1];
    [shaderSourceButton selectItemAtIndex:currentIndex + 1];
    metalScene = nil;
    [self loadIsfScene];
}

- (IBAction)onExportFrameClicked:(id)sender
{
    shouldExportNextFrame = YES;
}

//    this method is called from the displaylink callback
- (void)glAndMetalrenderCallback
{
    /// METAL RENDERING
    passIndex += 1;

    // Debug tool to explore passes one by one (for metal only) - to enable manually in ISF Framework
    metalScene.choosePassIndex = sliderExplorePasses.intValue;

    if( metalScene == nil )
    {
        return;
    }
    if( commandQueue == nil )
    {
        commandQueue = [metalImageView.device newCommandQueue];
    }
    if( screenTexture == nil )

    {
        // WIP: IO Surface backed mode
        /*
        // init our texture and IOSurface
                NSDictionary<NSString *, id> *surfaceAttributes = @{(NSString*)kIOSurfaceIsGlobal: @(YES),
                                                                    (NSString*)kIOSurfaceWidth: @(RENDER_RES_WIDTH),
                                                                    (NSString*)kIOSurfaceHeight: @(RENDER_RES_HEIGHT),
                                                                    (NSString*)kIOSurfacePixelFormat: @(80),
                                                                    (NSString*)kIOSurfacePlaneBase: @(0),
                                                                    (NSString*)kIOSurfaceBytesPerElement: @(4U)};
                screenTextureSurfaceRef = IOSurfaceCreate((CFDictionaryRef) surfaceAttributes);
         */
        screenTexture = [self createTextureForDevice:metalImageView.device
                                               width:RENDER_RES_WIDTH
                                              height:RENDER_RES_HEIGHT
                                         pixelFormat:metalImageView.colorPixelFormat];
        //                         ioSurface:screenTextureSurfaceRef];
    }

    {
        [metalScene setNSObjectVal:inputImage forInputKey:@"inputImage"];
        // TODO: nothing to do here?
        [glScene setFilterInputImageBuffer:glImageBuffer];
    }

    /// RENDER
    id<MTLCommandBuffer> commandBuffer = [commandQueue commandBuffer];
    commandBuffer.label = @"Metal ISF Test App Command Buffer";

    NSError *renderError;
    BOOL success = [metalScene renderOnTexture:screenTexture onCommandBuffer:commandBuffer withError:&renderError];

    if( !success )
    {
        NSLog(@"RENDER ERROR %@", renderError);
    }

    // Needed to save frame

    if( shouldExportNextFrame )
    {
        id<MTLBlitCommandEncoder> syncRenderEncoder = [commandBuffer blitCommandEncoder];
        syncRenderEncoder.label = @"Sync Encoder";
        [syncRenderEncoder synchronizeTexture:screenTexture slice:0 level:0];
        [syncRenderEncoder endEncoding];
    }

    [commandBuffer commit];
    [commandBuffer waitUntilCompleted];
    metalImageView.image = screenTexture;
    CIImage *metalCiImage = [CIImage imageWithMTLTexture:screenTexture options:nil];
    NSCIImageRep *metalRepresentation = [NSCIImageRep imageRepWithCIImage:metalCiImage];
    NSImage *metalNsImage = [[NSImage alloc] initWithSize:metalRepresentation.size];
    [metalNsImage addRepresentation:metalRepresentation];

    [[NSOperationQueue mainQueue] addOperationWithBlock:^{
      [metalImageView setNeedsDisplay:YES];
    }];

    /// GL RENDERING
    @try
    {
        //    tell the ISF scene to render a buffer (this renders to a GL texture)
        VVBuffer *newTex = [glScene allocAndRenderABuffer];
        //    draw the GL texture i just rendered in the buffer view
        [glBufferView drawBuffer:newTex];

        if( shouldExportNextFrame )
        {
            VVBuffer *copyBuffer =
                [[VVBufferPool globalVVBufferPool] allocBufferForTexBackedIOSurfaceSized:newTex.size];
            BOOL success = [[VVBufferCopier globalBufferCopier] copyThisBuffer:newTex toThisBuffer:copyBuffer];
            if( !success )
            {
                NSLog(@"ERROR: could not copy gl frame.");
                shouldExportNextFrame = NO;
            }
            else
            {
                IOSurfaceRef ref = [copyBuffer localSurfaceRef];
                CIImage *ciImage = [CIImage imageWithIOSurface:ref];
                NSCIImageRep *rep = [NSCIImageRep imageRepWithCIImage:ciImage];
                NSImage *nsImage = [[NSImage alloc] initWithSize:rep.size];
                [nsImage addRepresentation:rep];
                [self saveImage:nsImage withName:ISF_EXPORT_GL_PATH];

                // Metal export
                id<MTLTexture> lastDrawableDisplayed = metalImageView.image;

                int width = (int)[lastDrawableDisplayed width];
                int height = (int)[lastDrawableDisplayed height];
                int rowBytes = width * 4;
                int selfturesize = width * height * 4;

                void *p = malloc(selfturesize);

                [lastDrawableDisplayed getBytes:p
                                    bytesPerRow:rowBytes
                                     fromRegion:MTLRegionMake2D(0, 0, width, height)
                                    mipmapLevel:0];

                CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
                CGBitmapInfo bitmapInfo = kCGBitmapByteOrder32Little | kCGImageAlphaFirst;

                CGDataProviderRef provider = CGDataProviderCreateWithData(nil, p, selfturesize, nil);
                CGImageRef cgImageRef = CGImageCreate(width, height, 8, 32, rowBytes, colorSpace, bitmapInfo, provider,
                                                      nil, true, (CGColorRenderingIntent)kCGRenderingIntentDefault);

                NSImage *getImage = [[NSImage alloc] initWithCGImage:cgImageRef size:NSMakeSize(width, height)];
                [self saveImage:getImage withName:ISF_EXPORT_METAL_PATH];
                CFRelease(cgImageRef);
                free(p);

                [self compareImagesWithImageMagick];
                shouldExportNextFrame = NO;
                frameExportCount++;
            }
        }

        VVRELEASE(newTex);
        //    tell the buffer pool to do its housekeeping (releases any "old" resources in the pool that have been
        //    sticking around for a while)
        [[VVBufferPool globalVVBufferPool] housekeeping];
    }
    @catch( NSException *e )
    {
        NSLog(@"GL render crashed: %@", e);
    }
}

#pragma mark Pure utils

// Don't expext this to work if you dont have imagemagick on your machine
- (void)compareImagesWithImageMagick
{
    //    compare -metric MSE a.jpg b.jpg /dev/null

    NSTask *transpileTask = [[NSTask alloc] init];

    NSString *binaryPath = @"/usr/local/bin/compare";
    NSString *bashPath = @"/bin/bash";
    transpileTask.launchPath = binaryPath;
    NSArray *taskArguments =
        [NSArray arrayWithObjects:@"-metric", @"MSE", ISF_EXPORT_GL_PATH, ISF_EXPORT_METAL_PATH, @"/dev/null", nil];
    [transpileTask setArguments:taskArguments];

    NSPipe *outputPipe = [NSPipe pipe];
    [transpileTask setStandardOutput:outputPipe];

    NSPipe *errorPipe = [NSPipe pipe];
    [transpileTask setStandardError:errorPipe];

    [transpileTask launch];
    NSData *data = [[outputPipe fileHandleForReading] readDataToEndOfFile];
    NSString *result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    NSLog(@"Comparison result: %@", result);
    NSData *dataErr = [[errorPipe fileHandleForReading] readDataToEndOfFile];
    NSString *resultErr = [[NSString alloc] initWithData:dataErr encoding:NSUTF8StringEncoding];
    NSLog(@"Comparison error? : %@", resultErr);
    [transpileTask waitUntilExit];
}

- (id<MTLTexture>)createTextureForDevice:(id<MTLDevice>)theDevice
                                   width:(int)width
                                  height:(int)height
                             pixelFormat:(MTLPixelFormat)thePixelFormat
{
    MTLTextureDescriptor *textureDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:thePixelFormat
                                                                                                 width:width
                                                                                                height:height
                                                                                             mipmapped:NO];
    textureDescriptor.usage = MTLTextureUsageRenderTarget | MTLTextureUsageShaderRead;
    textureDescriptor.storageMode = MTLStorageModeManaged; // GPU only for better performance
    id<MTLTexture> texture = [theDevice newTextureWithDescriptor:textureDescriptor];
    return texture;
}

- (id<MTLTexture>)createTextureForDevice:(id<MTLDevice>)theDevice
                                   width:(int)width
                                  height:(int)height
                             pixelFormat:(MTLPixelFormat)thePixelFormat
                               ioSurface:(IOSurfaceRef)ioSurface
{
    MTLTextureDescriptor *textureDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:thePixelFormat
                                                                                                 width:width
                                                                                                height:height
                                                                                             mipmapped:NO];
    textureDescriptor.usage = MTLTextureUsageRenderTarget | MTLTextureUsageShaderRead;
    textureDescriptor.storageMode = MTLStorageModeManaged; // GPU only for better performance
    id<MTLTexture> texture = [theDevice newTextureWithDescriptor:textureDescriptor iosurface:ioSurface plane:0];
    return texture;
}

- (id<MTLTexture>)loadTextureUsingMetalKit:(NSURL *)url device:(id<MTLDevice>)device
{
    MTKTextureLoader *loader = [[MTKTextureLoader alloc] initWithDevice:device];
    // bottom left origin for jpg
    NSDictionary<MTKTextureLoaderOption, id> *options =
        @{MTKTextureLoaderOptionSRGB : @NO, MTKTextureLoaderOptionOrigin : MTKTextureLoaderOriginBottomLeft};
    id<MTLTexture> texture = [loader newTextureWithContentsOfURL:url options:options error:nil];

    if( !texture )
    {
        NSLog(@"Failed to create the texture from %@", url.absoluteString);
        return nil;
    }
    return texture;
}

- (void)saveImage:(NSImage *)image withName:(NSString *)fileName
{
    NSData *imageData = [image TIFFRepresentation];
    NSBitmapImageRep *imageRep = [NSBitmapImageRep imageRepWithData:imageData];
    imageData = [imageRep representationUsingType:NSBitmapImageFileTypeTIFF properties:@{}];
    [imageData writeToFile:fileName atomically:NO];
}

@end

CVReturn displayLinkCallback(CVDisplayLinkRef displayLink, const CVTimeStamp *inNow, const CVTimeStamp *inOutputTime,
                             CVOptionFlags flagsIn, CVOptionFlags *flagsOut, void *displayLinkContext)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    [(AppDelegate *)displayLinkContext glAndMetalrenderCallback];
    [pool release];

    return kCVReturnSuccess;
}
