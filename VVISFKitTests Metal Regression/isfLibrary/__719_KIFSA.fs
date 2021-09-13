
/*{
	"DESCRIPTION": "by Uaralab forked from The Art of Code",
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
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
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
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
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

vec2 N(float angle){
 return vec2(sin(angle), cos(angle));
}

void main()	{
	vec2 uv = (gl_FragCoord.xy-0.5*RENDERSIZE.xy)/RENDERSIZE.y;
	vec2 mouse = pointInput.xy;
    uv*=1.25;
	vec3 col = vec3(0.0);
	
    uv.x = abs(uv.x);
    uv.y += tan((5.0/6.0)*3.1415)*0.5;
	vec2 n = N((5.0/6.0)*3.1415);
	float d = dot(uv-vec2(0.5, 0.0),n);
	
	uv -= n*max(0.0,d)*2.0;
	
	//col+=smoothstep(0.001,0.0,abs(d));
	
	n = N((2.0/3.0)*3.1415);
	float scale = 1.0;
	uv.x += 0.5;
	for(int i = 0; i<4; i++){
	    
	   	uv*=3.0;
	   	scale *=3.0;
    	uv.x -= 1.5;
    	
    	uv.x = abs(uv.x);
    	uv.x -=0.5;
    	uv -= n*min(0.0,dot(uv,n))*2.0;

	}
	
	d = length(uv-vec2(clamp(uv.x, -1.0, 1.0), 0.0));
	col+=smoothstep(1.0/RENDERSIZE.y,0.0,d/scale);
	col.rb +=uv/scale;

	gl_FragColor = vec4(col,1.0);
}
