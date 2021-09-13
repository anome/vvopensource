/*{
	"CREDIT": "by IMIMOT",
	"DESCRIPTION": "Simple Black & White shader",
	"CATEGORIES": [
		"Color Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	vec4 original = IMG_THIS_PIXEL(inputImage);
	gl_FragColor = vec4(vec3(original.r+original.g+original.b)/3.0, original.a);
}