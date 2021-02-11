#import "MISFAttribBufferDefinition.h"
#import "MISFMetalModel.h"
#import "MISFPreloadedMedia.h"
#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <VVBasics/VVBasics.h>
#include <simd/simd.h>

@interface MISFRenderer : NSObject

/// Basic initialisation
- (instancetype)initWithDevice:(id<MTLDevice>)device
              colorPixelFormat:(MTLPixelFormat)colorPixelFormat
                      forModel:(MISFMetalModel *)model;

/// Initialisation with preloading & error checks
+ (MISFPreloadedMedia *)preloadModel:(MISFMetalModel *)model
                            onDevice:(id<MTLDevice>)device
                           withError:(NSError **)errorPtr;

- (instancetype)initWithDevice:(id<MTLDevice>)device
              colorPixelFormat:(MTLPixelFormat)colorPixelFormat
             forPreloadedMedia:(MISFPreloadedMedia *)model;

/// Rendering
- (void)renderIsfOnTexture:(id<MTLTexture>)outputTexture
           onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
                withInputs:(MutLockArray *)inputs;

//// All default isf types to inject
@property(readwrite, nonatomic) int builtin_PASSINDEX;
@property(readwrite, nonatomic) NSSize builtin_RENDERSIZE;
@property(readwrite, nonatomic) float builtin_TIME;
@property(readwrite, nonatomic) float builtin_TIMEDELTA;
@property(readwrite, nonatomic) vector_float4 builtin_DATE;
@property(readwrite, nonatomic) int builtin_FRAMEINDEX;

@end
