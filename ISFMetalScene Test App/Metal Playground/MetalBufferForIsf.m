//
//  MetalBufferForIsf.m
//  VVBasics-mac
//
//  Created by MTO on 03/11/2020.
//

#import "MetalBufferDefinition.h"

@implementation MetalBufferDefinition

- (id)initWithVariableName:(NSString *)theVariableName
               bufferIndex:(int)theBufferIndex
                      type:(NSString *)theType
            allocationSize:(size_t)theAllocationSize
{
    self = [super init];
    if( self )
    {
        _variableName = theVariableName;
        _bufferIndex = theBufferIndex;
        _type = theType;
        _allocationSize = theAllocationSize;
    }
    return self;
}

@end
