#import "MISFAttribBufferDefinition.h"
#import "MISFInputDataType.h"
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface MISFShaderConverter : NSObject

/// Translations
+ (NSString *)translateFragmentToMetal:(NSString *)glCode
                         inputUniforms:(NSString *)isfInputUniforms
                             withError:(NSError **)errorPtr;
+ (NSString *)translateVertexToMetal:(NSString *)glCode
                       inputUniforms:(NSString *)isfInputUniforms
                           withError:(NSError **)errorPtr;

/// Operations on translated code
+ (NSArray<MISFAttribBufferDefinition *> *)parseVertexBuffers:(NSString *)msl withError:(NSError **)errorPtr;
+ (NSArray<MISFAttribBufferDefinition *> *)parseFragmentBuffers:(NSString *)msl withError:(NSError **)errorPtr;
+ (NSString *)injectCode:(NSString *)codeToInject
        atTopOfMainInMsl:(NSString *)msl
                isVertex:(BOOL)isVertex
               withError:(NSError **)errorPtr;
+ (NSString *)injectCode:(NSString *)codeToInject
         beforeMainInMsl:(NSString *)msl
                isVertex:(BOOL)isVertex
               withError:(NSError **)errorPtr;
+ (NSString *)replaceBuffers:(NSString *)bufferForReplacement
                       inMsl:(NSString *)msl
                    isVertex:(BOOL)isVertex
                   withError:(NSError **)errorPtr;

// Utils
+ (MISFInputDataType)typeOfPartialTypeToken:(NSString *)partialType;
+ (NSString *)stringForMtlDataType:(MISFInputDataType)dataType;
+ (BOOL)isABuiltInIsfVariable:(NSString *)variableName;

@end

NS_ASSUME_NONNULL_END
