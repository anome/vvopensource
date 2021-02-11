#import "MISFInputsBuffer.h"
#import "ISFAttrib.h"
#import "MISFShaderConverter.h"
#include <simd/simd.h>

// The maximum number of a single type entry. We use arrays instead of single variables to bypass the 14 parameters
// limit for fragment/vertex prototype methods. A better solution would be to use runtime-defined Argument Buffers but
// it's not available on Metal v1
static const int INPUTS_BUFFER_ARRAY_SIZES = 20;
static NSString *const STRUCT_DEFINITION_NAME = @"IsfInputsBufferType";
static NSString *const BUFFER_INSTANCE_NAME = @"isf_inputs";

typedef struct
{
    float ISF_FLOATS[INPUTS_BUFFER_ARRAY_SIZES];
    // Note: using ints instead of long because long are not handled in metal
    // before 2.2
    int ISF_LONGS[INPUTS_BUFFER_ARRAY_SIZES];
    vector_float4 ISF_COLORS[INPUTS_BUFFER_ARRAY_SIZES];
    vector_float2 ISF_POINT2DS[INPUTS_BUFFER_ARRAY_SIZES];
    bool ISF_BOOLS[INPUTS_BUFFER_ARRAY_SIZES];
#warning mto-anomes TODO: event, audio, image
} IsfInputsBufferType;

static const size_t BUFFER_ALLOCATION_SIZE = sizeof(IsfInputsBufferType);

@interface MISFInputsBufferScalarEntry : NSObject
@property(readwrite, nonatomic, assign) NSString *arrayNameInBuffer;
@property(readwrite, nonatomic, assign) int arrayIndex;
@property(readwrite, nonatomic, assign) MISFInputDataType dataType;
@end

@implementation MISFInputsBufferScalarEntry
@end

@interface MISFInputsBufferTextureEntry : NSObject
@property(readwrite, nonatomic, assign) int textureBufferIndex;
@end

@implementation MISFInputsBufferTextureEntry
@end

@implementation MISFInputsBuffer
{
    // More than 14 elements will cause Metal compilation to fail
    NSMutableDictionary<NSString *, MISFInputsBufferScalarEntry *> *scalarEntries;
    NSMutableDictionary<NSString *, MISFInputsBufferTextureEntry *> *textureEntries;
    // Samplers are not sent via buffers but created in msl at top of vertex/fragment
    // programs. So we need to store only the sampler names.
    NSMutableArray<NSString *> *samplerNames;

    int floatBufferUsageCounter;
    int intBufferUsageCounter;
    int colorBufferUsageCounter;
    int pointBufferUsageCounter;
    int boolBufferUsageCounter;
    int textureUsageCounter;
    // Forbid adding new entries once the buffer has been created
    bool lockSchema;
    // Metal resources
    IsfInputsBufferType bufferCpuData;
    id<MTLBuffer> _buffer;
}

#pragma mark Init
- (id)init
{
    self = [super init];
    if( self )
    {
        floatBufferUsageCounter = 0;
        intBufferUsageCounter = 0;
        colorBufferUsageCounter = 0;
        pointBufferUsageCounter = 0;
        boolBufferUsageCounter = 0;
        // First slot is used by output texture
        textureUsageCounter = 1;
        lockSchema = NO;
        _buffer = nil;
        scalarEntries = [NSMutableDictionary<NSString *, MISFInputsBufferScalarEntry *> new];
        textureEntries = [NSMutableDictionary<NSString *, MISFInputsBufferTextureEntry *> new];
        samplerNames = [NSMutableArray<NSString *> new];

        // Initialise cpu data with default values
        const simd_float4 defaultColor = {1.f, 0.f, 0.f, 1.f};
        const vector_float2 defaultPoint = {0.0f, 0.0f};
        const float defaultFloat = 0.;
        const float defaultLong = 0.;
        const bool defaultBool = false;
        for( int index = 0; index < INPUTS_BUFFER_ARRAY_SIZES; index++ )
        {
            bufferCpuData.ISF_LONGS[index] = defaultLong;
            bufferCpuData.ISF_FLOATS[index] = defaultFloat;
            bufferCpuData.ISF_COLORS[index] = defaultColor;
            bufferCpuData.ISF_POINT2DS[index] = defaultPoint;
            bufferCpuData.ISF_BOOLS[index] = defaultBool;
        }
    }
    return self;
}

