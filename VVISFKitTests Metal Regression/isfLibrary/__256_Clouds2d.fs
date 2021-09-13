/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "clouds",
    "noise",
    "fbm"
  ],
  "DESCRIPTION" : "based on https://www.shadertoy.com/view/4tdSWr by drift.",
  "INPUTS" : [
	{
		"NAME": 	"scale",
		"TYPE": 	"float",
		"DEFAULT": 	2.0,
		"MIN": 		0.5,
		"MAX": 		3.0
	},
	{
		"NAME": 	"rate",
		"TYPE": 	"float",
		"DEFAULT": 	-0.025,
		"MIN": 		-0.1,
		"MAX": 		0.1
	},
	{
		"NAME": 	"rot",
		"TYPE": 	"float",
		"DEFAULT":	0.0,
		"MIN": 		-1.0,
		"MAX": 		1.0
	},
    {
      	"NAME": 	"cutoff",
      	"TYPE": 	"float",
      	"DEFAULT": 	3.5,
      	"MIN": 		0.5,
      	"MAX": 		24.0
    },
    {
      	"NAME": 	"shadow",
      	"TYPE": 	"float",
      	"DEFAULT": 	0.4,
      	"MIN": 		0.0,
      	"MAX": 		0.6
    },
    {
      	"NAME": 	"deep",
      	"TYPE": 	"float",
      	"DEFAULT": 	 -0.5,
      	"MIN": 		-0.99,
      	"MAX": 		0.99
    },
    {
      	"NAME": 	"glow",
      	"TYPE": 	"float",
      	"DEFAULT": 	0.5,
      	"MIN": 		0.1,
      	"MAX": 		0.9
    },
    {
      	"NAME": 	"density",
      	"TYPE": 	"float",
      	"DEFAULT": 	0.5,
      	"MIN":		0.1,
      	"MAX": 		6.0
    },
    {
      	"NAME": 	"depth",
      	"TYPE": 	"float",
      	"DEFAULT": 	5.0,
      	"MIN":		2.0,
      	"MAX": 		11.0
    },
   	{
     	"NAME" :	"seed",
      	"TYPE" :	"float",
     	"DEFAULT" :	293254.39,
     	"MIN" :		43349.4437,
     	"MAX" :		297121.5073
    },
    {
      	"NAME": 	"warp",
      	"TYPE": 	"float",
      	"DEFAULT": 	0.1,
      	"MIN":		0.05,
      	"MAX": 		2.0
    },
	{
      	"NAME" : 	"color1",
      	"TYPE" : 	"color",
      	"DEFAULT" :	[ 0.7, 0.0, 0.3, 1.0 ]
    },
    {
      	"NAME" : 	"color2",
      	"TYPE" : 	"color",
      	"DEFAULT" :	[ 0.9, 0.2, 0.0, 1.0 ]
    }
  ]
}
*/


////////////////////////////////////////////////////////////
// Clouds2d  by mojovideotech
//
// mod of :
//
// 2D Clouds by drift.
// shadertoy.com/view/4tdSWr
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////
 



#define 	twpi  	6.283185307179586  	// two pi, 2*pi

const float K1 = 0.366025404; // (sqrt(3)-1)/2;
const float K2 = 0.211324865; // (3-sqrt(3))/6;
const mat2	m = mat2( 1.6,  1.2, -1.2,  1.6 );


mat2 rmat(float t) { float a = cos(t), b = sin(t); return mat2(a,-b,b,a); }

vec2 hash( vec2 p ) {
	p = vec2(dot(p,vec2(127.1,311.7)), dot(p,vec2(269.5,183.3)));
	return -1.0 + 2.0*fract(sin(p)*seed);
}

float noise( in vec2 p ) {
	vec2 i = floor(p + (p.x+p.y)*K1);	
    vec2 a = p - i + (i.x+i.y)*K2;
    vec2 o = (a.x>a.y) ? vec2(1.0,0.0) : vec2(0.0,1.0); 
    vec2 b = a - o + K2;
	vec2 c = a - 1.0 + 2.0*K2;
    vec3 h = max(0.5-vec3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );
	vec3 n = h*h*h*h*vec3( dot(a,hash(i+0.0)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));
    return dot(n, vec3(60.0));	
}

float fbm(vec2 n) {
	float total = 0.0, amplitude = warp, b = floor(depth);
	for (int i = 0; i < 12; i++) {
		total += noise(n) * amplitude;
		n = m * n;
		amplitude *= 0.4;
        b -= 1.0;
        if (b <= 0.0) { break; }
		
	}
	return total;
}

// -----------------------------------------------

void main() 
{
	float cloudscale = 3.25 - scale;
    vec2 p = 2.0 * (gl_FragCoord.xy / RENDERSIZE.xy) - 1.0;  
    p.x *= RENDERSIZE.x/RENDERSIZE.y;
    vec2 st = p * cloudscale;
    st *= rmat(rot*twpi);
    float T1 = TIME * rate;
    float q = fbm(st * 0.5);
	float r = 0.0, f = 0.0, c = 0.0, d = 0.0;
	vec2 uv = st;
    uv -= q - T1;
    float weight = 0.8;
    for (int i=0; i<8; i++){
		r += abs(weight*noise( uv ));
        uv = m*uv + T1;
		weight *= 0.7;
    }
    uv = st;
    uv -= q - T1;
    weight = 0.7;
    for (int i=0; i<8; i++){
		f += weight*noise( uv );
        uv = m*uv + T1;
		weight *= 0.6;
    }
    f *= r + f;
    float T2 = TIME * rate * 2.0;
    uv = st*2.0;
    uv -= q - T2;
    weight = 0.4;
    for (int i=0; i<7; i++){
		c += weight*noise( uv );
        uv = m*uv + T2;
		weight *= 0.6;
    }
    float T3 = TIME * rate * 3.0;
    uv = st*3.0;
    uv -= q - T3;
    weight = 0.4;
    for (int i=0; i<7; i++){
		d += abs(weight*noise( uv ));
        uv = m*uv + T3;
		weight *= 0.6;
    }
    c += d;
    f = density + cutoff * f * r;
    float s = 0.6 - shadow;
    vec3 skycolor = mix(color2.rgb, color1.rgb, p.y);
    vec3 cloudcolor = vec3(1.1, 1.1, 0.9) * clamp((s + glow*c), 0.0, 1.0);
    vec3 col = mix(skycolor, clamp(-deep * skycolor + cloudcolor, 0.0, 1.0), clamp(f + c, 0.0, 1.0));
    
	gl_FragColor = vec4( col, 1.0 );
}

