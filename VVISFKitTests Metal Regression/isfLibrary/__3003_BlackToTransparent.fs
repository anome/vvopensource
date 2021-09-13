/*{
	"CREDIT": "by DavidLublin",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "iThresh",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.05
		},
		{
			"NAME": "iColor",
			"TYPE": "color",
			"DEFAULT": [
    			0,
				0,
				0,
				0]
		}
	]
}*/

void main() {
	vec4	result = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	if (result.r+result.g+result.b < iThresh)
		result = iColor;
	gl_FragColor = result;
}