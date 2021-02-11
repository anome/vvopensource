#import "MISFInputDataType.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface MISFAttribBufferDefinition : NSObject

@property(readonly, retain) NSString *variableName;
@property(readonly) int bufferIndex;
@property(readonly) MISFInputDataType type;

- (id)initWithVariableName:(NSString *)theVariableName bufferIndex:(int)theBufferIndex type:(MISFInputDataType)theType;

- (NSString *)description;

@end

NS_ASSUME_NONNULL_END
