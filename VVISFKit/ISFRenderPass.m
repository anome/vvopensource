#import "ISFRenderPass.h"




@implementation ISFRenderPass


+ (id) create	{
	id		returnMe = [[ISFRenderPass alloc] init];
	if (returnMe == nil)
		return returnMe;
	return returnMe;
}

- (id) init	{
	if (self = [super init])	{
		targetName = nil;
		return self;
	}
	return nil;
}
- (void) dealloc	{
	VVRELEASE(targetName);
}


@synthesize targetName;


@end
