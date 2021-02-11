/*{
	"CREDIT": "by Anomes",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": -1000.0,
			"MAX": 1000.0,
			"DEFAULT": 1.0
		}
	],
}*/


void main()
{
	gl_FragColor = IMG_THIS_PIXEL(inputImage);
}

