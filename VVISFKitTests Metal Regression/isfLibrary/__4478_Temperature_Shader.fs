
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
			"NAME": "temperature",
			"TYPE": "float",
			"DEFAULT": 6600,
			"MIN": 1500,
			"MAX": 15000
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
	vec4		inputPixelColor;
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
    float temp = temperature / 100.0;
    vec4 tempColor = vec4(255.0);
    
    if (temp <= 66.0) {
        // Red should be 255.0, which it already is
        
        // Green
        tempColor.g = temp;
	    tempColor.g = 99.4708025861 * log(tempColor.g) - 161.1195681661;

        // Blue should be 255.0, which it already is
    } else {
        // Red
        tempColor.r = temp - 60.0;
	    tempColor.r = 329.698727446 * pow(tempColor.r, -0.1332047592);
	    
	    // Green
	    tempColor.g = temp - 60.0;
	    tempColor.g = 288.1221695283 * pow(tempColor.g, -0.0755148492);
	    
	    // Blue
	    if (temp <= 19.0) {
	        tempColor.b = 0.0;
	    } else {
	        tempColor.b = temp - 10.0;
		    tempColor.b = 138.5177312231 * log(tempColor.b) - 305.0447927307;
	    }
    }

    tempColor = tempColor / 255.0;
    tempColor = clamp(tempColor, 0.0, 1.0);
	
	gl_FragColor = inputPixelColor * tempColor;
}
