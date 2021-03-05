#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface FileTools : NSObject

+ (NSString *)uuid;
+ (NSString *)pathForTemporaryFileWithPrefix:(NSString *)prefix;

@end

NS_ASSUME_NONNULL_END
