/*
	{
	"DESCRIPTION": "Talismans",
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
			"MAX": 2.0,
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
		],
  "PASSES":
		[
			{
			"TARGET": "buffer",
			"persistent": true
			},
			{}
		]
	}
*/
// Import from: https://www.shadertoy.com/view/sd2GRG
// Created by: Kali


#define PI 3.141592653589
#define LB2_3 1.58496250072
#define PILB PI * LB2_3

#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


vec4 disk(vec2 p, float w)
	{
	float t=TIME+w*6.5;
	p*=2.+dot(p,p)*2.;
	float s=sin(t*.1*PILB);
	p.x+=s*s*s;
	p*=rotate2D(tan(t*PILB*.2)*.1);
	p*=7.-atan(5.*cos(t*2.*PILB*.25)*PILB)*1.5;
	float m=1000., it=0.,cir=smoothstep(7.5,7.,length(p));
	vec2 p2=p;
	for (int i=0; i<4; i++)
		{
		p*=rotate2D(radians(45.));
		p=abs(p*.9)-.5;
		}
	for (float i=0.; i<7.; i++)
		{
		p.x=abs(p.x)-fract(floor(t*LB2_3)*.1);
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
	col.xz*=rotate2D(it*.3);
	return vec4(abs(col)*cir,cir)+smoothstep(1.,.5,cir)*cir;
	}


vec4 pass_0(vec2 uv)
	{
	vec4 f1=disk(uv,0.);
	vec4 f2=disk(uv,1.);
	vec3 col=mix(f2.rgb,f1.rgb,f1.w);
//	col+=smoothstep(.4,.5,abs(.5-fract(uv.y*50.)))*(1.-max(f1.w,f2.w))*.3;
	return vec4(col,1.);
	}

vec4 pass_1(vec2 uv)
	{
	// Normalized pixel coordinates (from 0 to 1)
	vec3 col = vec3(0.);
	float s=1.;
	for (float i=0.; i<100.; i++)
		{
		s*=.99;
		vec3 t=IMG_NORM_PIXEL(buffer, uv).rgb;
		col+=t*exp(-.05*i)*.07*step(1.,length(t));
		}
  col = mix(IMG_NORM_PIXEL(buffer, uv).rgb, col, 0.25);
	return vec4(col,1.0);
	}

	
void main()
	{
	// keep coordinate system and color manipulation here
	if (PASSINDEX == 0)
		{
		vec2 uv=(gl_FragCoord.xy-RENDERSIZE.xy*.5)/RENDERSIZE.y;
		uv = (uv-uOffset) * 1.0/uZoom;  // zoom at original location, then offset result
		gl_FragColor = pass_0(uv);
		}
	else if (PASSINDEX == 1)
		{
		vec2 uv1 = gl_FragCoord.xy/RENDERSIZE.xy;
		vec4 cShad = pass_1(uv1);  
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
	}
