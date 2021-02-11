#import "MISFSize.h"

@implementation MISFSize
{
    DDExpression *widthExpression;
    DDExpression *heightExpression;
}

@synthesize width;
@synthesize height;

- (id)init
{
    if( self = [super init] )
    {
        width = 1.0;
        widthExpression = nil;
        height = 1.0;
        heightExpression = nil;
        return self;
    }
    [self release];
    return nil;
}

- (void)dealloc
{
    VVRELEASE(widthExpression);
    VVRELEASE(heightExpression);
    [super dealloc];
}

- (void)setWidthString:(NSString *)newWidthString
{
    VVRELEASE(widthExpression);
    if( newWidthString != nil )
    {
        NSError *err = nil;
        widthExpression = [[DDExpression expressionFromString:newWidthString error:&err] retain];
    }
}

- (void)setHeightString:(NSString *)newHeightString
{
    VVRELEASE(heightExpression);
    if( newHeightString != nil )
    {
        NSError *err = nil;
        heightExpression = [[DDExpression expressionFromString:newHeightString error:&err] retain];
    }
}
#warning mto-anomes: useless?
- (void)setTargetSize:(VVSIZE)newTargetSize
{
    width = newTargetSize.width;
    height = newTargetSize.height;
}

//    returns a YES if there's a target width string
- (BOOL)needsEval
{
    if( heightExpression != nil || widthExpression != nil )
    {
        return YES;
    }
    else
    {
        return NO;
    }
}

- (void)evalWithSubstitutionsDict:(NSDictionary *)substitutionDict
{
    if( widthExpression == nil && heightExpression == nil )
    {
        return;
    }

    VVSIZE newSize = VVMAKESIZE(0, 0);
    NSNumber *tmpNum = nil;
    tmpNum = [substitutionDict objectForKey:@"WIDTH"];
    if( tmpNum != nil )
    {
        newSize.width = [tmpNum doubleValue];
    }

    tmpNum = [substitutionDict objectForKey:@"HEIGHT"];
    if( tmpNum != nil )
    {
        newSize.height = [tmpNum doubleValue];
    }

    NSError *err = nil;
    if( widthExpression != nil )
    {
        newSize.width = [[widthExpression evaluateWithSubstitutions:substitutionDict evaluator:nil
                                                              error:&err] floatValue];
    }
    if( heightExpression != nil )
    {
        newSize.height = [[heightExpression evaluateWithSubstitutions:substitutionDict evaluator:nil
                                                                error:&err] floatValue];
    }
    if( err != nil )
    {
        NSLog(@"\t\terror evaluating term in %s: %@", __func__, err);
    }

    [self setSize:newSize];
}

- (void)setSize:(VVSIZE)newSize
{
    width = newSize.width;
    height = newSize.height;
}

@end
