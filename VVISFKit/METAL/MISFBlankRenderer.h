#import <Foundation/Foundation.h>
#import <Metal/Metal.h>

@interface MISFBlankRenderer : NSObject

- (instancetype)initWithDevice:(id<MTLDevice>)device colorPixelFormat:(MTLPixelFormat)colorPixelFormat;
- (void)renderBlankOnTexture:(id<MTLTexture>)textureToBlank onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer;

@end
