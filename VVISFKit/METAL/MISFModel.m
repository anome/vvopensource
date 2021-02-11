#import "MISFModel.h"
#include "ISFStringAdditions.h"
#import "MISFErrorCodes.h"
#import "NSString+DDMathParsing.h"

@implementation MISFModelBuffer

- (id)init
{
    self = [super init];
    if( self )
    {
        _name = nil;
        _evalWidth = nil;
        _evalHeight = nil;
        _floatFlag = NO;
        _persistent = YES;
    }
    return self;
}

- (BOOL)requiresEval
{
    return (_evalWidth != nil || _evalHeight != nil);
}
- (void)dealloc
{
    VVRELEASE(_name);
    VVRELEASE(_evalWidth);
    VVRELEASE(_evalHeight);
    [super dealloc];
}
@end

@implementation MISFModelPass
- (id)init
{
    self = [super init];
    if( self )
    {
        _targetBuffer = nil;
    }
    return self;
}
- (void)dealloc
{
    VVRELEASE(_targetBuffer);
    [super dealloc];
}
@end

@implementation MISFModelImportedImage
- (id)init
{
    self = [super init];
    if( self )
    {
        _path = nil;
        _name = nil;
        _cubeFlag = NO;
    }
    return self;
}
- (void)dealloc
{
    VVRELEASE(_path);
    VVRELEASE(_name);
    [super dealloc];
}

@end

@implementation MISFModel
{
    NSString *rawFragmentString;
    id jsonObject;
}

- (instancetype)initWithFilePath:(NSString *)filePath withError:(NSError **)errorPtr
{
    self = [super init];
    if( self )
    {
        _filePath = [filePath retain];
        _jsonString = nil;
        _passes = [NSArray<MISFModelPass *> new];
        _persistentBuffers = [NSArray<MISFModelBuffer *> new];
        _importedImages = [NSArray<MISFModelImportedImage *> new];
        _inputs = [NSArray<ISFAttrib *> new];
    }
    BOOL openFilesSuccess = [self openFilesWithError:errorPtr];
    if( !openFilesSuccess )
    {
        return nil;
    }
    BOOL parseSuccess = [self parseWithError:errorPtr];
    if( !parseSuccess )
    {
        return nil;
    }
    return self;
}

- (BOOL)openFilesWithError:(NSError **)errorPtr
{
    // Open & load fragment
    NSError *fileError;
    VVRELEASE(rawFragmentString);
    rawFragmentString = [[NSString stringWithContentsOfFile:_filePath encoding:NSUTF8StringEncoding
                                                      error:&fileError] retain];

    // Not sure this can occur, it's taken from GL implementation
    if( rawFragmentString == nil )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                @"Invalid File" :
                    [NSString stringWithFormat:@"file %@ couldn't be loaded, FileError --> %@", _filePath, fileError]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFParsingError userInfo:userInfo];
        }
        return NO;
    }

    VVRELEASE(_fileName);
    _fileName = [[_filePath lastPathComponent] retain];

    // Check for vertex, open & load vertex
    //    look for a vert shader that matches the name of the frag shader
    NSString *noExtPath = [_filePath stringByDeletingPathExtension];
    NSString *tmpPath = nil;
    NSFileManager *fileManager = [NSFileManager defaultManager];
    tmpPath = VVFMTSTRING(@"%@.vs", noExtPath);
    if( [fileManager fileExistsAtPath:tmpPath] )
    {
        VVRELEASE(_vertShaderSource);
        _vertShaderSource = [NSString stringWithContentsOfFile:tmpPath encoding:NSUTF8StringEncoding error:nil];
    }
    else
    {
        tmpPath = VVFMTSTRING(@"%@.vert", noExtPath);
        if( [fileManager fileExistsAtPath:tmpPath] )
        {
            VVRELEASE(_vertShaderSource);
            _vertShaderSource = [NSString stringWithContentsOfFile:tmpPath encoding:NSUTF8StringEncoding error:nil];
        }
    }
    return YES;
}

- (BOOL)parseWithError:(NSError **)errorPtr
{
    BOOL success = NO;
    success = [self parseJsonBlobWithError:errorPtr];
    if( !success )
    {
        return success;
    }
    success = [self parseDescriptionsWithError:errorPtr];
    if( !success )
    {
        return success;
    }
    success = [self parsePersistentBuffersWithError:errorPtr];
    if( !success )
    {
        return success;
    }
    success = [self parsePassesWithError:errorPtr];
    if( !success )
    {
        return success;
    }
    success = [self parseImportedImagesWithError:errorPtr];
    if( !success )
    {
        return success;
    }
    [self parseInputs];
    return success;
}