- (void)addEntry:(NSString *)entryName dataType:(MISFInputDataType)dataType
{
    if( lockSchema )
    {
        NSLog(@"WARN: tried to add entry but entries have been locked already !");
        return;
    }

    // SPECIFIC CASE FOR IMAGES
    if( dataType == MisfDataTypeTexture )
    {
        MISFInputsBufferTextureEntry *property = [MISFInputsBufferTextureEntry new];
        property.textureBufferIndex = textureUsageCounter;
#warning mto-anomes: this value has no limit, can cause metal compilation error if there's just too much (100+ limit)
        textureUsageCounter += 1;
        [textureEntries setValue:property forKey:entryName];
        return;
    }
    if( dataType == MisfDataTypeSampler )
    {
        [samplerNames addObject:entryName];
        return;
    }

    MISFInputsBufferScalarEntry *property = [MISFInputsBufferScalarEntry new];

    switch( dataType )
    {
    case MisfDataTypeFloat:
    {
        if( INPUTS_BUFFER_ARRAY_SIZES < floatBufferUsageCounter )
        {
            NSLog(@"WARN: float array in buffer is full. value `%@` will be ignored", entryName);
            break;
        }
        NSString *arrayNameInBuffer = [NSString stringWithFormat:@"ISF_FLOATS[%i]", floatBufferUsageCounter];
        property.arrayNameInBuffer = arrayNameInBuffer;
        property.arrayIndex = floatBufferUsageCounter;
        property.dataType = dataType;
        [scalarEntries setValue:property forKey:entryName];
        floatBufferUsageCounter += 1;
        break;
    }
    case MisfDataTypeInt:
    {
        if( INPUTS_BUFFER_ARRAY_SIZES < floatBufferUsageCounter )
        {
            NSLog(@"WARN: int array in buffer is full. value `%@` will be ignored", entryName);
            break;
        }
        NSString *arrayNameInBuffer = [NSString stringWithFormat:@"ISF_LONGS[%i]", intBufferUsageCounter];
        property.arrayNameInBuffer = arrayNameInBuffer;
        property.arrayIndex = intBufferUsageCounter;
        property.dataType = dataType;
        [scalarEntries setValue:property forKey:entryName];
        intBufferUsageCounter += 1;
        break;
    }
    case MisfDataTypeFloat4:
    {
        if( INPUTS_BUFFER_ARRAY_SIZES < colorBufferUsageCounter )
        {
            NSLog(@"WARN: float4 array in buffer is full. value `%@` will be ignored", entryName);
            break;
        }
        NSString *arrayNameInBuffer = [NSString stringWithFormat:@"ISF_COLORS[%i]", colorBufferUsageCounter];
        property.arrayNameInBuffer = arrayNameInBuffer;
        property.arrayIndex = colorBufferUsageCounter;
        property.dataType = dataType;
        [scalarEntries setValue:property forKey:entryName];
        colorBufferUsageCounter += 1;
        break;
    }
    case MisfDataTypeFloat2:
    {
        if( INPUTS_BUFFER_ARRAY_SIZES < pointBufferUsageCounter )
        {
            NSLog(@"WARN: float2 array in buffer is full. value `%@` will be ignored", entryName);
            break;
        }
        NSString *arrayNameInBuffer = [NSString stringWithFormat:@"ISF_POINT2DS[%i]", pointBufferUsageCounter];
        property.arrayNameInBuffer = arrayNameInBuffer;
        property.arrayIndex = pointBufferUsageCounter;
        property.dataType = dataType;
        [scalarEntries setValue:property forKey:entryName];
        pointBufferUsageCounter += 1;
        break;
    }
    case MisfDataTypeBool:
    {
        if( INPUTS_BUFFER_ARRAY_SIZES < boolBufferUsageCounter )
        {
            NSLog(@"WARN: bool array in buffer is full. value `%@` will be ignored", entryName);
            break;
        }
        NSString *arrayNameInBuffer = [NSString stringWithFormat:@"ISF_BOOLS[%i]", boolBufferUsageCounter];
        property.arrayNameInBuffer = arrayNameInBuffer;
        property.arrayIndex = boolBufferUsageCounter;
        property.dataType = dataType;
        [scalarEntries setValue:property forKey:entryName];
        boolBufferUsageCounter += 1;
        break;
    }
    default:
    {
#warning mto-anomes TODO: handle all types!
        NSLog(@"Type %lu not handled for entry `%@`", dataType, entryName);
        break;
    }
    }
}

