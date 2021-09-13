
/*{
	"DESCRIPTION": "warpLines ( forked from lewis lepton tutorial)",
	"CREDIT": "uara lab",
	"ISFVSN": "2",
	"CATEGORIES": [
		"GENERATOR"
	],
	"INPUTS": [
		{
			"NAME": "floatInput1",
			"TYPE": "float",
			"DEFAULT": 50.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "floatInput2",
			"TYPE": "float",
			"DEFAULT": 20.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "floatInput3",
			"TYPE": "float",
			"DEFAULT": 30.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "floatInput4",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 0.92,
			"MIN": 0.1,
			"MAX": 20.0
		},
		{
			"NAME": "floatInput6",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
	
	
}*/

void main()	{
    vec2 coord = zoom*(gl_FragCoord.xy/RENDERSIZE);
    float color = 0.0;
	color+=sin(coord.x*floatInput1+cos(TIME+coord.y*10.0+sin(coord.x*50.0+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color,color,color),1.0);
	color+=cos(coord.x*floatInput2+sin(TIME+coord.y*10.0+cos(coord.x*50.0+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color,color,color),1.0);
	color+=sin(coord.x*floatInput3+cos(TIME+coord.y*10.0+sin(coord.x*50.0+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color,color,color),1.0);
	color+=cos(coord.x*floatInput4+sin(TIME+coord.y*10.0+cos(coord.x*50.0+TIME*2.0)))*2.0;
	gl_FragColor = vec4(vec3(color+coord.y,color+coord.x,color),1.0);
}
