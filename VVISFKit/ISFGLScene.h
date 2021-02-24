#import <TargetConditionals.h>
#if !TARGET_OS_IPHONE
#import <Cocoa/Cocoa.h>
#else
#import <UIKit/UIKit.h>
#endif
#import <VVBufferPool/VVBufferPool.h>
#import "ISFAttrib.h"
#import "ISFFileManager.h"
#import <VVBasics/VVBasics.h>
#import "ISFTargetBuffer.h"
#import "ISFRenderPass.h"
#import "ISFSceneBase.h"



//	key is path to the file, object is VVBuffer instance.  the userInfo of this VVBuffer instance has an NSNumber, which serves as its "retain count": it's incremented when the buffer is loaded/created, and decremented when it's deleted- when it hits 0, the file is removed from the dict entirely.
extern MutLockDict		*_ISFImportedImages;
extern NSString			*_ISFESCompatibility;	//	a #define with some precisions modifiers used if we want to render ISF content in OpenGL ES
extern NSString			*_ISFVertPassthru;	//	passthru vertex shader
extern NSString			*_ISFVertVarDec;	//	string declaring functions and variables for the vertex shader, imported from a .txt file in this framework.  "pasted" into the assembled vertex shader.
extern NSString 		*_ISFVertInitFunc;	//	string of source code that performs variable initialization and general environment setup for the vertex shader, imported from a .txt file in this framework, and "pasted" into the vertex shader during its assembly.
extern NSString			*_ISFMacro2DString;	//	string of source containing function bodies that fetch pixel data from a (2D) GL texture.  IMGNORM and IMGPIXEL actually call one of these "Macro" functions.
extern NSString			*_ISFMacro2DBiasString;	//	same as above, slightly different texture format
extern NSString			*_ISFMacro2DRectString;	//	same as above, slightly different texture format
extern NSString			*_ISFMacro2DRectBiasString;	//	same as above, slightly different texture format




///	Subclass of GLShaderScene- loads and renders ISF files
/**
\ingroup VVISFKit
*/
@interface ISFGLScene : GLShaderScene <ISFSceneBase>	{
	BOOL				throwExceptions;	//	NO by default
	
	OSSpinLock			propertyLock;	//	locks the file* vars and categoryNames (everything before the empty line)
	BOOL				loadingInProgress;
	NSString			*filePath;	//	full path to the loaded file
	NSString			*fileName;	//	just the file name (including its extension)
	NSString			*fileDescription;	//	description of whatever the file does
	NSString			*fileCredits;	//	credits
	ISFFunctionality	fileFunctionality;
	NSMutableArray		*categoryNames;	//	array of NSStrings of the category names this filter should be listed under
	
	MutLockArray		*inputs;	//	array of ISFAttrib instances for the various inputs
	MutLockArray		*imageInputs;	//	array of ISFAttrib instances for the image inputs (the image inputs are stored in two arrays).
	MutLockArray		*audioInputs;	//	array of ISFAttrib instances for the audio inputs
	MutLockArray		*imageImports;	//	array of ISFAttrib instances that describe imported images. 'attribName' is the name of the sampler, 'attribDescription' is the path to the file.
	
	VVSIZE				renderSize;	//	the last size at which i was requested to render a buffer (used to produce vals from normalized point inputs that need a render size to be used)
	VVStopwatch			*swatch;	//	used to pass time to shaders
	unsigned long		renderFrameIndex;	//	used to pass FRAMEINDEX to shaders
	double				renderTime;
	double				renderTimeDelta;
	BOOL				bufferRequiresEval;	//	NO by default, set to YES during file open if any of the buffers require evaluation (faster than checking every single buffer every pass)
	MutLockArray		*persistentBufferArray;	//	array of ISFTargetBuffer instances describing the various persistent buffers. these buffers are retained until a different file is loaded.
	MutLockArray		*tempBufferArray;	//	array of ISFTargetBuffer instances- temp buffers are available while rendering, but are returned to the pool when rendering's complete
	MutLockArray		*passes;	//	array of ISFRenderPass instances.  right now, passes basically just describe a (ISFTargetBuffer)
	
	int					passIndex;	//	only has a valid value while rendering
	