#pragma mark Buffer work
- (void)createBufferOnDevice:(id<MTLDevice>)device
{
    lockSchema = YES;
    _buffer = [device newBufferWithLength:BUFFER_ALLOCATION_SIZE options:MTLResourceStorageModeShared];
}

- (void)feedInputs:(MutLockArray *)inputs forRenderEncoder:(id<MTLRenderCommandEncoder>)renderEncoder
{
    [inputs rdlock];
    for( ISFAttrib *attrib in [inputs array] )
    {
        NSString *attribName = attrib.attribName;
        ISFAttribValType attribType = attrib.attribType;

        // SPECIFIC CASE FOR IMAGES
        if( attribType == ISFAT_Image )
        {
            id<MTLTexture> theImage = attrib.currentVal.metalImageVal;
            if( theImage == nil )
            {
                NSLog(@"ERR: missing MTLTexture. Skip image input `%@`.", attribName);
                continue;
            }

            MISFInputsBufferTextureEntry *property = [textureEntries objectForKey:attribName];
            if( property == nil )
            {
                //                NSLog(@"WARN: No match for texture entry `%@` inside IsfInputBuffer. Could be caused
                //                by a defined ISF "
                //                      @"input but unused",
                //                      attribName);
            }
            else
            {
                [renderEncoder setFragmentTexture:theImage atIndex:property.textureBufferIndex];
            }
            continue;
        }

        MISFInputsBufferScalarEntry *scalarEntry = [scalarEntries objectForKey:attribName];
        if( scalarEntry == nil )
        {
            //            NSLog(@"WARN: No match for scalar entry `%@` inside IsfInputBuffer. Could be caused by a
            //            defined ISF input "
            //                  @"but unused",
            //                  attribName);
            continue;
        }

        const int index = scalarEntry.arrayIndex;
        if( INPUTS_BUFFER_ARRAY_SIZES <= index )
        {
            NSLog(@"WARN: Scalar entry index (%i) is above hard-coded limit (%i) for %@. Ignored.", index,
                  INPUTS_BUFFER_ARRAY_SIZES, attribName);
            continue;
        }

        const ISFAttribVal attribVal = [attrib currentVal];
        switch( attribType )
        {
        case ISFAT_Float:
        {
            bufferCpuData.ISF_FLOATS[index] = attribVal.floatVal;
            break;
        }
        case ISFAT_Bool:
        {
            bufferCpuData.ISF_BOOLS[index] = attribVal.boolVal;
            break;
        }
        case ISFAT_Long:
        {
            // longs are converted to ints, longs not handled in msl before 2.2
            bufferCpuData.ISF_LONGS[index] = (int)attribVal.longVal;
            break;
        }
        case ISFAT_Color:
        {
            const vector_float4 color = simd_make_float4(attribVal.colorVal[0], attribVal.colorVal[1],
                                                         attribVal.colorVal[2], attribVal.colorVal[3]);
            bufferCpuData.ISF_COLORS[index] = color;
            break;
        }
        case ISFAT_Point2D:
        {
            const vector_float2 point = simd_make_float2(attribVal.point2DVal[0], attribVal.point2DVal[1]);
            bufferCpuData.ISF_POINT2DS[index] = point;
            break;
        }
        default:
        {
            NSLog(@"WARN: attrib type %lu not handled", attribType);
            break;
        }
        }
    }
    [inputs unlock];
    IsfInputsBufferType *pointer = _buffer.contents;
    *pointer = bufferCpuData;
}

