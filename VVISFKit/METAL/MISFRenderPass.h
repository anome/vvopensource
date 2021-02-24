

@interface MISFRenderPass : NSObject

+ (id)create;

@property(retain, readwrite) NSString *targetName;
@property(readwrite, nonatomic) BOOL targetIsFloat;

@end
