/*{
	"CREDIT": "Shane https://www.shadertoy.com/view/XsG3z1 adapted to IFS my morisil",
	"DESCRIPTION": "Reaction Diffusion - 2 Pass",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	]
}*/

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}
