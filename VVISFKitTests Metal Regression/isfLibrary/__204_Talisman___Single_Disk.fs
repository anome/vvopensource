/*
	{
	"DESCRIPTION": "Talisman - Single Disk",
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
			"MAX": 5.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Rotate Center: ",
			"LABELS":
				[
				"Talisman ",
				"Viewport "
				],
			"NAME": "uOffMode",
			"TYPE": "long",
			"VALUES": [0,1],
			"DEFAULT": 0
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
			"LABEL": "Short Segment? ",
			"NAME": "uSegShort",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Segment Select:",
			"NAME": "uSegSel",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 0.0
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
// Import from: https://www.shadertoy.com/view/sd2GRG
// Created by: Kali


#define PI 3.141592653589
#define LB2_3 1.58496250072
#define PILB PI * LB2_3

#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))
	
void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.
  vec2 p;
	if (uOffMode==0)
		{
		p = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * uZoom);  // all on one line
		p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0); // rotate 
		}
	else
		{
		p = (gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE * RENDERSIZE/RENDERSIZE.y;   // normalize, correct aspect ratio
		p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0); // rotate
		p = (p - uOffset)/uZoom;                       // apply offset and zoom
		}

	p = p*2.75;
	float t=TIME;
	p*=2.+dot(p,p)*2.;
	float m=1000., it=0.,cir=smoothstep(7.5,7.,length(p));
	vec2 p2=p;
	for (int i=0; i<4; i++)
		{
		p*=rotate2D(radians(45.));
		p=abs(p*.9)-.5;
		}
	for (float i=0.; i<7.; i++)
		{
		p.x = uSegShort ? abs(p.x) - uSegSel : abs(p.x)-fract(floor(t*LB2_3)*.1);
		p=p/dot(p,p);
		float l=abs(p.x)+abs(.5-fract(p.y*.2+.25*t*LB2_3+i*.2))*.5;
		if (l<m)
			{
			m=l;
			it=i;
			}
		}
	m=.2/(.2+m*m*15.);
	vec3 col = vec3(m,m*m,m*m*m);
	col.xz*=rotate2D(it*0.3);
	vec4 cShad = vec4(abs(col)*cir,cir)+smoothstep(1.,.5,cir)*cir; 
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,1.0);
	}
