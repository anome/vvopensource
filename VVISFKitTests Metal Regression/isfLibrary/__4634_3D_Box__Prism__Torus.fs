/*
	{
	"DESCRIPTION": "3D Box, Prism, Torus_WIP",
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
			"MAX": 1.0,
			"MIN": -1.0,
			"DEFAULT": 0.0
			},
			{
			"LABEL": "Continuous Rotation? ",
			"NAME": "uContRot",
			"TYPE": "bool",
			"DEFAULT": 1
			},
			{
			"LABEL": "XY Rotation(or R Speed):",
			"NAME": "uXYrotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 60.0
			},
			{
			"LABEL": "YZ Rotation(or R Speed):",
			"NAME": "uYZrotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 68.0
			},
			{
			"LABEL": "XZ Rotation(or R Speed):",
			"NAME": "uXZrotate",
			"TYPE": "float",
			"MAX": 180.0,
			"MIN": -180.0,
			"DEFAULT": 52.0
			},
			{
			"LABEL": "Toroid Thickness:",
			"NAME": "uTorThick",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.00,
			"DEFAULT": 0.25
			},
			{
			"LABEL": "Display: ",
			"LABELS":
				[
				"Object and Background ",
				"Object Only ",
				"Background Only "
				],
			"NAME": "uDisplay",
			"TYPE": "long",
			"VALUES": [0,1,2],
			"DEFAULT": 0
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
// Import from: http://glslsandbox.com/e#73426.0

#define PI 3.141592653589
#define TwoPI PI * 2.0
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


float sdBox2d(vec2 p, vec2 s)
	{
	p = abs(p) - s;
	return length(max(p, 0.0))+min(max(p.x, p.y), 0.0);
	}

float sdBox(vec3 p, vec3 s)
	{
	p = abs(p) - s;
	return length(max(p, 0.0))+min(max(p.x, max(p.y, p.z)), 0.0);
	}

float sdTorus(vec3 p, float inRadius, float outRadius)
	{
	vec2 q = vec2(length(p.xz) - outRadius, p.y);
	return length(q) - uTorThick * outRadius;
	}

float sdTorusKnots(vec3 p, float inRadius, float outRadius)
	{
	vec2 cp = vec2(length(p.xz) - outRadius, p.y);
	float a = atan(p.x, p.z);
	cp *= rotate2D(a*8.0);
	cp.y = abs(cp.y)-0.3;
	float d = sdBox2d(cp, vec2(uTorThick * outRadius * 0.5 , uTorThick * outRadius));
	return d;
	}

float sdTriPrism(vec3 p, vec2 h)
	{
	vec3 q = abs(p);
	return max(q.z-h.y,max(q.x*sin(PI/3.0)+p.y*sin(PI/6.0),-p.y)-h.x*sin(PI/6.0));
	}

float morphing(vec3 p)
	{
	float t = TIME / 18.0;
	int index = int(mod(t, 4.0));
	float a = smoothstep(0.2, 0.8, mod(t, 1.0));
	if(index == 0) return mix(sdTriPrism(p, vec2(1.0, 1.5)), sdBox(p, vec3(1.0)), a);
	else if(index == 1) return mix(sdBox(p, vec3(1.0)), sdTorus(p, 0.15, 2.0), a);
	else if(index == 2) return mix(sdTorus(p, 0.15, 2.0), sdTorusKnots(p, 0.15, 2.0)*0.4, a);
	else return mix(sdTorusKnots(p, 0.15, 2.0)*0.4, sdTriPrism(p, vec2(1.0, 1.5)), a);
	}

float distanceFunc(vec3 p)
	{
	vec3 p1 = p;
	float dist = 0.0;
	if(uContRot)
		{
		p1.xy *= rotate2D(TIME*uXYrotate/36.0);
		p1.yz *= rotate2D(TIME*uYZrotate/36.0);
		p1.xz *= rotate2D(TIME*uXZrotate/36.0);
		}
	else
		{
		p1.xy *= rotate2D(uXYrotate*PI/180.0);
		p1.yz *= rotate2D(uYZrotate*PI/180.0);
		p1.xz *= rotate2D(uXZrotate*PI/180.0);
		}
	float d1 = morphing(p1);
	dist += d1;
	return dist;
	}

vec3 getNormal(vec3 p)
	{
	vec2 err = vec2(0.1, 0.0);
	return normalize(vec3(distanceFunc(p + err.xyy) - distanceFunc(p - err.xyy),
												distanceFunc(p + err.yxy) - distanceFunc(p - err.yxy),
												distanceFunc(p + err.yyx) - distanceFunc(p - err.yyx)));
	}

// https://www.youtube.com/watch?v=-FvnsYbzpfc
vec3 background(vec3 rayDir)
	{
	vec3 colT = vec3(0.313, 0.816, 0.816);
	vec3 colM = vec3(0.745, 0.118, 0.243);
	vec3 colK = vec3(0.475, 0.404, 0.765);
	vec3 colH = vec3(1.0, 0.776, 0.224);
	vec3 bgColor = vec3(0.0);
	float k = rayDir.y * 0.5 + 0.5;
	bgColor += (1.0-k);
	float a = atan(rayDir.x, rayDir.z);
	float wave1 = sin(a*2.0+TIME)*sin(a*10.0+TIME)*sin(a*4.0);
	wave1 *= smoothstep(0.8, 0.5, k);
	bgColor += wave1*colT;
	float wave2 = sin(a*10.0+TIME+10.0)*sin(a*2.0+TIME+10.0)*sin(a*6.0+10.0);
	wave1 *= smoothstep(0.8, 0.5, k);
	bgColor += wave2*colM;
	float wave3 = sin(a*5.0+TIME+20.0)*sin(a*3.0+TIME+30.0)*sin(a*8.0+20.0);
	wave1 *= smoothstep(0.8, 0.5, k);
	bgColor += wave3*colK;
	float wave4 = sin(a*3.0+TIME+30.0)*sin(a*5.0+TIME+20.0)*sin(a*10.0+30.0);
	wave1 *= smoothstep(0.8, 0.5, k);
	bgColor += wave4*colH;
	return bgColor;
	}


void main()
	{
#ifdef XL_SHADER
	discard;
#endif
	float zoom = (uZoom < 0.0) ? (1.0-abs(uZoom))*0.25 : (1.0+uZoom*3.0)*0.25;
	vec2 uv =((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE.y- uOffset)/zoom;  // normalize coordinates (origin at center)
	vec3 color = vec3(0.0);
	vec3 camPos = vec3(0.0, 0.0, -4.0);
	vec3 lookPos = vec3(0.0, 0.0, 0.0);
	vec3 forward = normalize(lookPos - camPos);
	vec3 up = vec3(0.0, 1.0, 0.0);
	vec3 right = normalize(cross(up, forward));
	up = normalize(cross(forward, right));
	float fov = 1.0;
	vec3 rayDir = normalize(uv.x * right + uv.y * up + forward * fov);
	vec3 p;
	float df = 0.0;
	float d = 0.0;
	for(int i = 0; i < 64; i++)
		{
		p = camPos + rayDir * d;
		df = distanceFunc(p);
		if(df > 100.0) break;
		if(df <= 0.001) break;
		d += df;
		}
	if(uDisplay == 2) color = background(rayDir);
	else
		{
		if(df <= 0.001)
			{
			vec3 normal = getNormal(p);
			rayDir = refract(rayDir, normal, 0.1);
			if(uDisplay<2) color = mix(color, background(rayDir), smoothstep(0.0, 4.0, d));
			}
		else if(uDisplay == 0) color = mix(color, background(rayDir), smoothstep(0.0, 4.0, d));
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
	gl_FragColor = vec4(cOut.rgb,1.0);	
}
	