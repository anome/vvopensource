#import <Cocoa/Cocoa.h>

@interface CheckboxView : NSView

- (id)initWithFrame:(NSRect)frame name:(NSString*)name defaultVal:(BOOL)defaultVal;

@property (nonatomic, copy) void (^onChange)(BOOL value);

@end
