/*{
	"DESCRIPTION": "Invert Bright Spot",
	"CREDIT": "by Sadler Johnson",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Color Adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "SamplePoint",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{
			"NAME": "Threshold",
			"TYPE": "float",
			"DEFAULT": 0.3
		}
	],
	"PASSES": [
		{
			"TARGET":"BufferA",
			"PERSISTENT": true,
			"WIDTH": "1.0",
			"HEIGHT": "1.0"
		},
		{
		
		}
	]
}*/

void main()
{
	vec4 col; // = IMG_THIS_PIXEL(inputImage);
	vec4 col2; // = IMG_THIS_PIXEL(BufferA);
	
	
	if (PASSINDEX == 0)	{
		col2 = IMG_THIS_NORM_PIXEL(BufferA);
		
	    
	}
	//	second pass: read from "bufferVariableNameA".  output looks chunky and low-res.
	else if (PASSINDEX == 1)	{


		col = IMG_THIS_NORM_PIXEL(inputImage);

     	// Convert sample pixel to grayscale to measure brightness
    	col2 = vec4( (col2.x + col2.y + col2.z) / 3.);

    	if (col2.x > Threshold) {
    		col = vec4(1.0-col.r, 1.0-col.g, 1.0-col.b, col.a);
    	}
    	
    	gl_FragColor = col;

	}

}