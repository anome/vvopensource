#import "MISFTexturePoolsManager.h"
#import "MISFTexturePool.h"
#import <VVBasics/VVBasics.h>
/**
 MISFTexturePool manager.
 Role:
 - creating texturePools for each callerId (one callerId keeps its own textures to itself // This behaviour was created to allow re-use for multi-resolution renderings / transitions)
 - Handling cleaning of those texturePools with a single thread
 */

static MISFTexturePoolsManager *reference = nil;
static const NSTimeInterval periodicCleanInterval = 2.0;

@interface MISFTexturePoolsManager()
{
    dispatch_source_t cleanTimer;
    BOOL isTimerRunning;
}
@property (readwrite, retain) NSDictionary<NSString*, MISFTexturePool*> *texturePools;
@end

@implementation MISFTexturePoolsManager

+ (MISFTexturePoolsManager *)sharedManager
{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        reference = [MISFTexturePoolsManager new];
    });
    return reference;
}

- (instancetype)init
{
    self = [super init];
    if( self )
    {
        self.texturePools = [NSDictionary<NSString*, MISFTexturePool*> dictionary];
        
        // Create a timer to clean the pool periodically, suspended by default, and resumed when pool is filled
        dispatch_queue_t queue = dispatch_get_global_queue(QOS_CLASS_UTILITY, 0);
        cleanTimer = dispatch_source_create(DISPATCH_SOURCE_TYPE_TIMER, 0, 0, queue);
        dispatch_source_set_timer(cleanTimer,
                                  dispatch_time(DISPATCH_TIME_NOW, (int64_t)(periodicCleanInterval * NSEC_PER_SEC)),
                                  (uint64_t)(periodicCleanInterval * NSEC_PER_SEC),
                                  (uint64_t)(1.0 * NSEC_PER_SEC)); // Allow some leeway
        __block MISFTexturePoolsManager *blockSelf = self;
        dispatch_source_set_event_handler(cleanTimer, ^{
            [blockSelf cleanPools];
        });
        isTimerRunning = NO;
    }
    return self;
}

- (MISFTexturePool*)lazyGetPoolForCallerId:(NSString*)callerId
{
    MISFTexturePool *pool = nil;
    @synchronized(self)
    {
        pool = self.texturePools[callerId];
        if( pool == nil )
        {
            pool = [MISFTexturePool new];
            NSMutableDictionary *mutablePools = [self.texturePools mutableCopy];
            mutablePools[callerId] = pool; // +1 retain count
            self.texturePools = [NSDictionary dictionaryWithDictionary:mutablePools];
        }
        
        // If we moved from 0 to 1 element, start clean timer
        if( self.texturePools.count == 1 && !isTimerRunning )
        {
//            NSLog(@"ISF TexturePool: Start Clean Timer");
            dispatch_resume(cleanTimer);
            isTimerRunning = YES;
        }
    }
    return pool;
}

/**
 @return texture that you must retain
 */
- (id<MTLTexture>)acquireTextureWithWidth:(NSUInteger)width
                                   height:(NSUInteger)height
                              pixelFormat:(MTLPixelFormat)pixelFormat
                                   device:(id<MTLDevice>)device
                                 withCallerId:(NSString*)callerId
{
    MISFTexturePool* pool = [self lazyGetPoolForCallerId:callerId];
    if( !pool )
    {
        NSLog(@"ISF Error: no pool allocated for acquire - unexpected");
        return nil;
    }
    return [pool acquireTextureWithWidth:width height:height pixelFormat:pixelFormat device:device];
}


- (void)recycleTexture:(id<MTLTexture>)texture withCallerId:(NSString*)callerId
{
    MISFTexturePool* pool = [self lazyGetPoolForCallerId:callerId];
    if( !pool )
    {
        NSLog(@"ISF Error: no pool allocated for recycle - unexpected");
        return;
    }
    [pool recycleTexture:texture];
}

- (void)cleanPools
{
    @synchronized (self)
    {
        NSArray<NSString*> *keysToRemove = @[];
        
        for( NSString* key in self.texturePools )
        {
            [self.texturePools[key] cleanPool];
            if( self.texturePools[key].entries.count == 0 )
            {
                keysToRemove = [keysToRemove arrayByAddingObject:key];
            }
        }
        
        if (keysToRemove.count > 0)
        {
            NSMutableDictionary *mutablePools = [self.texturePools mutableCopy];
            [mutablePools removeObjectsForKeys:keysToRemove];
            self.texturePools = [NSDictionary dictionaryWithDictionary:mutablePools];
        }
        
        // If pools are empty, suspend timer
        if( self.texturePools.count == 0 && isTimerRunning )
        {
            dispatch_suspend(cleanTimer);
            isTimerRunning = NO;
        }
    }
}

// Singleton, dealloc will happen at app exit only
- (void) dealloc
{
    if( cleanTimer )
    {
        // Ensure timer is not paused before being destroyed
       if( !isTimerRunning )
       {
           dispatch_resume(cleanTimer);
       }
        dispatch_source_cancel(cleanTimer);

    }
}


@end
