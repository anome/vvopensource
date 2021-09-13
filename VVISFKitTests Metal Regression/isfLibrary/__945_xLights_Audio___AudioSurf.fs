/*
{
  "ISFVSN": "2",
  "CREDIT": "from https://www.shadertoy.com/view/Mss3Dr",
  "DESCRIPTION": "xLights AudioFFT",
  "INPUTS" : [
	{
	    "NAME": "inputImage",
	    "TYPE": "image"
	},
	{
	    "NAME": "thickness",
	    "TYPE": "float",
	    "DEFAULT": "0.08",
	    "MIN": "0.02",
	    "MAX": "0.60"
	},
	{
	    "NAME": "width",
	    "TYPE": "float",
	    "DEFAULT": "20.0",
	    "MIN": "5.0",
	    "MAX": "100.0"
	},
	{
	    "NAME": "amplitude",
	    "TYPE": "float",
	    "DEFAULT": "0.19",
	    "MIN": "0.05",
	    "MAX": "1.0"
	},
	{
	    "NAME": "velocity",
	    "TYPE": "float",
	    "DEFAULT": "1.0",
	    "MIN": "0.5",
	    "MAX": "5.0"
	}
  ]
}
*/

// by nikos papadopoulos, 4rknova / 2013
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#define P 3.14159
#define E .001

void main()
{
	vec2 c = gl_FragCoord.xy / RENDERSIZE.xy;
	float s = texture2D(inputImage, c * .5).r;
	c = vec2(0, amplitude*s*sin((c.x*width+TIME*velocity)* 2.5)) + (c*2.-1.);
	float g = max(abs(s/(pow(c.y, 2.1*sin(s*P))))*thickness,
				  abs(.1/(c.y+E)));
	gl_FragColor = vec4(g*g*s*.6, g*s*.44, g*g*.7, 1);
}