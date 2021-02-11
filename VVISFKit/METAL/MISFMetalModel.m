#import "MISFMetalModel.h"

@implementation MISFMetalModel

- (void)dealloc
{
    VVRELEASE(_parentModel);
    VVRELEASE(_convertedFragmentCode);
    VVRELEASE(_convertedVertexCode);
    VVRELEASE(_fragmentBufferDefinitions);
    VVRELEASE(_vertexBufferDefinitions);
    [super dealloc];
}

@end
