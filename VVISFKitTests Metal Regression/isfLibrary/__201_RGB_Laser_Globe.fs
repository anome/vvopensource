/*
	{
	"DESCRIPTION": "RGB Laser Globe",
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
			"LABEL": "Dispersion: ",
			"NAME": "uDisperse",
			"TYPE": "float",
			"MAX": 2.0,
			"MIN": 0,
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
			"MAX": 2.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Original from: https://editor.isf.video/shaders/5e7a804a7c113618206dee9b
// By: By paulofalcao


float makePoint(float x, float y, float fx, float fy, float t)
	{
	float xx=x*cos(t*fx);
	float yy=y*sin(t*fy);
	xx = sqrt(length(xx+yy) + length(xx*yy) * (uDisperse * 2.0 - 2.0));
	return 1.0 / xx;
	}

	
void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv = (uv-uOffset);
	float x=uv.x;
	float y=uv.y;

	float a = makePoint(x,y,3.3,2.9,TIME);
	a += makePoint(x,y,1.9,2.0,TIME);
	a += makePoint(x,y,0.8,0.7,TIME);
	a += makePoint(x,y,2.3,0.1,TIME);
	a += makePoint(x,y,0.8,1.7,TIME);
	a += makePoint(x,y,0.3,1.0,TIME);
	a += makePoint(x,y,1.4,1.7,TIME);
	a += makePoint(x,y,1.3,2.1,TIME);
	a += makePoint(x,y,1.8,1.7,TIME);

	float b=makePoint(x,y,1.2,1.9,TIME);
	b += makePoint(x,y,0.7,2.7,TIME);
	b += makePoint(x,y,1.4,0.6,TIME);
	b += makePoint(x,y,2.6,0.9,TIME);
	b += makePoint(x,y,0.7,1.4,TIME);
	b += makePoint(x,y,0.7,1.7,TIME);
	b += makePoint(x,y,0.8,0.5,TIME);
	b += makePoint(x,y,1.4,0.7,TIME);
	b += makePoint(x,y,0.7,1.3,TIME);

	float c=makePoint(x,y,3.7,0.3,TIME);
	c += makePoint(x,y,1.9,1.3,TIME);
	c += makePoint(x,y,0.8,0.9,TIME);
	c += makePoint(x,y,1.2,1.7,TIME);
	c += makePoint(x,y,0.3,0.6,TIME);
	c += makePoint(x,y,0.3,0.3,TIME);
	c += makePoint(x,y,1.4,0.8,TIME);
	c += makePoint(x,y,0.2,0.6,TIME);
	c += makePoint(x,y,1.3,0.5,TIME);
   
	vec3 d=vec3(a,b,c)*0.005;
	vec4 cShad = vec4(d, 1.0);  
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
	