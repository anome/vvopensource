
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
			"NAME": "numColors",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 0.0,
			"MAX": 100.0
		},
        {
			"NAME": "gamma",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		}],
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
    // https://www.geeks3d.com/20091027/shader-library-posterization-post-processing-effect-glsl/
	vec4		inputPixelColor;
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
	vec3 c = inputPixelColor.rgb;
	
	c = pow(c, vec3(gamma));
	c = c * numColors;
	c = floor(c);
	c = c / numColors;
	c = pow(c, vec3(1.0 / gamma));
	
	gl_FragColor = vec4(c, 1.0);
}
