/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"fractal",
		"psychedelic"
	],
	"INPUTS": [
		{
			"NAME": 	"mX",
			"TYPE": 	"float",
			"DEFAULT": 	0.25,
			"MIN": 		0.0,
			"MAX": 		2.0
		},
		{
			"NAME": 	"mY",
			"TYPE": 	"float",
			"DEFAULT": 	-0.75,
			"MIN": 		-2.0,
			"MAX": 		1.0
		},
		{
			"NAME": 	"rate",
			"TYPE": 	"float",
			"DEFAULT": 	1.5,
			"MIN": 		-5.0,
			"MAX": 		5.0
		},
		{
			"NAME": 	"sides",
			"TYPE": 	"float",
			"DEFAULT": 	4.0,
			"MIN": 		2.0,
			"MAX": 		40.0
		},
		{
			"NAME" : 	"scale",
			"TYPE" : 	"float",
			"DEFAULT" : 2.0,
			"MIN" : 	0.5,
			"MAX" : 	5.0
		},
		{
			"NAME" : 	"multiply",
			"TYPE" : 	"float",
			"DEFAULT" :	4.0,
			"MIN" : 	0.0,
			"MAX" : 	6.0
		},
		{
			"NAME" : 	"loops",
			"TYPE" : 	"float",
			"DEFAULT" :	24.0,
			"MIN" : 	6.0,
			"MAX" : 	72.0
		},
		{
   			"NAME" : 	"iR",
     		"TYPE" : 	"bool",
     		"DEFAULT" :  false
   		},
   		{
   			"NAME" : 	"iG",
    	 	"TYPE" : 	"bool",
    	 	"DEFAULT" :  true
   		},
   		{
   			"NAME" : 	"iB",
     		"TYPE" : 	"bool",
     		"DEFAULT" :  false
   		},
   		{
   		"NAME" : 		"flip",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	 false
   	},	
	{
   		"NAME" : 		"flop",
     	"TYPE" : 		"bool",
     	"DEFAULT" : 	false
	}
	]
}*/

////////////////////////////////////////////////////////////
// Equirec_MandalaScope  by mojovideotech
//
// mod of :
// interactiveshaderformat.com/\2150
// based on :
// glslsandbox.com/\e#42620.1
// shadertoy.com/\llXcRl 
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	rcpi  	0.318309886183791	// reciprocal of pi  , 1/pi 

float T = TIME * rate;

vec2 Kscope(vec2 uv, float k) {
  	float angle = abs (mod (atan (uv.y, uv.x), 2.0 * k) - k) + 0.1*T;
  	return length(uv) * vec2(cos(angle), sin(angle));
}

void main ()
{
  	vec2 sph =  (gl_FragCoord.xy / RENDERSIZE.xy) * vec2(twpi, pi);
    vec3 rd = vec3(sin(sph.y) * sin(sph.x), cos(sph.y), sin(sph.y) * cos(sph.x)); 
    vec3 uv;
    if (flip) {
  			uv = rd.zyx;
  			if (flop) uv = rd/uv.xzy;
  		} 
  		else if (flop) uv = rd.xzy;
  		else uv = rd.xyz;
	float k = pi / floor(sides);
  	vec2 s = Kscope(uv.xy,k);
  	vec2 t = Kscope(s,k);
  	float v = dot(t,s);
	vec2 u = (5.5-scale)*mix(s,t,cos(v));
	if (multiply>0.001) { u.yx += mod(mod(u.yx,floor(multiply)),-u.xy); }
  	vec3 p = vec3 (u, mX*v);
  	for (int i = 0; i < 73; i++) {
  		if (float(i)>floor(loops)) { break; }
    	p.xzy = vec3(1.3,0.999,0.678)*(abs((abs(p)/dot(p,p)-vec3(1.0,1.02,mY*rcpi))));
  	}
  	if (iR) { p.x = 1.0-p.x; }
  	if (iG) { p.y = 1.0-p.y; }
  	if (iB) { p.z = 1.0-p.z; }
  	
  	gl_FragColor = vec4(p,1.0);
  	
}
