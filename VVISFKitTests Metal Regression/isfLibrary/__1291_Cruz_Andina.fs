
/*{
	"DESCRIPTION": "Andean Cross by Ivan Verdugo (Uará Lab)",
	"CREDIT": "Ivan Verdugo",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
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

#define pi 3.14159235659

void main()	{
    vec2 uv = gl_FragCoord.xy*0.5/RENDERSIZE.xy;
    uv.x*=RENDERSIZE.x/RENDERSIZE.y;
    
    float forma = sin(uv.x*10.0*pi);
    forma =forma + sin(uv.y*10.0*pi+TIME);
    forma =forma + fract(uv.x*10.0*pi-TIME);
    forma =forma + fract(uv.y*10.0*pi-TIME);
    
    float r = forma;
    float g = forma;
    float b = forma;
    
    gl_FragColor = vec4(r,g,b,1.0);
}
