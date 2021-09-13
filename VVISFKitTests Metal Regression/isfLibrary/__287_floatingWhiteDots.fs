
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

#define S(a,b,t) smoothstep(a,b,t)

float DistLine(vec2 p, vec2 a, vec2 b){
    vec2 pa = p-a;
    vec2 ba = b-a;
    float t = clamp(dot(pa,ba)/dot(ba,ba),0.0,1.0);
    return length(pa-ba*t);
    
}

float N21(vec2 p){
    p = fract(p*vec2(233.34,851.73));
    p += dot(p,p+23.45);
    return fract(p.x*p.y);
}

vec2 N22(vec2 p){
    float n = N21(p);
    return vec2(n, N21(p+n));
}

vec2 GetPos(vec2 id){
    
    vec2 n = N22(id)*TIME;

    return sin(n)*0.4;
    
}

void main()	{
    vec2 uv = (10.0*gl_FragCoord.xy-0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    
    //float d = DistLine(uv, vec2(0.0),vec2(1.0));
    float m = 0.0;
    uv+=5.0;
    
    vec2 gv = fract(uv)-0.5;
    vec2 id = floor(uv);
    
    vec2 p = GetPos(id);
    
    float d = length(gv-p);
    m = S(0.1,0.05,d);
    vec3 col = vec3(m);
    

    
    //col.rg = gv;
    //if(gv.x>0.48||gv.y>0.48){
     //   col = vec3(1,0,0);
    //}
    
    gl_FragColor = vec4(col,1.0);
    

}
