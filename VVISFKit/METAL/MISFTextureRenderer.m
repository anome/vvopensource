#import "MISFTextureRenderer.h"
#include <simd/simd.h>

// Buffer index values shared between shader and C code to ensure Metal shader buffer inputs match
//   Metal API buffer set calls
typedef enum MetalBitsVertexInputIndex
{
    MetalBitsVertexInputIndexVertices = 0,
    MetalBitsVertexInputIndexViewportSize = 1,
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

static NSString *const SHADER_CODE =
    @""

     "#include <simd/simd.h>\n"
     "#include <metal_stdlib>\n"
     "using namespace metal;\n"

     // Buffer index values shared between shader and C code to ensure Metal shader buffer inputs match
     //   Metal API buffer set calls
     "typedef enum MetalBitsVertexInputIndex\n"
     "{\n"
     "    MetalBitsVertexInputIndexVertices     = 0,\n"
     "    MetalBitsVertexInputIndexViewportSize =  1,\n"
     "} MetalBitsVertexInputIndex;\n"

     // Texture index values shared between shader and C code to ensure Metal shader buffer inputs match
     //   Metal API texture set calls
     "typedef enum MetalBitsTextureIndex\n"
     "{"
     "    MetalBitsTextureIndexZero = 0,\n"
     "    MetalBitsTextureIndexOne = 1,\n"
     "    MetalBitsTextureIndexTwo = 2,\n"
     "    MetalBitsTextureIndexThree = 3,\n"
     "} MetalBitsTextureIndex;\n"

     //  This structure defines the layout of each vertex in the array of vertices set as an input to our
     //    Metal vertex shader.  Since this header is shared between our .metal shader and C code,
     //    we can be sure that the layout of the vertex array in the Ccode matches the layour that
     //    our vertex shader expects
     "typedef struct\n"
     "{\n"
     //  Positions in pixel space (i.e. a value of 100 indicates 100 pixels from the origin/center)
     "    vector_float2 position;\n"
     // Floating point RGBA colors
     "    vector_float4 color;\n"
     "} MetalBitsColorVertex;\n"

     "typedef struct\n"
     "{\n"
     // Positions in pixel space (i.e. a value of 100 indicates 100 pixels from the origin/center)
     "    vector_float2 position;\n"
     // 2D texture coordinate
     "    vector_float2 textureCoordinate;\n"
     "} MetalBitsTextureVertex;\n"

     "typedef struct\n"
     "{\n"
     "    float4 clipSpacePosition [[position]];\n"
     "    float4 color;\n"
     "    float2 textureCoordinate;\n"
     "   float2 pixelSpacePosition;"
     "} RasterizerData;\n"

     "vertex RasterizerData \n"
     "ISF_textureToScreenVertexShader(uint vertexID [[ vertex_id ]],\n"
     "                            constant MetalBitsTextureVertex *vertexArray [[ "
     "buffer(MetalBitsVertexInputIndexVertices) ]],\n"
     "                            constant vector_uint2 *viewportSizePointer  [[ "
     "buffer(MetalBitsVertexInputIndexViewportSize) ]])\n"
     "{\n"
     "    RasterizerData out;\n"
     "    float2 pixelSpacePosition = vertexArray[vertexID].position.xy;\n"
     "    float2 viewportSize = float2(*viewportSizePointer);\n"
     "    out.clipSpacePosition.xy = pixelSpacePosition / (viewportSize / 2.0);\n"
     "    out.clipSpacePosition.z = 0.0;\n"
     "    out.clipSpacePosition.w = 1.0;\n"
     "    out.textureCoordinate = vertexArray[vertexID].textureCoordinate;\n"
     "   out.pixelSpacePosition = vertexArray[vertexID].position.xy;\n"
     "    return out;\n"
     "}\n"

#warning mto-anomes: here, the isf final render should not be stretched. If the render is smaller, it should draw in real size in bottom left corner
     "fragment float4 \n"
     "ISF_textureToScreenSamplingShader(RasterizerData in [[stage_in]],\n"
     "                              texture2d<half> colorTexture [[ texture(MetalBitsTextureIndexZero) ]])\n"
     "{\n"
     "    constexpr sampler pixelTextureSampler (mag_filter::linear,\n"
     "                                      min_filter::linear, coord::pixel, s_address::clamp_to_zero, "
     "t_address::clamp_to_zero,  r_address::clamp_to_zero);\n"
     "    constexpr sampler textureSampler (mag_filter::linear,\n"
     "                                      min_filter::linear, s_address::clamp_to_zero, t_address::clamp_to_zero,  "
     "r_address::clamp_to_zero);\n"
     "   const float2 pixelTextureCoordinate = float2(colorTexture.get_width(), colorTexture.get_height());"
     //"    const half4 colorSample = colorTexture.sample(pixelTextureSampler, in.pixelSpacePosition);\n"
     "    const half4 colorSample = colorTexture.sample(textureSampler, in.textureCoordinate);\n"
     "    return float4(colorSample);\n"
     "}\n";

// Note: this class wont work properly with alpha things because for some reason the colorTarget is already filled with
// the texture we want to render Maybe because we use tow different mtllibraries, metal cant order commands properly?
// Using index 4 = clean result. Using index=0 = blending over something else !
// Both cases, using LoadClear = we get nothing.
@implementation MISFTextureRenderer
{
    vector_uint2 _viewportSize;
    id<MTLRenderPipelineState> pipeline;
}

- (instancetype)initWithDevice:(id<MTLDevice>)device colorPixelFormat:(MTLPixelFormat)colorPixelFormat
{
    return [self initWithDevice:device
                     colorPixelFormat:colorPixelFormat
                       customFragment:nil
        numberOfExtraColorAttachments:0];
}

- (instancetype)initWithDevice:(id<MTLDevice>)device
                 colorPixelFormat:(MTLPixelFormat)colorPixelFormat
                   customFragment:(id<MTLFunction>)customFragment
    numberOfExtraColorAttachments:(int)theNumberOfExtraColorAttachments
{
    self = [super init];
    if( self )
    {
        NSError *error = NULL;

        MTLCompileOptions *compileOptions = [MTLCompileOptions new];
        compileOptions.languageVersion = MTLLanguageVersion1_1;
        id<MTLLibrary> defaultLibrary = [device newLibraryWithSource:SHADER_CODE options:compileOptions error:&error];
        if( error )
        {
            NSLog(@"SHADER COMPILER STATUS (error or warnings):%@", error);
        }
        else
        {
            NSLog(@"compiled perfectly!");
        }

        // Load the vertex/fragment functions from the library
        id<MTLFunction> vertexFunction = [defaultLibrary newFunctionWithName:@"ISF_textureToScreenVertexShader"];
        id<MTLFunction> fragmentFunction = [defaultLibrary newFunctionWithName:@"ISF_textureToScreenSamplingShader"];
        // Set up a descriptor for creating a pipeline state object
        MTLRenderPipelineDescriptor *pipelineStateDescriptor = [[MTLRenderPipelineDescriptor alloc] init];
        pipelineStateDescriptor.label = @"Texture Renderer Pipeline State Descriptor";
        pipelineStateDescriptor.vertexFunction = vertexFunction;
        pipelineStateDescriptor.fragmentFunction = fragmentFunction;
        pipelineStateDescriptor.colorAttachments[0].pixelFormat = colorPixelFormat;

        pipeline = [device newRenderPipelineStateWithDescriptor:pipelineStateDescriptor error:&error];

        if( !pipeline )
        {
            NSLog(@"Failed to created pipeline state, error %@", error);
            return nil;
        }

        self.flip = YES;
        self.clearColor = MTLClearColorMake(0, 0, 0, 0);
    }
    return self;
}

- (void)renderFromTexture:(id<MTLTexture>)offScreenTexture
                inTexture:(id<MTLTexture>)texture
          onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
{
    const MTLViewport viewport = (MTLViewport){0.0, 0.0, offScreenTexture.width, offScreenTexture.height, -1.0, 1.0};

    if( offScreenTexture == nil )
    {
        NSLog(@"Render aborted. offscreen texture is nil");
        return;
    }
    if( texture == nil )
    {
        NSLog(@"Render aborted. output texture is nil");
        return;
    }

    _viewportSize.x = viewport.width;
    _viewportSize.y = viewport.height;

    const float w = viewport.width / 2;
    const float h = viewport.height / 2;
    const float flipValue = self.flip ? -1 : 1;

    const MetalBitsTextureVertex quadVertices[] = {
        // Pixel positions, Texture coordinates
        {{w, flipValue * h}, {1.f, 1.f}}, {{-w, flipValue * h}, {0.f, 1.f}},  {{-w, flipValue * -h}, {0.f, 0.f}},

        {{w, flipValue * h}, {1.f, 1.f}}, {{-w, flipValue * -h}, {0.f, 0.f}}, {{w, flipValue * -h}, {1.f, 0.f}},
    };

    const NSUInteger numberOfVertices = sizeof(quadVertices) / sizeof(MetalBitsTextureVertex);
    MTLRenderPassDescriptor *renderPassDescriptor = [MTLRenderPassDescriptor renderPassDescriptor];
    renderPassDescriptor.colorAttachments[0].texture = texture;
    //        renderPassDescriptor.colorAttachments[4].loadAction = MTLLoadActionClear;
    renderPassDescriptor.colorAttachments[0].loadAction =
        MTLLoadActionClear; // Using the load makes sure metal waits for other passes to be finished...
    renderPassDescriptor.colorAttachments[0].storeAction = MTLStoreActionStore;
    renderPassDescriptor.colorAttachments[0].clearColor = self.clearColor;

    // Create a render command encoder so we can render into something
    id<MTLRenderCommandEncoder> renderEncoder = [commandBuffer renderCommandEncoderWithDescriptor:renderPassDescriptor];
    renderEncoder.label = @"ISF Texture Renderer Render Encoder";
    [renderEncoder setViewport:viewport];
    [renderEncoder setRenderPipelineState:pipeline];
    [renderEncoder setVertexBytes:quadVertices length:sizeof(quadVertices) atIndex:MetalBitsVertexInputIndexVertices];
    [renderEncoder setVertexBytes:&_viewportSize
                           length:sizeof(_viewportSize)
                          atIndex:MetalBitsVertexInputIndexViewportSize];
    [renderEncoder
        setFragmentTexture:offScreenTexture
                   atIndex:MetalBitsTextureIndexZero]; // Apparently he doesnt care about this line to wait for others
    [renderEncoder drawPrimitives:MTLPrimitiveTypeTriangle vertexStart:0 vertexCount:numberOfVertices];
    [renderEncoder endEncoding];
}

@end
