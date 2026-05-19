#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface MISFTexturePoolsManager : NSObject

+ (MISFTexturePoolsManager *)sharedManager;

- (id<MTLTexture>)acquireTextureWithWidth:(NSUInteger)width
                                   height:(NSUInteger)height
                              pixelFormat:(MTLPixelFormat)pixelFormat
                                   device:(id<MTLDevice>)device
                                 withCallerId:(NSString*)uuid;

- (void)recycleTexture:(id<MTLTexture>)texture withCallerId:(NSString*)callerId;

@end

NS_ASSUME_NONNULL_END
