#import "MISFAttribBufferDefinition.h"
#import <VVBasics/VVBasics.h>

@implementation MISFAttribBufferDefinition

- (id)initWithVariableName:(NSString *)theVariableName bufferIndex:(int)theBufferIndex type:(MISFInputDataType)theType
{
    self = [super init];
    if( self )
    {
        _variableName = [theVariableName retain];
        _bufferIndex = theBufferIndex;
        _type = theType;
    }
    return self;
}

- (NSString *)description
{
    return [NSString
        stringWithFormat:@"Buffer Definition: VarName=%@ BufferIndex=%i Type=%lu", _variableName, _bufferIndex, _type];
}

- (void)dealloc
{
    VVRELEASE(_variableName);
    [super dealloc];
}

@end
