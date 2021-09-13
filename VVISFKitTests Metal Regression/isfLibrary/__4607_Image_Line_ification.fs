
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "amountPixelsVisible",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "amountPixelsBlack",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "hardness",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
        "NAME": "stretchImage",
        "TYPE": "bool",
        "DEFAULT": 0
        }
		
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	vec2 xy = vec2(gl_FragCoord.xy);
	vec2 res = IMG_SIZE(inputImage);
	
	float pixels = amountPixelsVisible + amountPixelsBlack;
	xy.x -= res.x/2.;
	float curLines = xy.x / pixels;
	float off = fract(xy.x / pixels)*pixels;
    if (!stretchImage) xy.x += curLines;
	xy.x += res.x/2.;
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, xy);
	inputPixelColor = mix(inputPixelColor, inputPixelColor*(step(amountPixelsBlack, off)), hardness);
	
	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
