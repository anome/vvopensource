#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface RegexTools : NSObject

+ (NSString *)detectPatternForVariableName:(NSString *)variableName;
+ (NSString *)detectPatternForFunctionName:(NSString *)functionName;
+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                    variableName:(NSString *)variableName
                        byString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr;
+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                    functionName:(NSString *)functionName
                        byString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr;
+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                         pattern:(NSString *)pattern
                      withString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr;
+ (NSRange)getRangeInString:(NSString *)stringToSearch pattern:(NSString *)pattern withError:(NSError **)errorPtr;
+ (NSString *)injectString:(NSString *)stringToInject
                  inString:(NSString *)baseString
                  atMarker:(NSString *)marker
                 withError:(NSError **)errorPtr;
+ (int)extractNumberFromString:(NSString *)val;

@end

NS_ASSUME_NONNULL_END
