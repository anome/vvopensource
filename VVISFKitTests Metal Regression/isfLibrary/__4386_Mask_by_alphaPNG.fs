
/*{
	"DESCRIPTION": "Cover alpha PNG mask",
	"CREDIT": "VJ_RYO",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Masking"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "overImage",
			"TYPE": "image"
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
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
	//	both of these are also the same
	vec4		coverPixelColor;
	coverPixelColor = IMG_THIS_PIXEL(overImage);
	
	//second in
	
	
	gl_FragColor = mix(inputPixelColor,coverPixelColor,coverPixelColor.a);
}
