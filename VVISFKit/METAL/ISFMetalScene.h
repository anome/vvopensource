#import "ISFAttrib.h"
#import "ISFFileManager.h"
#import "ISFSceneBase.h"
#import "MISFPreloadedMedia.h"
#import <Cocoa/Cocoa.h>
#import <TargetConditionals.h>

/// implements ISFSceneBase protocol - loads and renders ISF files in Metal
/**
\ingroup VVISFKit
*/
@interface ISFMetalScene : NSObject <ISFSceneBase>

- (id)initWithDevice:(id<MTLDevice>)device
         pixelFormat:(MTLPixelFormat)pixelFormat
    fragmentFilePath:(NSString *)filePath
           withError:(NSError **)errorPtr;
- (id)initWithDevice:(id<MTLDevice>)device
         pixelFormat:(MTLPixelFormat)pixelFormat
      preloadedMedia:(MISFPreloadedMedia *)preloadedMedia
           withError:(NSError **)errorPtr;

// Preload API - make sure to use the same device than the one for your rendering !
+ (MISFPreloadedMedia *)preloadFile:(NSString *)filePath onDevice:(id<MTLDevice>)device withError:(NSError **)errorPtr;

///    allocates and renders into a buffer/GL texture of the passed size, then returns the buffer
- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
              withError:(NSError **)errorPtr;

- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
             renderTime:(double)t
              withError:(NSError **)errorPtr;

@property(readwrite, nonatomic) BOOL inputImagesArePremultipled;
@property(readwrite, nonatomic) BOOL inputImagesAreFlipped;
// Gain one redraw for performance, but could cause flip,color issues or glitches if the shader re-uses its output (persistent buffers)
@property(readwrite, nonatomic) BOOL bypassSinglePassRenderIsolation;
@property(readwrite, nonatomic) int choosePassIndex; // debug only

@end
