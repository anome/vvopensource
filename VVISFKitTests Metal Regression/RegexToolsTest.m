#import <Foundation/Foundation.h>
#import <Metal/Metal.h>
#import <MetalKit/MetalKit.h>
#import <VVISFKit/VVISFKit.h>
#import <XCTest/XCTest.h>


/// Partial test for latest RegexTools features
@interface RegexToolsTest : XCTestCase

@end

@implementation RegexToolsTest
{
    NSArray<NSString*> *shouldMatches;
    NSString* shouldNotMatch;
    NSString *parameterName;
    NSString *functionNames;
    NSString *pattern;
}

- (void)setUp
{
  
    functionNames = @"IMG_NORM_PIXEL|IMG_PIXEL|texture2D|IMG_THIS_PIXEL|IMG_NORM_THIS_PIXEL";
    parameterName = @"BufferA";
    pattern = [RegexTools detectPatternForFunctions:functionNames withFirstParameter:parameterName];
    
    shouldNotMatch = @""
    "vec4 sourceUv = IMG_NORM_PIXEL(BufferB, uv);\n"
    "vec4 sourceUv = IMG_PIXEL(BufferB, uv);\n"
    "vec4 sourceUv = IMG_NORM_THIS_PIXEL(BufferB, uv);\n"
    "vec4 sourceUv = IMG_THIS_PIXEL(BufferB, uv);\n"
    "vec4 sourceUv = texture2D(BufferB, uv);\n"
    "vec4 sourceUv = texture2D(buffera, uv);\n"
    "vec4 sourceUv = texture2d(buffera, uv);\n";
    
    shouldMatches = @[
    @"vec4 sourceUv = IMG_NORM_PIXEL(BufferA, uv);"
    @"vec4 sourceUv = IMG_PIXEL(BufferA, uv);"
    @"vec4 sourceUv = IMG_NORM_THIS_PIXEL(BufferA, uv);"
    @"vec4 sourceUv = IMG_THIS_PIXEL(BufferA, uv);"
    @"vec4 sourceUv = texture2D(BufferA, uv);"
    @"vec4 sourceUv = IMG_NORM_PIXEL(\n"
   "BufferA, uv);"
    @"vec4 sourceUv = IMG_NORM_PIXEL(\n"
       "\t  BufferA, uv);"
    @" vec4 sourceUv = IMG_NORM_PIXEL\n"
    "(\n"
      " BufferA, uv);"
    @"    IMG_NORM_PIXEL(\n"
    "BufferA   , uv);"
    ];
    


}

- (void)tearDown
{
    functionNames = nil;
    parameterName = nil;
    shouldMatches = nil;
    shouldNotMatch = nil;
}

#pragma mark Helpers



#pragma mark Tests

- (void)testPattern
{
    NSString *expected = @"(?<=(\\s|[(,*\\/+-]))(IMG_NORM_PIXEL|IMG_PIXEL|texture2D|IMG_THIS_PIXEL|IMG_NORM_THIS_PIXEL)(?=(\\s*\\(\\s*BufferA))";
    XCTAssertEqualObjects(pattern, expected);
}

- (void)testPatternCreationOneFunction
{
    NSString *functions = @"fn1";
    NSString *parameterName = @"param";
    NSString *somePattern = [RegexTools detectPatternForFunctions:functions withFirstParameter:parameterName];
    NSString *expected = @"(?<=(\\s|[(,*\\/+-]))(fn1)(?=(\\s*\\(\\s*param))";
    XCTAssertEqualObjects(somePattern, expected);
}

- (void)testShouldMatch
{
    for(NSString* shouldMatch in shouldMatches)
    {
        NSError *error = nil;
        bool found = [RegexTools searchString:shouldMatch forPattern:pattern withError:&error];
        XCTAssertNil(error);
        XCTAssertTrue(found);
    }
}

- (void)testShouldNotMatch
{
    NSError *error = nil;
    bool found = [RegexTools searchString:shouldNotMatch forPattern:pattern withError:&error];
    XCTAssertNil(error);
    XCTAssertFalse(found);
}

@end
