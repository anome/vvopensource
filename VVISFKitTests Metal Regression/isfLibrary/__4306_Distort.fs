
/*{
	"DESCRIPTION": "apply distortion",
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
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
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

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	
	float k0,k1,k2,k3,k4,r2,r4,r6;
	
	k0 = 0.0;
	k1 = 0.0;
	k2 = 0.0;
	k3 = 0.0;
	k4 = 0.0;
	
	float x = gl_FragCoord.x;
	float y = gl_FragCoord.y;
	
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	gl_FragColor = inputPixelColor;
	gl_FragColor.x = 0.0;
}
