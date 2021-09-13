
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
		}
	]
}*/

void main()	{
    float arr[6] = float[6](1., 2., 3., 4., 5., 6.);
	gl_FragColor = IMG_THIS_PIXEL(inputImage);
}
