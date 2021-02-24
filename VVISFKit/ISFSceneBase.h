#ifndef ISFSceneBase_h
#define ISFSceneBase_h

#import "ISFAttrib.h"

@protocol ISFSceneBase

///    applies the passed value to the input with the passed key
/**
@param n the value you want to pass, as an ISFAttribVal union
@param k the key of the input you want to pass the value to
*/
- (void) setValue:(ISFAttribVal)n forInputKey:(NSString *)k;
///    applies the passed value to the input with the passed key
/**
@param n the value you want to pass, as an NSObject of some sort.  if it's a color, pass NSColor- if it's a point, pass an NSValue created from an NSPoint- if it's an image, pass a VVBuffer- else, pass an NSNumber.
@param k the key of the input you want to pass the value to.
*/
- (void) setNSObjectVal:(id)n forInputKey:(NSString *)k;

///    returns the path of the currently-loaded ISF file
@property (readonly) NSString *filePath;
///    returns the name of the currently-loaded ISF file
@property (readonly) NSString *fileName;
///    returns a string with the description (pulled from the JSON blob) of the ISF file
@property (readonly) NSString *fileDescription;
///    returns a string with the credits (pulled from the JSON blob) of the ISF file
@property (readonly) NSString *fileCredits;
@property (readonly) ISFFunctionality fileFunctionality;
///    returns an array with the category names (as NSStrings) of this ISF file.  pulled from the JSON blob.
@property (readonly) NSArray<NSString*> *categoryNames;
///    returns a MutLockArray (from VVBasics) of ISFAttrib instances, one for each of the inputs
@property (readonly) MutLockArray *inputs;
@property (readonly) VVSIZE renderSize;
@property (readonly) int passCount;
@property (readonly) NSArray<NSString*> *passTargetNames;
@property (readonly) NSString *jsonString;
@property (readonly) NSString *vertShaderSource;
@property (readonly) NSString *fragShaderSource;

///    returns an array with all the inputs matching the passed type (an array of ISFAttrib instances)
/**
@param t the type of attributes you want returned
*/
- (NSMutableArray *) inputsOfType:(ISFAttribValType)t;
///    returns a ptr to the ISFAttrib instance used by this scene which describes the input at the passed key
- (ISFAttrib *) attribForInputWithKey:(NSString *)k;

@end


#endif /* ISFSceneBase_h */
