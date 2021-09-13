/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator",
    "fractal",
    "2d",
    "iteration"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 3.2,
      "MIN": 2,
      "MAX": 8
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 2.49,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 3.92,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 0.96,
      "MIN": 0.001,
      "MAX": 3
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.95,
      "MIN": 0.6667,
      "MAX": 0.9999
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": -0.02,
      "MIN": -0.025,
      "MAX": 0.025
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 61,
      "MIN": 2,
      "MAX": 120
    },
    {
      "NAME": "shape",
      "TYPE": "bool",
      "DEFAULT": "0"
    },
    {
      "NAME": "quad",
      "TYPE": "bool",
      "DEFAULT": "1"
    },
    {
      "NAME": "curve",
      "TYPE": "bool",
      "DEFAULT": "1"
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "0"
    }
  ]
}*/


///////////////////////////////////////////
// FractalianArchitecture  by mojovideotech
//
// mod of :
// FractalNoiseCarpet by mojovideotech
//
// based on :
// glslsandbox.com/\e#28193.0
// by Nikos Papadopoulos, 4rknova  2015
// adapted from shadertoy.com/\4lSSRy  by J.
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
///////////////////////////////////////////


#ifdef GL_ES
precision highp float;
#endif


#define 	ptpi 	1385.455731367011089 // powten(pi)
#define		ptphi	41.49865190338333	// powten(phi)
#define 	chpi 	11.591953275521521  // cosh(pi)
#define 	shpi 	11.548739357257749 	// sinh(pi)
#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// 
#define 	phicu  	4.23606797749979 	// phi cubed, phi^3
#define     sqfv	2.23606797749979   	// sq root of five
#define 	phi   	1.618033988749895 	// golden ratio
#define 	phrphi 	1.34636082003487	// phi root of phi
#define 	thpi  	0.996272076220750	// tanh(pi)
#define 	rcphi  	0.61803398874989	// reciprocal of phi  , 1/phi 
#define 	rcsqfv	0.44721359549996	// reciprocal of sqrt of five 
#define 	rcpi  	0.318309886183791	// reciprocal of pi  , 1/pi 


vec2 mash(vec2 x) {
	return mod(mod(x, -sin(TIME*0.05))*rcsqfv, x*phi);	
}

vec2 bash(vec2 x) {
	return mod(mod(x, cos(TIME*x)), sin(x*twpi)*x), cos(x*pi);		
}

float triangleEQ( vec2 p, float t) {
	return max(abs(p.x)*sqfv+p.y*0.5,-p.y) - 0.1+ 0.2*sin(t);
}

float quadTorusEQ(vec2 p,float t) {
	float x2 = p.x*p.x;
	float y2 = p.y*p.y;
	float fv = mod(y2,x2)-1.0*sin(y2);
	fv *= fv;
    fv -= 2.0 + 1.0*sin(t);
    
    return fv;
}

float squidEQ(vec2 p,float t) {
	float fv = p.x;
	fv = (p.y+length(p*fv)-cos(t+p.y));
	fv += (p.y+length(p*fv)-0.5*cos(t+p.y));
	fv *= fv*0.1;
	
	return fv;
}

vec3 hueToRGB(float hue) {
	    return clamp(abs(mod(hue * 6.0 + vec3(0.0,color3,2.0), color1) - color2) - 1.0,0.0,1.0);
}

void main( void ) {
    vec2 uv = scale * (2.0*gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;
    float t = TIME*rate, k = cos(t/chpi), l = sin(t/shpi), b = 1.0;
    if (curve) {
    	uv *= cos(uv/shpi);
    	uv /= squidEQ(uv,mod(l,k)*ptphi);
    }
    if (shape) { uv *= triangleEQ(uv,mod(l,k)*phicu); 
   	}
    if (quad) {
     	uv = pow(uv/phi*sqfv,cos(uv/phrphi));
    	uv *= quadTorusEQ(uv,mod(l,k)*phi);
    }
    float s = pow(ptphi,dot(k,l));
    for(int i=0; i<120; ++i) {
        b += 1.0;
    	if (b>depth) break;
        uv  = abs(uv) - s; 
        uv *= mat2(k,-l,l,k);
        s  *= freq;  
    }
     float x =  cos(rcpi*(ptpi*length(uv)));
     x -= exp(x);
     if (invert) {
		x = x * -1.0 + 1.0;
	}
    gl_FragColor = vec4(vec3(hueToRGB(x)),1.);
}