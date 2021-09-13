/*
	{
	"DESCRIPTION": "Aurora_xL",
	"CATEGORIES": 
		[
		"generator",
		"Optimized for xLights"
		],
	"ISFVSN": "2",
	"CREDIT": "Optimized for xLights by: Old Salt",
	"VSN": "1.0",
	"INPUTS":
		[
			{
			"LABEL": "Center: ",
			"NAME": "offset",
			"TYPE": "point2D",
			"MAX":
				[
				1.0,
				1.0
				],
			"MIN":
				[
				0.0,
				0.0
				],
			"DEFAULT": 
				[
				0.5,
				0.5
				]
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Default                  ",
				"xLights Color Pallete (1 used)  "
				],
			"NAME": "colorMode",
			"TYPE": "long",
			"VALUES":
				[
				0,
				1
				],
			"DEFAULT": 0
			},
			{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT":
				[
				1.0,
				1.0,
				0.0,
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
// original: http://www.glslsandbox.com/e#58544.0
// ISF conversion by: Old Salt

#ifdef GL_ES
precision mediump float;
#endif

#define clock TIME

vec2 rotz(vec2 p, float ang)
	{
	return vec2(p.x*cos(ang)-p.y*sin(ang),p.x*sin(ang)+p.y*cos(ang));
	}

void main(void)
	{
	vec2 OffsetCoord = (gl_FragCoord.xy/RENDERSIZE.xy - offset + 0.5)*RENDERSIZE.xy;
	vec2 p = (OffsetCoord * 2.0 - RENDERSIZE) / min(RENDERSIZE.x, RENDERSIZE.y); 
	float d = 2.*length(p);
	vec3 col = vec3(0); 
	p = rotz(p, clock*0.5+atan(p.x,p.y)*8.0);
	for (int i = 0; i < 18; i++)
		{
		float dist = abs(p.y + sin(float(i)+clock*0.3+3.0*p.x)) - 0.2;
		if (dist < 1.0)
			{
			col += (1.0-pow(abs(dist), 0.28))*vec3(0.8+0.2*sin(clock),0.9+0.1*sin(clock*1.1),1.2);
			}
		p *= 0.99/d; 
		p = rotz(p, 30.0);
		}
	col *= 0.49;
	col = col-d-0.4;
	if (colorMode == 1) col = col * color1.rgb;
	col = clamp((col* 2.0 * intensity), 0.0, 1.0);
	gl_FragColor = vec4(col, 1.0); 
	}