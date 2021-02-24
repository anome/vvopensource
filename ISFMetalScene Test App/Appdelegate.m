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
    ISFMetalScene *isfScene;
    NSString *shaderFileToRender;
}

- (id)init
{
    if( self = [super init] )
    {
        passIndex = 0;
        shaderFileToRender = @"Auto-rotate";
        return self;
    }
    [self release];
    return nil;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    NSArray<NSString *> *shaderFiles = @[
        /// Generators
        @"Stripes",
        @"Seascape",
        @"Constellations_m",
        @"Digital Clock",
        @"Controlled Chaos",
        @"Disc Spin",
        @"Grid",
        @"IQ_SmoothXOR",
        @"Radial Gradient",
        @"Stripes",
        @"VoronoiLines_m",
        @"Wisps",
        /// Effects
        @"Auto-rotate",
        @"3D Rotate",
        @"Edge Blur",
        @"Multi Pass Gaussian Blur",
        @"Fast Blur",
        @"Bad TV Auto Scroll",
        @"Edges",
        @"Sharpen RGB",
        @"Neon",
        @"City Lights",
        @"Amatorka",
        @"ASCII Art",
        @"Dither-Bayer",
        @"v002 Vignette",
        @"Old Video",
        @"Shift RGB",
        @"Borders around Alpha",
        @"v002 Crosshatch",
        @"Pixelize",
        @"Dither BW",
        @"Round Corner",
        @"Bump Distortion_m",
        @"CMYK Halftone-Lookaround",
        @"RGB Trails",
        @"VHS Glitch",
        @"Time Glitch RGB_m",
        @"v002 Technicolor",
        @"Exposure Adjust",
        @"False Color",
        @"Color Monochrome",
        @"Color Posterize",
        @"MissEtikate",
        @"v002 Bleach Bypass",
        @"Vibrance",
        @"Zoom Blur",
        @"Dilate-Fast",
        @"Erode-Fast",
        @"VVMotionBlur",
        @"Bokeh Disc+",
        @"Auto-move",
        @"Delay",
        @"Auto-color",
        @"Auto-scale",
        @"Quake",
        @"Freeze Frame_m",
        @"Black & White",
        @"Auto-scale",
        @"Auto-color",
        @"Bokeh Disc+",
        /// Tests
        @"emptyMain",
        @"variables",
        @"multipass-eval",
        @"multipass-eval-texture",
        @"multipass",
        @"multipasstexture",
        @"textureimport",
        @"lazytextureimport",
        @"multipleImageInputs",
        @"varying",
        @"nonExistingFile",  // NSCocoaErrorDomain 258 The File name is invalid
        @"errorBadJsonBlob", // NSCocoaErrorDomain invalid value around character 53
        @"errorImportedImageNoName",
        @"errorImportedImagepathNotAString",
        @"errorSpirVfnRedefined",
    ];
    for( NSString *file in shaderFiles )
    {
        [shaderSourceButton addItemWithTitle:file];
    }
    [shaderSourceButton selectItemWithTitle:shaderFileToRender];

    NSBundle *bundle = [NSBundle mainBundle];
    NSURL *imageUrl = [bundle URLForResource:@"inputImage" withExtension:@"jpg"];
    NSURL *secondImageUrl = [bundle URLForResource:@"inputImage" withExtension:@"jpg"];
    inputImage = [[self loadTextureUsingMetalKit:imageUrl device:metalImageView.device] retain];
    secondInputImage = [[self loadTextureUsingMetalKit:secondImageUrl device:metalImageView.device] retain];

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
}
//	this method is called from the displaylink callback
- (void)renderCallback
{
    passIndex += 1;

    if( isfScene == nil )
    {
        return;
    }
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
        NSLog(@"RENDER ERROR %@", renderError);
    }
    [commandBuffer commit];
    [commandBuffer waitUntilCompleted];
    metalImageView.image = screenTexture;
    [[NSOperationQueue mainQueue] addOperationWithBlock:^{
      [metalImageView setNeedsDisplay:YES];
    }];
}

- (void)loadIsfScene
{
    NSBundle *bundle = [NSBundle mainBundle];
    NSString *filePath = [bundle pathForResource:shaderFileToRender ofType:@"fs"];

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
        isfScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                             pixelFormat:metalImageView.colorPixelFormat
                                          preloadedMedia:preloadedModel
                                               withError:&error];
        if( isfScene == nil )
        {
            NSLog(@"ERROR ! %@", error);
            return;
        }
    }

    // Classic API
    /*
        NSError *error;
        isfScene = [[ISFMetalScene alloc] initWithDevice:metalImageView.device
                                             pixelFormat:metalImageView.colorPixelFormat
                                        fragmentFilePath:filePath
                                               withError:&error];
        if( isfScene == nil )
        {
            NSLog(@"ERROR ! %@", error);
        }
     */
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

- (IBAction)onShaderSourceButtonClicked:(id)sender
{

    shaderFileToRender = shaderSourceButton.selectedItem.title;
    isfScene = nil;
    [self loadIsfScene];
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
