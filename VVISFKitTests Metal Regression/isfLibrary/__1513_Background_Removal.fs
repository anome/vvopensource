/*{
	"CREDIT": "by msfeldstein",
	"DESCRIPTION": "Background Removal",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	gl_FragColor = IMG_THIS_PIXEL(inputImage);
}