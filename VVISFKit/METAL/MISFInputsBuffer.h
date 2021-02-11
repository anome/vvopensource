#import "MISFInputDataType.h"
#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <VVBasics/VVBasics.h>

NS_ASSUME_NONNULL_BEGIN

/// Manages a metal buffer with runtime-defined data schema. Fill buffer data from ISF Attrib inputs. Expected to be
/// only one per metal program
@interface MISFInputsBuffer : NSObject

// First add entries to define your buffer schema
- (void)addEntry:(NSString *)entryName dataType:(MISFInputDataType)type;
// Second, create it. This will lock the buffer schema
- (void)createBufferOnDevice:(id<MTLDevice>)device;
// Third, feed inputs at each frame during rendering
- (void)feedInputs:(MutLockArray *)inputs forRenderEncoder:(id<MTLRenderCommandEncoder>)renderEncoder;

/// --- Strings to insert inside your shader code before compiling it.
- (NSString *)structDefinition;
/// Create variables with the same name than in the struct, and initialise them with their corresponding value inside
/// the struct
- (NSString *)structToVariables;
/// The buffer parameters to insert inside the shader main function parameter list
- (NSString *)bufferParametersStringWithBufferIndex:(NSString *)bufferIndex;

@property(readonly, nonatomic) id<MTLBuffer> buffer;

@end

NS_ASSUME_NONNULL_END
