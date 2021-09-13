/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "inspired by https://mouaif.wordpress.com/2009/01/28/levels-control-shader/",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "whiteMin",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "blackMin",
			"TYPE": "float",
			"DEFAULT": 0.18,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "blackPoint",
			"TYPE": "float",
			"DEFAULT": 0.4,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "whitePoint",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

void main() {
	vec4		inColor = IMG_THIS_PIXEL(inputImage);

	
	vec3 outColor = min(max(inColor.rgb - vec3(blackPoint, blackPoint, blackPoint), vec3(0.0, 0.0, 0.0)) / (vec3(1.0-whitePoint, 1.0-whitePoint, 1.0-whitePoint) - vec3(blackPoint, blackPoint, blackPoint)), vec3(1.0, 1.0, 1.0));
	outColor = mix(vec3(blackMin), vec3(1.0-whiteMin), outColor);
	
	
	
	gl_FragColor = vec4(outColor, inColor.a);
}