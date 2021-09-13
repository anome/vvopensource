/*
	{
	"DESCRIPTION": "Wispy Background",
	"CATEGORIES": 
		[
		"generator"
		],
	"ISFVSN": "2",
	"CREDIT": "ISF conversion, and modifications by: Old Salt",
	"VSN": "1.0",
	"INPUTS": 
		[
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Default                  ",
				"Alternate Color Palette (3 used) "
				],
			"NAME": "colorMode",
			"TYPE": "long",
			"VALUES":
				[0,1],
			"DEFAULT": 0
			},
			{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT":
				[
				1.0,
				0.0,
				0.0,
				1.0
				]
			},
			{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": 
				[
				0.0,
				1.0,
				0.0,
				1.0
				]
			},
			{
			"NAME": "color3",
			"TYPE": "color",
			"DEFAULT": 
				[
				0.0,
				0.0,
				1.0,
				1.0
				]
			},			
			{
			"LABEL": "Intensity: ",
			"NAME": "intensity",
			"TYPE": "float",
			"MAX": 1,
			"MIN": 0,
			"DEFAULT": 0.5
			}
    ]
	}
*/

// Original: http://www.glslsandbox.com/e#71643.0
// Converted to ISF by: Old Salt 2021/3/12
// Wispy Background

#define TAU 7.28318530718
#define MAX_ITER 10

void main()
	{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 p = mod(uv*TAU, TAU)-250.0;
	vec2 i = vec2(p);
	float c = .5;
	float inten = .005;
	for (int n = 0; n < MAX_ITER; n++)
		{
		float t = 0.16*(TIME+23.0) * (1.0 - (3.5 / float(n+1)));
		i = p + vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(vec2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
		}
	c /= float(MAX_ITER);
	c = 1.0-pow(c, 2.0);
	vec3 colour = vec3(pow(abs(c), 12.0));
	colour = clamp(colour, 0.0, 1.0);
	vec3 tint = vec3(uv.x, uv.y, (1.0 - uv.x) * (1.0 - uv.y) );
	// gl_FragColor = vec4(colour * tint, 1.0);
	// allow use of Alternate Color Palette (3 used)  or shader defaults
	colour = colour * tint;
	if (colorMode == 1)
		{
		// Alternate Color Palette (3 used) 
		vec3 d = color1.rgb * colour.r;
		d = d + color2.rgb * colour.g;
		d = d + color3.rgb * colour.b;
		colour = clamp(d, 0.0, 1.0);
		}
	// added intensity control (0 - 10x original)
	gl_FragColor = vec4(colour * 10.0 * intensity, 1.0);
	}