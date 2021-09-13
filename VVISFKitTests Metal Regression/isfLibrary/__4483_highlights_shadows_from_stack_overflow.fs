
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
			"NAME": "shadows",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "highlights",
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

// copied from: https://stackoverflow.com/a/57248822

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	float lumR = 0.299;
    float lumG = 0.587;
    float lumB = 0.114;
    
    float luminance = sqrt(lumR*pow(inputPixelColor.r,2.0) + 
        lumG*pow(inputPixelColor.g,2.0) + 
        lumB*pow(inputPixelColor.b,2.0));
        
    float h = highlights * 0.05 * ( pow(8.0, luminance) - 1.0 );
    float s = shadows * 0.05 * ( pow(8.0, 1.0 - luminance) - 1.0 );
    
    inputPixelColor.r = inputPixelColor.r + h + s;
    inputPixelColor.g = inputPixelColor.g + h + s;
    inputPixelColor.b = inputPixelColor.b + h + s;
	
	gl_FragColor = inputPixelColor;
}
