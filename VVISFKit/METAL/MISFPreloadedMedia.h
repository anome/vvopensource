#import "MISFMetalModel.h"
#import <Foundation/Foundation.h>
#import <Metal/Metal.h>

NS_ASSUME_NONNULL_BEGIN

/// Container of ISF model + as many pre-loaded GPU ressources as possible. Use this object as starting point for rendering
@interface MISFPreloadedMedia : NSObject

@property(readwrite, retain) MISFMetalModel *model;
@property(readwrite, retain) NSString *fragmentCode;
@property(readwrite, retain) NSString *vertexCode;
@property(readwrite, retain) id<MTLLibrary> fragmentLibrary;
@property(readwrite, retain) id<MTLLibrary> vertexLibrary;

@end

NS_ASSUME_NONNULL_END
