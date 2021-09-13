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
    }
  ]
}*/

////////////////////////////////////////////////////////////
// FractilianLace  by mojovideotech
//
// based on :
// glslsandbox.com/\e#28193.0
// by Nikos Papadopoulos, 4rknova 2015
// Adapted from shadertoy.com/\4lSSRy by J.
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	phi   	1.618033988749895 	// golden ratio
#define 	twpi  	6.283185307179586  	// two pi, 2*pi

vec3 hueToRGB(float hue) {
	    return clamp(abs(mod(hue * 6.0 + vec3(0.0, color3, 2.0), color1) - color2) - 1.0, 0.0, 1.0);
}

void main( void ) {
	vec2 uv = scale * (gl_FragCoord.xy / RENDERSIZE.y);
    float t = TIME*rate, k = cos(t), l = sin(t), s = phi;
    s -= dot(k,l);
    float b = 1.0;
    for(int i=0; i<120; ++i) {
        b += 1.0;
    	if (b>depth) break;
        uv  = abs(uv) - s; 
        uv *= mat2(k,-l,l,k);
        s  *= freq;  
    }
    float x = cos(twpi*(359.0*length(uv)));
    gl_FragColor = vec4(vec3(hueToRGB(x)),1.0);
}