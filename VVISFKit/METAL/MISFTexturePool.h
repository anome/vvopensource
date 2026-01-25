#import <Foundation/Foundation.h>
#import <Metal/Metal.h>

NS_ASSUME_NONNULL_BEGIN
/**
 This texture pool is designed for the sole purpose of solving this issue:
 multi-pass shader rendered at multiple resolutions simultaneously
 keep track of persistant image buffers instead of re-creating them, for memory optimisation, but foremost for visual continuity (example: vvmotionblur)
 
 It must be used only: on local contexts for MISFTargetBuffers
 It does not: clear textures (to allow the visual continuity in case of recycling)
 */

@interface MISFTextureEntry : NSObject
@property (nonatomic, retain) id<MTLTexture> texture;
@property (nonatomic, assign) NSTimeInterval returnedAt;
@end

@interface MISFTexturePool : NSObject

- (id<MTLTexture>)acquireTextureWithWidth:(NSUInteger)width
                                   height:(NSUInteger)height
                              pixelFormat:(MTLPixelFormat)pixelFormat
                                   device:(id<MTLDevice>)device;

- (void)recycleTexture:(id<MTLTexture>)texture;
- (void)cleanPool;

@property (readwrite, retain) NSArray<MISFTextureEntry*> *entries;

@end

NS_ASSUME_NONNULL_END
