#import "FileTools.h"

@implementation FileTools

+ (NSString *)uuid
{
    NSString *result;
    CFUUIDRef uuid;
    CFStringRef uuidStr;

    uuid = CFUUIDCreate(NULL);
    assert(uuid != NULL);

    uuidStr = CFUUIDCreateString(NULL, uuid);
    assert(uuidStr != NULL);

    result = [NSString stringWithFormat:@"%@", uuidStr];
    assert(result != nil);

    CFRelease(uuidStr);
    CFRelease(uuid);
    return result;
}

+ (NSString *)pathForTemporaryFileWithPrefix:(NSString *)prefix
{
    NSString *uuid = [FileTools uuid];

    NSString *result =
        [NSTemporaryDirectory() stringByAppendingPathComponent:[NSString stringWithFormat:@"%@-%@", prefix, uuid]];
    assert(result != nil);

    return result;
}

@end
