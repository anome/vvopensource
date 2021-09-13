
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
			"NAME": "tint",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 1.5
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
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	vec3 tintedPixelColor = inputPixelColor.rgb * vec3(1.0, tint, 1.0);
	gl_FragColor = vec4(tintedPixelColor, inputPixelColor.a);
}


