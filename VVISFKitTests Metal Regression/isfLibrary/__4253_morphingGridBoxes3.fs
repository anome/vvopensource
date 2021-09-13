
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
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
    vec2 coord = gl_FragCoord.xy*1.0-RENDERSIZE;
	vec3 color = vec3(0.0);
	color+= abs(cos(coord.x/20.0)+sin(coord.y/20.0)-cos(TIME));

	gl_FragColor = vec4(vec3(color.r/3.0+cos(TIME),0.0,color.g/2.0*sin(TIME)),1.0);
}
