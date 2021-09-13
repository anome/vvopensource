
/*{
	"DESCRIPTION": "Slowly Invert an image.",
	"CREDIT": "zerbzman",
	"ISFVSN": "2",
	"CATEGORIES": [
		"color adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "invert",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"PASSES": [
	]
	
}*/

void main()	{
	vec4 inputPixelColor;
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
	inputPixelColor.rgb = abs((invert) - inputPixelColor.rgb);
	
	gl_FragColor = inputPixelColor;
}
