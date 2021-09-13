/*
	{
	"DESCRIPTION": "Color Change Options",
	"CATEGORIES": 
		[
		"filter"
		],
	"ISFVSN": "2",
	"CREDIT": "Created  by: Old Salt",
	"VSN": "1.0",
  "INPUTS":
		[
			{
			"NAME" : "inputImage",
			"TYPE" : "image"
			},
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT": [1.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":  [1.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT": [0.0,1.0,1.0,1.0]
			},
			{
			"LABEL": "Options: ",
			"LABELS":
				[
				"Use Original Colors - Adjust Brightness and/or Opacity ",
				"Exchange RGB Channels With Alternate Colors 1-3 ",
				"Convert to Black and White ",
				"Gradient from Black to Alternate Color 1 "
				],
			"NAME": "uColSel",
			"TYPE": "long",
			"VALUES": [0,1,2,3],
			"DEFAULT": 1
			},
			{
			"LABEL": "Brightness: ",
			"NAME": "uBright",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Opacity: ",
			"NAME": "uOpacity",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Invert Colors? ",
			"NAME": "uInvert",
			"TYPE": "bool",
			"DEFAULT": 0
			}
		]
	}
*/
// Function: Provides Color Options

vec4 colMod(vec4 cIn)      // input color
	{
	float cBright = length(cIn.rgb)/1.732; // normalized intensity of the input color
	float opac;
	vec3 cOut = cIn.rgb;     // set for color swap / Use Original Colors
                           // Use Original Colors, Adjust Intensity and/or Opacity 
	if (uColSel==1)          // Exchange RGB Channels With Alternate Colors 1-3
		{
		cOut = uC1.rgb * cIn.r;
		cOut += uC2.rgb * cIn.g;
		cOut += uC3.rgb * cIn.b;
		cOut = cOut * cBright;
		}
	if (uColSel==2)          // Convert to Black and White
		{
		cOut = vec3(cBright);
		}
	if (uColSel==3)          // "Gradient: Black to Alternate Color #1
		{
		cOut = uC1.rgb * cBright;
		}
	if (uInvert)             // Invert Colors
		{
		cOut = vec3(1.0) - cOut;
		}
	return vec4(cOut * uBright, uOpacity);
	}

void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.
	gl_FragColor = colMod(IMG_THIS_PIXEL(inputImage));
	}
