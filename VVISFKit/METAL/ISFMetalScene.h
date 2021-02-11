#import "ISFAttrib.h"
#import "ISFFileManager.h"
#import "ISFTargetBuffer.h"
#import <Cocoa/Cocoa.h>
#import <TargetConditionals.h>
#import <VVBasics/VVBasics.h>
#import <VVBufferPool/VVBufferPool.h>

#import "MISFPreloadedMedia.h"

///    Subclass of GLShaderScene- loads and renders ISF files
/**
\ingroup VVISFKit
*/
@interface ISFMetalScene : NSObject

- (id)initWithDevice:(id<MTLDevice>)device
         pixelFormat:(MTLPixelFormat)pixelFormat
    fragmentFilePath:(NSString *)filePath
           withError:(NSError **)errorPtr;
- (id)initWithDevice:(id<MTLDevice>)device
         pixelFormat:(MTLPixelFormat)pixelFormat
      preloadedMedia:(MISFPreloadedMedia *)preloadedMedia
           withError:(NSError **)errorPtr;
///    applies the passed value to the input with the passed key
/**
 @param n the value you want to pass, as an ISFAttribVal union
 @param k the key of the input you want to pass the value to
 */
- (void)setValue:(ISFAttribVal)n forInputKey:(NSString *)k;
- (void)setNSObjectVal:(id)object forInputKey:(NSString *)key;

// Preload API - make sure to use the same device than the one for your rendering !
+ (MISFPreloadedMedia *)preloadFile:(NSString *)filePath onDevice:(id<MTLDevice>)device withError:(NSError **)errorPtr;

///    allocates and renders into a buffer/GL texture of the passed size, then returns the buffer
- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
              withError:(NSError **)errorPtr;

- (BOOL)renderOnTexture:(id<MTLTexture>)outputTexture
        onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
             renderTime:(double)t
              withError:(NSError **)errorPtr;

// Debug only
@property(readwrite, nonatomic) int choosePassIndex;

///    returns a MutLockArray (from VVBasics) of ISFAttrib instances, one for each of the inputs
@property(readonly) MutLockArray *inputs;
///    returns the path of the currently-loaded ISF file
@property(readonly) NSString *filePath;
///    returns the name of the currently-loaded ISF file
@property(readonly) NSString *fileName;
///    returns a string with the description (pulled from the JSON blob) of the ISF file
@property(readonly) NSString *fileDescription;
///    returns a string with the credits (pulled from the JSON blob) of the ISF file
@property(readonly) NSString *fileCredits;
@property(readonly) ISFFunctionality fileFunctionality;
///    returns an array with the category names (as NSStrings) of this ISF file.  pulled from the JSON blob.
@property(readonly) NSMutableArray *categoryNames;
@property(readonly) NSArray<NSString *> *passTargetNames;
@property(readonly) NSString *jsonString;
@property(readonly) NSString *vertShaderSource;
@property(readonly) NSString *fragShaderSource;

@end
