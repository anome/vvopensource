/*
	{
	"DESCRIPTION": "Railroad Tracks",
	"CATEGORIES": 
		[
		"generator",
		"Christmas"
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
			"LABEL": "Center: ",
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
			"LABEL": "Uprights? ",
			"NAME": "uUprights",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Curves? ",
			"NAME": "uCurves",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Default                  ",
				"Alternate Color Palette (3 used)"
				],
			"NAME": "uColMode",
			"TYPE": "long",
			"VALUES":
				[0,1],
			"DEFAULT": 0
			},
			{
			"LABEL": "Intensity: ",
			"NAME": "uIntensity",
			"TYPE": "float",
			"MAX": 2.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Original URL: http://glslsandbox.com/e#71512.1

#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

float sdBox(vec3 p, vec3 b)
	{
	vec3 q = abs(p) - b;
	return length(max(q, 0.)) + min(max(q.x, max(q.y, q.z)), 0.);
	}

float sdVerticalCapsule(vec3 p, float h, float r)
	{
	p.y -= clamp(p.y, 0., h);
	return length(p) - r;
	}

float slalom(float z)
	{
	if (uCurves) return sin(z * .1 + TIME * .5) * 4.;
	else return (0.0);
	}

float map(vec3 p)
	{
	const float dz = 1.;
	float dx0 = slalom(0.);
	p.x -= slalom(p.z) - dx0;
	p.zx *= rotate2D(atan(slalom(dz) - dx0, dz));
	p.x = abs(p.x);
	p -= vec3(0, -2, -TIME * 10.);
	float d = 1. / 0.;
	p.z = mod(p.z, 32.) - 16.;
	if(uUprights) d = min(d, sdVerticalCapsule(p - vec3(4, 0, 0), 6., .1));
	p.z = mod(p.z, 2.) - 1.;
	d = min(d, sdBox(p - vec3(1.2, .2, 0), vec3(.1, .1, 4.)));
	d = min(d, sdBox(p, vec3(2, .1, .2)));
	return d;
	}

void main()
	{
	vec2 uv = (((gl_FragCoord.xy/RENDERSIZE.xy - 0.5) / uZoom) - uOffset + 0.5)*RENDERSIZE.xy;
	uv = (uv * 2. - RENDERSIZE) / RENDERSIZE.y;
	vec3 rd = normalize(vec3(uv - vec2(0, .2), 2));
	vec3 light = normalize(vec3(2, 1, -1));
	vec3 color = vec3(0);
	float dist = 0.;
	for (int i = 0; i < 100; i++)
		{
		vec3 p = rd * dist;
		float d = map(p);
		if (d < .01)
			{
			color = vec3(1, 1, 2) * 5.0 / float(i);
			break;
			}
		dist += d;
		if (dist > 100.) break;
		}
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