- (BOOL)parseJsonBlobWithError:(NSError **)errorPtr
{
    NSRange openCommentRange = [rawFragmentString rangeOfString:@"/*"];
    NSRange closeCommentRange = [rawFragmentString rangeOfString:@"*/"];

    if( openCommentRange.length <= 0 || closeCommentRange.length <= 0 )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{@"Missing JSON Blob" : @"No JSON blob in ISF File parsed"};
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFParsingError userInfo:userInfo];
        }
        return NO;
    }
    else
    {
        NSRange fragShaderSourceRange;
        fragShaderSourceRange.location = closeCommentRange.location + closeCommentRange.length;
        fragShaderSourceRange.length = [rawFragmentString length] - fragShaderSourceRange.location;
        NSRange jsonStringRange;
        jsonStringRange.location = openCommentRange.location + openCommentRange.length;
        jsonStringRange.length = closeCommentRange.location - jsonStringRange.location;
        VVRELEASE(_jsonString);
        _jsonString = [[rawFragmentString substringWithRange:jsonStringRange] retain];
        VVRELEASE(_fragShaderSource);
        _fragShaderSource = [[rawFragmentString substringWithRange:fragShaderSourceRange] retain];
    }
    return YES;
}

- (BOOL)parseDescriptionsWithError:(NSError **)errorPtr
{
    Class stringClass = [NSString class];
    Class dictClass = [NSDictionary class];
    Class arrayClass = [NSArray class];

    //    parse the JSON dict, turning it into a dictionary and values
    VVRELEASE(jsonObject);
    jsonObject = (_jsonString == nil) ? nil : [[_jsonString objectFromJSONStringWithError:errorPtr] retain];
    if( jsonObject == nil )
    {
        // Probably got an error object too
        return NO;
    }

    //    run through the dictionaries and values parsed from JSON, creating the appropriate attributes
    if( ![jsonObject isKindOfClass:dictClass] )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{@"Missing JSON Blob" : @"JSON blob was malormed. It should be a dict."};
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFParsingError userInfo:userInfo];
        }
        return NO;
    }

    // Optional fields
    id unsafeFileDescription = [jsonObject objectForKey:@"DESCRIPTION"];
    id unsafeCredits = [jsonObject objectForKey:@"CREDIT"];
    id unsafeCategoryNames = [jsonObject objectForKey:@"CATEGORIES"];
    if( unsafeFileDescription != nil && [unsafeFileDescription isKindOfClass:stringClass] )
    {
        VVRELEASE(_fileDescription);
        _fileDescription = [unsafeFileDescription retain];
    }
    if( unsafeCredits != nil && [unsafeCredits isKindOfClass:stringClass] )
    {
        VVRELEASE(_credits);
        _credits = [unsafeCredits retain];
    }
    if( unsafeCategoryNames != nil && [unsafeCategoryNames isKindOfClass:arrayClass] )
    {
        VVRELEASE(_categoryNames);
        _categoryNames = [unsafeCategoryNames retain];
    }
    return YES;
}

