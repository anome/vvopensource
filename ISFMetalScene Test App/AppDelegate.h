#import "MetalImageView.h"
#import <Cocoa/Cocoa.h>
#import <VVISFKit/VVISFKit.h>

@interface AppDelegate : NSObject <NSApplicationDelegate>
{
    CVDisplayLinkRef displayLink;
    NSOpenGLContext *sharedContext;

    IBOutlet MetalImageView *metalImageView;
    ISFMetalScene *isfScene; //	tell this to load an ISF file and it renders buffers/textures
    IBOutlet NSSlider *sliderOne;
    IBOutlet NSSlider *sliderTwo;
    IBOutlet NSSlider *sliderThree;
    IBOutlet NSSlider *sliderFour;
    IBOutlet NSSlider *sliderFive;
    IBOutlet NSSlider *sliderInteger;
}

- (void)renderCallback;

@end

CVReturn displayLinkCallback(CVDisplayLinkRef displayLink, const CVTimeStamp *inNow, const CVTimeStamp *inOutputTime,
                             CVOptionFlags flagsIn, CVOptionFlags *flagsOut, void *displayLinkContext);
