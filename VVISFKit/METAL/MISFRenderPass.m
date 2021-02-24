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
    return [returnMe autorelease];
}

- (id)init
{
    if( self = [super init] )
    {
        targetName = nil;
        targetIsFloat = NO;
        return self;
    }
    [self release];
    return nil;
}
- (void)dealloc
{
    VVRELEASE(targetName);
    [super dealloc];
}

@end
