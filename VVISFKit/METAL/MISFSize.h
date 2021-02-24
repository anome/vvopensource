#import <DDMathParser/DDExpression.h>
#import <Foundation/Foundation.h>
#import <VVBasics/VVBasics.h>
#import <VVBufferPool/VVBufferPool.h>

NS_ASSUME_NONNULL_BEGIN

@interface MISFSize : NSObject

- (void)setSize:(VVSIZE)n; // This is bad if there is eval involved this makes no sense to set the Size entirely
- (void)setWidthString:(NSString *)newTargetWidthString;
- (void)setHeightString:(NSString *)newTargetHeightString;

- (void)evalSizeWithSubstitutionDict:(NSDictionary *)d;
- (BOOL)needsEval;

@property(readonly) double width;
@property(readonly) double height;

@end

NS_ASSUME_NONNULL_END
