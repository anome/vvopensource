#import "AppDelegate.h"
#import <MetalKit/MetalKit.h>
#import <OpenGL/CGLMacro.h>

@implementation AppDelegate
{
    id<MTLCommandQueue> commandQueue;
    id<MTLTexture> screenTexture;
    id<MTLTexture> inputImage;
    id<MTLTexture> secondInputImage;
    int passIndex;
}

- (id)init
{
    if( self = [super init] )
    {
        passIndex = 0;
        return self;
    }
    [self release];
    return nil;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *imageUrl = [bundle URLForResource:@"isfLogo" withExtension:@"jpg"];
    NSURL *secondImageUrl = [bundle URLForResource:@"isfLogoBW" withExtension:@"jpg"];
    inputImage = [[self loadTextureUsingMetalKit:imageUrl device:metalImageView.device] retain];
    secondInputImage = [[self loadTextureUsingMetalKit:secondImageUrl device:metalImageView.device] retain];

    NSString *fileName = @"ISFSupportTest";

//    fileName = @"multipass-eval";
//    fileName = @"multipass-eval-texture";
//    fileName = @"multipass";
//    fileName = @"multipasstexture";
//    fileName = @"textureimport";
//    fileName = @"variables";
//    fileName = @"lazytextureimport";
//    fileName = @"multipleImageInputs";
//    fileName = @"varying";
    fileName = @"emptyMain";

//    fileName = @"nonExistingFile";  // NSCocoaErrorDomain 258 The File name is invalid
//    fileName = @"errorBadJsonBlob"; // NSCocoaErrorDomain invalid value around character 53
//    fileName = @"errorImportedImageNoName";
//    fileName = @"errorImportedImagepathNotAString";
//    fileName = @"errorSpirVfnRedefined";

//    fileName = @"Auto-rotate";
//    fileName = @"3D Rotate";
//    fileName = @"Edge Blur";
//    fileName = @"Multi Pass Gaussian Blur";
//    fileName = @"Fast Blur";
//    fileName = @"Bad TV Auto Scroll";
//    fileName = @"Edges";
//    fileName = @"Sharpen RGB";
//    fileName = @"Neon";
//    fileName = @"City Lights";
//    fileName = @"Stripes";
//    fileName = @"Seascape";
    fileName = @"Constellations_m";
//    fileName = @"Digital Clock";
//    fileName = @"Controlled Chaos";
//    fileName = @"Disc Spin";
//    fileName = @"Grid";
//    fileName = @"IQ_SmoothXOR";
//    fileName = @"Radial Gradient";
//    fileName = @"Stripes";
//    fileName = @"VoronoiLines_m";
//    fileName = @"Wisps";
//    fileName = @"Amatorka";
//    fileName = @"ASCII Art";
//    fileName = @"Dither-Bayer";
//    fileName = @"v002 Vignette";
//    fileName = @"Old Video";
//    fileName = @"Shift RGB";
//    fileName = @"Borders around Alpha";
//    fileName = @"v002 Crosshatch";
//    fileName = @"Pixelize";
//    fileName = @"Dither BW";
//    fileName = @"Round Corner";
//    fileName = @"Bump Distortion_m";
//    fileName = @"CMYK Halftone-Lookaround";
//    fileName = @"RGB Trails";
//    fileName = @"VHS Glitch";
//    fileName = @"Time Glitch RGB_m";
//    fileName = @"v002 Technicolor";
//    fileName = @"Exposure Adjust";
//    fileName = @"False Color";
//    fileName = @"Color Monochrome";
//    fileName = @"Color Posterize";
//    fileName = @"MissEtikate";
//    fileName = @"v002 Bleach Bypass";
//    fileName = @"Vibrance";
//    fileName = @"Zoom Blur";
//    fileName = @"Dilate-Fast";
//    fileName = @"Erode-Fast";
//    fileName = @"VVMotionBlur";
//    fileName = @"Bokeh Disc+";
//    fileName = @"Auto-move";
//    fileName = @"Delay";
//    fileName = @"Auto-color";
//    fileName = @"Auto-scale";
//    fileName = @"Quake";
//    fileName = @"Freeze Frame_m";
//    fileName = @"Black & White";
//    fileName = @"Auto-scale";
//    fileName = @"Auto-color";
//    fileName = @"Bokeh Disc+";

    NSString *filePath = [bundle pathForResource:fileName ofType:@"fs"];

    // Preload API
    {
        NSError *preloadError;
        MISFPreloadedMedia *preloadedModel = [ISFMetalScene preloadFile:filePath
                                                               onDevice:metalImageView.device
                                                              withError:&preloadError];
        if( preloadedModel == nil )
        {
            NSLog(@"PRELOAD ERROR ! %@", preloadError);
        }
        NSError *error;
        isfScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                             pixelFormat:metalImageView.colorPixelFormat
                                          preloadedMedia:preloadedModel
                                               withError:&error];
        if( isfScene == nil )
        {
            NSLog(@"ERROR ! %@", error);
        }
    }

    // Classic API
    NSError *error;
    isfScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                         pixelFormat:metalImageView.colorPixelFormat
                                    fragmentFilePath:filePath
                                           withError:&error];
    if( isfScene == nil )
    {
        NSLog(@"ERROR ! %@", error);
    }

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
}
//	this method is called from the displaylink callback
- (void)renderCallback
{
    passIndex += 1;

    if( commandQueue == nil )
    {
        commandQueue = [metalImageView.device newCommandQueue];
    }
    if( screenTexture == nil )
    {
#warning mto-anomes: currently, if this resolution is not exactly the same as inputImage, texture sampling is not working correctly
        screenTexture = [self createTextureForDevice:metalImageView.device
                                               width:1280
                                              height:720
                                         pixelFormat:metalImageView.colorPixelFormat];
    }

    /// ISF
    //    {
    //        ISFAttribVal val;
    //           val.floatVal = sliderOne.floatValue / 100;
    //           [isfScene setValue:val forInputKey:@"xrot"];
    //    }
    //    {
    //        ISFAttribVal val;
    //           val.floatVal = sliderTwo.floatValue / 100;
    //           [isfScene setValue:val forInputKey:@"yrot"];
    //    }
    //    {
    //        ISFAttribVal val;
    //           val.floatVal = sliderThree.floatValue / 100;
    //           [isfScene setValue:val forInputKey:@"zrot"];
    //    }
    //    {
    //        ISFAttribVal val;
    //           val.floatVal = sliderFour.floatValue / 100;
    //           [isfScene setValue:val forInputKey:@"zoom"];
    //    }
    //        {
    //            ISFAttribVal val;
    //               val.floatVal = sliderThree.floatValue;
    //               [isfScene setValue:val forInputKey:@"blurAmount"];
    //        }
    //

    isfScene.choosePassIndex = sliderTwo.intValue; // Disabled if you don't enable it manually

    {
        ISFAttribVal val;
        val.floatVal = sliderOne.floatValue;
        // Two options to send an input
        //        [isfScene setValue:val forInputKey:@"width"];
        [isfScene setNSObjectVal:[NSNumber numberWithFloat:sliderOne.floatValue / 100] forInputKey:@"width"];
    }

    {
        ISFAttribVal imageVal;
        imageVal.metalImageVal = inputImage;
        [isfScene setValue:imageVal forInputKey:@"inputImage"];
    }

    /// RENDER
    id<MTLCommandBuffer> commandBuffer = [commandQueue commandBuffer];
    commandBuffer.label = @"Metal ISF Test App Command Buffer";

    NSError *renderError;
    BOOL success = [isfScene renderOnTexture:screenTexture onCommandBuffer:commandBuffer withError:&renderError];
    if( !success )
    {
        NSLog(@"Render error %@", renderError);
    }
    [commandBuffer commit];
    [commandBuffer waitUntilCompleted];
    metalImageView.image = screenTexture;
    [[NSOperationQueue mainQueue] addOperationWithBlock:^{
      [metalImageView setNeedsDisplay:YES];
    }];
}

#pragma mark Pure utils

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
    textureDescriptor.storageMode = MTLStorageModePrivate; // GPU only for better performance
    id<MTLTexture> texture = [theDevice newTextureWithDescriptor:textureDescriptor];
    return texture;
}

- (id<MTLTexture>)loadTextureUsingMetalKit:(NSURL *)url device:(id<MTLDevice>)device
{
    MTKTextureLoader *loader = [[MTKTextureLoader alloc] initWithDevice:device];
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

@end

CVReturn displayLinkCallback(CVDisplayLinkRef displayLink, const CVTimeStamp *inNow, const CVTimeStamp *inOutputTime,
                             CVOptionFlags flagsIn, CVOptionFlags *flagsOut, void *displayLinkContext)
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
    [(AppDelegate *)displayLinkContext renderCallback];
    [pool release];
    return kCVReturnSuccess;
}
