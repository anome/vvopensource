/*
{
  "CATEGORIES" : [
    "INKA"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/Msc3Wf by dila.  Just a test of the multi-pass feature.",
 
  "INPUTS" : [
	  {
	  	"NAME": "inputImage",
	  	"TYPE": "image"
	  },
	{
	"NAME": "color",
	"TYPE": "color", 
	"DEFAULT": [
	1.0,1.0,1.0,1.0]
	},
	{
	"NAME": "width",
	"TYPE": "float",
	"DEFAULT": 0.59
	},
	{
	"NAME": "foggy",
	"TYPE": "float",
	"DEFAULT": 0.13
	},
	{
	"NAME": "exposure",
	"TYPE": "float",
	"DEFAULT": 0.53
	},
	{
	"NAME": "noise",
	"TYPE": "float", 
	"DEFAULT": 0.29
	},
	{
	"NAME": "xx",
	"TYPE": "float"
	},
	{
	"NAME": "yy",
	"TYPE": "float"
	},
	{
	"NAME": "zz",
	"TYPE": "float"
	},
	{
	"NAME": "scale",
	"TYPE": "float",
	"DEFAULT": 0.65
	}
  ]
}
*/

// https://www.shadertoy.com/view/4djSRW
#define MOD3 vec3(443.8975,397.2973, 491.1871)
#define STEPS 10
#define PI 3.141592654

//  1 out, 2 in...
float hash12(vec2 p)
{
    vec3 p3 = fract(vec3(p.xyx) * MOD3);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}


float addNoise() {
	vec2 npos = (gl_FragCoord.xy + TIME);
	npos = vec2(atan(npos.x, npos.y), length(npos));
	return hash12(npos) * noise;
}

float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}

float map(vec3 p, float s)
{
	p *= s;
	
    vec2 a = sin(vec2(1.57, 0) + PI * 2. * xx); // cos(a) = sin(a + PI/2.)
    vec2 b = sin(vec2(1.57, 0) + PI * 2. * yy); // cos(a) = sin(a + PI/2.)
    vec2 c = sin(vec2(1.57, 0) + PI * zz); // cos(a) = sin(a + PI/2.)
    
    p.xz *= mat2(a, -a.y, a.x);
    p.xy *= mat2(b, -b.y, b.x);    
    p.yz *= mat2(c, -c.y, c.x);
    
    // Essentially wrapping a unit cube around a lattice of tube width 0.85.
    p = abs(p); 
    p = max(p, p.yzx );
    return max(max(p.x, p.y) - 1., (.5 + width * .75) - min(p.x, min(p.y, p.z))) * (1.3 - scale * 0.5);
}

float trace(vec3 o, vec3 r)
{
 	float t = 0.0;
    for (int i = 0; i < STEPS; ++i) {
        vec3 p = o + r * t;
        float d = map(p, 0.5 + scale) * (1.3 - foggy * .5);
        t += d + (.1 - d) * addNoise();
    }
    return t;
}


void main() {

		vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	    uv = uv * 2.0 - 1.0;
	    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
	    
	    vec3 o = vec3(0.0, 0.0, -2.5);
	    vec3 r = vec3(uv, 0.8);
	    
	    float t = trace(o, r);
	    
	    vec3 fog = vec3(1.0) / (1.0 + t * t * 0.1) * 0.1;
		fog = fog * pow(2.0, exposure * 5.);
	
		gl_FragColor = vec4(fog, 1.0) * color;
}