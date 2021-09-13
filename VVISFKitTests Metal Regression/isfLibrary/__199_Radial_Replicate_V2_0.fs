/*
	{
	"DESCRIPTION": "Radial Replicate V2.0",
	"CATEGORIES": 
		[
		"filter",
		"Geometry Adjustment",
		"Tile Effect"
		],
	"ISFVSN": "2",
	"CREDIT": "Modified by: Old Salt",
	"VSN": "2.0",
  "INPUTS":
		[
			{
			"NAME" : "inputImage",
			"TYPE" : "image"
			},
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT": [0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":  [0.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT": [1.0,0.0,0.0,1.0]
			},
			{
			"NAME": "uC4",
			"TYPE": "color",
			"DEFAULT":  [1.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC5",
			"TYPE": "color",
			"DEFAULT":  [0.0,1.0,1.0,1.0]
			},
			{
			"NAME": "uC6",
			"TYPE": "color",
			"DEFAULT": [1.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC7",
			"TYPE": "color",
			"DEFAULT": [1.0,0.5,1.0,1.0]
			},
			{
			"NAME": "uC8",
			"TYPE": "color",
			"DEFAULT": [0.0,0.0,1.0,1.0]
			},
			{
			"LABEL": "Pre Effect Rotation Rough:",
			"NAME": "uPreRotateAngle",
			"TYPE": "float",
			"MIN": -180.0,
			"MAX": 180.0,
			"DEFAULT": -11.0
			},
			{
			"LABEL": "Pre Effect Rotation Fine: ",
			"NAME": "uPreRAfine",
			"TYPE": "float",
			"MIN": -5.0,
			"MAX": 5.0,
			"DEFAULT": 1.39
			},
			{
			"LABEL": "Radius Start: ",
			"NAME": "uCenterRadiusStart",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.06
			},
			{
			"LABEL": "Radius End: ",
      "NAME": "uCenterRadiusEnd",
      "TYPE": "float",
			"MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 0.37
			},
			{
			"LABEL": "Number Of Divisions: ",
			"NAME": "uNumberOfDivisions",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 360.0,
			"DEFAULT": 12.0
			},
			{
			"LABEL": "Offset Result: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [-0.26,0.09]
			},
			{
			"LABEL": "Post Effect Rotate Angle: ",
			"NAME": "uPostRotateAngle",
			"TYPE": "float",
			"MIN": -180.0,
			"MAX": 180.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uRotate",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Center Core Color: ",
			"LABELS":
				[
				"Use Original Colors                            ",
				"Convert to Black and White                     ",
				"Gradient: Black to Alternate Color #7           ",
				"Exchange RGB Channels With Alternate Colors 1-3 ",
				"Exchange RGB Channels With Alternate Colors 4-6 ",
				"Mask: Gradient (Color #8 Intensity is Degree)  ",
				"Mask: Absolute (Opacity Slider Sets Threshold) "
				],
			"NAME": "cCore",
			"TYPE": "long",
			"VALUES": [0,1,2,3,4,5,6],
			"DEFAULT": 0
			},
			{
			"LABEL": "Center Core Intensity: ",
			"NAME": "iCore",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Center Core Opacity: ",
			"NAME": "oCore",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Invert Core Mask? ",
			"NAME": "cMinvert",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Show Core Mask? ",
			"NAME": "cMask",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Effect Color: ",
			"LABELS":
				[
				"Use Original Colors                            ",
				"Convert to Black and White                     ",
				"Gradient: Black to Alternate Color #7           ",
				"Exchange RGB Channels With Alternate Colors 1-3 ",
				"Exchange RGB Channels With Alternate Colors 4-6 ",
				"Mask: Gradient (Color #8 Intensity is Degree)  ",
				"Mask: Absolute (Opacity Slider Sets Threshold) "
				],
			"NAME": "cEffect",
			"TYPE": "long",
			"VALUES": [0,1,2,3,4,5,6],
			"DEFAULT": 0
			},
			{
			"LABEL": "Effect Intensity: ",
			"NAME": "iEffect",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Effect Opacity: ",
			"NAME": "oEffect",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Invert Effect Mask? ",
			"NAME": "eMinvert",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Show Effect Mask? ",
			"NAME": "eMask",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Background Color: ",
			"LABELS":
				[
				"Use Original Colors                            ",
				"Convert to Black and White                     ",
				"Gradient: Black to Alternate Color #7           ",
				"Exchange RGB Channels With Alternate Colors 1-3 ",
				"Exchange RGB Channels With Alternate Colors 4-6 ",
				"Mask: Gradient (Color #8 Intensity is Degree)  ",
				"Mask: Absolute (Opacity Slider Sets Threshold) "
				],
			"NAME": "mBkgd",
			"TYPE": "long",
			"VALUES": [0,1,2,3,4,5,6],
			"DEFAULT": 0
			},
			{
			"LABEL": "Background Intensity: ",
			"NAME": "iBkgd",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Background Opacity: ",
			"NAME": "oBkgd",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Invert Background Mask? ",
			"NAME": "bMinvert",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Show Background Mask? ",
			"NAME": "bMask",
			"TYPE": "bool",
			"DEFAULT": 1
			}
		]
	}
*/
// Function: Repllcates a radial slice of an image
// Original by: VIDVOX
// Source: https://editor.isf.video/shaders/5e7a7feb7c113618206de6f6

#define pi 3.1415926535897932384626433832795
#define sqrt3 1.7320508075688772935274463415059