- (BOOL)parsePersistentBuffersWithError:(NSError **)errorPtr
{
    Class stringClass = [NSString class];
    Class dictClass = [NSDictionary class];
    Class arrayClass = [NSArray class];

    NSMutableArray<MISFModelBuffer *> *tempPersistentBuffers = [NSMutableArray<MISFModelBuffer *> new];
    id anObj = [jsonObject objectForKey:@"PERSISTENT_BUFFERS"];
    // Could be empty. It's optional.
    if( anObj == nil )
    {
        return YES;
    }

    //    if the persistent buffers object is an array, check that they're strings and add accordingly
    if( [anObj isKindOfClass:arrayClass] )
    {
        for( NSString *bufferName in(NSArray *)anObj )
        {
            if( [bufferName isKindOfClass:stringClass] )
            {
                MISFModelBuffer *newBuffer = [MISFModelBuffer new];
                newBuffer.name = bufferName;
                [tempPersistentBuffers addObject:newBuffer];
            }
            else
            {
#warning mto-anomes: TODO error case that could be catched instead of ignored to give user some info about its mistake
            }
        }
    }
    //    else if the persistent buffers object is a dict, add and populate the dict accordingly
    else if( [anObj isKindOfClass:dictClass] )
    {

        for( NSString *bufferName in(NSDictionary *)[anObj allKeys] )
        {
            NSDictionary *bufferDescription = [anObj objectForKey:bufferName];
            if( bufferDescription != nil && [bufferDescription isKindOfClass:dictClass] )
            {
                MISFModelBuffer *newBuffer = [MISFModelBuffer new];
                newBuffer.name = bufferName;
                id tmpObj = nil;
                tmpObj = [bufferDescription objectForKey:@"WIDTH"];
                if( tmpObj != nil )
                {
                    if( [tmpObj isKindOfClass:[NSString class]] )
                    {
                        newBuffer.evalWidth = tmpObj;
                    }
                    else if( [tmpObj isKindOfClass:[NSNumber class]] )
                    {
                        newBuffer.evalWidth = VVFMTSTRING(@"%d", [tmpObj intValue]);
                    }
                    else
                    {
#warning mto-anomes: TODO error case that could be catched instead of ignored to give user some info about its mistake
                    }
                }
                tmpObj = [bufferDescription objectForKey:@"HEIGHT"];
                if( tmpObj != nil )
                {
                    if( [tmpObj isKindOfClass:[NSString class]] )
                    {
                        newBuffer.evalHeight = tmpObj;
                    }
                    else if( [tmpObj isKindOfClass:[NSNumber class]] )
                    {
                        newBuffer.evalHeight = tmpObj;
                    }
                    else
                    {
#warning mto-anomes: TODO error case that could be catched instead of ignored to give user some info about its mistake
                    }
                }
                NSNumber *tmpNum = [bufferDescription objectForKey:@"FLOAT"];
                if( tmpNum != nil && [tmpNum isKindOfClass:[NSNumber class]] && [tmpNum boolValue] )
                {
                    newBuffer.floatFlag = YES;
                }
                else
                {
                    newBuffer.floatFlag = NO;
                }

                [tempPersistentBuffers addObject:newBuffer];
            }
        }
    }

    VVRELEASE(_persistentBuffers);
    _persistentBuffers = [tempPersistentBuffers mutableCopy];
    return YES;
}

