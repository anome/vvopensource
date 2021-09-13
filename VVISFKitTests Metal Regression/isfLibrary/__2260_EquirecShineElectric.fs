/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "equirectangular",
    "electric"
  ],
  "INPUTS" : [
	{
		"NAME": 	"rate",
		"TYPE": 	"float",
		"DEFAULT": 	0.5,
		"MIN": 		-1.0,
		"MAX": 		1.0
	},
	{
		"NAME": 	"shine",
		"TYPE": 	"float",
		"DEFAULT":	2.6,
		"MIN": 		0.0,
		"MAX": 		5.0
	},
	{
		"NAME": 	"density",
		"TYPE": 	"float",
		"DEFAULT": 	2.0,
		"MIN": 		1.0,
		"MAX": 		5.0
	},
	{
		"NAME": 	"nudge",
		"TYPE": 	"float",
		"DEFAULT": 	0.1,
		"MIN": 		0.01,
		"MAX": 		2.0
	},
	{
    	"NAME" :	"seed1",
     	"TYPE" : 	"float",
     	"DEFAULT" :	55,
     	"MIN" : 	8,
     	"MAX" :		233
	},
   	{
     	"NAME" :	"seed2",
      	"TYPE" :	"float",
      	"DEFAULT" :	89,
      	"MIN" : 	55,
      	"MAX" :		98	
	}
  ]
}
*/


////////////////////////////////////////////////////////////
// EquirecShineElectric   by mojovideotech
//
// based on 
// glslsandbox/e#38756.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


float T = TIME * rate;

vec2 rotate(vec2 p, float a) {
	return vec2(p.x*cos(a)-p.y*sin(a), p.x*sin(a)+p.y*cos(a)); 
}

float hash(float n) { return fract(sin(n) * atan(seed1, seed2)); }

vec2 rash(in vec2 p) {
	return fract(vec2(sin(p.x * seed1 + p.y * seed2 + T), cos(p.x * seed2 + p.y * seed1 + T))/nudge);
}

float noise1(float p) {
	float fl = floor(p), fc = fract(p);
	return mix(hash(fl), hash(fl + 1.0), fc);
}

float voronoi(in vec2 x) {
	vec2 p = floor(x), f = fract(x), res = vec2(9.0);
	for(int j = -1; j <= 1; j ++) {
		for(int i = -1; i <= 1; i ++) {
			vec2 b = vec2(i, j);
			vec2 r = vec2(b) - f + rash(p + b);
			float d = max(abs(r.x), abs(r.y));
			if(d < res.x) {
				res.y = res.x;
				res.x = d; 
			}
			else if(d < res.y) {
				res.y = d;
			}
		}
	}
	return res.y - res.x;
}

void main(void) {
	float yy = radians(180.*(gl_FragCoord.y/RENDERSIZE.y-.5));
	float xz = radians(270.*(gl_FragCoord.x/RENDERSIZE.x-.5));
	vec3 rd = vec3(sin(xz)*cos(yy), sin(yy), cos(xz)*cos(yy));
	vec2 uv = rd.xy;
	vec2 suv = uv;
	float v = 0.0, a = 0.6,  f = 1.0;;
	v = 1.0 - length(uv) * 1.3;
	for(int i = 0; i < 3; i ++)	{	
		float v1 = voronoi(uv * f + 50.0);
		float v2, va, vb;
		if(i > 0) {
			va = 1.0 - smoothstep(0.0, 0.05, v1);
			vb = 1.0 - smoothstep(0.0, 0.01, v2);
			v += a * pow(va * (0.5 + vb), v1);
		}
		v1 = 1.0 - smoothstep(0.0, 0.03, v1);
		v2 = a * (noise1(v1 * 5.5 + 0.1));
		if(i == 0) { v += v2; }
		f *= density;
		a *= 1.7;
	}
	v *= exp(-0.9 * length(suv)) * 1.2;
	vec3 cexp = vec3(2.0, 2.0, 0.4);
	cexp *= 6.0 - shine;
	vec3 col = vec3(pow(v, cexp.x), pow(v, cexp.y), pow(v, cexp.z)); // * 2.0;
	
	gl_FragColor = vec4(sqrt(col), 1.0);
}