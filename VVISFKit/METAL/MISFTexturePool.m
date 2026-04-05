#import "MISFTexturePool.h"
#import <VVBasics/VVBasics.h>

// Pool contains texture to be re-used WITHOUT content clean. Old textures are irrelevant
static const NSTimeInterval lifetimeInSeconds = 0.5;


@implementation MISFTextureEntry
@end


@implementation MISFTexturePool

- (instancetype)init
{
    self = [super init];
    if( self )
    {
        self.entries = [NSArray<MISFTextureEntry*> array];
    }
    return self;
}

/**
 @returns : a texture if one is available, or nil
 */
- (id<MTLTexture>)acquireTextureWithWidth:(NSUInteger)width
                                   height:(NSUInteger)height
                              pixelFormat:(MTLPixelFormat)pixelFormat
                                   device:(id<MTLDevice>)device
{
	MISFTextureEntry *matchedEntry = nil;
	@synchronized (self)
    {
		for( MISFTextureEntry *entry in self.entries )
		{
			if( entry.texture.width == width &&
			   entry.texture.height == height &&
			   entry.texture.pixelFormat == pixelFormat )
			{
				matchedEntry = entry;
				break;
			}
		}
		
		if( matchedEntry )
		{
            id<MTLTexture> textureToReturn = matchedEntry.texture;
			self.entries = [self.entries filteredArrayUsingPredicate:
							[NSPredicate predicateWithBlock:^BOOL(id obj, NSDictionary *bindings) {
					return obj != matchedEntry;
				}]];
			
            return textureToReturn;
		}
	}
    
    return nil;
}

- (void)cleanPool
{
    NSTimeInterval now = CFAbsoluteTimeGetCurrent();
	@synchronized (self)
    {
		self.entries = [self.entries filteredArrayUsingPredicate:
					[NSPredicate predicateWithBlock:^BOOL(MISFTextureEntry *entry, NSDictionary *bindings) {
			return lifetimeInSeconds >= (now - entry.returnedAt);
		}]];
	}
}

- (void)recycleTexture:(id<MTLTexture>)texture
{
    if( !texture )
    {
        return;
    }
    
    MISFTextureEntry *entry = [[MISFTextureEntry alloc] init];
    entry.texture = texture;
    entry.returnedAt = CFAbsoluteTimeGetCurrent();
	@synchronized (self)
    {
		self.entries = [self.entries arrayByAddingObject:entry];
	}
}

@end
