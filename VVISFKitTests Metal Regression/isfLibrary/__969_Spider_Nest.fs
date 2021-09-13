/*
	{
	"DESCRIPTION": "Spider Nest",
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
// Import from: https://www.shadertoy.com/view/WlGBzc
// Created by: gaz

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv = uv/uZoom - uOffset;
	uv.x *= RENDERSIZE.y/RENDERSIZE.x;
	uv = (uv + 0.5)	* RENDERSIZE;

/**** Start of Imported Shader Code main() *****/
	vec4 O = vec4(0.0);
	vec3 p,
		r=vec3(RENDERSIZE,1.0),
		d=normalize(vec3((uv.xy-.5*r.xy)/r.y,1));  
	float g=0.,e,s;
	for(float i=0.;i<99.;++i)
		{
		p=g*d;
		p-=vec3(0,-.9,1.5);
		r=normalize(vec3(1,8,0));
		s=TIME*.2;
		p=mix(r*dot(p,r),p,cos(s))+sin(s)*cross(p,r);
		s=2.;
		s*=e=3./min(dot(p,p),20.);
		p=abs(p)*e;
		for(int i=0;i<4;++i)
				p=vec3(2,4,2)-abs(p-vec3(4,4,2)),
				s*=e=8./min(dot(p,p),9.),
				p=abs(p)*e;
		g+=e=min(length(p.xz)-.15,p.y)/s;
		e<.001?O+=3.*(cos(vec4(3,8,25,0)+log(s)*.5)+3.)/dot(p,p)/i:O;
		}
/****    End of Imported Shader main()     *****/

	vec4 cShad = O;  
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
