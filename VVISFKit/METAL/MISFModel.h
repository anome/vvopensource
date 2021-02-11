#import "ISFAttrib.h"
#import "ISFFileManager.h"
#import "ISFRenderPass.h"
#import "ISFTargetBuffer.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

// Model based on GL implementation and ISF docs - Not fully implemented, use with caution!
// Such a model could be implemented directly in the C++ version of ISF and used by all Graphic Pipelines to implement
// their own resources
// It should be entirely readonly with no pointer access whatsoever (IT'S NOT at this point, so pointers are traveling
// around possibly causing memory leaks)

@interface MISFModelBuffer : NSObject

@property(readwrite, retain) NSString *name;
#warning mto-anomes: seems like this is never used (see parsing impl) fix impl or keep it like gl?
//@property (readwrite) NSNumber *width;
//@property (readwrite) NSNumber *height;
@property(readwrite, retain) NSString *evalWidth;
@property(readwrite, retain) NSString *evalHeight;
@property(readwrite) BOOL floatFlag;
@property(readwrite) BOOL persistent;
@property(readonly) BOOL requiresEval;
@end

@interface MISFModelPass : NSObject
@property(readwrite, retain) MISFModelBuffer *targetBuffer;
@end

@interface MISFModelImportedImage : NSObject
@property(readwrite, retain) NSString *path;
@property(readwrite, retain) NSString *name;
@property(readwrite) BOOL cubeFlag;
@end

@interface MISFModel : NSObject

- (instancetype)initWithFilePath:(NSString *)filePath withError:(NSError **)errorPtr;

///    returns a string with the credits (pulled from the JSON blob) of the ISF file
@property(readonly, retain) NSString *credits;
///    returns an array with the category names (as NSStrings) of this ISF file.  pulled from the JSON blob.
@property(readonly, retain) NSArray<NSString *> *categoryNames;
///    returns a string with the description (pulled from the JSON blob) of the ISF file
@property(readonly, retain) NSString *fileDescription;
///    returns the path of the currently-loaded ISF file
@property(readonly, retain) NSString *filePath;
///    returns the name of the currently-loaded ISF file
@property(readonly, retain) NSString *fileName;
@property(readonly, retain) NSString *vertexShader;
@property(readonly, retain) NSString *fragmentShader;
@property(readonly, retain) NSArray<MISFModelBuffer *> *persistentBuffers;
@property(readonly, retain) NSArray<MISFModelPass *> *passes;
@property(readonly, retain) NSArray<MISFModelImportedImage *> *importedImages;
///    returns a MutLockArray (from VVBasics) of ISFAttrib instances, one for each of the inputs
#warning mto-anomes: ISFAttrib is used for model definition and implementation. Don't use these attribs directly, copy them instead using ISFAttrib createFromAttrib:
@property(readonly, retain) NSArray<ISFAttrib *> *inputs;
@property(readonly, retain) NSString *jsonString;
@property(readonly, retain) NSString *fragShaderSource;
@property(readonly, retain) NSString *vertShaderSource;
@property(readonly) BOOL hasVertexShader;

@end

NS_ASSUME_NONNULL_END
