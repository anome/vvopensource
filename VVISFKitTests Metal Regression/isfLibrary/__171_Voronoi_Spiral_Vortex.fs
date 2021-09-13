/*
	{
	"DESCRIPTION": "Voronoi Spiral Vortex",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "Modified by: Old Salt",
	"VSN": "1.0",
	"INPUTS": 
		[
			{
			"LABEL": "Center: ",
			"NAME": "offset",
			"TYPE": "point2D",
			"MAX":
				[
				1.0,
				1.0
				],
			"MIN":
				[
				0.0,
				0.0
				],
			"DEFAULT": 
				[
				0.5,
				0.5
				]
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "intensity",
			"TYPE": "float",
			"MAX": 0.99,
			"MIN": 0.01,
			"DEFAULT": 0.4
			},
			{
			"LABEL": "Rate: ",
			"NAME": "rate",
			"TYPE": "float",
			"MAX": 5.0,
			"MIN": -5.0,
			"DEFAULT": 2.0
			},
			{
			"LABEL": "Divisions: ",
			"NAME": "divisions",
			"TYPE": "float",
			"MAX": 40.0,
			"MIN": 1.0,
			"DEFAULT": 9.0
			},
			{
			"LABEL": "Loops: ",
			"NAME": "loops",
			"TYPE": "float",
			"MAX": 9.0,
			"MIN": 1.0,
			"DEFAULT": 2.0
			},
			{
			"LABEL": "Offset1: ",
			"NAME": "offset1",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.75
			},
			{
			"LABEL": "Offset2: ",
			"NAME": "offset2",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.025
			},
			{
			"LABEL": "Center Shape: ",
			"NAME": "shape",
			"TYPE": "float",
			"MAX": 0.5,
			"MIN": 0.01,
			"DEFAULT": 0.125
			},
			{
			"LABEL": "Red:   ",
			"NAME": "R",
			"TYPE": "float",
			"DEFAULT": 80.0,
			"MIN": 0.0,
			"MAX": 100.0
			},
			{
			"LABEL": "Green: ",
			"NAME": "G",
			"TYPE": "float",
			"MAX": 100.0,
			"MIN": 0.0,
			"DEFAULT": 40.0
			},
			{
			"LABEL": "Blue:  ",
			"NAME": "B",
			"TYPE": "float",
			"MAX": 100.0,
			"MIN": 0.0,
			"DEFAULT": 10.0
			},
			{
			"LABEL": "Invert Colors: ",
			"NAME": "invert",
			"TYPE": "bool",
			"DEFAULT": false
			}
		]
	}
*/

// original: shadertoy.com/4tXGW4
// modified by: by mojovideotech
// https://editor.isf.video/shaders/5e7a801d7c113618206deaf0

#define rctwpi 0.159154943091895		// reciprocal of twpi  , 1/2*pi 

void main() 
	{
	vec2  p = (gl_FragCoord.xy - RENDERSIZE.xy*offset)-0.5;
	float d = length(p) / RENDERSIZE.y, x = pow(d, shape), y = atan(p.x, p.y) * rctwpi;
	float T = TIME * rate * 0.01, M = floor(divisions) * 2.0, c = 1.0, z = 0.0;
	for (float i = 1.0; i < 9.0; ++i)
		{   
		z = y;
		c = min(c, length(fract(vec2(x - T*i*offset1, fract(z - i*offset1)*0.5)*M)*2.0-1.0));
		y += sin(T*offset2)*i;
		if (i >= loops) { break; }
		}
	float f = c*d*d*(intensity-d);
	vec3 g = vec3(d+R*f,d+G*f,d+B*f);
	if (invert) { g.rgb = g.brg; } 
	gl_FragColor = vec4(g,1.0);
	}
