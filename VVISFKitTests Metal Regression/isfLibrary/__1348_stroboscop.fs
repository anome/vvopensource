/*{
	"CREDIT": "by kawaiiidesu",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"yura"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "freq",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/
uniform float t;

void main() {
	t = freq*TIME;
	
	gl_FragColor = colorInput;
}