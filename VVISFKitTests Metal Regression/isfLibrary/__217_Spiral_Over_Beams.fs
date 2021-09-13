/*
	{
	"DESCRIPTION": "Spiral Over Beams",
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
			"LABEL": "Core Size:",
			"NAME": "uCore",
			"TYPE": "float",
			"MAX": 1,
			"MIN": 0,
			"DEFAULT": 0.25
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
// Import from: http://www.glslsandbox.com/e#71179.0
// by: WonkyKIM


#define PI 3.141592653589
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))


void main()
	{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE - 0.5; // normalize coordinates
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;          // correct aspect ratio
	uv = (uv-uOffset) * 1.0/uZoom;              // zoom at original location, then offset result
/**** Start of Imported Shader Code main() *****/

//	vec2 p = (2.0* gl_FragCoord.xy - RENDERSIZE.xy )/min(RENDERSIZE.x , RENDERSIZE.y);
	vec2 p = uv*1.5;	
	float h = atan(p.y, p.x);
	h = h/PI * 0.5 + 0.5;
	h = sin(h * PI * 32.0);
	
	float d = length(p) - 2.0*uCore;
	//d = 1.0 - d;
	
	float tw = sin((atan(p.y, p.x)/PI*0.5+0.5)*PI*32.0 + d*PI*12.0 + TIME*-30.0);
	
	
	//float c = d+h*0.05;
	//c = smoothstep( 0.01, 0.0, c);

	h = clamp(h, 0.1, 0.9);
	float c = d+h*0.1;

	c = smoothstep( 0.02, 0.0, c);
	
	
	vec3 c1 = vec3(h*0.5,0,0);
	vec3 c2 = vec3(0,d,0);
	vec3 c3 = vec3(0,0,tw);
	
//	gl_FragColor = vec4( c1+c2+c3, 1.0 );
	//gl_FragColor = vec4(c,c,c, 1.0);
/****    End of Imported Shader main()     *****/

	vec4 cShad = vec4( c1+c2+c3, 1.0 );
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
