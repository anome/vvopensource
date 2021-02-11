#import "MISFAttribBufferDefinition.h"
#import "MISFModel.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
/// It's just a dumb container for a few types
@interface MISFMetalModel : NSObject

@property(readwrite, retain) MISFModel *parentModel;
@property(readwrite, retain) NSString *convertedFragmentCode;
@property(readwrite, retain) NSString *convertedVertexCode;
#warning mto-anomes these two are infos parsed directly from the converted code - useless data duality
@property(readwrite, retain) NSArray<MISFAttribBufferDefinition *> *fragmentBufferDefinitions;
@property(readwrite, retain) NSArray<MISFAttribBufferDefinition *> *vertexBufferDefinitions;

@end

NS_ASSUME_NONNULL_END
