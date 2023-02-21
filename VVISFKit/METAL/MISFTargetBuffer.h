#import "MISFModel.h"
#import "MISFSize.h"
#import <DDMathParser/DDExpression.h>
#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <VVBasics/VVBasics.h>
#import <VVBufferPool/VVBufferPool.h>

NS_ASSUME_NONNULL_BEGIN

@interface MISFTargetBuffer : NSObject
{
    id<MTLDevice> device;
    MTLPixelFormat pixelFormat;
    MISFSize *bufferSize;
    NSString *name; //    the name of this buffer
    // Usefull for metal limit cases (see usage of getBufferReadonlyTextureWithCommandBuffer)
    id<MTLTexture> readonlyTexture;
}

+ (id)createForDevice:(id<MTLDevice>)theDevice pixelFormat:(MTLPixelFormat)thePixelFormat;
+ (id)createForDevice:(id<MTLDevice>)theDevice
          pixelFormat:(MTLPixelFormat)thePixelFormat
            fromModel:(MISFModelBuffer *)model;

// Used if the buffer should be temporary
- (void)clearBuffer;
- (id<MTLTexture>)getBufferTextureWithCommandBuffer:(id<MTLCommandBuffer>)commandBuffer;
- (id<MTLTexture>)getBufferReadonlyTextureWithCommandBuffer:(id<MTLCommandBuffer>)commandBuffer;

// Quick access to MISFSize API
#warning mto-anomes: bad design, but kept so far to have a similar API with GL
#warning mto-anomes: possible edge case: if only one eval string is given and not two
- (BOOL)targetSizeNeedsEval;
- (void)evalTargetSizeWithSubstitutionsDict:(NSDictionary *)d;
- (void)setTargetSize:(VVSIZE)n;
- (void)setTargetWidthString:(NSString *)n;
- (void)setTargetHeightString:(NSString *)n;

@property(retain, readwrite) NSString *name;
@property(readwrite) BOOL isPersistent;
@property(retain, readwrite, nullable) id<MTLTexture> texture;

@end

NS_ASSUME_NONNULL_END
