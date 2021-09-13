/*{
    "CATEGORIES": [
        "Blur", "Masking"
    ],
    "CREDIT": "original implementation as v002.blur in QC by anton marini and tom butterworth, ported by zoidberg",
    "DESCRIPTION": "Applies a blur to only part of an image using a shape mask",
    "INPUTS": [
        {
            "NAME": "inputImage",
            "TYPE": "image"
        },
        {
            "DEFAULT": 24,
            "MAX": 24,
            "MIN": 0,
            "NAME": "blurAmount",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "LABEL": "Mask Shape Mode",
            "LABELS": [
                "Rectangle",
                "Triangle",
                "Circle",
                "Diamond"
            ],
            "NAME": "maskShapeMode",
            "TYPE": "long",
            "VALUES": [
                0,
                1,
                2,
                3
            ]
        },
        {
            "DEFAULT": 0.5,
            "LABEL": "Shape Width",
            "MAX": 2,
            "MIN": 0,
            "NAME": "shapeWidth",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "LABEL": "Shape Height",
            "MAX": 2,
            "MIN": 0,
            "NAME": "shapeHeight",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                0.5,
                0.5
            ],
            "MAX": [
                1,
                1
            ],
            "MIN": [
                0,
                0
            ],
            "NAME": "center",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": false,
            "LABEL": "Invert Mask",
            "NAME": "invertMask",
            "TYPE": "bool"
        }
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "DESCRIPTION": "Pass 0",
            "HEIGHT": "floor($HEIGHT/3.0)",
            "TARGET": "halfSizeBaseRender",
            "WIDTH": "floor($WIDTH/3.0)"
        },
        {
            "DESCRIPTION": "Pass 1",
            "HEIGHT": "floor($HEIGHT/6.0)",
            "TARGET": "quarterSizeBaseRender",
            "WIDTH": "floor($WIDTH/6.0)"
        },
        {
            "DESCRIPTION": "Pass 2",
            "HEIGHT": "floor($HEIGHT/12.0)",
            "TARGET": "eighthSizeBaseRender",
            "WIDTH": "floor($WIDTH/12.0)"
        },
        {
            "DESCRIPTION": "Pass 3",
            "HEIGHT": "floor($HEIGHT/12.0)",
            "TARGET": "eighthGaussA",
            "WIDTH": "floor($WIDTH/12.0)"
        },
        {
            "DESCRIPTION": "Pass 4",
            "HEIGHT": "floor($HEIGHT/12.0)",
            "TARGET": "eighthGaussB",
            "WIDTH": "floor($WIDTH/12.0)"
        },
        {
            "DESCRIPTION": "Pass 5",
            "HEIGHT": "floor($HEIGHT/6.0)",
            "TARGET": "quarterGaussA",
            "WIDTH": "floor($WIDTH/6.0)"
        },
        {
            "DESCRIPTION": "Pass 6",
            "HEIGHT": "floor($HEIGHT/6.0)",
            "TARGET": "quarterGaussB",
            "WIDTH": "floor($WIDTH/6.0)"
        },
        {
            "DESCRIPTION": "Pass 7",
            "HEIGHT": "floor($HEIGHT/3.0)",
            "TARGET": "halfGaussA",
            "WIDTH": "floor($WIDTH/3.0)"
        },
        {
            "DESCRIPTION": "Pass 8",
            "HEIGHT": "floor($HEIGHT/3.0)",
            "TARGET": "halfGaussB",
            "WIDTH": "floor($WIDTH/3.0)"
        },
        {
            "DESCRIPTION": "Pass 9",
            "TARGET": "fullGaussA"
        },
        {
            "DESCRIPTION": "Pass 10",
            "TARGET": "fullGaussB"
        }
    ],
    "VSN": null
}
*/


/*
																			eighth
													quarter					0	1	2	3	4	5		"blurRadius" (different resolutions have different blur radiuses based on the "blurAmount" and its derived "blurLevel")
							half					0	1	2	3	4	5								"blurRadius"
	normal					0	1	2	3	4	5														"blurRadius"
	0	1	2	3	4	5	5																			"blurRadius"
	0						6						12						18						24	"blurAmount" (attrib)
				0						1						2						3				"blurLevel" (local var)
*/


varying vec2		texOffsets[5];
const float pi = 3.1415926535;


vec2 rotatePoint(vec2 pt, float angle, vec2 center)
{
	vec2 returnMe;
	float s = sin(angle * pi);
	float c = cos(angle * pi);

	returnMe = pt;

	// translate point back to origin:
	returnMe.x -= center.x;
	returnMe.y -= center.y;

	// rotate point
	float xnew = returnMe.x * c - returnMe.y * s;
	float ynew = returnMe.x * s + returnMe.y * c;

	// translate point back:
	returnMe.x = xnew + center.x;
	returnMe.y = ynew + center.y;
	return returnMe;
}

