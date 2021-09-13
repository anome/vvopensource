/*
	{
	"DESCRIPTION": "Blue Streaks",
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
			"MIN": 1.0,
			"DEFAULT": 5.0
			},
			{
			"LABEL": "Rotation(or R Speed): ",
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
// Import from: http://glslsandbox.com/e#72446.0

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

/**** Start of Core Imported Shader Code *****/
	vec2 p = uv * 1.5;
	float t = TIME / 2.5;
	float l = 0.0;
	for (float i = 1.0; i < 23.0; i++)
		{
		p.x += 0.1 / i * cos(i * 8.0 * p.y + t + sin(t / 75.0));
		p.y += 0.1 / i * sin(i * 12.0 * p.x + t + cos(t / 120.0));
		l = length(vec2(0, p.y + sin(p.x * PI * i * ((sin(t / 3.0) + cos(t / 2.0)) * 0.25 + 0.5))));
		}

	float g = 1.0 - pow(l, 0.9);
	//a.x 0->-5 makes black red
	//a.y -23->-2 makes torcoise thicker
	vec3 a = vec3(-0.0, -16.0, 150.0);
	vec3 b = vec3(0.0, -80.0, 0.0);
	vec3 c = vec3(0.0, -400.0, -800.0);

	// factor behind g-> higher makes color line thicker 
	vec3 color = mix(a, mix(b, c, g / 2.0), g * 5.0);
	/****  End of Core Imported Shader Code  *****/

	vec4 cShad = vec4(color, 1.0);  
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
