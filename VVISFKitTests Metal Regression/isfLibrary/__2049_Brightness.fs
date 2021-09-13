/*{
	"CREDIT": "by INKA, based on color controls",
	"DESCRIPTION": "just a simple brightness filter",
	"CATEGORIES": [
		"Color Adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "multiply",
			"TYPE": "bool",
			"DEFAULT": false
		}
	]
}*/


void main() {
	vec4		color = IMG_THIS_PIXEL(inputImage);
	
	if(multiply) 
		color = color * vec4(brightness + 1.0, brightness + 1.0, brightness + 1.0, 1.0);
	else 
		color = color + vec4(brightness, brightness, brightness, 0.0);
	
	gl_FragColor = color;
}


