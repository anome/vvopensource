/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES" : [
    	"generator",
    	"celtic knot",
    	"equirectangular"
  ],
  	"DESCRIPTION" : "based on http://glslsandbox.com/e#37654.0",
  	"INPUTS" : [
	{
		"NAME": 	"scale",
		"TYPE": 	"float",
		"DEFAULT": 	1.5,
		"MIN": 		0.5,
		"MAX": 		2.0
	},
	{
		"NAME": 	"rate",
		"TYPE": 	"float",
		"DEFAULT": 	0.125,
		"MIN": 		-0.5,
		"MAX": 		0.5
	},
	{
     	"NAME" :	"seed1",
     	"TYPE" : 	"float",
     	"DEFAULT" :	13,
     	"MIN" : 	8,
     	"MAX" :		233
	},
    {
      	"NAME" :	"seed2",
      	"TYPE" :	"float",
      	"DEFAULT" :	91,
      	"MIN" : 	55,
      	"MAX" :		98	
	},
    {
     	"NAME" :	"seed3",
      	"TYPE" :	"float",
     	"DEFAULT" :	514229,
     	"MIN" :		75025,
    	"MAX" :		3524578
    }
  ]
}
*/

////////////////////////////////////////////////////////////
// CelticSpheroid  by mojovideotech
//
// based on :  
// glslsandbox.com/e#37654.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define pi   3.141592653589793  // pi


float hash(float n) { return fract(sin(n)*seed3); }

float hash(vec2 n) { return hash(dot(n, vec2(seed1, seed2))); }

float de(vec2 p) {
	p.x += TIME * rate;
	float m = 1.0;
	vec2 q = p * scale;
	for(int i = 0; i < 3; i++) {
		p = q;
		vec2 t = floor(p);
		p = fract(p) - 0.5;
		p.x *= 2.0*floor(1.2*fract(hash(t))) - 1.0;
		float d = abs(1.0 - 2.0*abs(p.x + p.y))/(2.0*sqrt(5.0));
		m = min(m, smoothstep(0.0, 0.05, d));
		q *= 2.0;
	}
	return m;
}

vec3 bump(vec2 p, float z) {
	const vec2 r = vec2(0.01, 0.0); 
	const vec2 l = vec2(0.0, 0.01);
	vec3 g = vec3(de(p + r) - de(p - r),
		         de(p + l) - de(p - l),
		         z);
	return normalize(g);
}

vec3 render(vec2 p) {
	vec3 rd = normalize(vec3(p, pi)), sn = bump(p, -0.6);
	return vec3(pow(clamp(dot(-rd, sn), 0.0, 1.0), 16.0));
}

void main( void ) {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float th =  uv.t * pi, ph = uv.s * 2.0 * pi;
	vec3 p = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
	float q = de(p.yz);
	vec3 i = bump(p.yx,q),  j = render(p.yz), color = vec3(0.3, -0.2, 0.9);
	gl_FragColor = vec4(color-mix(i,j,q), 1.0);
}