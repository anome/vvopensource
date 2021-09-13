/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    {
      "NAME": "offset",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.01,
      "MAX": 0.5
    },
    {
      "NAME": "cells1",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 3,
      "MAX": 13
    },
    {
      "NAME": "cells2",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 2,
      "MAX": 21
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0.01,
      "MAX": 0.5
    },
    {
      "NAME": "phase",
      "TYPE": "float",
      "DEFAULT": 0.42,
      "MIN": 0.1,
      "MAX": 0.49
    },
    {
        "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 5.5,
      "MIN": -6.0,
      "MAX": 6.0
    },
    {
      "NAME": "flip",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "flop",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#33354.23"
}*/

///////////////////////////////////////////
// CellSpiral  by mojovideotech
//
// based on :
// glslsandbox.com/\e#33354.23
//
// inspired by :
// shadertoy.com/\Mdd3D7
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////


#define 	PI   	3.14159265358

void main() 
{
	float ti = TIME*phase;
	vec2 cst = vec2(cos(ti),sin(ti));
	vec2 uv = (isf_FragNormCoord) + (vec2(cos(ti),sin(ti))*offset)-0.5;
	float duv = dot(uv,uv);
	uv = mix(uv/duv, uv*duv, sin(length(uv)/(PI*depth)));
	float N0 = floor(cells1), N1 = floor(cells2);
	float theta = atan(uv.y,uv.x);
	float t = ti*rate;
	float lr = log(length(uv));
	float t0 = lr-theta+0.25*t;
	t0 = fract(N0*(t0/2.0/PI+0.5));
	float t1 = fract(N1*(lr+theta+1.00*t)/2.0/PI+0.5);
	if (flip) { t0 = fract(N0*t1); }
	if (flop) { t1 = fract(N1*t0); }
	float v = pow(t0*t1,abs(fract(ti*2e-2)));
	
	gl_FragColor = vec4(vec3(v), 1.0);

}