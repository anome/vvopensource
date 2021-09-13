/*
	{
	"DESCRIPTION": "Star 2-12 Points",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"NAME": "uC1",
			"TYPE": "color",
			"DEFAULT":[0.0,1.0,0.0,1.0]
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
			"DEFAULT": 2.0
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
			"DEFAULT": 0
			},
			{
			"LABEL": "Number of Points: ",
			"LABELS":
				[
				"  2 ","  3 ","  4 ","  5 ","  6 ","  7 ",
				"  8 ","  9 "," 10 "," 11 "," 12 "],
			"NAME": "uPoints",
			"TYPE": "long",
			"VALUES":
				[0,1,2,3,4,5,6,7,8,9,10],
			"DEFAULT": 3
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Defaults ",
				"Alternate Color Palette (1 used) "
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
// inspired by: https://shadertoy.com/view/4lGfDc

#define PI 3.1415926535897932384626433832795
#define pid (0.75 * float(int(uPoints)+2)) / PI
#define size 2.5/uZoom

#define S(r)  smoothstep(  9.0/RENDERSIZE.y, 0.0, abs( uv.x -r ) -.1 )


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
	uv = (uv - (uOffset)/2.0)*RENDERSIZE.xy;
	uv = uv+uv-RENDERSIZE;
	float angle = 0.0;
	angle = uContRot ? TIME*uRotate/36.0 : (uRotate-24.0)*PI/180.0;
	float color;
	float l = length(uv+uv)/RENDERSIZE.y * size;
	float a = (mod(pid*atan(uv.y,uv.x) + angle, 1.5) - 0.72) * 1.55;
	uv = l * cos(a - vec2(0,1.97));
	if (uv.x+uv.y < 1.85) color = mix(0.5* S(0.5), S(0.7), 0.5 + 0.5*uv.y);
	vec3 cOut = vec3(color);
	if (uColMode == 1) cOut = cOut* uC1.rgb;
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,1.0);
	}
	