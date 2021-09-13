/*{
	"CREDIT": "by hellothisiscass",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"transition"
	],
	"INPUTS": [
		{
			"NAME": "startImage",
			"TYPE": "image"
		},
		{
			"NAME": "endImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}