#pragma mark Definitions

- (NSString *)structDefinition
{
    return [NSString stringWithFormat:@""
                                       "typedef struct\n"
                                       "{\n"
                                       "    float ISF_FLOATS[%i];\n"
                                       "    int ISF_LONGS[%i]; \n"
                                       "    vector_float4 ISF_COLORS[%i];\n"
                                       "    vector_float2 ISF_POINT2DS[%i];\n"
                                       "    bool ISF_BOOLS[%i];\n"
                                       "} %@;\n",
                                      INPUTS_BUFFER_ARRAY_SIZES, INPUTS_BUFFER_ARRAY_SIZES, INPUTS_BUFFER_ARRAY_SIZES,
                                      INPUTS_BUFFER_ARRAY_SIZES, INPUTS_BUFFER_ARRAY_SIZES, STRUCT_DEFINITION_NAME];
}

- (NSString *)structToVariables
{
    NSString *variableDeclarations = @"";
    // Instanciate variable with original name from variable from struct buffer data
    for( NSString *entryName in scalarEntries )
    {
        MISFInputsBufferScalarEntry *property = scalarEntries[entryName];
        MISFInputDataType dataType = property.dataType;
        NSString *type = [MISFShaderConverter stringForMtlDataType:dataType];
        NSString *arrayNameInBuffer = property.arrayNameInBuffer;
        NSString *declarationLine =
            [NSString stringWithFormat:@"%@ %@ = %@.%@;\n", type, entryName, BUFFER_INSTANCE_NAME, arrayNameInBuffer];
        variableDeclarations = [variableDeclarations stringByAppendingString:declarationLine];
    }
    // Create samplers
    for( NSString *samplerName in samplerNames )
    {
        NSString *declarationLine = [NSString
            stringWithFormat:@"constexpr sampler %@ (mag_filter::linear, min_filter::linear, s_address::clamp_to_zero, "
                             @"t_address::clamp_to_zero,  r_address::clamp_to_zero);\n",
                             samplerName];
        variableDeclarations = [variableDeclarations stringByAppendingString:declarationLine];
    }
    return variableDeclarations;
}

- (NSString *)bufferParametersStringWithBufferIndex:(NSString *)bufferIndex;
{
    NSString *scalarInputBuffer =
        [NSString stringWithFormat:@"const device %@ &%@", STRUCT_DEFINITION_NAME, BUFFER_INSTANCE_NAME];
    NSString *textureInputBuffers = @"";
    for( NSString *textureName in textureEntries )
    {
        MISFInputsBufferTextureEntry *property = textureEntries[textureName];
        if( property == nil )
        {
            NSLog(@"WARN: unexpected missing property with name'%@' from texture Entries. Ignoring it.", textureName);
            continue;
        }
        NSString *declaration = [NSString
            stringWithFormat:@"texture2d<float> %@ [[texture(%i)]], ", textureName, property.textureBufferIndex];
        textureInputBuffers = [textureInputBuffers stringByAppendingString:declaration];
    }
    NSString *bufferParametersString =
        [NSString stringWithFormat:@"%@ %@ [[ %@ ]]", textureInputBuffers, scalarInputBuffer, bufferIndex];
    return bufferParametersString;
}

@end
