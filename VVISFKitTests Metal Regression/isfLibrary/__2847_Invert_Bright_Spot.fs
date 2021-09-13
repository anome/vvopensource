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
			"DEFAULT": 0.9
		}
	]
	
}*/

void main()
{
    vec2 p = isf_FragNormCoord;

    
	vec4 col = IMG_THIS_PIXEL(inputImage);
	vec4 col2 = IMG_NORM_PIXEL(inputImage, SamplePoint);
	
 	// Convert sample pixel to grayscale to measure brightness
	col2 = vec4( (col2.x + col2.y + col2.z) / 3.);
	
	if (col2.x > Threshold) {
		col = vec4(1.0-col.r, 1.0-col.g, 1.0-col.b, col.a);
	}
	
	gl_FragColor = col;
}