
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
			"NAME": "strength",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 1.0
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
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
	ic = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	vec3 oc = ic.rgb * clamp((1.5 - sin(clamp(distance(center, isf_FragNormCoord.xy)/4.0, 0.0, 1.0) * PI)) * (1.5 - strength), 0.0, 1.0);
	
// 	gl_FragColor = vec4(0.5 - sin(distance(vec2(0.5, 0.5), isf_FragNormCoord.xy) * PI), 0.0, 0.0, 1.0);
	gl_FragColor = vec4(oc.rgb, ic.a);
	
}