/*
	{
	"DESCRIPTION": "Tie Die",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF Import by: Old Salt",
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
			"NAME": "uC3",
			"TYPE": "color",
			"DEFAULT":[1.0,0.0,0.0,1.0]
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
				"Alternate Color Palette (3 used) "
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
// original by: Jonathan Proxy
// Import from: http://glslsandbox.com/e#70560.0

#define PI 3.141592653589
#define INV2PI 1.0/(2.0*PI)
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

vec3 rainbow1(in float h)
	{
	h = mod(h, 1.0);
	return vec3(smoothstep(0.3, 1.0, h)+smoothstep(0.5, 0.3, h), smoothstep(0.0, 0.3, h)*smoothstep(0.7, 0.4, h), smoothstep(0.3, 0.7, h)*smoothstep(1.0, 0.8, h));
	}

vec2 cln(in vec2 uv)
	{
	float r = length(uv);
	return vec2(log(r), atan(uv.y, uv.x)) * INV2PI;
	}

void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // zoom at original location, then offset result
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

/**** Start of Imported Shader Code main() *****/

	uv = cln(uv);
	vec3 bg_color = mix(rainbow1(uv.x*3.0+(uv.y-0.25*TIME)), vec3(1.0, 0.5, 1.0), 0.25);
	vec3 fg_color = mix(rainbow1(uv.x+(uv.y*13.0)),	vec3(1.0, 1.0, 1.0), 0.25);

/****    End of Imported Shader main()     *****/

	vec4 cShad = vec4(bg_color*fg_color, 1.0);
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	cOut = clamp(cOut, vec3(0.0), vec3(1.0));
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
