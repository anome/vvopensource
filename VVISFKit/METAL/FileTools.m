#import "FileTools.h"

@implementation FileTools

// Utils
+ (NSString *)pathForTemporaryFileWithPrefix:(NSString *)prefix
{
    NSString *result;
    CFUUIDRef uuid;
    CFStringRef uuidStr;

    uuid = CFUUIDCreate(NULL);
    assert(uuid != NULL);

    uuidStr = CFUUIDCreateString(NULL, uuid);
    assert(uuidStr != NULL);

    result =
        [NSTemporaryDirectory() stringByAppendingPathComponent:[NSString stringWithFormat:@"%@-%@", prefix, uuidStr]];
    assert(result != nil);

    CFRelease(uuidStr);
    CFRelease(uuid);

    return result;
}

@end
