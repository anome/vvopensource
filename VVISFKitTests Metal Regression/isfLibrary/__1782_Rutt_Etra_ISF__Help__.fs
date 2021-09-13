/*{
	"CREDIT": "by visuality77",
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
varying vec2 texcoord0;
 
uniform inputImage tex0;
uniform vec2 imageSize;
uniform float useRaw;
uniform float extrude;

const vec4 lumcoeff = vec4(0.299,0.587,0.114,0.);

void main(void) {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord.xy);
	vec4 pixel = texture2DRect(tex0, texcoord0);
    float luma = dot(lumcoeff, pixel);
    float normX = floor(gl_FragCoord.x) / (imageSize.x - 1.0);
    float normY = floor(gl_FragCoord.y) / (imageSize.y - 1.0);
    vec4 lumaPixel = vec4(normX, normY, luma * extrude, 1.0);
 
 gl_FragData[0] = mix(lumaPixel, vec4(pixel.rgb, 1.0), useRaw);
 gl_FragData[1] = vec4(normX, normY, 0.0, 1.0);
}