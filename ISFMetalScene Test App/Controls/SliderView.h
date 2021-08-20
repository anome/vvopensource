#import <Cocoa/Cocoa.h>

@interface SliderView : NSView

- (id)initWithFrame:(NSRect)frame name:(NSString*)name minVal:(float)minValue defaultVal:(float)defaultValue maxVal:(float)maxValue;

@property (nonatomic, copy) void (^onChange)(float value);

@end
