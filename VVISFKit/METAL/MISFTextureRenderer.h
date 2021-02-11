#import <Foundation/Foundation.h>
#import <Metal/Metal.h>

@interface MISFTextureRenderer : NSObject

- (instancetype)initWithDevice:(id<MTLDevice>)device colorPixelFormat:(MTLPixelFormat)colorPixelFormat;

- (void)renderFromTexture:(id<MTLTexture>)offScreenTexture
                inTexture:(id<MTLTexture>)texture
          onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer;

@property(readwrite) bool flip;
@property(readwrite, nonatomic) MTLClearColor clearColor;

@end
