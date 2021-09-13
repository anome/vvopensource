/*
	{
	"DESCRIPTION": "Hex Play",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "Import by: Old Salt",
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
			"MIN": 1.0,
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
// Import from: http://www.glslsandbox.com/e#57529.0

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset);                          // offset only, shader core handles zoom
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

	/**** Start of Core Imported Shader Code *****/
	vec3 col = vec3(0.);
	vec2 k = vec2(.5, .86);
	vec2 m = k / 2.;
	float w = 0.0005 + (11.0-uZoom)*0.12;
	vec2 suv = vec2(length(uv)/w, atan(uv.x, uv.y));
	if(suv.x < 1.){ suv.x = 1./pow(suv.x, pow(suv.x, 1.0-w)-1.); }
	uv = suv.x*vec2(sin(suv.y), cos(suv.y))*w;
	vec2 a = mod(uv, k) - m;
	vec2 b = mod(uv - m, k) - m;
	vec2 f = abs(length(a) < length(b) ? a: b);
	vec2 i = uv - f;
	float T = TIME,
	d = max(dot(f, k), f.x);
	col += smoothstep(0.02, 0.0, abs(d - 0.24));
	col += smoothstep(0.02, 0.0, abs(d - 0.1));
	col += smoothstep(0.02, 0.0, abs(d - 0.05));
	d = max(abs(f.x), abs(f.y));
	col += smoothstep(0.01, 0., abs(d - 0.17));
/****  End of Core Imported Shader Code  *****/

// gl_FragColor = vec4(col, 0.5);
	vec4 cShad = vec4(col, 1.0);  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1) cOut = cOut*uC1.rgb + (1.0-cOut)*uC2.rgb;
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
	