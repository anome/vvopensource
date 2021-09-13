/*
	{
	"DESCRIPTION": "LogTransSpiral",
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
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
			},
			{
			"NAME": "uC2",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,1.0,1.0]
			},
			{
			"LABEL": "Offset: ",
			"NAME": "uOffset",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Rotation(or R Speed):",
			"NAME": "uRotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Defaults ",
				"Alternate Color Palette (2 used) "
				],
			"NAME": "uColMode",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 4.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Import from: https://editor.isf.video/shaders/5eb8b21e9a2c7b001769383b
// https://www.shadertoy.com/view/Msd3Dn
// as modified by: mojovideotech 
// Old Salt: fixed glitch in spiral with mod to line 96 twpi -> twpi - .0265

#define PI 3.141592653589
#define twpi 1.283185307179586                  // (not 2 * pi, unsure where this comes from)
#define	piphi	2.3999632297286533222315555066336	// pi*(3-sqrt(5))#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation
  vec2 p = uv;

	p = vec2(1.0, -log2(length(p.xy))) + atan(p.y, p.x) / (twpi-.0265); 
	p.x = ceil(p.y) - p.x;
	p.x *= piphi;
	float r = fract(p.x);
	float b = fract(p.y);

	vec4 cShad = vec4(r,0.0,b,9.0);;  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	cOut = clamp(cOut, vec3(0.0), vec3(1.0));
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
	