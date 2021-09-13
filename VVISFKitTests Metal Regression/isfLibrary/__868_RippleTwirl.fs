/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
  	"generator",
  	"ripple",
  	"radial"
  ],
  "INPUTS": [
    {
      "NAME": "sectors",
      "TYPE": "float",
      "DEFAULT": 44,
      "MIN": 1,
      "MAX": 100
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": -1.5,
      "MAX": 1.5
    },
    {
      "NAME": "R1",
      "TYPE": "float",
      "DEFAULT": 0.6,
      "MIN": 0,
      "MAX": 0.8
    },
    {
      "NAME": "R2",
      "TYPE": "float",
      "DEFAULT": 0.7,
      "MIN": 0,
      "MAX": 0.8
    },
    {
      "NAME": "R3",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 0.8
    },
    {
      "NAME": "vanishingpoint",
      "TYPE": "float",
      "DEFAULT": 0.95,
      "MIN": 0.25,
      "MAX": 1
    },
    {
      "NAME": "R",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "G",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "B",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "Y",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "U",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "V",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 0.9
    },
    {
      "NAME": "colorspace",
      "TYPE": "point2D",
      "DEFAULT": [ 0.5, 0.5 ],
      "MAX": [ 3, 3 ],
      "MIN": [ -3, -3 ]
    }
  ]
}*/


////////////////////////////////////////////////////////////
// RippleTwirl  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif

#define 	pi   	3.141592653589793 	// pi

void main( void ) {
	vec2 position = (gl_FragCoord.xy - RENDERSIZE * 0.5) / RENDERSIZE.y;
	float th = atan(position.y, position.x) / (2.0 * pi);
	float dd = length(position);
	float d = 0.05*sin(dd*sectors) / dd + (TIME*rate);
	vec3 uv = vec3(th + d, th - d, th + sin(d) * 0.1);
	float a = 0.5 + cos(uv.x * pi * 4.0) * R1;
	float b = 0.5 + sin(uv.y * pi * 8.0) * R2;
	float c = 0.5 + cos(uv.z * pi * 6.0) * R3;
	vec3 color = mix(vec3(1.0, 01.8, 01.9), vec3(01.1, 01.1, 01.2), pow(a, 1.25)) * 0.75;
	color *= mix(vec3(0.9, 0.9, 0.9), vec3(1.25),  pow(b, 0.111)) * 0.999;
	color /= mix(vec3(0.0, 0.5, 0.9), vec3(1.125),  pow(c, 0.333)) * 0.667;
	gl_FragColor = vec4(fract(color) * clamp(dd, 0.05, 1.0), 1.0);
 	mix(vec3(1.0, 01.8, 01.9), vec3(01.1, 01.1, 01.2), pow(a, 1.25)) * 0.75;
	color *= mix(vec3(R, G, B), vec3(colorspace.x),  pow(b, 0.111)) * 0.999;
	color += mix(vec3(V, Y, U), vec3(colorspace.y),  pow(c, 0.333)) * 0.667;
	gl_FragColor = vec4(fract(color) * clamp(dd, 1.0-vanishingpoint, 1.0), 1.0);
}