float sign(vec2 p1, vec2 p2, vec2 p3)
{
	return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
}

bool PointInTriangle(vec2 pt, vec2 v1, vec2 v2, vec2 v3)
{
	bool b1, b2, b3;

	b1 = sign(pt, v1, v2) < 0.0;
	b2 = sign(pt, v2, v3) < 0.0;
	b3 = sign(pt, v3, v1) < 0.0;

	return ((b1 == b2) && (b2 == b3));
}

bool RotatedPointInTriangle(vec2 pt, vec2 v1, vec2 v2, vec2 v3, vec2 center)
{
	bool b1, b2, b3;
	
	vec2 v1r = v1;
	vec2 v2r = v2;
	vec2 v3r = v3;

	b1 = sign(pt, v1r, v2r) < 0.0;
	b2 = sign(pt, v2r, v3r) < 0.0;
	b3 = sign(pt, v3r, v1r) < 0.0;

	return ((b1 == b2) && (b2 == b3));
}


float isPointInShape(vec2 pt, int shape, vec4 shapeCoordinates)	{
	float returnMe = 0.0;
	
	//	rectangle
	if (shape == 0)	{
		if (RotatedPointInTriangle(pt, shapeCoordinates.xy, shapeCoordinates.xy + vec2(0.0, shapeCoordinates.w), shapeCoordinates.xy + vec2(shapeCoordinates.z, 0.0), shapeCoordinates.xy + shapeCoordinates.zw / 2.0))	{
			returnMe = 1.0;
			// soft edge if needed
			if ((pt.x > shapeCoordinates.x) && (pt.x < shapeCoordinates.x)) {
				returnMe = clamp(((pt.x - shapeCoordinates.x) / RENDERSIZE.x), 0.0, 1.0);
				returnMe = pow(returnMe, 0.5);
			}
			else if ((pt.x > shapeCoordinates.x + shapeCoordinates.z) && (pt.x < shapeCoordinates.x + shapeCoordinates.z)) {
				returnMe = clamp(((shapeCoordinates.x + shapeCoordinates.z - pt.x) / RENDERSIZE.x), 0.0, 1.0);
				returnMe = pow(returnMe, 0.5);
			}
		}
		else if (RotatedPointInTriangle(pt, shapeCoordinates.xy + shapeCoordinates.zw, shapeCoordinates.xy + vec2(0.0, shapeCoordinates.w), shapeCoordinates.xy + vec2(shapeCoordinates.z, 0.0), shapeCoordinates.xy + shapeCoordinates.zw / 2.0))	{
			returnMe = 1.0;
			// soft edge if needed
			if ((pt.x > shapeCoordinates.x) && (pt.x < shapeCoordinates.x)) {
				returnMe = clamp(((pt.x - shapeCoordinates.x) / RENDERSIZE.x), 0.0, 1.0);
				returnMe = pow(returnMe, 0.5);
			}
			else if ((pt.x > shapeCoordinates.x + shapeCoordinates.z) && (pt.x < shapeCoordinates.x + shapeCoordinates.z)) {
				returnMe = clamp(((shapeCoordinates.x + shapeCoordinates.z - pt.x) / RENDERSIZE.x), 0.0, 1.0);
				returnMe = pow(returnMe, 0.5);
			}
		}
	}
	//	triangle
	else if (shape == 1)	{
		if (RotatedPointInTriangle(pt, shapeCoordinates.xy, shapeCoordinates.xy + vec2(shapeCoordinates.z / 2.0, shapeCoordinates.w), shapeCoordinates.xy + vec2(shapeCoordinates.z, 0.0), shapeCoordinates.xy + shapeCoordinates.zw / 2.0))	{
			returnMe = 1.0;
		}
	}
	//	oval
	else if (shape == 2)	{
		returnMe = distance(pt, vec2(shapeCoordinates.xy + shapeCoordinates.zw / 2.0));
		if (returnMe < min(shapeCoordinates.z,shapeCoordinates.w) / 2.0)	{
			returnMe = 1.0;
		}
		else	{
			returnMe = 0.0;
		}
	}
	//	diamond
	else if (shape == 3)	{
		if (RotatedPointInTriangle(pt, shapeCoordinates.xy + vec2(0.0, shapeCoordinates.w / 2.0), shapeCoordinates.xy + vec2(shapeCoordinates.z / 2.0, shapeCoordinates.w), shapeCoordinates.xy + vec2(shapeCoordinates.z, shapeCoordinates.w / 2.0), shapeCoordinates.xy + shapeCoordinates.zw / 2.0))	{
			returnMe = 1.0;
		}
		else if (RotatedPointInTriangle(pt, shapeCoordinates.xy + vec2(0.0, shapeCoordinates.w / 2.0), shapeCoordinates.xy + vec2(shapeCoordinates.z / 2.0, 0.0), shapeCoordinates.xy + vec2(shapeCoordinates.z, shapeCoordinates.w / 2.0), shapeCoordinates.xy + shapeCoordinates.zw / 2.0))	{
			returnMe = 1.0;
		}
	}

	return returnMe;	
}


