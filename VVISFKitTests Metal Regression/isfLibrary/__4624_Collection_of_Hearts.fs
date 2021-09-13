/*
	{
	"DESCRIPTION": "Collection of Hearts",
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
			"LABEL": "Style: ",
			"LABELS":
				[
				"Heart 2D ",
				"Heart Beat 3D ",
				"I love you all ",
				"Into You ",
				"Growing Hearts ",
				"Ribbon Heart - Single Color - Fixed ",
				"Ribbon Heart - Single Color - Drawn ",
				"Ribbon Heart - Multi Color - Fixed ",
				"Ribbon Heart - Multi Color - Drawn "
				],
			"NAME": "uStyle",
			"TYPE": "long",
			"VALUES": [0,1,2,3,5,6,7,8,9],
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
// Imported from various sources, credit included with style


#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


// ***** functions for Heart Beat 3D
// http://mathworld.wolfram.com/HeartSurface.html
float f(vec3 p)
	{
	vec3 pp = p * p;
	vec3 ppp = pp * p;
	float a = pp.x + 2.25 * pp.y + pp.z - 1.0;
	return a * a * a - (pp.x + 0.1125 * pp.y) * ppp.z;
	}

// Bisection solver for y
float h(float x, float z)
	{
	float a = 0.0, b = 0.75, y = 0.5;
	for (int i = 0; i < 10; i++)
		{
		if (f(vec3(x, y, z)) <= 0.0)a = y;
		else b = y;
		y = (a + b) * 0.5;
		}
	return y;
	}

// Analytical gradient
// (-2 x z^3+6 x (-1.+x^2+2.25 y^2+z^2)^2) 
// (-0.225 y z^3+13.5 y (-1.+x^2+2.25 y^2+z^2)^2)
// (z (-3 x^2 z-0.3375 y^2 z+6 (-1.+x^2+2.25 y^2+z^2)^2))
vec3 normal(vec2 p)
	{
	vec3 v = vec3(p.x, h(p.x, p.y), p.y);
	vec3 vv = v * v;
	vec3 vvv = vv * v;
	float a = -1.0 + dot(vv, vec3(1, 2.25, 1));
	a *= a;
	return normalize(vec3(
		-2.0 * v.x * vvv.z +  6.0 * v.x * a,
		-0.225 * v.y * vvv.z + 13.5 * v.y * a,
		v.z * (-3.0 * vv.x * v.z - 0.3375 * vv.y * v.z + 6.0 * a)));
	}

// ***** functions for I love you all
// Heart curve suggested by IQ, improved by Dave_Hoskins.
float heart(vec2 p)
	{
	// Center it more, vertically:
	p.y += .6;
	// This offset reduces artifacts on the center vertical axis.
	const float offset = .3;
	// (x^2+(1.2*y-sqrt(abs(x)))^2−1)
	float k = 1.2 * p.y - sqrt(abs(p.x) + offset);
	return p.x * p.x + k * k - 1.;
	}

// Gradient of heart function.
vec2 grad(vec2 p)
	{
	vec2 h = vec2(0.01, 0.0);
	return vec2(heart(p + h.xy) - heart(p - h.xy),
		heart(p + h.yx) - heart(p - h.yx)) / (2.0 * h.x);
	}

// Return 0-1 scalar based on abs distance from heart line.
float color(vec2 p, float vol)
	{
	float v = heart(p);
	vec2  g = grad(p);
	float de = abs(v) / length(g);
	// Thickness: vary with volume
	float eps = 30.0/RENDERSIZE.x;
	return smoothstep(1.0 * eps, 2.0 * eps, de);
	}

// from https://github.com/hughsk/glsl-hsv2rgb/blob/master/index.glsl
vec3 hsv2rgb(vec3 c)
	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
	}

vec3 heart1(vec2 p, int i, float vol)
	{
	vec2 offset = vec2(0.);
	float scale = .7;
	float a;
	vec3 hue;
	if (i==0)  // no 'switch' in ISF
		{
		hue = vec3(1., .3, .3); 
		a = PI * 0. - TIME * .3; 
		offset = vec2(cos(a), sin(a)) * .03;
		}
	else if (i==1)
		{
		hue = vec3(.6, .6, 1.); 
		a = PI * 1. - TIME * .3; 
		offset = vec2(cos(a), sin(a)) * .03;
		}
	else
		{
		hue = hsv2rgb(vec3(float(i) / 5. + TIME * .2, .4, .9));
		a = PI * (float(i) - 2.) * (2. / 5.) + TIME * .3; 
		offset = vec2(cos(a), sin(a)) * .2;
		scale = .3 + (float(i) * .05);
		}
	vec2 q = (p + offset) / (scale * (1.0 + vol * .3) * .8);
	float brightness = 1. - color(q * 10., vol);
	return brightness * hue;
	}

// ***** functions & defines for Into You
#define S(a, b, t) smoothstep(a, b, t)
#define sat(x) clamp(x, 0., 1.)
#define HEARTCOL vec3(1., .01, .01)
#define NUM_HEARTS 75.
#define LIGHT_DIR vec3(.577, -.577, -.577)

// Polynomial smooth max from IQ
float smax( float a, float b, float k )
	{
	float h = sat( .5 + .5*(b-a)/k );
	return mix( a, b, h ) + k*h*(1.-h);
	}

// Quaternion rotation functions from Ollj
vec4 qmulq(vec4 q1, vec4 q2){return vec4(q1.xyz*q2.w+q2.xyz*q1.w+cross(q1.xyz,q2.xyz),(q1.w*q2.w)-dot(q1.xyz,q2.xyz));}
vec4 aa2q(vec3 axis, float angle){return vec4(normalize(axis)*sin(angle*0.5),cos(angle*0.5));}
vec4 qinv(vec4 q){return vec4(-q.xyz,q.w)/dot(q,q);}
vec3 qmulv(vec4 q, vec3 p){return qmulq(q,qmulq(vec4(p,.0),qinv(q))).xyz;}

vec2 RaySphere(vec3 rd, vec3 p)
	{
	float l = dot(rd, p);
	float det = l*l - dot(p, p) + 1.;
	if (det < 0.) return vec2(-1);
	float sd = sqrt(det);
	return vec2(l - sd, l+sd);
	}

struct sphereInfo
	{
	vec3 p1, p2, n1, n2;
	vec2 uv1, uv2;
	};

sphereInfo GetSphereUvs(vec3 rd, vec2 i, vec2 rot, vec3 s)
	{
	sphereInfo res;
	rot *= 6.2831;
	vec4 q = aa2q(vec3(cos(rot.x),sin(rot.x),0), rot.y);
	vec3 o = qmulv(q, -s)+s;
	vec3 d = qmulv(q, rd);
	res.p1 = rd*i.x;
	vec3 p = o+d*i.x-s;
	res.uv1 = vec2(atan(p.x, p.z), p.y);
	res.n1 = res.p1-s;
	res.p2 = rd*i.y;
	p = o+d*i.y-s;
	res.uv2 = vec2(atan(p.x, p.z), p.y);
	res.n2 = s-res.p2;
	return res;
	}

float Heart(vec2 uv, float b)
	{
	uv.x*=.5;
	float shape = smax(sqrt(abs(uv.x)), b, .3*b)*.5;
	uv.y -= shape*(1.-b);
	return S(b, -b, length(uv)-.5);
	}

vec4 HeartBall(vec3 rd, vec3 p, vec2 rot, float t, float blur)
	{
	vec2 d = RaySphere(rd, p);
	vec4 col = vec4(0);
	if(d.x>0.)
		{
		sphereInfo info = GetSphereUvs(rd, d, rot, p);
		float sd = length(cross(p, rd));
		float edge =  S(1., mix(1., 0.1, blur), sd);
		float backMask = Heart(info.uv2, blur)*edge; 
		float frontMask = Heart(info.uv1, blur)*edge; 
		float frontLight = sat(dot(LIGHT_DIR, info.n1)*.8+.2);
		float backLight = sat(dot(LIGHT_DIR, info.n2)*.8+.2)*.9;
		col = mix(vec4(backLight*HEARTCOL, backMask), 
			vec4(frontLight*HEARTCOL, frontMask), 
			frontMask);
		}
	return col;
	}
	
/***** Start of Imported Shaders  *****/		
void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.
	vec2 p = ((gl_FragCoord.xy-RENDERSIZE*0.5)/RENDERSIZE - uOffset)* RENDERSIZE/(RENDERSIZE.y * uZoom);
	p = uContRot ? p*rotate2D(TIME*uRotate/36.0) : p*rotate2D(uRotate*PI/180.0);    // rotate
	vec4 col;