vec4 colMod
		(
		vec4 cIn,      // input color
		int cMod,      // select color modification
		float iMod,    // intensity modifier
		float oSet,    // opacity
		bool iMask,    // invert mask
		bool mask      // show mask if set
		)
	{
	float iInt = length(cIn.rgb); // intensity of the input color
	float opac;
	
	vec3 cOut = cIn.rgb;          // set for color swap / Use Original Colors
	if (cMod==1)                  // Convert to Black and White
		{
		cOut = vec3(length(cOut)/sqrt3);
		}
	if (cMod==2)                  // "Gradient: Black to Alternate Color #7
		{
		cOut = uC7.rgb * vec3(length(cOut)/length(uC7.rgb));
		}
	if (cMod==3)                  // Exchange RGB Channels With Alternate Colors 1-3
		{
		cOut = uC1.rgb * cIn.r;
		cOut += uC2.rgb * cIn.g;
		cOut += uC3.rgb * cIn.b;
		cOut = cOut * iInt/sqrt3;
		}
	if (cMod==4)                  // Exchange RGB Channels With Alternate Colors 4-6
		{
		cOut = uC4.rgb * cIn.r;
		cOut += uC5.rgb * cIn.g;
		cOut += uC6.rgb * cIn.b;
		cOut = cOut * iInt/sqrt3;
		}
	if (cMod==5)                  // Mask: Gradient (Set Color #8 for Best Mask View)
		{
		opac = length(cOut*iMod)*oSet;
		if (iMask) opac = 1.0 - opac;
		if (! mask) cOut = vec3(0.0);
    else cOut = uC8.rgb * opac;
		}
	if (cMod==6)                  // Mask: Absolute (Opacity Slider Sets Threshhold)
		{
		opac = length(clamp(cOut*iMod, vec3(0.0), vec3(1.0)));
		opac = step(oSet, opac);
		if (iMask) opac = 1.0 - opac;
		if (! mask) cOut = vec3(0.0);
    else cOut = uC8.rgb * opac;
		}
	if (cMod > 4)                 // these are the masks
		{
		return vec4(cOut, opac);
		}
	else
		{														// these are color routines
		cOut = clamp(cOut*iMod, vec3(0.0), vec3(1.0));
		return vec4(cOut, oSet);
		}
	}

void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - uOffset; // normalize and offset coordinates
	vec2 loc = IMG_SIZE(inputImage) * uv;  // changing this shifts the output

	//	'r' is the radius - the distance in pixels from 'loc' to the center of the rendering space
	float		r = distance(IMG_SIZE(inputImage)/2.0, loc);
	//	'a' is the angle of the line segment from the center to loc is rotated
	float		a = atan ((loc.y-IMG_SIZE(inputImage).y/2.0),(loc.x-IMG_SIZE(inputImage).x/2.0));

	float		modAngle = 2.0 * pi / floor(uNumberOfDivisions);
	float		scaledCenterRadiusStart = uCenterRadiusStart * max(RENDERSIZE.x,RENDERSIZE.y);
	float		scaledCenterRadiusEnd = uCenterRadiusEnd * max(RENDERSIZE.x,RENDERSIZE.y);
	if (scaledCenterRadiusStart > scaledCenterRadiusEnd)
		{
		scaledCenterRadiusStart = scaledCenterRadiusEnd;
		scaledCenterRadiusEnd = uCenterRadiusStart * max(RENDERSIZE.x,RENDERSIZE.y);
		}

/*	****************************************************************
		*****        All output color is determined below          *****
		***** Changes implemented to allow different color handling ****
		for the three distinct zones in this shader:
			- area inside the replication zone - 'core'
			- radial replication zone - 'effect'
			- area outside the replication zone - 'bkgd'

	'colMod' is called to change the output color for each
	(vec4 orginalColor, 
	int modificationMethod,     // determined by JSON user input
	float intensityMultiplier,  // determined by JSON user input
	float opacity               // determined by JSON user input
	bool mInvert                // invert mask if set
	bool mask                   // show mask if set
*/

	vec4 colorOut = vec4(0.0);
	if (r <= scaledCenterRadiusStart)
		{ // this is the core - shifted input pixel location used
		colorOut = IMG_NORM_PIXEL(inputImage, uv);
		colorOut = colMod(colorOut, cCore, iCore, oCore, cMinvert, cMask);
		}
	else
		{ // this is the background - it's not shifted!
		colorOut = IMG_NORM_PIXEL(inputImage,isf_FragNormCoord);
		colorOut = colMod(colorOut, mBkgd, iBkgd, oBkgd, bMinvert, bMask);
		}
	if ((uCenterRadiusEnd != uCenterRadiusStart)&&(r >= scaledCenterRadiusStart)&&(r <= scaledCenterRadiusEnd))
		{
		r = (r - scaledCenterRadiusStart) / (uCenterRadiusEnd - uCenterRadiusStart);
		// add rotation
		if (uRotate) a = mod(a + TIME + pi * uPostRotateAngle/360.0,modAngle);
			else a = mod(a + pi * uPostRotateAngle/360.0,modAngle);
		

		//	now modify 'a', and convert the modified polar coords (radius/angle) back to cartesian coords (x/y pixels)
		loc.x = r * cos(a + 2.0 * pi * (uPreRotateAngle + uPreRAfine) / 360.0);
		loc.y = r * sin(a + 2.0 * pi * (uPreRotateAngle + uPreRAfine) / 360.0);

		loc = loc / IMG_SIZE(inputImage).xy + vec2(0.5);// *** here

		if ((loc.x >= 0.0)&&(loc.y >= 0.0)&&(loc.x <= 1.0)&&(loc.y <= 1.0))  // boundary check
			{
			colorOut = IMG_NORM_PIXEL(inputImage,loc);               // this is the effect
			colorOut = colMod(colorOut, cEffect, iEffect, oEffect, eMinvert, eMask);  // modify it!
			}
		}
	gl_FragColor = colorOut;
	}