void main() {
	int			blurLevel = int(floor(blurAmount/6.0));
	float		blurLevelModulus = mod(blurAmount, 6.0);
	//	first three passes are just copying the input image into the buffer at varying sizes
	if (PASSINDEX==0)	{
		gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord);
	}
	else if (PASSINDEX==1)	{
		gl_FragColor = IMG_NORM_PIXEL(halfSizeBaseRender, isf_FragNormCoord);
	}
	else if (PASSINDEX==2)	{
		gl_FragColor = IMG_NORM_PIXEL(quarterSizeBaseRender, isf_FragNormCoord);
	}
	//	start reading from the previous stage- each two passes completes a gaussian blur, then 
	//	we increase the resolution & blur (the lower-res blurred image from the previous pass) again...
	else if (PASSINDEX == 3)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[2]);
		vec4		sample3 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[3]);
		vec4		sample4 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[4]);
		//gl_FragColor = vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0);
		gl_FragColor = vec4((sample0 + sample1 + sample2 + sample3 + sample4).rgba / (5.0));
	}
	else if (PASSINDEX == 4)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[2]);
		vec4		sample3 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[3]);
		vec4		sample4 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[4]);
		//gl_FragColor = vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0);
		gl_FragColor = vec4((sample0 + sample1 + sample2 + sample3 + sample4).rgba / (5.0));
	}
	//	...writes into the quarter-size
	else if (PASSINDEX == 5)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
	}
	else if (PASSINDEX == 6)	{
		vec4		sample0 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
	}
	//	...writes into the half-size
	else if (PASSINDEX == 7)	{
		vec4		sample0 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
	}
	else if (PASSINDEX == 8)	{
		vec4		sample0 = IMG_NORM_PIXEL(halfGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(halfGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(halfGaussA,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
	}
	//	...writes into the full-size
	else if (PASSINDEX == 9)	{
		vec4		sample0 = IMG_NORM_PIXEL(halfGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(halfGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(halfGaussB,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
	}
	else if (PASSINDEX == 10)	{
		vec2		centerPt = RENDERSIZE * center;
		vec2		tmpVec = RENDERSIZE * vec2(shapeWidth,shapeHeight) / 2.0;
		vec4		patternRect = vec4(vec2(centerPt - tmpVec),tmpVec * 2.0);
		vec2		thisPoint = RENDERSIZE * isf_FragNormCoord;
	
		if ((thisPoint.x >= patternRect.x) && (thisPoint.x <= patternRect.x + abs(patternRect.z)))	{
			patternRect.x = patternRect.x + abs(patternRect.z) * floor((thisPoint.x - patternRect.x) / abs(patternRect.z));
		}
	
		if ((thisPoint.y >= patternRect.y) && (thisPoint.y <= patternRect.y + abs(patternRect.w)))	{
			patternRect.y = patternRect.y + abs(patternRect.w) * floor((thisPoint.y - patternRect.y) / abs(patternRect.w));
		}
	
		float		luminance = isPointInShape(thisPoint.xy, maskShapeMode, patternRect);
	
		if (invertMask)
			luminance = 1.0 - luminance;
		
		if (luminance > 0.0)	{
			//	this is the last pass- calculate the blurred image as i have in previous passes, then mix it in with the full-size input image using the blur amount so i get a smooth transition into the blur at low blur levels
			vec4		sample0 = IMG_NORM_PIXEL(fullGaussA,texOffsets[0]);
			vec4		sample1 = IMG_NORM_PIXEL(fullGaussA,texOffsets[1]);
			vec4		sample2 = IMG_NORM_PIXEL(fullGaussA,texOffsets[2]);
			vec4		blurredImg =  vec4((sample0 + sample1 + sample2).rgba / (3.0));
		
			if (blurLevel == 0)
				gl_FragColor = mix(IMG_NORM_PIXEL(inputImage,isf_FragNormCoord), blurredImg, (blurLevelModulus/6.0));
			else
				gl_FragColor = blurredImg;
		}
		else	{
			gl_FragColor = IMG_NORM_PIXEL(inputImage,isf_FragNormCoord);
		}
	}
	
}
