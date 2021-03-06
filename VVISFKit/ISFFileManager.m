#import "ISFFileManager.h"
#import "ISFStringAdditions.h"




@implementation ISFFileManager


+ (NSMutableArray *) allFilesForPath:(NSString *)path recursive:(BOOL)r	{
	return [self _filtersInDirectory:path recursive:r matchingFunctionality:ISFF_All];
}
+ (NSMutableArray *) imageFiltersForPath:(NSString *)path recursive:(BOOL)r	{
	return [self _filtersInDirectory:path recursive:r matchingFunctionality:ISFF_Filter];
}
+ (NSMutableArray *) generativeSourcesForPath:(NSString *)path recursive:(BOOL)r	{
	return [self _filtersInDirectory:path recursive:r matchingFunctionality:ISFF_Source];
}
+ (NSMutableArray *) defaultImageFilters	{
	NSMutableArray		*sys = [self imageFiltersForPath:@"/Library/Graphics/ISF" recursive:YES];
	[sys retain];
	NSMutableArray		*user = [self imageFiltersForPath:[@"~/Library/Graphics/ISF" stringByExpandingTildeInPath] recursive:YES];
	[sys addObjectsFromArray:user];
	return [sys autorelease];
}
+ (NSMutableArray *) defaultGenerativeSources	{
	NSMutableArray		*sys = [self generativeSourcesForPath:@"/Library/Graphics/ISF" recursive:YES];
	[sys retain];
	NSMutableArray		*user = [self generativeSourcesForPath:[@"~/Library/Graphics/ISF" stringByExpandingTildeInPath] recursive:YES];
	[sys addObjectsFromArray:user];
	return [sys autorelease];
}
+ (BOOL) fileIsProbablyAnISF:(NSString *)pathToFile	{
	if (pathToFile==nil)
		return NO;
	//	if there's no extension, ignore it
	NSString		*extension = [pathToFile pathExtension];
	if (extension==nil)
		return NO;
	//	if it's not a .fs or a .fsf file, it's probably not an ISF file.
	if ([extension caseInsensitiveCompare:@"fs"]!=NSOrderedSame && [extension caseInsensitiveCompare:@"fsf"]!=NSOrderedSame)
		return NO;
	NSString		*rawFile = [NSString stringWithContentsOfFile:pathToFile encoding:NSUTF8StringEncoding error:nil];
	if (rawFile == nil)	{
		NSLog(@"\t\terr: couldn't load file %@ in %s",pathToFile,__func__);
		return NO;
	}
	//	there should be a JSON blob at the very beginning of the file describing the script's attributes and parameters- this is inside comments...
	NSRange			openCommentRange;
	NSRange			closeCommentRange;
	openCommentRange = [rawFile rangeOfString:@"/*"];
	closeCommentRange = [rawFile rangeOfString:@"*/"];
	if (openCommentRange.length!=0 && closeCommentRange.length!=0)	{
		//	parse the JSON string, turning it into a dictionary and values
		NSString		*jsonString = [rawFile substringWithRange:NSMakeRange(openCommentRange.location+openCommentRange.length, closeCommentRange.location-(openCommentRange.location+openCommentRange.length))];
		id				jsonObject = [jsonString objectFromJSONString];
		if (jsonObject==nil)	{
			NSLog(@"\t\terr: couldn't make jsonObject in %s, string was %@",__func__,jsonString);
			return NO;
		}
		else	{
			if ([jsonObject isKindOfClass:[NSDictionary class]])	{
				//	at this point, just assume that it's a valid ISF file (we could check for DESCRIPTION/INPUTS/PASSES/etc, but that might inadvertently rule out an ISF that doesn't use- or need- any of those keywords)
				/*
				//	check the "INPUTS" section of the JSON dict
				NSArray		*inputs = [jsonObject objectForKey:@"INPUTS"];
				if (inputs==nil || ![inputs isKindOfClass:[NSArray class]])	{
					NSLog(@"\t\terr: inputs was nil, or was the wrong type, %s",__func__);
					return NO;
				}
				*/
				return YES;
			}
			else	{
				NSLog(@"\t\terr: jsonObject was wrong class, %s",__func__);
				NSLog(@"\t\terr: file was %@",pathToFile);
				return NO;
			}
		}
	}
	return NO;
}
+ (NSArray *) categoriesForISF:(NSString *)pathToFile	{
	if (pathToFile==nil)
		return [NSArray array];
	NSString		*rawFile = [NSString stringWithContentsOfFile:pathToFile encoding:NSUTF8StringEncoding error:nil];
	if (rawFile == nil)	{
		NSLog(@"\t\terr: couldn't load file %@ in %s",pathToFile,__func__);
		return [NSArray array];
	}
	//	there should be a JSON blob at the very beginning of the file describing the script's attributes and parameters- this is inside comments...
	NSRange			openCommentRange;
	NSRange			closeCommentRange;
	openCommentRange = [rawFile rangeOfString:@"/*"];
	closeCommentRange = [rawFile rangeOfString:@"*/"];
	if (openCommentRange.length!=0 && closeCommentRange.length!=0)	{
		//	parse the JSON string, turning it into a dictionary and values
		NSString		*jsonString = [rawFile substringWithRange:NSMakeRange(openCommentRange.location+openCommentRange.length, closeCommentRange.location-(openCommentRange.location+openCommentRange.length))];
		id				jsonObject = [jsonString objectFromJSONString];
		if (jsonObject==nil)	{
			NSLog(@"\t\terr: couldn't make jsonObject in %s, string was %@",__func__,jsonString);
			return [NSArray array];
		}
		else	{
			if ([jsonObject isKindOfClass:[NSDictionary class]])	{
				//	check the "INPUTS" section of the JSON dict
				NSArray		*categories = [jsonObject objectForKey:@"CATEGORIES"];
				if (categories==nil || ![categories isKindOfClass:[NSArray class]])	{
					NSLog(@"\t\terr: categories was nil, or was the wrong type, %s",__func__);
					return [NSArray array];
				}
				return [[categories retain] autorelease];
			}
			else	{
				NSLog(@"\t\terr: jsonObject was wrong class, %s",__func__);
				NSLog(@"\t\terr: file was %@",pathToFile);
				return [NSArray array];
			}
		}
	}
	return [NSArray array];
}
+ (NSMutableArray *) _filtersInDirectory:(NSString *)folder recursive:(BOOL)r matchingFunctionality:(ISFFunctionality)func	{
	if (folder==nil)
		return nil;
	NSString			*trimmedPath = [folder stringByDeletingLastAndAddingFirstSlash];
	NSFileManager		*fm = [NSFileManager defaultManager];
	BOOL				isDirectory = NO;
	if (![fm fileExistsAtPath:trimmedPath isDirectory:&isDirectory])
		return nil;
	if (!isDirectory)
		return nil;
	NSMutableArray			*rawFiles = [[NSMutableArray alloc] initWithCapacity:0];
	if (r)	{
		NSDirectoryEnumerator	*it = [fm enumeratorAtPath:trimmedPath];
		NSString				*file = nil;
		while (file = [it nextObject])	{
			NSString		*ext = [file pathExtension];
			if (ext!=nil && ([ext isEqualToString:@"fs"] || [ext isEqualToString:@"frag"]))	{
				NSString		*fullPath = [NSString stringWithFormat:@"%@/%@",trimmedPath,file];
				if (func == ISFF_All)
					[rawFiles addObject:fullPath];
				else	{
					ISFFunctionality	fileFunc = [self _functionalityForFile:fullPath];
					if (func == fileFunc)
						[rawFiles addObject:fullPath];
					/*
					if ([self _isAFilter:fullPath])	{
						if (func == ISFF_Filter)
							[rawFiles addObject:fullPath];
					}
					else	{
						if (func == ISFF_Source)
							[rawFiles addObject:fullPath];
					}
					*/
				}
			}
		}
	}
	//	else non-recursive (shallow) listing
	else	{
		NSArray		*tmpArray = [fm contentsOfDirectoryAtPath:trimmedPath error:nil];
		for (NSString *file in tmpArray)	{
			NSString		*ext = [file pathExtension];
			if (ext!=nil && ([ext isEqualToString:@"fs"] || [ext isEqualToString:@"frag"]))	{
				NSString		*fullPath = [NSString stringWithFormat:@"%@/%@",trimmedPath,file];
				if (func == ISFF_All)
					[rawFiles addObject:fullPath];
				else	{
					ISFFunctionality	fileFunc = [self _functionalityForFile:fullPath];
					if (func == fileFunc)
						[rawFiles addObject:fullPath];					
					/*
					if ([self _isAFilter:fullPath])	{
						if (func == ISFF_Filter)
							[rawFiles addObject:fullPath];
					}
					else	{
						if (func == ISFF_Source)
							[rawFiles addObject:fullPath];
					}
					*/
				}
			}
		}
	}
	//return [rawFiles autorelease];
	//return [[[rawFiles sortedArrayUsingComparator:^(NSString *obj1, NSString *obj2)	{
	//	return [obj1 caseInsensitiveCompare:obj2];
	//}] mutableCopy] autorelease];
	
	NSArray				*sorted = [rawFiles sortedArrayUsingComparator:^(NSString *obj1, NSString *obj2)	{
		return [obj1 caseInsensitiveCompare:obj2];
	}];
	[rawFiles release];
	rawFiles = nil;
	return [[sorted mutableCopy] autorelease];
	
}
+ (BOOL) _isAFilter:(NSString *)pathToFile	{
	if (pathToFile==nil)
		return NO;
	NSString		*rawFile = [NSString stringWithContentsOfFile:pathToFile encoding:NSUTF8StringEncoding error:nil];
	if (rawFile == nil)	{
		NSLog(@"\t\terr: couldn't load file %@ in %s",pathToFile,__func__);
		return NO;
	}
	//	there should be a JSON blob at the very beginning of the file describing the script's attributes and parameters- this is inside comments...
	NSRange			openCommentRange;
	NSRange			closeCommentRange;
	openCommentRange = [rawFile rangeOfString:@"/*"];
	closeCommentRange = [rawFile rangeOfString:@"*/"];
	if (openCommentRange.length!=0 && closeCommentRange.length!=0)	{
		//	parse the JSON string, turning it into a dictionary and values
		NSString		*jsonString = [rawFile substringWithRange:NSMakeRange(openCommentRange.location+openCommentRange.length, closeCommentRange.location-(openCommentRange.location+openCommentRange.length))];
		id				jsonObject = [jsonString objectFromJSONString];
		if (jsonObject==nil)	{
			NSLog(@"\t\terr: couldn't make jsonObject in %s, string was %@",__func__,jsonString);
			return NO;
		}
		else	{
			if ([jsonObject isKindOfClass:[NSDictionary class]])	{
				//	check the "INPUTS" section of the JSON dict
				NSArray		*inputs = [jsonObject objectForKey:@"INPUTS"];
				if (inputs==nil || ![inputs isKindOfClass:[NSArray class]])	{
					NSLog(@"\t\terr: inputs was nil, or was the wrong type, %s - %@",__func__,pathToFile);
					return NO;
				}
				for (NSDictionary *inputDict in inputs)	{
					if ([inputDict isKindOfClass:[NSDictionary class]])	{
						NSString		*tmpString = nil;
						tmpString = [inputDict objectForKey:@"NAME"];
						if (tmpString!=nil && [tmpString isEqualToString:@"inputImage"])	{
							tmpString = [inputDict objectForKey:@"TYPE"];
							if (tmpString!=nil && [tmpString isEqualToString:@"image"])
								return YES;
						}
					}
				}
			}
			else	{
				NSLog(@"\t\terr: jsonObject was wrong class, %s",__func__);
				NSLog(@"\t\terr: file was %@",pathToFile);
				return NO;
			}
		}
	}
	return NO;
}
+ (ISFFunctionality) _functionalityForFile:(NSString *)pathToFile	{
	if (pathToFile==nil)
		return ISFF_Source;
	NSString		*rawFile = [NSString stringWithContentsOfFile:pathToFile encoding:NSUTF8StringEncoding error:nil];
	if (rawFile == nil)	{
		NSLog(@"\t\terr: couldn't load file %@ in %s",pathToFile,__func__);
		return ISFF_Source;
	}
	//	there should be a JSON blob at the very beginning of the file describing the script's attributes and parameters- this is inside comments...
	NSRange				openCommentRange;
	NSRange				closeCommentRange;
	openCommentRange = [rawFile rangeOfString:@"/*"];
	closeCommentRange = [rawFile rangeOfString:@"*/"];
	BOOL				hasTransitionStart = NO;
	BOOL				hasTransitionEnd = NO;
	BOOL				hasTransitionProgress = NO;
	
	if (openCommentRange.length!=0 && closeCommentRange.length!=0)	{
		//	parse the JSON string, turning it into a dictionary and values
		NSString		*jsonString = [rawFile substringWithRange:NSMakeRange(openCommentRange.location+openCommentRange.length, closeCommentRange.location-(openCommentRange.location+openCommentRange.length))];
		id				jsonObject = [jsonString objectFromJSONString];
		if (jsonObject==nil)	{
			NSLog(@"\t\terr: couldn't make jsonObject in %s, string was %@",__func__,jsonString);
			return ISFF_Source;
		}
		else	{
			if ([jsonObject isKindOfClass:[NSDictionary class]])	{
				//	check the "INPUTS" section of the JSON dict
				NSArray		*inputs = [jsonObject objectForKey:@"INPUTS"];
				if (inputs==nil || ![inputs isKindOfClass:[NSArray class]])	{
					NSLog(@"\t\terr: inputs was nil, or was the wrong type, %s - %@",__func__,pathToFile);
					return ISFF_Source;
				}
				for (NSDictionary *inputDict in inputs)	{
					if ([inputDict isKindOfClass:[NSDictionary class]])	{
						NSString		*tmpString = nil;
						tmpString = [inputDict objectForKey:@"NAME"];
						if (tmpString!=nil && [tmpString isEqualToString:@"inputImage"])	{
							tmpString = [inputDict objectForKey:@"TYPE"];
							if (tmpString!=nil && [tmpString isEqualToString:@"image"])
								return ISFF_Filter;
						}
						else if (tmpString!=nil && [tmpString isEqualToString:@"startImage"])	{
							//NSLog(@"\t\tstart image - %@", pathToFile);
							tmpString = [inputDict objectForKey:@"TYPE"];
							if (tmpString!=nil && [tmpString isEqualToString:@"image"])
								hasTransitionStart = YES;
						}
						else if (tmpString!=nil && [tmpString isEqualToString:@"endImage"])	{
							//NSLog(@"\t\tend image - %@", pathToFile);
							tmpString = [inputDict objectForKey:@"TYPE"];
							if (tmpString!=nil && [tmpString isEqualToString:@"image"])
								hasTransitionEnd = YES;
						}
						else if (tmpString!=nil && [tmpString isEqualToString:@"progress"])	{
							//NSLog(@"\t\tprogress float - %@", pathToFile);
							tmpString = [inputDict objectForKey:@"TYPE"];
							if (tmpString!=nil && [tmpString isEqualToString:@"float"])
								hasTransitionProgress = YES;
						}						
					}
					
					if ((hasTransitionStart == YES)&&(hasTransitionEnd == YES)&&(hasTransitionProgress == YES))	{
						return ISFF_Transition;
					}
				}
			}
			else	{
				NSLog(@"\t\terr: jsonObject was wrong class, %s",__func__);
				NSLog(@"\t\terr: file was %@",pathToFile);
				return ISFF_Source;
			}
		}
	}

	return ISFF_Source;
}

@end


