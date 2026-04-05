#import "MISFRenderPass.h"
#import <VVBasics/VVBasics.h>

@implementation MISFRenderPass
@synthesize targetName;
@synthesize targetIsFloat;

+ (id)create
{
    id returnMe = [[MISFRenderPass alloc] init];
    if( returnMe == nil )
        return returnMe;
    return returnMe;
}

- (id)init
{
    if( self = [super init] )
    {
        targetName = nil;
        targetIsFloat = NO;
        return self;
    }
    return nil;
}
- (void)dealloc
{
    VVRELEASE(targetName);
}

@end
