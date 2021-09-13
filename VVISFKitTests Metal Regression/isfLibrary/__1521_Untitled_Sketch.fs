/*{
	"CREDIT": "by radioexoticadj",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}