/*{
	"DESCRIPTION": "",
	"CREDIT": "shader by v002, ported by zoidberg",
	"CATEGORIES": [
		"Halftone Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "inMin",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "inMax",
			"TYPE": "float",
			"DEFAULT": 1.0
		},
		{
			"NAME": "outMin",
			"TYPE": "float",
			"DEFAULT": 0.0
		},
		{
			"NAME": "outMax",
			"TYPE": "float",
			"DEFAULT": 1.0
		},
		{
			"NAME": "midPoint",
			"TYPE": "float",
			"DEFAULT": 0.5
		}
	]
}*/


void main(void)
{
	vec4		color = IMG_PIXEL(inputImage,gl_FragCoord.xy);
	vec4 inputRange = min(max(color - vec4(inMin), vec4(0.0)) / (vec4(inMax) - vec4(inMin)), vec4(1.0));
	inputRange = pow(inputRange, vec4(1.0 / (1.5 - midPoint)));
	    
	gl_FragColor = mix(vec4(outMin), vec4(outMax), inputRange);
}
