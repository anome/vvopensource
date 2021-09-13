/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "fractal"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 5.55,
      "MIN": 2,
      "MAX": 8
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 1.81,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 1.23,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 2.22,
      "MIN": 0.001,
      "MAX": 3
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.925,
      "MIN": 0.6667,
      "MAX": 0.9999
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": -0.025,
      "MAX": 0.025
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 72,
      "MIN": 2,
      "MAX": 120
    },
    {
      "NAME": "shape",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "quad",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ]
}*/


////////////////////////////////////////////////////////////
// FractalNoiseCarpet by mojovideotech
//
// based on :
//
// by Nikos Papadopoulos, 4rknova / 2015
// Adapted from https://www.shadertoy.com/view/4lSSRy by J.
//http://glslsandbox.com/e#28193.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define     sqfv	2.23606797749979
#define 	ptpi 	1385.455731367011089 	// powten(pi)
#define		ptphi	41.49865190338333	// powten(phi)
#define 	chpi 	11.591953275521521  	// cosh(pi)
#define 	shpi 	11.548739357257749 	// sinh(pi)
#define 	phi   	1.618033988749895 	// golden ratio
#define 	phrphi 	1.34636082003487	// phi root of phi
#define 	rcpi  	0.318309886183791	// reciprocal of pi  , 1/pi 



vec2 mash(vec2 x) { return mod(mod(x, -sin(TIME*0.05))*(1.0/sqrt(5.)), x*1.61803);	}

vec2 bash(vec2 x) { return mod(mod(x, cos(TIME*x)), sin(x*6.2831853)*x), cos(x*3.141592653);	}

float triangleEQ( vec2 p, float t) { return max(abs(p.x)*sqfv+p.y*0.5,-p.y) - 0.1+ 0.2*sin(t);  }

float quadTorusEQ(vec2 p,float t) {
	float x2 = p.x*p.x, y2 = p.y*p.y;
	float fv = mod(y2,x2)-1.0*sin(y2);
	fv *= fv;
  	fv -= 2.0 + 1.0*sin(t);
	return fv;
}

vec3 hueToRGB(float hue) {
	    return clamp(abs(mod(hue * 6.0 + vec3(0.0,color3, 2.0), color1) - color2) - 1.0, 0.0, 1.0);
}

void main( void ) {
    vec2 uv = scale * (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;
    float t = TIME*rate, k = cos(t/chpi), l = sin(t/shpi), s = ptphi, b = 1.0; 
    if (shape) uv *= triangleEQ(uv,mod(l,k)*phi);
    uv = pow(uv/phi*sqfv,cos(uv/phrphi));
    if (quad) uv *= quadTorusEQ(uv,mod(l,k)*phi);
    s -= dot(k,l);
    for(int i=0; i<120; ++i) {
        b += 1.0;
    	if (b>depth) break;
        uv  = abs(uv) - s; 
        uv *= mat2(k,-l,l,k);
        s  *= freq;  
    }
    float x =  cos(rcpi*(ptpi*length(uv)));
    x -= exp(x);
    if (invert) { x = x * -1.0 + 1.0; }
    gl_FragColor = vec4(vec3(hueToRGB(x)),1.0);
}