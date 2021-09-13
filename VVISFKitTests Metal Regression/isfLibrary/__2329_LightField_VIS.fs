/*{
	"CREDIT": "by johannessaam",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "CamPos",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}