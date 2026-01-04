#import "MISFTexturePool.h"
#import <VVBasics/VVBasics.h>

static const NSTimeInterval lifetimeInSeconds = 0.5;

@interface MISFTextureEntry : NSObject
@property (nonatomic, retain) id<MTLTexture> texture;
@property (nonatomic, assign) NSTimeInterval returnedAt;
@end

@implementation MISFTextureEntry
- (void)dealloc
{
    VVRELEASE(_texture);
    [super dealloc];
}
@end


@interface MISFTexturePool ()
{
    NSMutableArray *_entries;
}
@end

@implementation MISFTexturePool

- (instancetype)init
{
    self = [super init];
    if( self )
    {
        _entries = [NSMutableArray new];
    }
    return self;
}

- (id<MTLTexture>)acquireTextureWithWidth:(NSUInteger)width
                                   height:(NSUInteger)height
                              pixelFormat:(MTLPixelFormat)pixelFormat
                                   device:(id<MTLDevice>)device
{
    NSTimeInterval now = CFAbsoluteTimeGetCurrent();
    
    // ---- CLEAN PHASE ----
    NSMutableArray *expiredEntries = nil;
    for( MISFTextureEntry *entry in _entries )
    {
        if( lifetimeInSeconds < (now - entry.returnedAt) )
        {
            if( !expiredEntries )
            {
                expiredEntries = [NSMutableArray array];
            }
            [expiredEntries addObject:entry];
        }
    }
    if( expiredEntries )
    {
        [_entries removeObjectsInArray:expiredEntries];
    }
    
    // ---- ACQUIRE PHASE ----
    MISFTextureEntry *matchedEntry = nil;
    for (MISFTextureEntry *entry in _entries)
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
        [_entries removeObject:matchedEntry];
        return matchedEntry.texture;
    }
    
    return nil;
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
    [_entries addObject:entry];
}

- (void)dealloc
{
    VVRELEASE(_entries);
    [super dealloc];
}

@end
