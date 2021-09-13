/*{
	"CREDIT": "by mojovideotech",
 "CATEGORIES" : [
    "procedural",
    "2d",
    "psychedelic"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  	{
			"NAME": "center",
			"TYPE": "point2D",
        	"DEFAULT": [
				0.0,
				0.0
	  		],
    		"MAX" : [
        		12.0,
        		12.0
      		],
      		"MIN" : [
        		-12.0,
        		-12.0
      		]
    	},
    	{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 180,
			"MIN": 100,
			"MAX": 200
		},
		{
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.01,
			"MAX": 0.0
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": -3.0,
			"MAX": 3.0
		},
		{
			"NAME": "blend",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"NAME": "color",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "freq",
			"TYPE": "float",
			"DEFAULT": 0.33,
			"MIN": 0.01,
			"MAX": 3.0
		},
		{
			"NAME": "amp",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 3.0
		},
		{
			"NAME": "depth",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.1,
			"MAX": 3.0
		},
		{
			"NAME": "mixer",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "warp",
			"TYPE": "point2D",
        	"DEFAULT": [
				1.0,
			   -0.1
	  		],
    		"MAX" : [
        		4.0,
        		4.0
      		],
      		"MIN" : [
        		-4.0,
        		-4.0
      		]
    	}
  ]
}
*/

// RippleRingVortex by mojovideotech
// interactiveshaderformat.com/sketches/976

#define r3  0.318309886183791

vec3 hsv(float h,float s,float v) {
	return mix(vec3(1.),clamp((abs(fract(h+vec3(3.,2.,1.)/3.)*6.-3.)-1.),0.,1.),s)*v;
}
float circle(vec2 p, float r) {
	return smoothstep(width, 0.0, abs(length(p)-r))*depth; 
}

void main(){
	vec2 uv = -1.0 + 2.0*gl_FragCoord.xy / RENDERSIZE.xy;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv -= center;
	uv *= 202.0 - size;
	float r = smoothstep(-blend, blend, sin(TIME*rate-length(uv)*freq))+amp;
//	vec2 rep = vec2(warp.x,warp.y);
	vec2 rep = vec2(sin(r-warp.x),cos(r3*r)-warp.y);
	vec2 p1 = pow(uv, rep)-sin(r*r3);
	vec2 p2 = mod(p1, rep)-cos(r*r3);
	vec2 p3 = mod(p2, rep)+sin(r*r3);
	vec2 p4 = mod(p3, rep)+cos(r*r3);
	vec2 p5 = mod(p4, rep)-sin(r*r3);
	vec2 p6 = mod(p5, rep)-cos(r*r3);
	vec2 p7 = mod(p6, rep)+sin(r*r3);
	vec2 p8 = mod(p7, rep)+cos(r*r3);
	
	float c; // = 0.0;
	float d;
	c += circle(p1, r);
	d += circle(p2, r);
	c += circle(p3, r);
	d += circle(p4, r);
	c += circle(p5, r);
	d += circle(p6, r);
	c += circle(p7, r);
	d += circle(p8, r);
	
	gl_FragColor = vec4(hsv(r-color,r+c, mix(c,d,mixer)), 1.0);
}