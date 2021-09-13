/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		""
	],
	"INPUTS": [	
	{
            "NAME": "scale",
            "TYPE": "float",
           "DEFAULT": 1.25,
            "MIN": -6.0,
            "MAX": 6.0
          },
            {
            "NAME": "rot",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": -1,
            "MAX": 1
          },
            {
            "NAME": "rate",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": -3,
            "MAX": 3
          },
             {
            "NAME": "shift",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 1
          },
           {
            "NAME": "morph",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 1
          },
             {
            "NAME": "mix1",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 1
          },
            {
            "NAME": "mix2",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0,
            "MAX": 1
          },
           {
			"NAME": "invert",
			"TYPE": "bool",
	        "DEFAULT": "FALSE"
		}
	]
}*/

// RippleTileMosiac by mojovideotech

#ifdef GL_ES
precision highp float;
#endif
 
#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	e     	2.718281828459045 	// eulers number

mat2 rotate2d(float _angle)
	{
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
	}
vec2 ran2(vec2 st)
	{
    st = vec2( dot(st,vec2(179,277)),
              dot(st,vec2(439,193)) );
    return -1.0 + 2.0*fract(sin(st)*9859.0);
	}
vec2 mash(vec2 x)
	{
	return mod(sin(mod(cos(x*pi), cos(x))*sin(x*twpi)), cos(x)*pi);	
	}

vec2 bash(vec2 x)
	{
	return mod(mod(sin(x*pi), cos(x)), sin(x*twpi)*x), cos(x*pi);		
	}

void main(void) {
	
	vec2 aspect = vec2(RENDERSIZE.x/RENDERSIZE.y, 1.0)*scale; 
	vec2 sc = (2.0*gl_FragCoord.xy/RENDERSIZE.xy - 1.0)*aspect;
		 sc += mix(sc*2.0,bash(sc),morph);
	     sc *= mix(cos(sc*e),sin(sc*pi)+2.,shift);
	float R = TIME * rot;
	float T = TIME * rate;
	vec2 scv = rotate2d(cos(R*pi)) * bash(sc);
	vec2 sch = rotate2d(sin(-R*pi)) * bash(sc);
	vec2 cvr = mix(mash(scv),ran2(scv),mix1);
	vec2 chr = mix(mash(sch),ran2(sch),mix2);
    vec3 vcol = .5 + .5 * cos(twpi * (fract(sin(scv.x)*1e1)) * vec3(1.0,cvr.x,cvr.y)+T);	
    vec3 hcol = .5 + .5 * sin(twpi * (fract(cos(sch.y)*1e1) + vec3(chr.x,1.0,chr.y))+T);
    vec3 zcol = .5 + .5 * sin(pi * (fract(cos(sch.x-scv.x)*1e1) + vec3(cvr.y,chr.x,1.0))+T);
    vec3 col = pow(mod(mod(vcol,zcol),mod(hcol,zcol)),vcol/hcol);
    if (invert) col.rgb *=1.2 - col.ggr;
	gl_FragColor = vec4 (col,1.0);
	
}
	