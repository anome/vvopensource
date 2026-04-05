#import "MISFTargetBuffer.h"
#import "MISFTexturePoolsManager.h"


@interface MISFTargetBuffer()
@property (readwrite, retain) NSString *callerId;
@end



@implementation MISFTargetBuffer

- (NSString *)description
{
    return VVFMTSTRING(@"<MisfTargetBuffer %@>", name);
}

+ (id)createForDevice:(id<MTLDevice>)device pixelFormat:(MTLPixelFormat)pixelFormat
{
    MISFTargetBuffer *returnMe = [[MISFTargetBuffer alloc] initWithDevice:device pixelFormat:pixelFormat];
    returnMe.isPersistent = NO;
    if( returnMe == nil )
    {
        return nil;
    }
    return returnMe;
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
    returnMe.isPersistent = model.persistent;
    return returnMe;
}

- (id)initWithDevice:(id<MTLDevice>)theDevice pixelFormat:(MTLPixelFormat)thePixelFormat
{
    if( self = [super init] )
    {
        device = theDevice;
        pixelFormat = thePixelFormat;
        name = nil;
        readonlyTexture = nil;
        self.texture = nil;
        self.isPersistent = NO;
        bufferSize = [MISFSize new];
        blankRenderer = [[MISFBlankRenderer alloc] initWithDevice:device
                                                                 colorPixelFormat:pixelFormat];
        textureRenderer = [[MISFTextureRenderer alloc] initWithDevice:device
                                                    colorPixelFormat:pixelFormat];
        self.callerId = [[NSUUID UUID] UUIDString];
        return self;
    }
    return nil;
}

- (void)dealloc
{
    VVRELEASE(name);
    VVRELEASE(_texture);
    VVRELEASE(readonlyTexture);
    VVRELEASE(bufferSize);
    VVRELEASE(blankRenderer);
    VVRELEASE(textureRenderer);
}

- (void)clearBuffer
{
    VVRELEASE(_texture);
    VVRELEASE(readonlyTexture);
}

@synthesize name;

- (id<MTLTexture>)getBufferTextureWithCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
{
    // Verify all aspects of the texture
    if( self.texture == nil )
    {
#warning mto-anomes : double retain - possible memory leak?
        self.texture = [self createTextureForDevice:device
                                              width:bufferSize.width
                                             height:bufferSize.height
                                        pixelFormat:pixelFormat];

        // Init texture with blank data, because it might be read before any render occurs on it
        [blankRenderer renderBlankOnTexture:self.texture onCommandBuffer:commandBuffer];
    }
    else
    {
        // If resize
        if( self.texture.width != bufferSize.width || self.texture.height != bufferSize.height )
        {
            if( self.isPersistent )
            {
                id<MTLTexture> pooledTexture = [[MISFTexturePoolsManager sharedManager] acquireTextureWithWidth:bufferSize.width
                                                                        height:bufferSize.height
                                                                   pixelFormat:pixelFormat
                                                                        device:device
                withCallerId:self.callerId]; // owned
                if( pooledTexture )
                {
                    [[MISFTexturePoolsManager sharedManager] recycleTexture:self.texture withCalledId:self.callerId];
                    self.texture = pooledTexture;
                }
                else
                {
                    id<MTLTexture> newTexture = [self createTextureForDevice:device
                                                                       width:bufferSize.width
                                                                      height:bufferSize.height
                                                                 pixelFormat:pixelFormat]; // owned
                    // Make sure it's not de-allocated before completedHandler
                    id<MTLTexture> __block oldTexture = self.texture;
                    // Init texture with blank data, because it might be read before any render occurs on it
                    [blankRenderer renderBlankOnTexture:self.texture onCommandBuffer:commandBuffer];
                    [commandBuffer addCompletedHandler:^(id<MTLCommandBuffer> _Nonnull _) {
                        [[MISFTexturePoolsManager sharedManager] recycleTexture:oldTexture withCalledId:self.callerId];

                    }];
                }
            }
            // non-persistent case
            else
            {
                id<MTLTexture> newTexture = [self createTextureForDevice:device
                                                                   width:bufferSize.width
                                                                  height:bufferSize.height
                                                             pixelFormat:pixelFormat];
                self.texture = newTexture;
            }
        }
    }
    
    return self.texture;
}

- (id<MTLTexture>)getBufferReadonlyTextureWithCommandBuffer:(id<MTLCommandBuffer>)commandBuffer
{
    if(readonlyTexture != nil)
    {
        BOOL sizeHasChanged = readonlyTexture.width != bufferSize.width || readonlyTexture.height != bufferSize.height;
        if(!sizeHasChanged)
        {
            return readonlyTexture;
        }
    }
    readonlyTexture = [self createTextureForDevice:device
                                          width:bufferSize.width
                                         height:bufferSize.height
                                    pixelFormat:pixelFormat];
   
    return readonlyTexture;
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
        NSLog(@"ISF: could not create texture for device");
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
