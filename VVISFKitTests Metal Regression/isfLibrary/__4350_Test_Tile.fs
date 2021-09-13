
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
			"NAME": "number",
			"TYPE": "float",
			"DEFAULT": 2,
			"MIN": 0.0,
			"MAX": 100.0
		},

		{
			"NAME": "pointInput",
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

void main()	{
	vec4		inputPixelColor;
	vec2 uv= gl_FragCoord.xy;
	
	uv.x=uv.x-RENDERSIZE.x;
	uv.y=uv.y-RENDERSIZE.y;
	
	uv*=number;
	
	
	
	uv=mod(uv,gl_FragCoord.xy);
	//uv= 2.0 * abs(uv - 0.5);
	
	
	//	both of these are the same
//	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, uv);
	
	//	both of these are also the same
//	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
//	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