	OSSpinLock			srcLock;
	NSString			*jsonSource;	//	the JSON string from the source *including the comments and any linebreaks before/after it*
	NSString			*jsonString;	//	the JSON string copied from the source- doesn't include any comments before/after it
	NSString			*vertShaderSource;	//	the raw vert shader source before being find-and-replaced
	NSString			*fragShaderSource;	//	the raw frag shader source before being find-and-replaced
	NSString			*compiledInputTypeString;	//	a sequence of characters, either "2" or "R", one character for each input image. describes whether the shader was compiled to work with 2D textures or RECT textures.
	long				renderSizeUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for the render size
	long				passIndexUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for the pass index
	long				timeUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for the time in seconds
	long				timeDeltaUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for time (in seconds) since the last frame was rendered
	long				dateUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for the date
	long				renderFrameIndexUniformLoc;	//	-1, or the location of the uniform var in the compiled GL program for the render frame index
	VVBuffer			*geoXYVBO;
}

#pragma mark - GL Specific API

///    Loads the ISF .fs file at the passed path
- (void) useFile:(NSString *)p;
- (void) useFile:(NSString *)p resetTimer:(BOOL)r;

//- (id) initWithSharedContext:(NSOpenGLContext *)c;
//- (id) initWithSharedContext:(NSOpenGLContext *)c pixelFormat:(NSOpenGLPixelFormat *)p sized:(VVSIZE)s;

///	if an ISF file has an input at the specified key, retains the buffer to be used at that input on the next rendering pass
/**
@param b the VVBuffer instance you want to send to the ISF file
@param k an NSString with the name of the image input you want to pass the VVBuffer to
*/
- (void) setBuffer:(VVBuffer *)b forInputImageKey:(NSString *)k;
///	convenience method- if the ISF file is an image filter (which has an explicitly-named image input), this applies the passed buffer to the filter input
- (void) setFilterInputImageBuffer:(VVBuffer *)b;
- (void) setBuffer:(VVBuffer *)b forInputAudioKey:(NSString *)k;
///	retrieves the current buffer being used at the passed key
- (VVBuffer *) bufferForInputImageKey:(NSString *)k;
- (VVBuffer *) bufferForInputAudioKey:(NSString *)k;


- (ISFTargetBuffer *) findPersistentBufferNamed:(NSString *)n;
- (ISFTargetBuffer *) findTempBufferNamed:(NSString *)n;

///	allocates and renders into a buffer/GL texture of the passed size, then returns the buffer
- (VVBuffer *) allocAndRenderToBufferSized:(VVSIZE)s;
- (VVBuffer *) allocAndRenderToBufferSized:(VVSIZE)s prefer2DTex:(BOOL)wants2D;
- (VVBuffer *) allocAndRenderToBufferSized:(VVSIZE)s prefer2DTex:(BOOL)wants2D passDict:(NSMutableDictionary *)d;
- (VVBuffer *) allocAndRenderToBufferSized:(VVSIZE)s prefer2DTex:(BOOL)wants2D renderTime:(double)t;
- (VVBuffer *) allocAndRenderToBufferSized:(VVSIZE)s prefer2DTex:(BOOL)wants2D renderTime:(double)t passDict:(NSMutableDictionary *)d;
- (void) renderToBuffer:(VVBuffer *)b sized:(VVSIZE)s;
///	lower-level rendering method- you have to provide your own buffer, explicitly state the size at which you want to render this scene, give it a render time, and supply an optional dictionary in which the various render passes will be stored
/**
@param b the buffer to render into.  it's your responsibility to make sure that thsi is the appropriate type of buffer (should be a texture)
@param s the size at which you want the scene to render
@param t the time at which you want the scene to render, in seconds
@param d a mutable dictionary, into which the output of the various render passes will be stored
*/
- (void) renderToBuffer:(VVBuffer *)b sized:(VVSIZE)s renderTime:(double)t passDict:(NSMutableDictionary *)d;


@property (assign,readwrite) BOOL throwExceptions;
///    returns a MutLockArray (from VVBasics) of all the image-type (ISFAT_Image) ISFAttrib instances, one for each input in the currently loaded ISF file
@property (readonly) MutLockArray *imageInputs;
@property (readonly) MutLockArray *audioInputs;
@property (readonly) int imageInputsCount;
@property (readonly) int audioInputsCount;

#warning mto-anomes: This is a copy of ISFSceneBase jsonString, it could be removed !
@property (readonly) NSString *jsonSource;


#pragma mark - Could be private ?

- (void) render;

- (void) _assembleShaderSource;
- (NSMutableString *) _assembleShaderSource_VarDeclarations;
- (NSMutableDictionary *) _assembleSubstitutionDict;
- (void) _clearImageImports;
- (void) _renderLock;
- (void) _renderUnlock;
- (void) purgeInputGLTextures;
@end

