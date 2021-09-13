/*{
	"DESCRIPTION": "simple background generator",
	"CREDIT": "by punker simon",
	"": "",
	"CATEGORIES": [
		"TEST-GLSL"
	],
	"INPUTS": [
		{
			"NAME": "Red",
			"TYPE": "float",
			"LABEL": "red Level",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
			{
			"NAME": "Green",
			"TYPE": "float",
			"LABEL": "Green Level",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},	{
			"NAME": "Blue",
			"TYPE": "float",
			"LABEL": "Gray Level",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
			{
			"NAME": "Alpha",
			"TYPE": "float",
			"LABEL": "Alphe Level",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

void main() {
	gl_FragColor = vec4(Red,Green,Blue,Alpha);
}


