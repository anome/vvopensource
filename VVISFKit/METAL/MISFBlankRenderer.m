#import "MISFBlankRenderer.h"

#include <simd/simd.h>

typedef enum MetalBitsVertexInputIndex
{
    MetalBitsVertexInputIndexVertices = 0,
    MetalBitsVertexInputIndexViewportSize = 1,
} MetalBitsVertexInputIndex;

typedef enum MetalBitsTextureIndex
{
    MetalBitsTextureIndexZero = 0,
} MetalBitsTextureIndex;

typedef struct
{
    vector_float2 position;
    vector_float4 color;
} MetalBitsColorVertex;

typedef struct
{
    vector_float2 position;
    vector_float2 textureCoordinate;
} MetalBitsTextureVertex;

NSString *types = @""
                   "#include <simd/simd.h>\n"
                   "typedef enum MetalBitsVertexInputIndex\n"
                   "{"
                   "    MetalBitsVertexInputIndexVertices     = 0,\n"
                   "    MetalBitsVertexInputIndexViewportSize =  1,\n"
                   "} MetalBitsVertexInputIndex;\n"
                   "typedef enum MetalBitsTextureIndex\n"
                   "{"
                   "    MetalBitsTextureIndexZero = 0,\n"
                   "} MetalBitsTextureIndex;\n"
                   "typedef struct\n"
                   "{"
                   "    vector_float2 position;\n"
                   "    vector_float4 color;\n"
                   "} MetalBitsColorVertex;\n"
                   "typedef struct\n"
                   "{"
                   "    vector_float2 position;\n"
                   "    vector_float2 textureCoordinate;\n"
                   "} MetalBitsTextureVertex;\n";

NSString *shaderCode = @""
                        "#include <metal_stdlib>\n"
                        "#include <simd/simd.h>\n"
                        "using namespace metal;\n"
                        "typedef struct\n"
                        "{"
                        "    float4 clipSpacePosition [[position]];\n"
                        "    float4 color;\n"
                        "    float2 textureCoordinate;\n"
                        "} RasterizerData;\n"
                        "vertex RasterizerData ISF_colorVertexShader(uint vertexID [[ vertex_id ]], constant "
                        "MetalBitsTextureVertex *vertexArray [[ buffer(MetalBitsVertexInputIndexVertices) ]], constant "
                        "vector_uint2 *viewportSizePointer  [[ buffer(MetalBitsVertexInputIndexViewportSize) ]]) \n"
                        "{\n"
                        "RasterizerData out;\n"
                        "float2 pixelSpacePosition = vertexArray[vertexID].position.xy;\n"
                        "float2 viewportSize = float2(*viewportSizePointer);\n"
                        "out.clipSpacePosition.xy = pixelSpacePosition / (viewportSize / 2.0);\n"
                        "out.clipSpacePosition.z = 0.0;\n"
                        "out.clipSpacePosition.w = 1.0;\n"
                        "out.textureCoordinate = vertexArray[vertexID].textureCoordinate;\n"
                        "return out;\n"
                        "}\n"

                        "fragment float4 ISF_colorSamplingShader()\n"
                        "{\n"
                        "   return float4(0.0,0.0,0.0,0.0);\n"
                        "}";

@implementation MISFBlankRenderer
{
    id<MTLRenderPipelineState> pipelineState;
}

- (nonnull instancetype)initWithDevice:(id<MTLDevice>)device colorPixelFormat:(MTLPixelFormat)colorPixelFormat
{
    self = [super init];
    if( self )
    {
        NSError *error = NULL;

        MTLCompileOptions *compileOptions = [MTLCompileOptions new];
        compileOptions.languageVersion = MTLLanguageVersion1_1;
        NSString *SHADER_CODE = [types stringByAppendingString:shaderCode];
        id<MTLLibrary> defaultLibrary = [device newLibraryWithSource:SHADER_CODE options:compileOptions error:&error];
        if( error )
        {
            NSLog(@"SHADER COMPILER STATUS (error or warnings):%@", error);
        }
        else
        {
            // NSLog(@"compiled perfectly!");
        }

        // Load the vertex/fragment functions from the library
        id<MTLFunction> vertexFunction = [defaultLibrary newFunctionWithName:@"ISF_colorVertexShader"];
        id<MTLFunction> fragmentFunction = [defaultLibrary newFunctionWithName:@"ISF_colorSamplingShader"];

        // Set up a descriptor for creating a pipeline state object
        MTLRenderPipelineDescriptor *pipelineStateDescriptor = [[MTLRenderPipelineDescriptor alloc] init];
        pipelineStateDescriptor.vertexFunction = vertexFunction;
        pipelineStateDescriptor.fragmentFunction = fragmentFunction;
        pipelineStateDescriptor.colorAttachments[0].pixelFormat = colorPixelFormat;
        pipelineState = [device newRenderPipelineStateWithDescriptor:pipelineStateDescriptor error:&error];

        if( !pipelineState )
        {
            NSLog(@"Failed to created screen2Tex pipeline state, error %@", error);
            return nil;
        }
    }
    return self;
}

- (void)renderBlankOnTexture:(id<MTLTexture>)textureToBlank onCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
{
    vector_uint2 _viewportSize;
    _viewportSize.x = textureToBlank.width;
    _viewportSize.y = textureToBlank.height;

    const float w = textureToBlank.width / 2;
    const float h = textureToBlank.height / 2;

    const MetalBitsTextureVertex quadVertices[] = {
        // Pixel positions, Texture coordinates
        {{w, h}, {1.f, 1.f}}, {{-w, h}, {0.f, 1.f}},  {{-w, -h}, {0.f, 0.f}},

        {{w, h}, {1.f, 1.f}}, {{-w, -h}, {0.f, 0.f}}, {{w, -h}, {1.f, 0.f}},
    };
    NSUInteger numberOfVertices = sizeof(quadVertices) / sizeof(MetalBitsTextureVertex);

    MTLRenderPassDescriptor *renderPassDescriptor = [MTLRenderPassDescriptor renderPassDescriptor];
    renderPassDescriptor.colorAttachments[0].texture = textureToBlank;
    renderPassDescriptor.colorAttachments[0].loadAction = MTLLoadActionClear;
    renderPassDescriptor.colorAttachments[0].clearColor = MTLClearColorMake(0.0, 0.0, 0.0, 0.0);

    // Create a render command encoder so we can render into something
    id<MTLRenderCommandEncoder> renderEncoder = [commandBuffer renderCommandEncoderWithDescriptor:renderPassDescriptor];
    //    [renderEncoder setViewport:viewport];
    [renderEncoder setRenderPipelineState:pipelineState];
    [renderEncoder setVertexBytes:quadVertices length:sizeof(quadVertices) atIndex:MetalBitsVertexInputIndexVertices];
    [renderEncoder setVertexBytes:&_viewportSize
                           length:sizeof(_viewportSize)
                          atIndex:MetalBitsVertexInputIndexViewportSize];
    [renderEncoder setFragmentTexture:textureToBlank atIndex:MetalBitsTextureIndexZero];
    [renderEncoder drawPrimitives:MTLPrimitiveTypeTriangle vertexStart:0 vertexCount:numberOfVertices];
    [renderEncoder endEncoding];
}

@end
