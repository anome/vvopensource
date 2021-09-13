/*{
	"CREDIT": "by miguerodri87",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
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
			"NAME": "topLeft",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "topRight",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bottomLeft",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bottomRight",
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
		}
	]
}*/

void main() {
	vec2 uv = gl_FragCoord.xy;
	
	uv.x += bottomLeft.x*uv.x*uv.y;
	uv.y += bottomLeft.y*uv.x*uv.y;
		
	uv.x += bottomRight.x*(1.0 - uv.x)*uv.y;	
	uv.y += bottomRight.y*(1.0 - uv.x)*uv.y;

	uv.x += topLeft.x*uv.x*(1.0 - uv.y);
	uv.y += topLeft.y*uv.x*(1.0 - uv.y);

	uv.x += topRight.x*(1.0 - uv.x)*(1.0 - uv.y);

	gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
}