- (BOOL)parsePassesWithError:(NSError **)errorPtr
{
    Class stringClass = [NSString class];
    Class dictClass = [NSDictionary class];
    Class arrayClass = [NSArray class];

    NSMutableArray<MISFModelPass *> *tempPasses = [NSMutableArray<MISFModelPass *> new];
    //    parse the PASSES array of dictionaries describing the various passes (which may need temp buffers)
    id anObj = [jsonObject objectForKey:@"PASSES"];
    // Could be empty. It's optional.
    if( anObj == nil )
    {
        return YES;
    }
    if( [anObj isKindOfClass:arrayClass] )
    {
        for( NSDictionary *rawPassDict in(NSArray *)anObj )
        {
            if( [rawPassDict isKindOfClass:dictClass] )
            {
                //    make a new render pass and populate it from the raw pass dict
                MISFModelPass *newPass = [MISFModelPass new];
                newPass.targetBuffer = [MISFModelBuffer new];
                NSString *tmpBufferName = [rawPassDict objectForKey:@"TARGET"];
                if( tmpBufferName == nil && ![tmpBufferName isKindOfClass:stringClass] )
                {
                    // Pass must have a name, otherwise ignore it
                    NSLog(@"ignore pass with no name"); // TODO: fire error?
                    continue;
                }
                newPass.targetBuffer.name = tmpBufferName;
                id persistentObj = [rawPassDict objectForKey:@"PERSISTENT"];
                @autoreleasepool
                {
                    NSNumber *persistentNum = nil;
                    if( [persistentObj isKindOfClass:[NSString class]] )
                    {
                        persistentNum = [(NSString *)persistentObj parseAsBoolean];
                        if( persistentNum == nil )
                        {
                            persistentNum = [(NSString *)persistentObj numberByEvaluatingString];
                        }
                    }
                    else if( [persistentObj isKindOfClass:[NSNumber class]] )
                    {
                        persistentNum = [[persistentObj retain] autorelease];
                    }
                    //    if there's a valid "PERSISTENT" flag in this pass dict and it's indicating a positive...
                    if( persistentNum != nil && [persistentNum intValue] > 0 )
                    {
                        //    add the target buffer as a persistent buffer
                        newPass.targetBuffer.persistent = YES;
                    }
                    //    else there's no "PERSISTENT" flag in this pass dict or it's indicating a negative...
                    else
                    {
                        //    add the target buffer as a temp buffer
                        newPass.targetBuffer.persistent = NO;
                    }
                }

                //    update the width/height stuff for the target buffer
                NSString *tmpString = nil;
                tmpString = [rawPassDict objectForKey:@"WIDTH"];
                if( tmpString != nil && [tmpString isKindOfClass:stringClass] )
                {
                    newPass.targetBuffer.evalWidth = tmpString;
                }
                else if( tmpString != nil && [tmpString isKindOfClass:[NSNumber class]] )
                {
                    newPass.targetBuffer.evalWidth = VVFMTSTRING(@"%d", [(NSNumber *)tmpString intValue]);
                }
                tmpString = [rawPassDict objectForKey:@"HEIGHT"];
                if( tmpString != nil && [tmpString isKindOfClass:stringClass] )
                {
                    newPass.targetBuffer.evalHeight = tmpString;
                }
                else if( tmpString != nil && [tmpString isKindOfClass:[NSNumber class]] )
                {
                    newPass.targetBuffer.evalHeight = VVFMTSTRING(@"%d", [(NSNumber *)tmpString intValue]);
                }
                NSNumber *tmpNum = [rawPassDict objectForKey:@"FLOAT"];
                if( tmpNum != nil && [tmpNum isKindOfClass:[NSNumber class]] && [tmpNum boolValue] )
                {
                    newPass.targetBuffer.floatFlag = YES;
                }
                else
                {
                    newPass.targetBuffer.floatFlag = NO;
                }
                [tempPasses addObject:newPass];
            }
        }
    }
    VVRELEASE(_passes);
    _passes = [tempPasses mutableCopy];
    return YES;
}

// Helper bock
- (BOOL)parseImportedImageDict:(NSDictionary *)importDict withError:(NSError **)errorPtr
{
    MISFModelImportedImage *importedImage = [MISFModelImportedImage new];
    NSString *samplerName = [importDict objectForKey:@"NAME"];
    NSString *cubeFlag = [importDict objectForKey:@"TYPE"];
    NSString *partialPath = [importDict objectForKey:@"PATH"];
    // Verify mandatory values
    if( !samplerName )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{@"Missing mandatory field" : @"IMPORTED images must have a name"};
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFParsingError userInfo:userInfo];
        }
        return NO;
    }
    if( ![partialPath isKindOfClass:[NSString class]] )
    {
        if( errorPtr )
        {
            NSDictionary *userInfo = @{
                @"Bad field type" :
                    [NSString stringWithFormat:@"supplied PATH for imported image named <%@> wasn't a string, %@",
                                               samplerName, partialPath]
            };
            *errorPtr = [NSError errorWithDomain:ISFErrorDomain code:ISFParsingError userInfo:userInfo];
        }
        return NO;
    }
    importedImage.name = samplerName;
    importedImage.path = partialPath;
    if( cubeFlag != nil && ![cubeFlag isEqualToString:@"cube"] )
    {
        importedImage.cubeFlag = YES;
    }
    NSArray *newImportedImages = [_importedImages arrayByAddingObject:importedImage];
    VVRELEASE(_importedImages);
    _importedImages = newImportedImages;
    return YES;
}

