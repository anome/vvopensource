/*
	{
	"DESCRIPTION": "Fire Within",
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
			"LABEL": "View Point: ",
			"NAME": "uView",
			"TYPE": "point2D",
			"MAX": [1.0,1.0],
			"MIN": [-1.0,-1.0],
			"DEFAULT": [0.0,0.0]
			},
			{
			"LABEL": "Zoom: ",
			"NAME": "uZoom",
			"TYPE": "float",
			"MAX": 4.0,
			"MIN": 0.01,
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
// Import from: https://www.shadertoy.com/view/tttfzl

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

// Imported Shader defined uniforms and functions:
#define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST .001

float Hash21(vec2 p) 
	{
	p = fract(p*vec2(123.34,233.53));
	p += dot(p, p+23.234);
	return fract(p.x*p.y);
	}

float sdBox(vec3 p, vec3 s)
	{
	p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, max(p.y, p.z)), 0.);
	}

float sdGyroid(vec3 p,float scale, float thickness, float bias)
	{
	p*=scale;
	// can also play with the p and p.zxy by adding some numbers 
	// scale needs to add the largest multiplers in either p or p.zxy 
	// return abs(dot(sin(p*2.),cos(p.zxy*1.23))-bias)/(scale*2.)-thickness;
	return abs(dot(sin(p),cos(p.zxy))-bias)/scale-thickness;
	}
    
vec3 Transform(vec3  p)
	{
	p.xy*=rotate2D(p.z*.15);
	p.z -=TIME*.1;
	p.y -= .3;
	return p;
	}

float GetDist(vec3 p)
	{
	p=Transform(p);
	float box = sdBox(p, vec3(1));

	// wanting something organic? the scales for two should not be multiplicable by each other (ex.: can't be 4 and 8)
	float g1 = sdGyroid(p, 5.23, .03, 1.4);
	float g2 = sdGyroid(p, 10.76, .03, .3);
	float g3 = sdGyroid(p, 20.76, .03, .3);
	float g4 = sdGyroid(p, 35.76, .03, .3);
	float g5 = sdGyroid(p, 60.76, .03, .3);
	float g6 = sdGyroid(p, 110.76, .03, .3);
	float g7 = sdGyroid(p, 210.76, .03, .3);

	//float g = max(g1,g2);//union 
	//float g = max(g1,-g2);//subtraction 
	//bump mapper
	g1 -= g2*.4;
	g1 -= g3*.3;
	g1 += g4*.2;
	g1 += g5*.2;
	g1 += g6*.3;
	g1 += g7*.5;

	//float d=max(box,g1*.8);// intersect the box 
	float d = g1*.8;
	return d;
	}

float RayMarch(vec3 ro, vec3 rd)
	{
	float dO=0.;
	for(int i=0; i<MAX_STEPS; i++)
		{
		vec3 p = ro + rd*dO;
		float dS = GetDist(p);
		dO += dS;
		if(dO>MAX_DIST || abs(dS)<SURF_DIST) break;
		}
		return dO;
	}

vec3 GetNormal(vec3 p)
	{
	float d = GetDist(p);

	// the e.x determines the smoothness
	vec2 e = vec2(.025, 0);
	vec3 n = d - vec3(
		GetDist(p-e.xyy),
		GetDist(p-e.yxy),
		GetDist(p-e.yyx));
	return normalize(n);
	}

vec3 GetRayDir(vec2 uv, vec3 p, vec3 l, float z)
	{
	vec3 f = normalize(l-p),
		r = normalize(cross(vec3(0,1,0), f)),
		u = cross(f,r),
		c = f*z,
		i = c + uv.x*r + uv.y*u,
		d = normalize(i);
	return d;
	}

vec3 Background(vec3 rd)
	{
	float t = TIME*.2;
	vec3 col = vec3(0);
	vec3 fireCol = vec3(1,.45,.1);
	float y = rd.y*.5+.5;
	col+=(1.-y)*fireCol;

	//make flames
	//smoothstep to make the top dark and bottom bright 
	float a = atan(rd.x,rd.z);
	float flames = sin(a*10.+t)*sin(a*7.-t)*sin(a*3.);
	flames *=smoothstep(0.8,0.6,y);
	col*=flames;
	col = max(col,0.);
	col+=smoothstep(.3,.0,y);
	return col;
	}


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv *= (1.0/uZoom);              // offset and zoom functions
	uv = uContRot ? uv*rotate2D(TIME*uRotate/36.0) : uv*rotate2D(uRotate*PI/180.0); // rotation

/**** Start of Core Imported Shader Code *****/
	vec2 m = (uView+1.0)/2.0;  //	iMouse.xy/iResolution.xy;
	float t = TIME*0.2;
	vec3 col = vec3(0);
	vec3 fireCol = vec3(1,.45,.1);

	//distort effect 
	uv +=sin(uv*20.+t)*.01;
	vec3 ro = vec3(0, 0, -1);
	ro.yz *= rotate2D(-m.y*3.14+1.);
	ro.xz *= rotate2D(-m.x*6.2831);

	vec3 lookat = vec3(0,0,0);
	//zoom factor 1
	vec3 rd = GetRayDir(uv, ro, lookat, .8);
	float d = RayMarch(ro, rd);
	if(d<MAX_DIST)
		{
		vec3 p = ro + rd * d;
		vec3 n = GetNormal(p);
		float height = p.y;
		p = Transform (p);
		float dif = n.y*.5+.5;
		col+=dif*dif*dif;// 0 <> 1 

		//ambient occulusion - cheaper 
		float g2 = sdGyroid(p, 10.76, .03, .3);
		col*=smoothstep(-.1,.05,g2);//blackening 
		float crackWidth = -.02+smoothstep(0., -.5, n.y)*.04;
		float cracks = smoothstep(crackWidth,-.025,g2);
		float g3 = sdGyroid(p+t*.1, 5.76, .03, .0);
		float g4 = sdGyroid(p-t*.05, 4.76, .03, .0);
		cracks *=g3*g4*20.+.1*smoothstep(.2,.0,n.y);
		col+=cracks*fireCol*3.;
		float g5 = sdGyroid(p-vec3(0,t,0),1.85,.02,1.3);
		col+=g5*fireCol;
		col +=smoothstep(0.,-2.,height)*fireCol;
		}
	col = mix(col, Background(rd), smoothstep(0., 7., d));
	//col = Background(rd);
	col*=1.-dot(uv,uv);

/****  End of Core Imported Shader Code  *****/
	vec4 cShad = vec4(col,1.0);  
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
	