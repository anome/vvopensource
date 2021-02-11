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
#warning mto-anomes: possible edge case: if only one eval string is given and not two
    if( model.evalWidth )
    {
        [returnMe setTargetWidthString:model.evalWidth];
    }
    if( model.evalHeight )
    {
        [returnMe setTargetWidthString:model.evalHeight];
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
        floatFlag = NO;
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

- (void)setFloatFlag:(BOOL)n
{
    // NSLog(@"%s ... %d",__func__,n);
    BOOL changed = (floatFlag == n) ? NO : YES;
    if( !changed )
    {
        return;
    }
    floatFlag = n;
}
- (BOOL)floatFlag
{
    return floatFlag;
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
#warning mto-anomes: float flag is ignored (no check and no allocation for it)
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
    [bufferSize evalWithSubstitutionsDict:dict];
}

@end
