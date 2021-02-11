#import <Metal/Metal.h>

#ifndef MisfDataType_h
#define MisfDataType_h

/// Copy of MTLDataType with texture and sampler types from 10.13 SDK
typedef NS_ENUM(NSUInteger, MISFInputDataType) {

    MisfDataTypeFloat = MTLDataTypeFloat,
    MisfDataTypeFloat2 = MTLDataTypeFloat2,
    MisfDataTypeFloat3 = MTLDataTypeFloat3,
    MisfDataTypeFloat4 = MTLDataTypeFloat4,

    MisfDataTypeInt = MTLDataTypeInt,

    MisfDataTypeBool = MTLDataTypeBool,
    MisfDataTypeTexture = 200001,
    MisfDataTypeSampler = 200002,
};

#endif /* MisfDataType_h */
