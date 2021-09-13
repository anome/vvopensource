/*
	{
	"DESCRIPTION": "Invisible Spiral Mask",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "original by Old Salt",
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
			"LABEL": "Spin Rate: ",
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": -1.0,
			"MAX": 1.0
			},
			{
			"LABEL": "Show Mask: ",
			"NAME": "showmask",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Angular Fade:       ",
			"NAME": "afade",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Radial Fade:        ",
			"NAME": "rfade",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Bidirectional Fade: ",
			"NAME": "bifade",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Fade Rate: ",
			"NAME": "fade",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1,
			"MIN": 0
			}
		]
	}
*/

#ifdef GL_ES
	precision highp float;
#endif

#define twopi 6.283185307179586  // 2*pi
#define	piphi	2.39996322972865   // pi*(3-sqrt(5))

void main() 
	{
	float T = TIME * 2.0 * rate;
	vec2 OffsetCoord = (gl_FragCoord.xy/RENDERSIZE.xy - offset + 0.5)*RENDERSIZE.xy;
	vec2 p = (OffsetCoord.xy+OffsetCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
	p = vec2(0.0, T - log2(length(p.xy))) + atan(p.y, p.x) / twopi; 
	p.x = ceil(p.y) - p.x;
	p.x *= piphi;
	float a = 0.0;
	float r = 0.0;
	if (afade) a = fract(p.x+T);
	if (rfade) r = fract(p.y+T);
	float mt = (a + r) * fade;
	if (bifade) mt = 4.0 * abs(a + r - 0.5) * fade;
	if (showmask) gl_FragColor = vec4(vec3(a + r),1.0 - mt);
	else gl_FragColor = vec4(vec3(0.0),1.0 - mt);
	}