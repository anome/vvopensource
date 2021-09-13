
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 30.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "amount",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 1.0,
			"MAX": 256.0
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
    vec2 coord = (gl_FragCoord.xy/RENDERSIZE.xy);
	vec3 color = vec3(colorInput);
	float size = amount;
	float alpha = sin(floor(coord.x*size)+TIME * speed)+1.0/2.0;
	gl_FragColor = vec4(color, alpha);
}
