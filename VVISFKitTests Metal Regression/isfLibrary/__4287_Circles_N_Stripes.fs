/*
{
	"CREDIT": "by mojovideotech",
    "CATEGORIES": [
        "generator",
        "circles"
    ],
    "DESCRIPTION": "",
    "INPUTS": [
	{
		"NAME" : 	"center",
		"TYPE" : 	"point2D",
		"DEFAULT" :	[ -0.5, 0.1 ],
		"MAX" : 	[ 1.5, 1.5 ],
     	"MIN" : 	[ -1.5, -1.5 ]
	},	
	{
		"NAME" : 	"seed",
		"TYPE" : 	"point2D",
		"DEFAULT" :	[ 0.5, 0.1 ],
		"MAX" : 	[ 1.0, 1.0 ],
     	"MIN" : 	[ -1.0, -1.0 ]
	},
	{
		"NAME" : 	"color",
		"TYPE" : 	"point2D",
		"DEFAULT" :	[ -0.25, -0.95 ],
		"MAX" : 	[ 1.0, 1.0 ],
     	"MIN" : 	[ -1.0, -1.0 ]
	},
	{
		"NAME" : 	"bright",
		"TYPE" : 	"float",
		"DEFAULT" : 0.5,
		"MIN" : 	-1.0,
		"MAX" :     1.0
	},
	{
		"NAME" : 	"rate",
		"TYPE" : 	"float",
		"DEFAULT" : 0.25,
		"MIN" : 	0.01,
		"MAX" :     1.0
	},
	{
		"NAME" : 	"zoom",
		"TYPE" : 	"float",
		"DEFAULT" : 13.0,
		"MIN" : 	1.0,
		"MAX" : 	20.0
	},
	{
		"NAME" : 	"flash",
		"TYPE" : 	"float",
		"DEFAULT" : 0.6,
		"MIN" : 	0.1,
		"MAX" : 	0.9
	}
  ],
  "ISFVSN" : "2"
}
*/


////////////////////////////////////////////////////////////
// Circles-N-Stripes  by mojovideotech
//
// based on :
// glslsandbox.com/e#60458.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define 	twpi  	6.283185307179586  	// two pi, 2*pi


float T = mod(TIME * 0.0005 + 500.0, 1500.0);

vec2 mash(vec2 p) { return fract(sin(mat2(T,cos(T*p.y), sin(T*p.x),-T)*p)*46839.32); }

float N21(vec2 co) { return fract(sin(dot(co,vec2(12.9898,78.233)))*43758.5453); }

vec2 N22(vec2 co) { return vec2(N21(co), N21(co+seed)); }

vec2 V(vec2 p) {
	vec2 g = floor(p), dist = fract(p), r;
	float minDist = 100.0;	
	for(int y = -1; y <= 1; ++y ) {
		for(int x = -1; x <= 1; ++x ) {
			vec2 l = vec2(x, y), o = N22(l + g);	
			float d = distance(l + vec2((sin(T * o) * 0.5 + 0.5)), dist);
			if(d < minDist) { minDist = d; r = o; }
			r = o;
		}
	}
	return r;
}

vec2 W(vec2 p) { return mash(floor(p)); }

void main() 
{
	vec2 uv = (2.0*gl_FragCoord.xy-RENDERSIZE)/max(RENDERSIZE.x,RENDERSIZE.y)+-center;
	uv.x = dot(uv,uv)-TIME*rate;
	vec2 st = vec2(uv*(21.0-zoom));
	vec2 b = V(st);
	vec2 c = mix(b,W(st)+(1.0-b),pow(0.1+flash, twpi)); 
	vec3 col = cos(c.x * 16.0 + vec3(3.0,2.0,1.0)+vec3(color.x,color.y,color.x+color.y)) * 0.5 + 0.5;
	col *= vec3(c+(bright*0.5),(c.x+c.y)*0.5+bright);

	gl_FragColor = vec4(col, 1.0);
}