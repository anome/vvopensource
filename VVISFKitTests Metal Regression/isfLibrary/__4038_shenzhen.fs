/*{
	"CREDIT": "me sam",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "tiles",
			"TYPE": "float",
			"DEFAULT": 5,
			"MIN": 4,
			"MAX": 20
		},
		{
			"NAME": "powa",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0,
			"MAX": 2
		},
		{
			"NAME": "image01",
			"TYPE": "image"
		}
	]
}*/

#define PI 3.14159265359

void main() {
	// Create the circle in the center
	
	float d = distance(isf_FragNormCoord.xy, vec2(0.5));
	float a = atan(isf_FragNormCoord.x - 0.5, isf_FragNormCoord.y - 0.5);
	a = (a + PI) / (2.0 * PI);
	
	vec2 texcoord0 = vec2(fract(a * tiles), fract(pow(d, powa) * tiles));
	gl_FragColor = IMG_NORM_PIXEL(image01, texcoord0);
}