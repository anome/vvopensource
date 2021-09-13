/*
	{
	"DESCRIPTION": "Spirograph Spectrum",
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
			"LABEL": "Size: ",
			"NAME": "uSize",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Center Size: ",
			"NAME": "uCenter",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 0.1
			},
			{
			"LABEL": "Multiplier: ",
			"NAME": "uMult",
			"TYPE": "float",
			"MAX": 8.0,
			"MIN": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Repeats: ",
			"NAME": "uReps",
			"TYPE": "float",
			"MAX": 10.0,
			"MIN": 1.0,
			"DEFAULT": 1.0
			},
			{
			"LABEL": "Repeat Shift: ",
			"NAME": "uShift",
			"TYPE": "float",
			"MAX": 1.0,
			"MIN": 0.0,
			"DEFAULT": 0.1
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
// Import from: https://www.shadertoy.com/view/lddGD4
// By Xor

#define PI 3.1415926535897932384626433832795

void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	uv = (uv-uOffset);

/**** Start of Imported Shader Code main() *****/
	float oSize = uSize * 0.5;
	float cSize = uCenter * oSize;
	float mult = floor(uMult)/8.0;
	float reps = floor(uReps);
	float shift = uShift;

	vec3 col1 = vec3(0.0);
	float len = length(uv);
	float aShift = shift/(14.0*reps);
	for(float i = 0.0;i<11.0;i+=1.0)
		{
		if (i>=reps) break;
		float sAng = i * aShift;
		float ang = fract(atan(-uv.y,-uv.x)/atan(1.0)*mult+0.5) + sAng+TIME*0.1;

		for(float j = 0.0;j<11.0;j+=1.0)
			{
			float tang = ((ang+j)*PI*2.0);
			float tlen = (cos((ang+j)*8.0)*0.5+0.5)*(oSize-cSize)+cSize;
			vec2 pos = normalize(uv)*tlen;
			col1 += smoothstep(0.005,0.0,distance(uv,pos)*pow(length(uv),0.25))
				*((cos(tang+vec3(0.0,2.0*PI,4.0*PI)/3.0)*0.5+0.5));
			}
		}

/****    End of Imported Shader main()     *****/

	vec4 cShad = vec4(col1,1.0);  
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
