/*{
	"CREDIT": "by kawaiiidesu",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
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
      "MAX": 0.1,
      "MIN": 0.01,
      "DEFAULT": 0.05
    }
	]
}*/

void main() {
	float t;
	t=ceil(cos(TIME/freq));
	gl_FragColor = colorInput*t;
}