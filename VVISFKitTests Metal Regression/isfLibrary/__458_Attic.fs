
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
		},
		{
			"NAME": "strength",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 1.0
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

#define PI 3.1415926538


void main()	{
	vec4		ic;
	//	both of these are the same
// 	inputPixelColor = IMG_THIS_PIXEL(inputImage);
// 	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	ic = IMG_THIS_NORM_PIXEL(inputImage);
	
	// Adapted from: https://www.shadertoy.com/view/XllSzM
    vec3 sepia = vec3(1.2, 1.0, 0.8);
	
    float grey = dot(ic.rgb, vec3(0.299, 0.587, 0.114));
    
    vec3 sepiaColour = vec3(grey) * sepia;
    
    vec3 sepified = mix(ic.rgb, vec3(sepiaColour), 0.75);
    
    vec2 center = vec2(0.5, 0.5);
	
	vec3 oc = sepified.rgb * clamp((1.5 - sin(clamp(distance(center, isf_FragNormCoord.xy)/1.5, 0.0, 1.0) * PI)) * (1.5 - strength), 0.0, 1.0);

	gl_FragColor = vec4(oc, ic.a);
}
