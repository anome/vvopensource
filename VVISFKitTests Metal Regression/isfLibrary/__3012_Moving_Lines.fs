/*{
	"CREDIT": "by nagl23_",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"Glitch"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/

void main() {

	
	float boxSize = 8.0;
	
	//isf_FragNormCoord.xy // (0, 1)
	
	vec2 pixelCoord;
	pixelCoord.x = isf_FragNormCoord.x * RENDERSIZE.x;
	pixelCoord.y = isf_FragNormCoord.y * RENDERSIZE.y;

	

	if ( mod(pixelCoord.x , boxSize*2.0) < boxSize || mod(pixelCoord.y , boxSize*2.0) < boxSize)  {
		gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	} else {
		gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
	}
	
	
}