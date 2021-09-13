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
      "NAME": "squish",
      "TYPE": "float",
      "DEFAULT": 180,
      "MIN": 0,
      "MAX": 360
    },
    {
      "NAME": "squash",
      "TYPE": "float",
      "DEFAULT": 180,
      "MIN": 0,
      "MAX": 360
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
      "NAME": "blend",
      "TYPE": "float",
      "DEFAULT": 12,
      "MIN": 2,
      "MAX": 120
    },
    {
     	"NAME" :	"seed",
     	"TYPE" : 	"float",
     	"DEFAULT" :	1.618,
     	"MIN" : 	0.1,
     	"MAX" : 	3.0
	},
	{
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ]
}*/

////////////////////////////////////////////////////////////
// FractilianVariation  by mojovideotech
// mod of :
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

void main( void ) {
	vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy);
	uv = vec2(uv * 2.0 - 1.0);
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
    float yy = radians(squish*(gl_FragCoord.y/RENDERSIZE.y));
    float xz = radians(squash*(gl_FragCoord.x/RENDERSIZE.x));
    vec2 rd = vec2(sin(xz)*cos(yy), cos(xz)*cos(yy)) * scale;
    float t = TIME*rate, k = cos(t), l = sin(t), s = mod(seed,t);
    s -= dot(k,l);
    float b = 1.0;
    for(int i=0; i<120; ++i) {
        b += 1.0;
    	if (b>depth) break;
        rd = abs(atan(rd)) - s; 
        uv = abs(rd) - s; 
        uv *= mat2(k,l,-l,k);
        rd *= mat2(k,-l,l,k);
        s  *= freq;  
    }
    float x = distance(cos(twpi*(359.0*length(uv))), -sin(twpi*(length(rd)))); //, atan(twpi*(90.*distance(uv,rd))));
    x += cos(twpi*(blend*distance(uv,rd)));
    vec3 c = vec3(color1,color2,color3) / 3.0;
    vec3 q = abs(x * 2.0 - c);
    vec3 g = smoothstep(0.0,x,q);
   if (invert) { g = 1.0 - g ; }
    gl_FragColor = sqrt(vec4(g,1.0));
}