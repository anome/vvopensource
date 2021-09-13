
/*{
	"DESCRIPTION": "Circle Color Pulse, forked from lewis lepton shader tutorials",
	"CREDIT": "uara Lab",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "drawRChannel",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "drawGChannel",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
				{
			"NAME": "drawBChannel",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "timeMultiplierR",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "timeMultiplierG",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "timeMultiplierB",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "rDisplacer",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 3.0,
			"MAX": 4.9
		},
		{
			"NAME": "gDisplacer",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 3.0,
			"MAX": 4.9
		},
		{
			"NAME": "bDisplacer",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 3.0,
			"MAX": 4.9
		},
		{
			"NAME": "rFade",
			"TYPE": "float",
			"DEFAULT": 0.0165,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "gFade",
			"TYPE": "float",
			"DEFAULT": 0.0165,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bFade",
			"TYPE": "float",
			"DEFAULT": 0.0165,
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
    vec2 coord = gl_FragCoord.xy/RENDERSIZE;
	vec3 color = vec3(0.0);
	vec2 translate = vec2(-0.5);
	coord+=translate;
	
	if(drawRChannel==true){
	    color.r+= abs(0.1+ length(coord)-0.6* abs(sin(TIME*timeMultiplierR/rDisplacer)));
	} else {
	    color.r = 0.0;
	}
	
	if(drawGChannel==true){
	    color.g+= abs(0.1+ length(coord)-0.6* abs(sin(TIME*timeMultiplierG/gDisplacer)));
	} else {
	    color.g = 0.0;
	}
	
	if(drawBChannel==true){
	    color.b+= abs(0.1+ length(coord)-0.6* abs(sin(TIME*timeMultiplierB/bDisplacer)));
	} else {
	    color.b = 0.0;
	}
	
	gl_FragColor =  vec4(rFade/color.r, gFade/color.g, bFade/color.b,1.0);
}
