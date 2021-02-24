#import "MISFTargetBuffer.h"

@implementation MISFTargetBuffer

- (NSString *)description
{
    return VVFMTSTRING(@"<MisfTargetBuffer %@>", name);
}

+ (id)createForDevice:(id<MTLDevice>)device pixelFormat:(MTLPixelFormat)pixelFormat
{
    MISFTargetBuffer *returnMe = [[MISFTargetBuffer alloc] initWithDevice:device pixelFormat:pixelFormat];
    if( returnMe == nil )
    {
        return nil;
    }
    return [returnMe autorelease];
}

+ (id)createForDevice:(id<MTLDevice>)device pixelFormat:(MTLPixelFormat)pixelFormat fromModel:(MISFModelBuffer *)model
{
    MISFTargetBuffer *returnMe = [[MISFTargetBuffer alloc] initWithDevice:device pixelFormat:pixelFormat];
    if( model.name )
    {
        returnMe.name = model.name;
    }
    // Only accept both evals at the same time
    if( model.evalWidth != nil && model.evalHeight != nil )
    {
        [returnMe setTargetWidthString:model.evalWidth];
        [returnMe setTargetHeightString:model.evalHeight];
    }

    if( returnMe == nil )
    {
        return nil;
    }
    return [returnMe autorelease];
}

- (id)initWithDevice:(id<MTLDevice>)theDevice pixelFormat:(MTLPixelFormat)thePixelFormat
{
    if( self = [super init] )
    {
        device = theDevice;
        pixelFormat = thePixelFormat;
        name = nil;
        texture = nil;
        bufferSize = [MISFSize new];
        return self;
    }
    [self release];
    return nil;
}
- (void)dealloc
{
    VVRELEASE(name);
    VVRELEASE(texture);
    VVRELEASE(bufferSize);
    [super dealloc];
}

- (void)clearBuffer
{
    VVRELEASE(texture);
}

@synthesize name;

- (id<MTLTexture>)getBufferTexture
{
    // Verify all aspects of the texture
    if( texture == nil )
    {
        texture = [self createTextureForDevice:device
                                         width:bufferSize.width
                                        height:bufferSize.height
                                   pixelFormat:pixelFormat];
    }
    else
    {
        if( texture.width != bufferSize.width || texture.height != bufferSize.height )
        {
            VVRELEASE(texture);
            texture = [self createTextureForDevice:device
                                             width:bufferSize.width
                                            height:bufferSize.height
                                       pixelFormat:pixelFormat];
        }
    }
    return texture;
}

- (id<MTLTexture>)createTextureForDevice:(id<MTLDevice>)theDevice
                                   width:(int)width
                                  height:(int)height
                             pixelFormat:(MTLPixelFormat)thePixelFormat
{
    MTLTextureDescriptor *textureDescriptor = [MTLTextureDescriptor texture2DDescriptorWithPixelFormat:thePixelFormat
                                                                                                 width:width
                                                                                                height:height
                                                                                             mipmapped:NO];
    textureDescriptor.usage = MTLTextureUsageRenderTarget | MTLTextureUsageShaderRead;
    textureDescriptor.storageMode = MTLStorageModePrivate; // GPU only for better performance
    id<MTLTexture> texture = [theDevice newTextureWithDescriptor:textureDescriptor];
    if( texture == nil )
    {
        NSLog(@"something went wrong");
    }
    return texture;
}

- (void)setTargetWidthString:(NSString *)newWidthString
{
    [bufferSize setWidthString:newWidthString];
}

- (void)setTargetHeightString:(NSString *)newHeightString
{
    [bufferSize setHeightString:newHeightString];
}

- (void)setTargetSize:(VVSIZE)n
{
    [bufferSize setSize:n];
}

//    returns a YES if there's a target width string
- (BOOL)targetSizeNeedsEval
{
    return bufferSize.needsEval;
}

- (void)evalTargetSizeWithSubstitutionsDict:(NSDictionary *)dict
{
    [bufferSize evalSizeWithSubstitutionDict:dict];
}

@end
