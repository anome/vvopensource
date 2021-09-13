
/*{
	"DESCRIPTION": "ACAB, forked from http://glslsandbox.com/e#65320.0",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "posX",
			"TYPE": "float",
			"DEFAULT": 2.2,
			"MIN": -4.0,
			"MAX": 10.0
		},
		{
			"NAME": "separation",
			"TYPE": "float",
			"DEFAULT": 1.1,
			"MIN": 0.0,
			"MAX": 3.0
		},
				{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 2.7,
			"MIN": 0.001,
			"MAX": 8.0
		},
		{
			"NAME": "letterR",
			"TYPE": "float",
			"DEFAULT": 0.44,
			"MIN": 0.0,
			"MAX": 1.0
		},
				{
			"NAME": "letterG",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "letterB",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "letterFade",
			"TYPE": "float",
			"DEFAULT": 0.17,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "letterWidth",
			"TYPE": "float",
			"DEFAULT": 0.72,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "lightR",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lightG",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lightB",
			"TYPE": "float",
			"DEFAULT": 0.7,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lightFade",
			"TYPE": "float",
			"DEFAULT": 0.16,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "lightWidth",
			"TYPE": "float",
			"DEFAULT": 60.0,
			"MIN": 60.0,
			"MAX": 10000.0
		},
		{
			"NAME": "backlight",
			"TYPE": "float",
			"DEFAULT": 1.4,
			"MIN": 0.1,
			"MAX": 3.00
		},
		{
			"NAME": "wiggle",
			"TYPE": "float",
			"DEFAULT": 0.02,
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


 
mat2 rot(float a)
{
 float sa = sin(a), ca = cos(a);
 return mat2(ca, -sa, sa, ca);
}    
 
float light(vec2 pos, float ang)
{
	pos = pos * rot(ang);
	pos.y -= 0.5;
	float mask = 1. - (pos.x * pos.x * lightWidth - pos.y + lightFade);
	float brightness = clamp(pow(0.1/(pos.y+.5),2.),0.,1.);
	return mask*brightness*15.;
}
 
#define CHS 0.18
float sdBox2(in vec2 p,in vec2 b) {vec2 d=abs(p)-b;return length(max(d,vec2(0))) + min(max(d.x,d.y),0.0);}
float line2(float d,vec2 p,vec4 l){vec2 pa=p-l.xy;vec2 ba=l.zw-l.xy;float h=clamp(dot(pa,ba)/dot(ba,ba),0.0,1.0);return min(d,length(pa-ba*h));}
float TB(vec2 p, float d){p.y=abs(p.y);return line2(d,p,vec4(2,3.25,-2,3.25)*CHS);}
float B(vec2 p,float d){
	p.y+=1.75*CHS;d=min(d,abs(sdBox2(p,vec2(2.0,1.5)*CHS)));
	p+=vec2(0.5,-3.25)*CHS;
	return min(d,abs(sdBox2(p,vec2(1.5,1.75)*CHS)));}

float A(vec2 p,float d){
	d=line2(d,p,vec4(2.0,-0.25,2,-3.25)*CHS);
	d=line2(d,p,vec4(-2,-3.25,-2,0.0)*CHS);
	p.y-=1.5*CHS;
	return min(d, abs(sdBox2(p,vec2(2.0,1.75)*CHS)));} 

float E(vec2 p,float d){
	d=TB(p,d);d=line2(d,p,vec4(-2,3.25,-2,-3.25)*CHS);
	return line2(d,p,vec4(0,-0.25,-2,-0.25)*CHS);
}
float C(vec2 p,float d){
	d=TB(p,d);d=line2(d,p,vec4(-2.,3.25,-2,-3.25)*CHS);
	return line2(d,p,vec4(-2.0,-2.0,-2.0,-2.0)*CHS);
}
float I(vec2 p,float d){
	d=line2(d,p,vec4(0,-3.25,0,3.25)*CHS);p.y=abs(p.y);
	return line2(d,p,vec4(1.5,3.25,-1.5,3.25)*CHS);
} 
float R(vec2 p,float d){
	d=line2(d,p,vec4(0.5,-0.25,2,-3.25)*CHS);
	d=line2(d,p,vec4(-2,-3.25,-2,0.0)*CHS);p.y-=1.5*CHS;
	return min(d, abs(sdBox2(p,vec2(2.0,1.75)*CHS)));
} 
float T(vec2 p,float d){
	d=line2(d,p,vec4(0,-3.25,0,3.25)*CHS);
	return line2(d,p,vec4(2,3.25,-2,3.25)*CHS);} 

float X(vec2 p,float d){
	d = line2(d,p,vec4(-2,3.25,2,-3.25)*CHS);
	return line2(d,p,vec4(-2,-3.25,2,3.25)*CHS);} 
 
float GetText(vec2 uv)
{
	uv.y -= 0.5;
	uv.x += posX;
	uv.x -= 0.5;
	float d = 0.0;
	d = A(uv,1.0);
	uv.x -= separation;
	d = C(uv,d);
	uv.x -= separation;
	d = A(uv,d);
	uv.x -= separation;
	d = B(uv,d);
	uv.x -= separation;
	//d = I(uv,d);
	uv.x -= separation;
	//d = T(uv,d);
	return smoothstep(0.0,letterFade,d-letterWidth*CHS);
}
 
void main()
{
	vec2 p = (gl_FragCoord.xy/RENDERSIZE - vec2(0.5));
	vec3 color = vec3(lightR,lightG,lightB);
	color *= (0.1- p.x * p.x + p.y * p.y)/backlight;
	vec2 p2 = p;
	p2.y += 0.5;
	color += clamp(light(vec2(p.x+0.25,p.y + 0.7),	sin(TIME/2. + 1.)/2.),0.,1.);
	color += clamp(light(vec2(p.x+0.0,p.y + 0.7),	sin(TIME/2. + 2.)/2.),0.,1.);
	color += clamp(light(vec2(p.x+-0.25,p.y + 0.7),	sin(TIME/1. + 3.)/2.),0.,1.);
	vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y);
	uv.y += abs(sin(TIME+uv.x)*wiggle);
	float dd= GetText(uv*zoom);
	color = mix(color+vec3(letterR,letterG,letterB), color,dd);
	gl_FragColor = vec4(color,1);
}
