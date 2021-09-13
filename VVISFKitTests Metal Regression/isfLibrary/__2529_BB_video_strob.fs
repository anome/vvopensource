/*{
	"CREDIT": "by kawaiiidesu",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"BB_effect"
	],
	"INPUTS": [
		{
      "NAME": "inputImage",
      "TYPE": "image"
    },
	 {
      "NAME": "freq",
      "TYPE": "float",
      "MAX": 10,
      "MIN": 1,
      "DEFAULT": 5
    }
	]
}*/

void main() {
	float t;
	t=step(0.9,cos(TIME*freq));
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy)*t;
}