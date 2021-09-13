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
		}
	]
}*/

void main() {
	switch (PASSINDEX)	{
	case 0:
		gl_FragColor = vec4(1.0);
		break;
	default:
		gl_FragColor = vec4(0.0);
		break;
}