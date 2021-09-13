/*
	{
	"DESCRIPTION": "Fuzzies Attack",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF Import by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,1.0,1.0]
			},
			{
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT":[1.0,0.0,0.0,1.0]
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 2.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Rotation(or R Speed):",
			"NAME": "uRotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Defaults ",
				"Alternate Color Palette (3 used) "
				],
			"NAME": "uColMode",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 4.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Import from: https://www.shadertoy.com/view/wlfGzB
// Fuzzies Attack by eiffie - some furry creatures running from the northern lights

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

// Imported Shader defined constnts, variables, and functions:
const vec3 p1=vec3(0.0,-0.15,0.0),p2=vec3(0.0,-0.5,0.05); 
const vec3 p3=vec3(-0.08,-0.15,0.0),p6=vec3(0.08,-0.15,0.0); 
const vec3 p9=vec3(-0.05,-0.5,0.05),p12=vec3(0.05,-0.5,0.05),rt=vec3(1.0,0.0,0.0); 
float time;

float noyz(vec3 co)
	{
	return sin(co.x+1.3*sin(co.y+2.4*sin(co.z)))*0.5;
	} 

float Limb2(vec3 p, vec3 p0, vec3 p2, vec3 rt, float d, float r)
	{ 
	vec3 p1=(p2-p0)*0.5;//a simple joint solver, corrected the math after iq's version 
	p1+=p0+normalize(cross(p1,rt))*sqrt(d*d-dot(p1,p1)); 
	vec3 v=p1-p0;v*=clamp(dot(p-p0,v)/dot(v,v),0.0,1.0); 
	vec3 v2=p1-p2;v2*=clamp(dot(p-p2,v2)/dot(v2,v2),0.0,1.0); 
	return min(distance(p-p0,v),distance(p-p2,v2))-r; 
	}

float Segment(vec3 p, vec3 p0, vec3 p1, float r)
	{
	//connect 2 points 
	vec3 v=p1-p0;v*=clamp(dot(p-p0,v)/dot(v,v),0.0,1.0);
	return distance(p-p0,v)-r; 
	} 

float DE(vec3 z)
	{z+=noyz(z*200.0)*0.01; 
	z.z+=time*0.1; 
	int i=int(floor(z.z)+floor(z.x)); 
	z.xz=mod(z.xz,1.0)-vec2(0.5,0.7)+0.25*vec2(sin(float(i))); 
	float tim=(time+float(i)),arm=sin(float(i))*0.25;//0.2 
	vec3 p5=vec3(-0.38+abs(arm),arm-0.1-abs(-sin(tim)*0.05),-0.15-cos(tim)*0.1); 
	vec3 p8=vec3(0.38-abs(arm),arm-0.1-abs(sin(tim+3.1416)*0.05),-0.15+cos(tim)*0.1); 
	vec3 p11=vec3(-0.075,-0.975+max(0.0,cos(tim+3.1416)*0.05),sin(tim)*0.2); 
	vec3 p14=vec3(0.075,-0.975+max(0.0,cos(tim)*0.05),-sin(tim)*0.2); 
	float d=min(z.y+1.0,min(length(z*vec3(1.5,1.0,1.25))-0.08,Segment(z,p1,p2,0.065))); 
	d=min(d,min(Limb2(z,p3,p5,rt,0.21,0.01),Limb2(z,p6,p8,rt,0.21,0.01))); 
	d=min(d,min(Limb2(z,p9,p11,-rt,0.27,0.025),Limb2(z,p12,p14,-rt,0.27,0.025)));//-noyz(z*100.0)*0.02; 
	return min(d-0.015,0.125); 
	} 

void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * RENDERSIZE.x/uZoom;              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

/**** Start of Core Imported Shader Code *****/
//	vec3 ro=vec3(0.1,-0.25,-3.0), rd=normalize(vec3((2.0*gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y,0.4));
	vec3 ro=vec3(0.1,-0.25,-3.0), rd=normalize(vec3((2.0*uv)/RENDERSIZE.y,0.4));	time=TIME*6.0; 
	float t=DE(ro)*noyz(vec3(gl_FragCoord.xy,TIME)),d,a=0.0; 
	for(int i=0;i<48;i++)
		{ 
		t+=d=DE(ro+t*rd); 
		a+=0.02;
		if(t>6.0 || d<0.01)break; 
		}
	t*=1.0/6.0*(noyz(rd*2.0+vec3(0.0,-time,TIME))+noyz(rd*7.3+vec3(0.0,-time*0.3,0.0))); 

/****  End of Core Imported Shader Code  *****/
	vec4 cShad = vec4(mix(vec3(0.3,0.0,0.3+rd.y*0.5),vec3(0.3,0.9,0.4),(t*t+a)*0.5)*pow(dot(rd,vec3(0.3,0.1,1.0)),3.0),1.0);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	cOut = clamp(cOut, vec3(0.0), vec3(1.0));
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
