
#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <MetalKit/MetalKit.h>
#import <VVISFKit/VVISFKit.h>
#import <XCTest/XCTest.h>

/// Automatically Generated
@interface __VVIFKit_Metal_Test___3419_Untitled_Sketch : XCTestCase

@end

@implementation __VVIFKit_Metal_Test___3419_Untitled_Sketch
{
    NSString *isfPath;
    NSBundle *bundle;
    MISFModel *model;
    id<MTLDevice> testDevice;
}

- (void)setUp
{
    bundle = [NSBundle bundleForClass:[self class]];
    
    isfPath = [bundle pathForResource:@"__3419_Untitled Sketch" ofType:@"fs" inDirectory:@"isfLibrary"];
    //[bundle pathsForResourcesOfType:@"fs" inDirectory:@"working"][0];
    testDevice = MTLCreateSystemDefaultDevice();
    self.continueAfterFailure = NO;
}

#pragma mark Tests

- (void)testLoad
{
    NSError *error;
    MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:isfPath onDevice:testDevice withError:&error];
    if( preloadedMedia == nil )
    {
        NSLog(@"failed to preload media for file: %@", isfPath);
        NSLog(@"error: %@", error);
    }
    XCTAssertNotNil(preloadedMedia, @"Preload Failed for file %@ output %@", isfPath, error);
}

- (void)testLoadAndRender
{
    NSError *error;
    MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:isfPath onDevice:testDevice withError:&error];
    if( preloadedMedia == nil )
    {
        NSLog(@"failed to preload media for file: %@", isfPath);
        NSLog(@"error: %@", error);
    }
    XCTAssertNotNil(preloadedMedia, @"Compilation Failed for file %@ output %@", isfPath, error);

    ISFMetalScene *isfScene = [[ISFMetalScene alloc] initWithDevice:testDevice
                                                        pixelFormat:MTLPixelFormatBGRA8Unorm
                                                     preloadedMedia:preloadedMedia
                                                          withError:&error];
    if( isfScene == nil )
    {
        NSLog(@"failed to create isfScene for file: %@", isfPath);
        NSLog(@"error: %@", error);
    }
    XCTAssertNotNil(isfScene, @"ISF Scene Failed for file %@ output %@", isfPath, error);

    id<MTLCommandQueue> commandQueue = [testDevice newCommandQueue];
    id<MTLCommandBuffer> commandBuffer = [commandQueue commandBuffer];

    id<MTLTexture> outputTexture = [self createTextureForDevice:testDevice
                                                          width:1920
                                                         height:1080
                                                    pixelFormat:MTLPixelFormatBGRA8Unorm];
    NSURL *inputImageUrl = [[NSBundle bundleForClass:[self class]] URLForResource:@"isfLogoBW" withExtension:@"jpg"];
    id<MTLTexture> inputImageForEffects = [self loadTextureUsingMetalKit:inputImageUrl device:testDevice];

    XCTAssertNotNil(inputImageForEffects,
                    @"failed to load test input image. This is an error due to test project, not vvisfkit");
    [isfScene setNSObjectVal:inputImageForEffects forInputKey:@"inputImage"];

    BOOL success = [isfScene renderOnTexture:outputTexture onCommandBuffer:commandBuffer withError:&error];
    if( !success )
    {
        NSLog(@"failed to render frame for file: %@", isfPath);
        NSLog(@"error: %@", error);
    }
    XCTAssertTrue(isfScene, @"ISF Render failed for file %@ output %@", isfPath, error);
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
