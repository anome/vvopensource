#ifndef MetalShaderTypes_h
#define MetalShaderTypes_h

#include <simd/simd.h>

// Buffer index values shared between shader and C code to ensure Metal shader buffer inputs match
//   Metal API buffer set calls
typedef enum MetalBitsVertexInputIndex
{
    MetalBitsVertexInputIndexVertices     = 0,
    MetalBitsVertexInputIndexViewportSize =  1,
} MetalBitsVertexInputIndex;

// Texture index values shared between shader and C code to ensure Metal shader buffer inputs match
//   Metal API texture set calls
typedef enum MetalBitsTextureIndex
{
    MetalBitsTextureIndexZero = 0,
    MetalBitsTextureIndexOne = 1,
    MetalBitsTextureIndexTwo = 2,
    MetalBitsTextureIndexThree = 3,
} MetalBitsTextureIndex;

//  This structure defines the layout of each vertex in the array of vertices set as an input to our
//    Metal vertex shader.  Since this header is shared between our .metal shader and C code,
//    we can be sure that the layout of the vertex array in the Ccode matches the layour that
//    our vertex shader expects
typedef struct
{
    //  Positions in pixel space (i.e. a value of 100 indicates 100 pixels from the origin/center)
    vector_float2 position;
    // Floating point RGBA colors
    vector_float4 color;
} MetalBitsColorVertex;

typedef struct
{
    // Positions in pixel space (i.e. a value of 100 indicates 100 pixels from the origin/center)
    vector_float2 position;
    // 2D texture coordinate
    vector_float2 textureCoordinate;
} MetalBitsTextureVertex;

#endif /* MetalShaderTypes_h */
