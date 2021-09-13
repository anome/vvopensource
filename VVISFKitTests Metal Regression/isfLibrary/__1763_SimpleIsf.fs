/*{
	"CREDIT": "by magikmusik001",
	"DESCRIPTION": "",
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

void main() {
	
	float x =  isf_FragNormCoord.x;
	float y =  isf_FragNormCoord.y;
	
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy + vec2(0.5, 0.));
}