//***** Heart 2D
// Import from: https://www.shadertoy.com/view/XsfGRn
// Created by: inigo quilez - iq/2013
	if(uStyle==0)
		{
		p*=1.5; //scale to fit
		float tt = mod(TIME,1.5)/1.5;
		float ss = pow(tt,.2)*0.5 + 0.5;
		ss = 1.0 + ss*0.5*sin(tt*6.2831*3.0 + p.y*0.5)*exp(-tt*4.0);
		p *= vec2(0.5,1.5) + ss*vec2(0.5,-0.5);
		p *= 0.8;
		p.y = -0.1 - p.y*1.2 + abs(p.x)*(1.0-abs(p.x));
		float r = length(p);
		float d = 0.5;
		float s = 0.75 + 0.75*p.x;
		s *= 1.0-0.4*r;
		s = 0.3 + 0.7*s;
		s *= 0.5+0.5*pow( 1.0-clamp(r/d, 0.0, 1.0 ), 0.1 );
		vec3 hcol = vec3(1.0,0.4*r,0.3)*s;
		col = vec4(mix(vec3(0.0), hcol, smoothstep( -0.01, 0.01, d-r)),1.0);
		}

//***** Heart Beat 3D
// Import from: https://www.shadertoy.com/view/XtXGR8
// Created by: miloyip
	if(uStyle==1)
		{
		float s = sin(TIME * 5.0);
		s *= s;
		s *= s;
		s *= 0.1;
		vec3 ph = vec3(p*2.0, 0.0);
		vec3 tp = ph * vec3(1.0 + s, 1.0 - s, 0.0) * 2.0;
		vec3 c;
		if (f(tp.xzy) <= 0.0)
			{
			vec3 n = normal(tp.xy);
			float diffuse = dot(n, normalize(vec3(-1, 1, 1))) * 0.5 + 0.5;
			float specular = pow(max(dot(n, normalize(vec3(-1, 2, 1))), 0.0), 64.0);
			float rim = 1.0 - dot(n, vec3(0.0, 1.0, 0.0));
			c = diffuse * vec3(1.0, 0, 0) + specular * vec3(0.8) + rim * vec3(0.5);
			}
		else c = vec3(0.0);
		col = vec4(c, 1.0);
		}

