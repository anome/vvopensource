#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <MetalKit/MetalKit.h>
#import <VVISFKit/VVISFKit.h>
#import <XCTest/XCTest.h>

/// Basic Regression testing
// Tests are not isolated. for working shaders, each test needs the previous one to work. The previous processes will
// re-run anyway
@interface VVISFKIT_METAL_Regression_Tests : XCTestCase

@end

@implementation VVISFKIT_METAL_Regression_Tests
{
    NSArray<NSString *> *workingIsfs;
     NSArray<NSString *> *IsfsneedingMinorChangesToWork;
    NSArray<NSString *> *workingIsfWithMinorChanges;
    NSArray<NSString *> *isfExpectedToParseFail;
    NSArray<NSString *> *isfExpectedToSpirvFail;
    NSBundle *bundle;
    MISFModel *model;
    id<MTLDevice> testDevice;
}

- (void)setUp
{
    // Put setup code here. This method is called before the invocation of each test method in the class.
    bundle = [NSBundle bundleForClass:[self class]];
    workingIsfs = [bundle pathsForResourcesOfType:@"fs" inDirectory:@"working"];
    workingIsfWithMinorChanges = [bundle pathsForResourcesOfType:@"fs" inDirectory:@"workingWithMinorChanges"];
    isfExpectedToParseFail = [bundle pathsForResourcesOfType:@"fs" inDirectory:@"expectParseFail"];
    isfExpectedToSpirvFail = [bundle pathsForResourcesOfType:@"fs" inDirectory:@"expectSpirvFail"];
    IsfsneedingMinorChangesToWork = [bundle pathsForResourcesOfType:@"fs" inDirectory:@"needsMinorChangesToWork"];
    testDevice = MTLCreateSystemDefaultDevice();
    self.continueAfterFailure = NO;
}

- (void)tearDown
{
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    workingIsfs = nil;
    workingIsfWithMinorChanges = nil;
    model = nil;
    bundle = nil;
    testDevice = nil;
}

#pragma mark Helpers

- (void)preloadShader:(NSString *)filePath
{
    NSError *error;
    MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:filePath onDevice:testDevice withError:&error];
    if( preloadedMedia == nil )
    {
        NSLog(@"failed to preload media for file: %@", filePath);
        NSLog(@"error: %@", error);
    }
    XCTAssertNotNil(preloadedMedia, @"Compilation Failed for file %@ output %@", filePath, error);

    ISFMetalScene *isfScene = [[ISFMetalScene alloc] initWithDevice:testDevice
                                                        pixelFormat:MTLPixelFormatBGRA8Unorm
                                                     preloadedMedia:preloadedMedia
                                                          withError:&error];
    if( isfScene == nil )
    {
        NSLog(@"failed to create isfScene for file: %@", filePath);
        NSLog(@"error: %@", error);
    }
    XCTAssertNotNil(isfScene, @"ISF Scene Failed for file %@ output %@", filePath, error);

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
    ISFAttribVal imageVal;
    imageVal.metalImageVal = inputImageForEffects;
    [isfScene setValue:imageVal forInputKey:@"inputImage"];

    BOOL success = [isfScene renderOnTexture:outputTexture onCommandBuffer:commandBuffer withError:&error];
    if( !success )
    {
        NSLog(@"failed to render frame for file: %@", filePath);
        NSLog(@"error: %@", error);
    }
    XCTAssertTrue(isfScene, @"ISF Render failed for file %@ output %@", filePath, error);
}

#pragma mark Tests

- (void)testLoadAndRenderWorkingShaders
{
    for( NSString *filePath in workingIsfs )
    {
        NSLog(@"\n\n\n\n\n==========\nTesting %@\n==========", filePath);
        [self preloadShader:filePath];
    }
}

- (void)testLoadAndRenderWorkingWithMinorChangesShaders
{
    for( NSString *filePath in workingIsfWithMinorChanges )
    {
        NSLog(@"\n\n\n\n\n==========\nTesting %@\n==========", filePath);
        [self preloadShader:filePath];
    }
}

- (void)testparseErrorShaders
{
    for( NSString *filePath in isfExpectedToParseFail )
    {
        NSLog(@"\n\n\n\n\n==========\nTesting %@\n==========", filePath);
        NSError *error;
        MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:filePath onDevice:testDevice withError:&error];
        XCTAssertNil(preloadedMedia, @"Parsing unexpectedly succeded for file %@ output %@", filePath, error);
    }
}

- (void)testspirVForErrorShaders
{
    for( NSString *filePath in isfExpectedToSpirvFail )
    {
        NSError *error;
        MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:filePath onDevice:testDevice withError:&error];
        XCTAssertNil(preloadedMedia, @"Parsing unexpectedly succeded for file %@ output %@", filePath, error);
        XCTAssertNil(preloadedMedia, @"Transpilation unexpectedly succeded for filePath %@ output %@", filePath, error);
    }
}

- (void)testspirVForNeedMinorChangesToWorkShaders
{
    for( NSString *filePath in IsfsneedingMinorChangesToWork )
    {
        NSError *error;
        MISFPreloadedMedia *preloadedMedia = [ISFMetalScene preloadFile:filePath onDevice:testDevice withError:&error];
        XCTAssertNil(preloadedMedia, @"Parsing unexpectedly succeded for file %@ output %@", filePath, error);
        XCTAssertNil(preloadedMedia, @"Transpilation unexpectedly succeded for filePath %@ output %@", filePath, error);
    }
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
