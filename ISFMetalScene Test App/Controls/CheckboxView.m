#import "CheckboxView.h"

#define HEIGHT 20
#define WIDTH 200

@implementation CheckboxView
{
    NSString *name;
    BOOL defaultValue;
    NSTextField *currentValueLabel;
}

- (id)initWithFrame:(NSRect)frame name:(NSString*)theName defaultVal:(BOOL)defaultVal;
{
    self = [super initWithFrame:frame];
    if (self) {

        defaultValue = defaultVal;
        name = theName;
        _onChange = ^(BOOL value) {};
        [self setBackgroundColor:[NSColor whiteColor]];
        [self.heightAnchor constraintEqualToConstant:HEIGHT].active = YES;
        [self.widthAnchor constraintEqualToConstant:WIDTH].active = YES;
    }
    return self;
}

- (void)drawRect:(NSRect)dirtyRect
{
    [super drawRect:dirtyRect];
}
- (void) awakeFromNib
{
    [self addCheckBoxWithName:name defaultValue:defaultValue];
}

- (void) insertView:(NSView*)view
{
    [self addSubview:view];
}

- (void) addCheckBoxWithName:(NSString*)name defaultValue:(BOOL)defaultValue
{
    float labelHeight = 20.0;
    CGRect labelRect = CGRectMake(0.0, HEIGHT - labelHeight, self.frame.size.width, labelHeight);
    NSTextField *label = [[NSTextField alloc] initWithFrame:labelRect];
    [label setBezeled:NO];
    [label setEditable:NO];
    [label setSelectable:NO];
    label.stringValue = name;
    [self addSubview:label];
    NSRect frame = CGRectMake(self.frame.size.width - 20, HEIGHT - 20, 20, 20);
    NSButton *checkBox = [[NSButton alloc] initWithFrame:frame];
    [checkBox setButtonType:NSSwitchButton];
    [checkBox setState:defaultValue];
    checkBox.target = self;
    checkBox.action = @selector(changed:);
    [self addSubview:checkBox];
}

- (void)changed:(NSButton*)button
{
    self.onChange(button.state == NSControlStateValueOn ? YES : NO);
}


@end