- (BOOL)parseImportedImagesWithError:(NSError **)errorPtr
{
    Class dictClass = [NSDictionary class];
    Class arrayClass = [NSArray class];

    id anObj = [jsonObject objectForKey:@"IMPORTED"];
    if( anObj != nil )
    {

        //    if i'm importing files from a dictionary, execute the block on all the elements in the dict (each element
        //    is another dict describing the thing to import)
        if( [anObj isKindOfClass:dictClass] )
        {
            //    each key is the name by which the imported image will be available, and the object is the dict
            //    describing the image to import
            for( id importDictKey in(NSDictionary *)anObj )
            {
#warning mto-anomes: is this line subject to fails?
                id importDict = anObj[importDictKey];
                if( [importDict isKindOfClass:[NSDictionary class]] )
                {
                    //    if the import dict doesn't have a "NAME" key, make a new mut dict and add it
                    if( [importDict objectForKey:@"NAME"] == nil )
                    {
                        NSMutableDictionary *tmpMutDict = [importDict mutableCopy];
                        [tmpMutDict setObject:importDictKey forKey:@"NAME"];
                        BOOL success = [self parseImportedImageDict:tmpMutDict withError:errorPtr];
                        if( !success )
                        {
                            return success;
                        }
                        [tmpMutDict autorelease];
                    }
                    //    else the import dict already had a name key, just add it straightaway
                    else
                    {
                        BOOL success = [self parseImportedImageDict:importDict withError:errorPtr];
                        if( !success )
                        {
                            return success;
                        }
                    }
                }
            }
        }
        //    else it's an array- an array full of dictionaries, each of which describes a file to import
        else if( [anObj isKindOfClass:arrayClass] )
        {
            //    run through all the dictionaries in 'IMPORTED' (each dict describes a file to be imported)
            for( id subObj in(NSArray *)anObj )
            {
                if( [subObj isKindOfClass:dictClass] )
                {
                    BOOL success = [self parseImportedImageDict:subObj withError:errorPtr];
                    if( !success )
                    {
                        return success;
                    }
                }
                else
                {
                    // TODO: maybe we could fire an error here, no? - this happens if there is only one element in
                    // dictionary
                }
            }
        }
    }
    return YES;
}

