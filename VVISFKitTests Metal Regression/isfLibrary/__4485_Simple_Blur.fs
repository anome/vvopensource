
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "strength",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

void main()	{
	vec4		color;
	
	color = IMG_PIXEL(inputImage, gl_FragCoord.xy) * 0.2270270270;
	
	vec2 off1 = vec2(1.3846153846) * strength;
    vec2 off2 = vec2(3.2307692308) * strength;
	
	color += IMG_PIXEL(inputImage, gl_FragCoord.xy + off1) * 0.3162162162;
	color += IMG_PIXEL(inputImage, gl_FragCoord.xy - off1) * 0.3162162162;
	color += IMG_PIXEL(inputImage, gl_FragCoord.xy + off2) * 0.0702702703;
	color += IMG_PIXEL(inputImage, gl_FragCoord.xy - off2) * 0.0702702703;
	
	gl_FragColor = color;
}
