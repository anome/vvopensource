#import "SliderView.h"

#define HEIGHT 50
#define WIDTH 200

@implementation SliderView
{
    float freeYPosition;
    float marginBetweenViews;
    NSString *name;
    float minValue;
    float maxValue;
    float defaultValue;
    NSTextField *currentValueLabel;
}

- (id)initWithFrame:(NSRect)frame name:(NSString*)theName minVal:(float)min defaultVal:(float)defaultVal maxVal:(float)max
{
    self = [super initWithFrame:frame];
    if (self) {
        minValue = min;
        maxValue = max;
        defaultValue = defaultVal;
        name = theName;
        _onChange = ^(float value) {};
        [self setBackgroundColor:[NSColor whiteColor]];
        freeYPosition = HEIGHT;
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
    [self addSliderWithName:name minVal:minValue defaultVal:defaultValue maxVal: maxValue];
}

- (void) addSliderWithName:(NSString*)name minVal:(float)minValue defaultVal:(float)defaultValue maxVal:(float)maxValue
{
    float labelHeight = 20.0;
    CGRect labelRect = CGRectMake(0.0, freeYPosition - labelHeight, self.frame.size.width, labelHeight);
    NSTextField *label = [[NSTextField alloc] initWithFrame:labelRect];
    [label setBezeled:NO];
    [label setDrawsBackground:NO];
    [label setEditable:NO];
    [label setSelectable:NO];
    label.stringValue = [NSString stringWithFormat:@"%@ %.02f", name, defaultValue];
    currentValueLabel = label;
    [self addSubview:label];
    
    freeYPosition -= 20;
    CGRect sliderRect = CGRectMake(0.0, freeYPosition - 20.0, self.frame.size.width, 20.0);
    NSSlider *slider = [[NSSlider alloc] initWithFrame:sliderRect];
    slider.minValue = minValue;
    slider.maxValue = maxValue;
    slider.floatValue = defaultValue;
    slider.continuous = YES;
    
    slider.action = @selector(sliderMoved:);
    slider.target = self;
    
    freeYPosition -=10;
    
    [self addSliderTextLabelWithMinVal: minValue maxVal:maxValue slider:slider];
    [self addSubview:slider];
}

- (void) addSliderTextLabelWithMinVal:(float)minValue maxVal:(float)maxValue slider:(NSSlider*)slider
{
    const float textSize = 10;
    // Left text Label : min Value
    {
        CGRect labelRect = CGRectMake(0.0, freeYPosition - textSize - 5, (self.frame.size.width/3), textSize);
        NSTextField *label = [[NSTextField alloc] initWithFrame:labelRect];
        [label setBezeled:NO];
        [label setDrawsBackground:NO];
        [label setEditable:NO];
        [label setSelectable:NO];
        NSFont *font = [NSFont fontWithName:label.font.fontName size:textSize];
        [label setTextColor:[NSColor colorWithWhite:0.5 alpha:1.0]];
        [label setFont:font];
        label.stringValue = [NSString stringWithFormat:@"%.02f", minValue];
        [self addSubview:label];
    }
    
    // Right text Label : max Value
    {
        CGRect labelRect = CGRectMake(2*(self.frame.size.width/3), freeYPosition - textSize - 5, (self.frame.size.width/3), textSize);
        NSTextField *label = [[NSTextField alloc] initWithFrame:labelRect];
        [label setBezeled:NO];
        [label setDrawsBackground:NO];
        [label setEditable:NO];
        [label setSelectable:NO];
        [label setAlignment:NSTextAlignmentRight];
        NSFont *font = [NSFont fontWithName:label.font.fontName size:textSize];
        [label setTextColor:[NSColor colorWithWhite:0.5 alpha:1.0]];
        [label setFont:font];
        label.stringValue = [NSString stringWithFormat:@"%.02f", maxValue];
        [self addSubview:label];
    }
}



- (void)sliderMoved:(NSSlider*)slider
{
    self.onChange(slider.floatValue);
    currentValueLabel.stringValue = [NSString stringWithFormat:@"%@ %.02f", name, slider.floatValue];
}

@end