//***** I love you all
// Import from: https://www.shadertoy.com/view/XdcyW8
// Created by: huttarl
	if(uStyle==2)
		{
		vec2 uv = p;
		vec3 hcol = vec3(0.);
		float vol = 1.0;
		for (int i = 0; i < 7; i++)
			{
			hcol += heart1(uv, i, vol);
			}
		col = vec4(hcol, 1.0);
		}

//***** Into You
// Import from: https://www.shadertoy.com/view/4sccWr
// Created by: BigWIngs
	if(uStyle==3)
		{
		vec3 rd = normalize(vec3(2.*p, 1));
		float t = TIME*.3;
		vec2 rot = t*vec2(.12, .18);
		for(float i=0.; i<1.; i+=(1./NUM_HEARTS))
			{
			float x = (fract(cos(i*536.3)*7464.4)-.5)*15.;
			float y = (fract(-t*.2+i*7.64)-.5)*15.;
			float z = mix(14., 2., i);
			float blur = mix(.03, .35, S(.0, .4, abs(.4-i)));
			rot += (fract(sin(i*vec2(536.3, 23.4))*vec2(764.4, 987.3))-.5);
			vec4 heart = HeartBall(rd, vec3(x, y, z), rot, t, blur);
			col = mix(col, heart, heart.a);
			col = vec4(col.rgb,1.0);
			}
		}

//***** Paper Kaleidoscope
// Import from: https://www.shadertoy.com/view/ls3GRr
	if(uStyle==4)
		{
		}//maybe someday

//***** Expanding Hearts
// Import from: http://glslsandbox.com/e#69360.0
	if(uStyle==5)
		{
		p.y += exp(-abs(p.x)) - 1.;
		col = vec4(pow(sin(10.*sqrt(pow(p.x,2.0) + pow(p.y,2.0)) - 2.*TIME),2.0), 0.0, 0.0, 1.0);
		}
	
//***** Single and Multi Color Ribbons, Fixed or Drawn
// Import from: http://glslsandbox.com/e#68952.0
//         and: http://glslsandbox.com/e#68956.0
	if(uStyle>5)
		{
		vec2 uv = p*vec2(7.0, 8.0);
		float ax = abs(uv.x);
		float lo = acos(1.0 - ax) - PI;
		float up = (1.0 - (ax - 1.0)*(ax - 1.0));
		float y = mix(lo, sqrt(up), step(0.0, uv.y));
		float a;
		if((uStyle==6)||(uStyle==8)){a = step(atan(-uv.y, ax), 2.0);}
		if((uStyle==7)||(uStyle==9)){a = step(1.5+atan(-uv.y, ax), mod(TIME,4.5));}
		if(uStyle<8) {col = vec4(step(abs(uv.y - y), 0.2)*a, 0.0, 0.0, 1.0);}
		else {col = vec4(step(abs(uv.y - y*1.1), 0.2)*a, step(abs(uv.y - y), 0.2)*a, step(abs(uv.y - y*1.1), 0.3)*a, 1.0);}
		}

// Color replacement:
	vec4 cShad = col;  
	vec3 cOut = cShad.rgb;
	if (uColMode == 1)
		{
		cOut = uC1.rgb * cShad.r;
		cOut += uC2.rgb * cShad.g;
		cOut += uC3.rgb * cShad.b;
		}
	cOut = cOut * uIntensity;
	gl_FragColor = vec4(cOut.rgb,cShad.a);
	}
	