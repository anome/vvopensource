/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES": [
    "generator"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
	{
		"NAME" : 	"center",
		"TYPE" : 	"point2D",
		"DEFAULT" :	[ -0.333, 0.5 ],
		"MAX" : 	[ 1.0, 1.0 ],
     	"MIN" : 	[ -1.0, -1.0 ]
	},
	{
		"NAME" : 	"scale",
		"TYPE" : 	"float",
		"DEFAULT" : 75.0,
		"MIN" : 	2.0,
		"MAX" : 	102.0
	},
	{
		"NAME" : 	"rate",
		"TYPE" : 	"float",
		"DEFAULT" : -0.667,
		"MIN" : 	-1.0,
		"MAX" : 	1.0
	},
	{
		"NAME" : 	"mult",
		"TYPE" : 	"float",
		"DEFAULT" : 5.95,
		"MIN" : 	4.0,
		"MAX" : 	10.0
	},
	{
		"NAME" : 	"seed1",
		"TYPE" : 	"float",
		"DEFAULT" : 55.0,
		"MIN" : 	33.0,
		"MAX" : 	111.0
	},
	{
		"NAME" : 	"seed2",
		"TYPE" : 	"float",
		"DEFAULT" : 202.0,
		"MIN" : 	123.0,
		"MAX" : 	321.0
	}
  ],
    "ISFVSN": "2.0"
}*/


////////////////////////////////////////////////////////////////////
// ChromathematicalPoem#3  by mojovideotech
//
// based on :
// glslsandbox.com/\e#28114.4
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////


#define 	cucupi	1.1356352767379		// cube root of cube root of pi
#define 	lgpi  	0.497149872694134 	// log(pi)  


#ifdef GL_ES
precision highp float;
#endif


float TN = TIME+30.0;

float hash( float n ) { return fract(fract(sin(n)*log2(TN))-(mod(n,-TN))*0.00336563); }

float noise( in vec3 x ) {
    vec3 p = floor(x), f = fract(x);
    float s1 = floor(seed1), s2 = floor(seed2);
    float s3 = s1+s2;
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*s1 + s2*p.z;
    return mix(mix(mix( hash(n+0.0), hash(n+1.0),f.x),
                   mix( hash(n+s1), hash(n+s1+1.0),f.x),f.y),
               mix(mix( hash(n+s2), hash(n+s2+1.0),f.x),
                   mix( hash(n+s3), hash(n+s3+1.0),f.x),f.y),f.z);
}

const mat3 m = mat3( 0.00,  0.80,  0.60,
                    -0.80,  0.36, -0.48,
                    -0.60, -0.48,  0.64 );

float flux(vec3 mo) {
	float v = 0.0, m = 1.0;
	for (int i=0; i < 3; i ++) {
		m *= lgpi;
		v += noise(mo) * m;
		mo *= cucupi;
	}
	return floor(v*mult);
}


void main( void ) 
{
	float T = TIME * rate;
	vec2 uv = ((gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / min(RENDERSIZE.y, RENDERSIZE.x)-center)*(100.0-scale);
	float f1 = flux(vec3(uv, T));
	float f2 = flux(vec3(uv, T + 1.0));
	float f3 = flux(vec3(uv, T + 2.0));
	vec3 col  = (f1 < 3.0) ? vec3(0.294, 0.127, 0.355) : vec3(0.899, 0.280, 0.037);
	col += (f2 < 3.0) ? col : vec3(0.741, 0.173, 0.337);
	col *= (f3 < 3.0) ? col : vec3(0.651, 0.020, 0.237);
 	col = pow(col, vec3(3.0)*0.33333); 
	
	gl_FragColor = vec4(0.777-col,1.0);

}