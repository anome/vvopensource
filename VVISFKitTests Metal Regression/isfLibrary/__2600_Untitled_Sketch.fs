/*{
	"CREDIT": "by hellothisiscass",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"color effect"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		}
	]
}*/

void main() {
	gl_FragColor = colorInput;
}