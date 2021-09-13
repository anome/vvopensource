/*
	{
	"DESCRIPTION": "Hex 3D Spiral",
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
			"LABEL": "Twist: ",
			"NAME": "uTwist",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Outline: ",
			"NAME": "uOutline",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Reverse: ",
			"NAME": "uReverse",
			"TYPE": "bool",
			"DEFAULT": 0
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
// Import from: http://www.glslsandbox.com/e#72103.0

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

/**** Start of Core Imported Shader Code *****/
	vec2 p;
	float c = 0.0;
	float twist = uTwist * 4.0;
	float outthick = 1.0-(0.78 * uOutline + 0.1);
	if (uReverse) twist = -1.0 * twist;
	float i,g,d=1.;
	for(float j=0.;j<128.;j++) 
		{
		++i;
		if (d<=.001) break;
		p=uv*g+vec2(.3)*rotate2D(g*twist);
		g+=d=-(length(p)-2.+g/9.)/2.;
		}
	p=vec2(atan(p.x,p.y),g)*8.28+TIME*2.;
	p=abs(fract(p+vec2(0,.5*ceil(p.x)))-.5);
	c+=30./i-.5/smoothstep(outthick, 0.9, 1.0 -abs(max(p.x*1.5+p.y,p.y*2.)-1.));
/****  End of Core Imported Shader Code  *****/

	vec3 cOut;
	if (uColMode == 0) cOut = vec3(c/4.0);
	else cOut = (c * uC1.rgb) + ((1.0-c)* uC2.rgb);
	cOut = clamp((cOut * 4.0 * uIntensity), 0.0, 1.0);
	gl_FragColor = vec4(cOut,1.0);
	}
	