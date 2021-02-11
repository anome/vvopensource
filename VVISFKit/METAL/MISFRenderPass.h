

@interface MISFRenderPass : NSObject
{
    NSString *targetName;
}

+ (id)create;

@property(retain, readwrite) NSString *targetName;

@end
