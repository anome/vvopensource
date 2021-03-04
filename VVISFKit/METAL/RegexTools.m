#import "RegexTools.h"
#import "MISFErrorCodes.h"

static NSString *const ERROR_STRING_OPERATION_KEY = @"RegexTools String Operation Failed";
@implementation RegexTools

+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                         pattern:(NSString *)pattern
                      withString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr
{
    NSError *regexError = nil;
    NSRegularExpression *regex = [NSRegularExpression regularExpressionWithPattern:pattern
                                                                           options:NSRegularExpressionAnchorsMatchLines
                                                                             error:&regexError];
    if( regexError )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:@"Abort replace Pattern. Regex error: %@", regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeInternal userInfo:userInfo];
        }
        return nil;
    }

    NSRange stringRange = NSMakeRange(0, [stringToSubstitute length]);
    NSString *result = [regex stringByReplacingMatchesInString:stringToSubstitute
                                                       options:0
                                                         range:stringRange
                                                  withTemplate:substitutionString];
    //    NSUInteger numberOfMatches = [regex numberOfMatchesInString:stringToSubstitute
    //                                                        options:0
    //                                                          range:stringRange];
    return result;
}

+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                    variableName:(NSString *)variableName
                        byString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr

{
    NSString *pattern = [RegexTools detectPatternForVariableName:variableName];
    return [RegexTools substituteInString:stringToSubstitute
                                  pattern:pattern
                               withString:substitutionString
                                withError:errorPtr];
}

+ (NSString *)substituteInString:(NSString *)stringToSubstitute
                    functionName:(NSString *)functionName
                        byString:(NSString *)substitutionString
                       withError:(NSError **)errorPtr
{
    NSString *pattern = [RegexTools detectPatternForFunctionName:functionName];
    return [RegexTools substituteInString:stringToSubstitute
                                  pattern:pattern
                               withString:substitutionString
                                withError:errorPtr];
}

+ (NSRange)getRangeInString:(NSString *)stringToSearch pattern:(NSString *)pattern withError:(NSError **)errorPtr
{
    NSError *regexError = nil;
    NSRegularExpression *regex = [NSRegularExpression regularExpressionWithPattern:pattern
                                                                           options:NSRegularExpressionAnchorsMatchLines
                                                                             error:&regexError];
    if( regexError )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                ERROR_STRING_OPERATION_KEY :
                    [NSString stringWithFormat:@"Abort range Pattern. Regex error: %@", regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeInternal userInfo:userInfo];
        }
        return NSMakeRange(NSNotFound, NSNotFound);
    }

    NSRange stringRange = NSMakeRange(0, [stringToSearch length]);
    NSRange result = [regex rangeOfFirstMatchInString:stringToSearch options:0 range:stringRange];
    return result;
}

+ (NSString *)detectPatternForVariableName:(NSString *)variableName
{
    // https://regex101.com/r/7YYye1/6
    NSString *part1 = @"(?<=(\\s|[(,*\\/!+-]))(";
    NSString *part2 = @")(?!(\\w|\\s*\\())";
    return [[part1 stringByAppendingString:variableName] stringByAppendingString:part2];
}

+ (NSString *)detectPatternForFunctionName:(NSString *)functionName
{
    // https://regex101.com/r/CeWWrG/1
    NSString *part1 = @"(?<=(\\s|[(,*\\/+-]))(";
    NSString *part2 = @")(?=(\\s*\\())";
    return [[part1 stringByAppendingString:functionName] stringByAppendingString:part2];
}

+ (int)extractNumberFromString:(NSString *)val
{
    NSString *numberString = @"";
    NSScanner *scanner = [NSScanner scannerWithString:val];
    NSCharacterSet *numbers = [NSCharacterSet characterSetWithCharactersInString:@"0123456789"];
    // Throw away characters before the first number.
    [scanner scanUpToCharactersFromSet:numbers intoString:NULL];
    // Collect numbers.
    [scanner scanCharactersFromSet:numbers intoString:&numberString];
    if( [numberString isEqualToString:@""] )
    {
        // negative value means no value found
        return -1;
    }
    else
    {
        return (int)numberString.integerValue;
    }
}

+ (NSString *)injectString:(NSString *)stringToInject
                  inString:(NSString *)baseString
                  atMarker:(NSString *)marker
                 withError:(NSError **)errorPtr
{
    NSRange rangeForMarker = [baseString rangeOfString:marker];
    if( rangeForMarker.location == NSNotFound )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo =
                @{ERROR_STRING_OPERATION_KEY : [NSString stringWithFormat:@"WARN: could not find marker for string"]};
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeInternal userInfo:userInfo];
        }
        return nil;
    }
    NSRange rangeBeginning = NSMakeRange(0, rangeForMarker.location + rangeForMarker.length);
    NSRange rangeEnd = NSMakeRange(rangeForMarker.location + rangeForMarker.length,
                                   baseString.length - rangeForMarker.location - rangeForMarker.length);
    NSString *beginning = [baseString substringWithRange:rangeBeginning];
    NSString *end = [baseString substringWithRange:rangeEnd];
    NSString *injected = [NSString stringWithFormat:@"%@%@%@", beginning, stringToInject, end];
    return injected;
}
@end
