
/*{
	"DESCRIPTION": "forked from http://glslsandbox.com/e#65429.1",
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

#extension GL_OES_standard_derivatives : enable

#define ITER 20
#define EPS 0.00015
#define NEAR 1.
#define FAR 40.
float n(float m){
	return fract(sin(m*14.17)*414.17);
}
float n(vec3 m,vec3 g){
	vec3 v=abs(m)-g;
	return min(max(v.r,max(v.g,v.b)),0.)+length(max(v,0.));
}
mat2 m(float m){
	float v=sin(m),r=cos(m);
	return mat2(r,v,-v,r);
}
vec2 m(vec2 v,float g){
	float r=3.14159/g-atan(v.r,v.g),R=6.28318/g;
	r=floor(r/R)*R;return v*m(r);}float s(vec3 v){v.g-=1.;
	v.gb*=m(.31);v.rb*=m(TIME*.2);
	v.r+=n(floor(v.b*.125))*6.;v.rb=mod(v.rb,6.)-3.;v.rb=m(v.rb,1.);
	v.rb*=m(1.54);
	vec3 r=vec3(.01,.9,.01);
	float g=min(n(v,r),v.g+1.5);
	for(int f=0;f<5;f++){
		vec3 s=v;
	s.r=abs(s.r);s.g-=r.g;s.rg*=m(-.94);g=min(g,n(v,r));v=s;r*=.95;
	}
	return g;
}
float s(vec3 v,vec3 m){
	float r=NEAR,g;
	for(int f=0;f<ITER;f++){
		g=s(v+m*r);

 		if(abs(g)<EPS||r>FAR){
			break;}
 		r+=smoothstep(g,.01,1.)*g*.3+g*.815;
	} 
	return min(r,FAR);
}
void main(){
	vec2 v=(gl_FragCoord.rg-.5*RENDERSIZE.rg)/RENDERSIZE.g;
	gl_FragColor=vec4(s(vec3(0,6,-20),vec3(v,.9))/FAR);
}
