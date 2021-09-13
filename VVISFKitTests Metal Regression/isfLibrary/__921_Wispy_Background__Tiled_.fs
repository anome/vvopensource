/*
	{
	"DESCRIPTION": "Wispy Background (Tiled)",
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
			"NAME": "NoXforms",
			"TYPE": "color",
			"DEFAULT":[0.0,0.0,0.0,0.0]
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
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.0
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
			"LABEL": "Density: ",
			"NAME": "uDensity",
			"TYPE": "float",
			"MAX": 20.0,
			"MIN": 1,
			"DEFAULT": 4.0
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
// Import from: http://glslsandbox.com/e#73288.0
// By: David Hoskins

#define PI 3.141592653589
#define TAU 6.28318530718
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.

	float zoom = (uZoom < 0.0) ? (1.0-abs(uZoom)) : (1.0+uZoom*9.0);
	vec2 uv = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * zoom);
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0);
	uv += 0.5;
	uv = mod(uv*TAU, TAU)-250.0;
	float t = TIME * .5+23.0;
	vec2 i = uv;
	float c = 1.0;
	float inten = .005;
	for (int n = 1; n < 21; n++) 
		{
		float t = t * (1.0 - (3.5 / float(n+1)));
		i = uv + vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(vec2(uv.x / (sin(i.x+t)/inten),uv.y / (cos(i.y+t)/inten)));
		if (float(n) >= uDensity) break;
		}
	c /= uDensity;
	c = 1.17-pow(c, 1.4);
	vec3 colour = vec3(pow(abs(c), 8.0));
	colour = clamp(colour + vec3(0.0, 0.35, 0.5), 0.0, 1.0);

	vec3 cOut = colour;
	if (uColMode == 1) cOut = cOut* uC1.rgb;
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut, 1.0);
}
