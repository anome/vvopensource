/*
	{
	"DESCRIPTION": "GridBlend3",
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
			"LABEL": "Wave Amplitude:",
			"NAME": "uWaveA",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Wave Frequency:",
			"NAME": "uWaveF",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Low Resolution Display? ",
			"NAME": "uLowRes",
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
			"MAX": 2.0,
			"MIN": 0,
			"DEFAULT": 1.0
			}
		]
	}
*/
// Import from: https://www.shadertoy.com/view/7ssGRj

#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

mat2 testinverse(mat2 m)
	{
	return mat2(m[1][1],-m[0][1],
		-m[1][0], m[0][0]) / (m[0][0]*m[1][1] - m[0][1]*m[1][0]);
	}
// more grid blending! (hex + tri grid)

vec4 HexGrid(vec2 uv, out vec2 id)
	{
	uv *= mat2(1.1547,0.0,-0.5773503,1.0);
	vec2 f = fract(uv);
	float triid = 1.0;
	if((f.x+f.y) > 1.0)
		{
		f = 1.0 - f;
		triid = -1.0;
		}
	vec2 co = step(f.yx,f) * step(1.0-f.x-f.y,max(f.x,f.y));
	id = floor(uv) + (triid < 0.0 ? 1.0 - co : co);
	co = (f - co) * triid * mat2(0.866026,0.0,0.5,1.0);    
	uv = abs(co);
	id*=testinverse(mat2(1.1547,0.0,-0.5773503,1.0)); // optional unskew IDs
	return vec4(0.5-max(uv.y,abs(dot(vec2(0.866026,0.5),uv))),length(co),co);
	}

// Triangle grid using the skewed, split rectangle method (quicker)
// this version based on fabrices excellent hexagonal tiling tutorial (I wish I'd found this earlier!!)
// https://www.shadertoy.com/view/4dKXR3
vec4 TriGrid(vec2 uv, out vec2 id)
	{
	float scaler = 0.866026;
	uv *= mat2(1,-1./1.73, 0,2./1.73)*scaler;
	vec3 g = vec3(uv,1.-uv.x-uv.y);
	vec3 _id = floor(g)+0.5;
	g = fract(g);
	float lg = length(g);
	if (lg>1.) g = 1.-g;
	vec3 g2 = abs(2.*fract(g)-1.);                  // distance to borders
	vec2 triuv = (g.xy-ceil(1.-g.z)/3.) * mat2(1,.5, 0,1.73/2.);
	float edge = max(max(g2.x,g2.y),g2.z);
	id = _id.xy;
	id*= mat2(1,.5, 0,1.73/2.); // Optional, unskew IDs
	id.xy += sign(lg-1.)*0.1; // Optional tastefully adjust ID's
	return vec4(((1.0-edge)*0.43)/scaler,length(triuv),triuv);
	}

float hbar(vec2 p, float nline, float t)
	{
	return 0.5+sin((p.y*nline)+t)*0.5;
	}

float smin(float a, float b, float k)
	{
	float h = clamp( 0.5 + 0.5*(b-a)/k, 0.0, 1.0 );
	return mix( b, a, h ) - k*h*(1.0-h);
	}


void main()
	{
#ifdef XL_SHADER // These three lines must be removed for versions of xLights above
  discard;       // 2021.12.  There is no guarantee they will function correctly after
#endif           // that, as xLights started modifying the shader coordinate system.

	vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;  // normalized, aspect corrected coordinates
	uv = (uv-uOffset) * 1.0/uZoom;              // zoom at original location, then offset result
	uv = uContRot ? uv*rotate2D(TIME*uRotate/180.0) : uv*rotate2D(uRotate*PI/180.0); // rotation
	uv.xy *= 1.0+sin(uWaveF*5.0*TIME+uv.y+(uv.x*uWaveF*5.0))*uWaveA/2.0;

	float t = TIME;

	// dirty grid blending attempt #3
	vec2 id;
	vec2 id2;
	float zoom = 8.0;

	vec4 h = HexGrid(uv*zoom, id);
	vec4 h2 = TriGrid(uv*zoom, id2);
	h.x = smin(h.x,h2.x,0.215); // blend distance
	id = mix(id,id2,0.5); // blend IDs
	vec3 bordercol = vec3(0.9,0.9,0.7);
	vec3 shapecol = vec3(0.41,0.32,0.15);//vec3(0.25,0.32,0.15);
	if(uColMode==1)
		{
		shapecol = uC1.rgb;
		bordercol = uC2.rgb;
		}
	// just do a simple patterned shape tint based on (blended) cell IDs
	float patternVal = 7.75; // 33.5
	float cm = 1.0;
	if (!uLowRes) cm = 1.0 + pow(sin(length(id)*patternVal + t*0.65), 4.0);	// pulse mult
	cm *= 1.0 + (hbar(h.zw,100.0,t*12.0)*0.1);					// bars mult
	shapecol *= cm;

	// Output to screen
	vec3 finalcol = mix(vec3(0.0),shapecol,smoothstep(0.0, 0.035, h.x-0.035)); // black outline edge
	float vv = smoothstep(0.0, 0.055, h.x);
	finalcol = mix(bordercol,finalcol,vv); // white edge
	finalcol *= uIntensity;
	gl_FragColor = vec4(finalcol,1.0);
	}
