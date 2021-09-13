/*
	{
	"DESCRIPTION": "RGB to Greyscale",
	"CATEGORIES": 
		[
		"filter"
		],
	"ISFVSN": "2",
	"CREDIT": "Created by: Old Salt",
	"VSN": "1.0",
  "INPUTS":
		[
			{
			"NAME" : "inputImage",
			"TYPE" : "image"
			},
			{
			"LABEL": "Saturation: ",
			"NAME": "uSaturate",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 3.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Contrast: ",
			"NAME": "uContrast",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 10.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Compression: ",
			"NAME": "uCompress",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Invert Mask? ",
			"NAME": "uMaskInv",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "ά Channel Mask? ",
			"NAME": "uMaskAlpha",
			"TYPE": "bool",
			"DEFAULT": 0
			}
		]
	}
*/
// Function: Prepares Layer for use as a Brightness Mask

void main()
	{
	float col = length(IMG_THIS_PIXEL(inputImage).rgb);
	col = clamp((col * uSaturate), 0.0, 1.0);
	col = pow(col, uContrast);
	if (uMaskInv) col = 1.0 - col;
	col = col * (1.0 - uCompress) + uCompress;
	if (uMaskAlpha) gl_FragColor = vec4(vec3(0.0), col);
	else gl_FragColor = vec4(vec3(col), 1.0);
	}
