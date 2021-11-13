#import "MetalImageView.h"
#import <Cocoa/Cocoa.h>
#import <VVBasics/VVBasics.h>
#import <VVBufferPool/VVBufferPool.h>
#import <VVISFKit/VVISFKit.h>

@interface AppDelegate : NSObject <NSApplicationDelegate>
{
    IBOutlet MetalImageView *metalImageView;
    IBOutlet NSSlider *sliderExplorePasses;
    IBOutlet NSPopUpButton *shaderSourceButton;
    IBOutlet NSStackView *controlsStackView;
    IBOutlet VVBufferGLView *glBufferView;
}

- (void)glAndMetalrenderCallback;

@end

CVReturn displayLinkCallback(CVDisplayLinkRef displayLink, const CVTimeStamp *inNow, const CVTimeStamp *inOutputTime,
                             CVOptionFlags flagsIn, CVOptionFlags *flagsOut, void *displayLinkContext);
