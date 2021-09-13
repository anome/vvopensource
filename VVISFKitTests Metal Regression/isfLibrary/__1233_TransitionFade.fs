/*{
	"CREDIT": "by axiomcrux",
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
		},
		{
			"NAME": "fade",
			"TYPE": "float"
		}
	]
}*/

void main() {
	
	gl_FragColor = mix(IMG_NORM_PIXEL(startImage, isf_FragNormCoord.xy),
	 IMG_NORM_PIXEL(endImage, isf_FragNormCoord.xy),
	 fade);
}