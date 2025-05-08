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

+ (BOOL)searchString:(NSString *)stringToSearch forPattern:(NSString *)pattern withError:(NSError **)errorPtr
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
                    [NSString stringWithFormat:@"Abort search Pattern. Regex error: %@", regexError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFErrorCodeInternal userInfo:userInfo];
        }
        return NO;
    }
    NSRange stringRange = NSMakeRange(0, [stringToSearch length]);
    NSArray* results = [regex matchesInString:stringToSearch options:0 range:stringRange];
    
    return [results count] != 0;
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

/**
Generates Rexgex pattern to search a specific varibaleName in code
 @param variableName to search
 @returns regex pattern of variable, usable for regex search
 // https://regex101.com/r/7YYye1/6
 --> This one was cool, but \K not handled by OBJC (\s|[(*\/+-])\K(variableName)(?!(\w|\s*\())

 --- SHOULD MATCH

 if ((!variableName))
 fmod(variableName)
 variableName
     variableName
 variableName;
 variableName+0
 variableName/0
 variableName-0
 variableName*0
 0+variableName
 0-variableName
 0/variableName
 0*variableName
 fn(variableName)
 (1,variableName,variableName,1)
 (mixOffset-variableName)/(tmpRadius-variableName));

 --- SHOULD PARTIALLY MATCH

 variableName.variableName;

 --- SHOULD NOT MATCH

 someOther.variableName
 someOthervariableName ;
 someOther_variableName
 variableName_someOther
 variableNameSomeOther;
 variableName()
 variableName ()
 */
+ (NSString *)detectPatternForVariableName:(NSString *)variableName
{
    NSString *part1 = @"(?<=(\\s|[(,*\\/!+-]))(";
    NSString *part2 = @")(?!(\\w|\\s*\\())";
    return [[part1 stringByAppendingString:variableName] stringByAppendingString:part2];
}

/**
 Generates Rexgex pattern to search a specific function in code
  @param functionName to search
  @returns regex pattern of function, usable for regex search
 // https://regex101.com/r/CeWWrG/1
 --- SHOULD MATCH

 functionName()
 functionName ()
 functionName ()
 a.functionName()
 .functionname()
 -functionName()
 +functionName()
 /functionName()
 *functionName()
 mod(functionName())
 mod(functionName(),functionName(), functionName(),)


 -- SHOULD NOT MATCH
 functionName2()
 notfunctionName()
 variableName
     variableName
 functionName;
 functionName+0
 functionName/0
 functionName-0
 functionName*0
 0+functionName
 0-functionName
 0/functionName
 0*functionName
 fn(functionName)
 (1,functionName,functionName,1)
 (mixOffset-functionName)/(tmpRadius-functionName));
 */
+ (NSString *)detectPatternForFunctionName:(NSString *)functionName
{
    NSString *part1 = @"(?<=(\\s|[(,*\\/+-]))(";
    NSString *part2 = @")(?=(\\s*\\())";
    return [[part1 stringByAppendingString:functionName] stringByAppendingString:part2];
}

/**
 Generates Rexgex pattern to search a specific functions with specific first parameter name in code
 @param functionNames to search, as a regex list such as fn1|fn2|fn3
 @param firstParameter to search
 @returns regex pattern of function, usable for regex search
 // https://regex101.com/r/6RRx7x/1
 --- SHOULD MATCH

 vec4 sourceUv = IMG_NORM_PIXEL(BufferA, uv);
 vec4 sourceUv = IMG_PIXEL(BufferA, uv);
 vec4 sourceUv = IMG_NORM_THIS_PIXEL(BufferA, uv);
 vec4 sourceUv = IMG_THIS_PIXEL(BufferA, uv);
 vec4 sourceUv = texture2D(BufferA, uv);

 vec4 sourceUv = IMG_NORM_PIXEL(
 BufferA, uv);

 vec4 sourceUv = IMG_NORM_PIXEL(
    BufferA, uv);

 vec4 sourceUv = IMG_NORM_PIXEL
 (
    BufferA, uv);

 IMG_NORM_PIXEL(
    BufferA   , uv);


 --- SHOULD NOT MATCH
 vec4 sourceUv = IMG_NORM_PIXEL(BufferB, uv);
 vec4 sourceUv = IMG_PIXEL(BufferB, uv);
 vec4 sourceUv = IMG_NORM_THIS_PIXEL(BufferB, uv);
 vec4 sourceUv = IMG_THIS_PIXEL(BufferB, uv);
 vec4 sourceUv = texture2D(BufferB, uv);
 vec4 sourceUv = texture2D(buffera, uv);
 vec4 sourceUv = texture2d(buffera, uv);
 */
+ (NSString *)detectPatternForFunctions:(NSString*)functions withFirstParameter:(NSString*)parameter;
{
    return [NSString stringWithFormat:@"(?<=(\\s|[(,*\\/+-]))(%@)(?=(\\s*\\(\\s*%@))", functions, parameter];
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
