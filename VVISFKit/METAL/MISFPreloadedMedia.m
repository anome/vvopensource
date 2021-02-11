#import "MISFPreloadedMedia.h"
#import <VVBasics/VVBasics.h>

@implementation MISFPreloadedMedia

- (void)dealloc
{
    VVRELEASE(_model);
    VVRELEASE(_fragmentCode);
    VVRELEASE(_vertexCode);
    VVRELEASE(_fragmentLibrary);
    VVRELEASE(_vertexLibrary);
    [super dealloc];
}
@end
