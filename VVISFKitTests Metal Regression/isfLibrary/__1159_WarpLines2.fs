
/*{
	"DESCRIPTION": "warpLines",
	"CREDIT": "uaralab from lewis lepton tutorial",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 40.0
		},
		{
			"NAME": "wiggle1",
			"TYPE": "float",
			"DEFAULT": 50.0,
			"MIN": 0.0,
			"MAX": 200.0
		},
		{
			"NAME": "wiggle2",
			"TYPE": "float",
			"DEFAULT": 50.0,
			"MIN": 0.0,
			"MAX": 200.0
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
    vec2 coord = zoom*(gl_FragCoord.xy/RENDERSIZE);
    float color = 0.0;
	color+=sin(coord.x*wiggle1+cos(TIME+coord.y*10.0+sin(coord.x*wiggle2+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color,color,color),1.0);
	color+=cos(coord.x*50.0+sin(TIME+coord.y*10.0+sin(coord.x*50.0+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color,color,color),1.0);
}
