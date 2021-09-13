/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "procedural",
    "2d",
    "sacedgeometry"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "center",
      "TYPE": "point2D",
      "DEFAULT": [ 0, 0 ],
      "MAX": [ 4, 4 ],
      "MIN": [ -4, -4 ]
    },
    {
      "NAME": "size",
      "TYPE": "float",
      "DEFAULT": 40,
      "MIN": 2,
      "MAX": 50
    },
    {
      "NAME": "width",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.01,
      "MAX": 0.25
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "blend",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0,
      "MAX": 5
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.01,
      "MAX": 3
    },
    {
      "NAME": "amp",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.01,
      "MAX": 2
    },
    {
      "NAME": "warp",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        0
      ],
      "MAX": [
        2,
        2
      ],
      "MIN": [
        -2,
        -2
      ]
    }
  ]
}*/


////////////////////////////////////////////////////////////
// RippleRingMosiac  by mojovideotech
//
// based on:
// shadertoy.com/\lslSR7  by gleurop
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define r3  1.772453850905516

vec3 hsv(float h,float s,float v) {
	return mix(vec3(1.),clamp((abs(fract(h+vec3(3.,2.,1.)/3.)*6.-3.)-1.),0.,1.),s)*v;
}
float circle(vec2 p, float r) {
	return smoothstep(width, 0.0, abs(length(p)-r)); 
}

void main(){
	vec2 uv = -1.0 + 2.0*gl_FragCoord.xy / RENDERSIZE.xy;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv -= center;
	uv *= 52.0 - size;
	float r = smoothstep(-blend, blend, sin(TIME*rate-length(uv)*freq))+amp;
	vec2 rep = vec2(4.0+warp.x,(r3*4.0)-warp.y);
	vec2 p1 = mod(uv, rep)-rep*0.5;
	vec2 p2 = mod(uv+vec2(2.0,0.0), rep)-rep*0.5;
	vec2 p3 = mod(uv+vec2(1.0,r3), rep)-rep*0.5;
	vec2 p4 = mod(uv+vec2(3.0,r3), rep)-rep*0.5;
	vec2 p5 = mod(uv+vec2(0.0,r3*2.0), rep)-rep*0.5;
	vec2 p6 = mod(uv+vec2(2.0,r3*2.0), rep)-rep*0.5;
	vec2 p7 = mod(uv+vec2(1.0,r3*3.0), rep)-rep*0.5;
	vec2 p8 = mod(uv+vec2(3.0,r3*3.0), rep)-rep*0.5;
	float c = 0.0;
	c += circle(p1, r);
	c += circle(p2, r);
	c += circle(p3, r);
	c += circle(p4, r);
	c += circle(p5, r);
	c += circle(p6, r);
	c += circle(p7, r);
	c += circle(p8 , r);
	gl_FragColor = vec4(hsv(r+color, 1.0, c), 1.0);
}