// This one is copy-pasted from GL code and half-implemented
- (void)parseInputs
{
    Class stringClass = [NSString class];
    Class dictClass = [NSDictionary class];
    Class arrayClass = [NSArray class];
    Class numClass = [NSNumber class];
    Class colorClass = [NSColor class];

    //    parse the INPUTS from the JSON dict (these form the basis of user interaction)
    NSArray *inputsArray = [jsonObject objectForKey:@"INPUTS"];
    NSMutableArray<ISFAttrib *> *tempInputs = [NSMutableArray<ISFAttrib *> new];
    if( inputsArray != nil && [inputsArray isKindOfClass:arrayClass] )
    {
        ISFAttrib *newAttrib = nil;
        ISFAttribValType newAttribType = ISFAT_Event;
        NSString *typeString = nil;
        NSString *descString = nil;
        NSString *labelString = nil;
        ISFAttribVal minVal;
        ISFAttribVal maxVal;
        ISFAttribVal defVal;
        ISFAttribVal idenVal;
        NSArray *labelArray = nil;
        NSArray *valArray = nil;
        BOOL isImageInput = NO;
        BOOL isAudioInput = NO;
        BOOL isFilterImageInput = NO;
        BOOL hasTransitionProgress = NO;

        for( NSDictionary *inputDict in inputsArray )
        {
            if( [inputDict isKindOfClass:dictClass] )
            {
                NSString *inputKey = [inputDict objectForKey:@"NAME"];
                if( inputKey != nil )
                {
                    newAttrib = nil;
                    labelArray = nil;
                    valArray = nil;
                    typeString = [inputDict objectForKey:@"TYPE"];
                    if( ![typeString isKindOfClass:stringClass] )
                    {
                        typeString = nil;
                    }
                    descString = [inputDict objectForKey:@"DESCRIPTION"];
                    if( ![descString isKindOfClass:stringClass] )
                    {
                        descString = nil;
                    }
                    labelString = [inputDict objectForKey:@"LABEL"];
                    if( ![labelString isKindOfClass:stringClass] )
                    {
                        labelString = nil;
                    }
                    // NSLog(@"\t\tattrib key is %@, typeString is %@",inputKey,typeString);
                    isImageInput = NO;
                    isAudioInput = NO;
                    isFilterImageInput = NO;

                    //    if the typeString is nil (or was set to nil because it wasn't a string), the attrib simply
                    //    shouldn't exist
                    if( typeString == nil )
                    {
                        inputKey = nil;
                    }
                    else if( [typeString isEqualToString:@"image"] )
                    {
                        newAttribType = ISFAT_Image;
                        minVal.imageVal = 0;
                        maxVal.imageVal = 0;
                        defVal.imageVal = 0;
                        idenVal.imageVal = 0;
                        isImageInput = YES;
                        // METAL IGNORE
                        //                         if ([inputKey isEqualToString:@"inputImage"])    {
                        //                         isFilterImageInput = YES;
                        //                         fileFunctionality = ISFF_Filter;
                        //                         }
                        //                         else if ([inputKey isEqualToString:@"startImage"])    {
                        //                         hasTransitionStart = YES;
                        //                         }
                        //                         else if ([inputKey isEqualToString:@"endImage"])    {
                        //                         hasTransitionEnd = YES;
                        //                         }
                    }
                    else if( [typeString isEqualToString:@"audio"] )
                    {
                        // METAL IGNORE
                        /*
                         newAttribType = ISFAT_Audio;
                         minVal.audioVal = 0;
                         maxVal.audioVal = 0;
                         defVal.audioVal = 0;
                         idenVal.audioVal = 0;
                         isAudioInput = YES;
                         NSNumber            *tmpNum = nil;
                         tmpNum = [inputDict objectForKey:@"MAX"];
                         maxVal.audioVal = (tmpNum==nil || ![tmpNum isKindOfClass:numClass]) ? 0 : [tmpNum intValue];
                         }
                         else if ([typeString isEqualToString:@"audioFFT"])    {
                         newAttribType = ISFAT_AudioFFT;
                         minVal.audioVal = 0;
                         maxVal.audioVal = 0;
                         defVal.audioVal = 0;
                         idenVal.audioVal = 0;
                         isAudioInput = YES;
                         NSNumber            *tmpNum = nil;
                         tmpNum = [inputDict objectForKey:@"MAX"];
                         maxVal.audioVal = (tmpNum==nil || ![tmpNum isKindOfClass:numClass]) ? 0 : [tmpNum intValue];
                         }
                         else if ([typeString isEqualToString:@"cube"])    {
                         newAttribType = ISFAT_Cube;
                         minVal.imageVal = 0;
                         maxVal.imageVal = 0;
                         defVal.imageVal = 0;
                         idenVal.imageVal = 0;
                         */
                    }
                    else if( [typeString isEqualToString:@"float"] )
                    {
                        newAttribType = ISFAT_Float;
                        NSNumber *tmpNum = nil;
                        tmpNum = [inputDict objectForKey:@"MIN"];
                        minVal.floatVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.0 : [tmpNum floatValue];
                        tmpNum = [inputDict objectForKey:@"MAX"];
                        maxVal.floatVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 1.0 : [tmpNum floatValue];
                        tmpNum = [inputDict objectForKey:@"DEFAULT"];
                        defVal.floatVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.5 : [tmpNum floatValue];
                        tmpNum = [inputDict objectForKey:@"IDENTITY"];
                        idenVal.floatVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.5 : [tmpNum floatValue];
                        if( [inputKey isEqualToString:@"progress"] )
                        {
                            hasTransitionProgress = YES;
                        }
                    }
                    else if( [typeString isEqualToString:@"bool"] )
                    {
                        newAttribType = ISFAT_Bool;
                        NSNumber *tmpNum = nil;
                        minVal.floatVal = (tmpNum == nil) ? NO : [tmpNum floatValue];
                        maxVal.floatVal = (tmpNum == nil) ? YES : [tmpNum floatValue];
                        tmpNum = [inputDict objectForKey:@"DEFAULT"];
                        defVal.boolVal = (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? YES : [tmpNum boolValue];
                        tmpNum = [inputDict objectForKey:@"IDENTITY"];
                        idenVal.boolVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? YES : [tmpNum boolValue];
                    }
                    else if( [typeString isEqualToString:@"long"] )
                    {
                        newAttribType = ISFAT_Long;
                        NSNumber *tmpNum = nil;
                        //    look for "VALUES" and "LABELS" arrays
                        valArray = [inputDict objectForKey:@"VALUES"];
                        labelArray = [inputDict objectForKey:@"LABELS"];
                        if( valArray != nil && [valArray isKindOfClass:arrayClass] && labelArray != nil &&
                            [labelArray isKindOfClass:arrayClass] && [valArray count] == [labelArray count] )
                        {
                            minVal.longVal = 0.0;
                            maxVal.longVal = 10.0;
                        }
                        else
                        {
                            valArray = nil;
                            labelArray = nil;
                            //    if i couldn't find the arrays, look for min/max
                            tmpNum = [inputDict objectForKey:@"MIN"];
                            minVal.longVal =
                                (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.0 : [tmpNum longValue];
                            tmpNum = [inputDict objectForKey:@"MAX"];
                            maxVal.longVal =
                                (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 10.0 : [tmpNum longValue];
                        }
                        tmpNum = [inputDict objectForKey:@"DEFAULT"];
                        defVal.longVal = (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.0 : [tmpNum longValue];
                        tmpNum = [inputDict objectForKey:@"IDENTITY"];
                        idenVal.longVal =
                            (tmpNum == nil || ![tmpNum isKindOfClass:numClass]) ? 0.0 : [tmpNum longValue];
                    }
                    else if( [typeString isEqualToString:@"event"] )
                    {
                        // METAL IGNORE
                        /*
                         //NSLog(@"********* ERR: %s",__func__);
                         newAttribType = ISFAT_Event;
                         minVal.eventVal = NO;
                         maxVal.eventVal = YES;
                         defVal.eventVal = NO;
                         idenVal.eventVal = NO;
                         */
                    }
                    else if( [typeString isEqualToString:@"color"] )
                    {
                        newAttribType = ISFAT_Color;
                        NSColor *tmpColor = nil;
                        for( int i = 0; i < 4; ++i )
                        {
                            minVal.colorVal[i] = 0.0;
                            maxVal.colorVal[i] = 1.0;
                        }
                        tmpColor = [inputDict objectForKey:@"DEFAULT"];
                        if( tmpColor == nil )
                            bzero(defVal.colorVal, sizeof(GLfloat) * 4);
                        else if( [tmpColor isKindOfClass:arrayClass] )
                        {
                            NSArray *tmpArray = (NSArray *)tmpColor;
                            int tmpInt = 0;
                            for( NSNumber *tmpNum in(NSArray *)tmpArray )
                            {
                                defVal.colorVal[tmpInt] = [tmpNum floatValue];
                                ++tmpInt;
                            }
                        }
                        else if( [tmpColor isKindOfClass:colorClass] )
                        {
                            CGFloat tmpVals[4];
                            [tmpColor getComponents:tmpVals];
                            for( int i = 0; i < 4; ++i )
                            {
                                defVal.colorVal[i] = tmpVals[i];
                            }
                        }

                        tmpColor = [inputDict objectForKey:@"IDENTITY"];
                        if( tmpColor == nil )
                            bzero(idenVal.colorVal, sizeof(GLfloat) * 4);
                        else if( [tmpColor isKindOfClass:arrayClass] )
                        {
                            NSArray *tmpArray = (NSArray *)tmpColor;
                            int tmpInt = 0;
                            for( NSNumber *tmpNum in(NSArray *)tmpArray )
                            {
                                idenVal.colorVal[tmpInt] = [tmpNum floatValue];
                                ++tmpInt;
                            }
                        }
                        else if( [tmpColor isKindOfClass:colorClass] )
                        {
                            CGFloat tmpVals[4];

                            [tmpColor getComponents:tmpVals];

                            for( int i = 0; i < 4; ++i )
                            {
                                idenVal.colorVal[i] = tmpVals[i];
                            }
                        }
                    }
                    else if( [typeString isEqualToString:@"point2D"] )
                    {
                        // NSLog(@"********* ERR: %s",__func__);
                        newAttribType = ISFAT_Point2D;
                        for( int i = 0; i < 2; ++i )
                        {
                            minVal.point2DVal[i] = 0.0;
                            maxVal.point2DVal[i] = 0.0;
                        }

                        NSArray *tmpArray = nil;
                        tmpArray = [inputDict objectForKey:@"DEFAULT"];
                        if( tmpArray != nil && [tmpArray isKindOfClass:arrayClass] )
                        {
                            NSNumber *tmpNum = [tmpArray objectAtIndex:0];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                defVal.point2DVal[0] = [tmpNum floatValue];
                            }

                            else
                            {
                                defVal.point2DVal[0] = 0.;
                            }
                            tmpNum = [tmpArray objectAtIndex:1];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                defVal.point2DVal[1] = [tmpNum floatValue];
                            }

                            else
                            {
                                defVal.point2DVal[1] = 0.;
                            }
                        }
                        else
                        {
                            defVal.point2DVal[0] = 0.;
                            defVal.point2DVal[1] = 0.;
                        }

                        tmpArray = [inputDict objectForKey:@"IDENTITY"];
                        if( tmpArray != nil && [tmpArray isKindOfClass:arrayClass] )
                        {
                            NSNumber *tmpNum = [tmpArray objectAtIndex:0];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                idenVal.point2DVal[0] = [tmpNum floatValue];
                            }
                            else
                            {
                                idenVal.point2DVal[0] = 0.;
                            }
                            tmpNum = [tmpArray objectAtIndex:1];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                idenVal.point2DVal[1] = [tmpNum floatValue];
                            }
                            else
                            {
                                idenVal.point2DVal[1] = 0.;
                            }
                        }
                        else
                        {
                            idenVal.point2DVal[0] = 0.;
                            idenVal.point2DVal[1] = 0.;
                        }

                        tmpArray = [inputDict objectForKey:@"MIN"];
                        if( tmpArray != nil && [tmpArray isKindOfClass:arrayClass] && [tmpArray count] == 2 )
                        {
                            NSNumber *tmpNum = [tmpArray objectAtIndex:0];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                minVal.point2DVal[0] = [tmpNum floatValue];
                            }
                            else
                            {
                                minVal.point2DVal[0] = 0.;
                            }
                            tmpNum = [tmpArray objectAtIndex:1];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                minVal.point2DVal[1] = [tmpNum floatValue];
                            }
                            else
                            {
                                minVal.point2DVal[1] = 0.;
                            }
                        }
                        else
                        {
                            minVal.point2DVal[0] = 0.;
                            minVal.point2DVal[1] = 0.;
                        }

                        tmpArray = [inputDict objectForKey:@"MAX"];
                        if( tmpArray != nil && [tmpArray isKindOfClass:arrayClass] && [tmpArray count] == 2 )
                        {
                            NSNumber *tmpNum = [tmpArray objectAtIndex:0];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                maxVal.point2DVal[0] = [tmpNum floatValue];
                            }
                            else
                            {
                                maxVal.point2DVal[0] = 0.;
                            }
                            tmpNum = [tmpArray objectAtIndex:1];
                            if( tmpNum != nil && [tmpNum isKindOfClass:numClass] )
                            {
                                maxVal.point2DVal[1] = [tmpNum floatValue];
                            }
                            else
                            {
                                maxVal.point2DVal[1] = 0.;
                            }
                        }
                        else
                        {
                            maxVal.point2DVal[0] = 0.;
                            maxVal.point2DVal[1] = 0.;
                        }
                    }
                    //    else the attribute type wasn't recognized- it simply shouldn't exist!
                    else
                    {
                        inputKey = nil;
                    }

                    // if (!isFilterImageInput)    {
                    if( inputKey != nil )
                    {
                        newAttrib = [ISFAttrib createWithName:inputKey
                                                  description:descString
                                                        label:labelString
                                                         type:newAttribType
                                                       values:minVal:maxVal:defVal:idenVal:labelArray:valArray];
                        [newAttrib setIsFilterInputImage:isFilterImageInput];
                        [tempInputs addObject:newAttrib];
                        //                        if (isImageInput)
                        //                            [imageInputs lockAddObject:newAttrib];
                        //                        if (isAudioInput)
                        //                            [audioInputs lockAddObject:newAttrib];
                    }
                    //}
                }
            }
        }

        //    if the file had all of the requirements for a transition, set the functionality
        // METAL IGNORE
        /*
         if ((hasTransitionStart == YES)&&(hasTransitionEnd == YES)&&(hasTransitionProgress == YES))    {
         fileFunctionality = ISFF_Transition;
         }
         */
    }

    VVRELEASE(_inputs);
    _inputs = [tempInputs mutableCopy];
}

- (BOOL)hasVertexShader
{
    return _vertShaderSource != nil;
}

- (void)dealloc
{
    VVRELEASE(_credits);
    VVRELEASE(_categoryNames);
    VVRELEASE(_fileDescription);
    VVRELEASE(_filePath);
    VVRELEASE(_fileName);
    VVRELEASE(_vertexShader);
    VVRELEASE(_fragmentShader);
    VVRELEASE(_persistentBuffers);
    VVRELEASE(_passes);
    VVRELEASE(_inputs);
    VVRELEASE(_jsonString);
    VVRELEASE(_fragShaderSource);
    VVRELEASE(_vertShaderSource);
    [super dealloc];
}

@end
