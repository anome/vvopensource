
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
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
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
// 	vec4		inputPixelColor;
// 	//	both of these are the same
// 	inputPixelColor = IMG_THIS_PIXEL(inputImage);
// 	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
// 	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);

    vec3 color = IMG_THIS_PIXEL(inputImage).rgb;
	vec3 colorContrasted = (color) * contrast;
	vec3 bright = colorContrasted + vec3(brightness,brightness,brightness);
	gl_FragColor.rgb = bright;
	gl_FragColor.a = 1.;
	
// 	gl_FragColor = inputPixelColor;
}
