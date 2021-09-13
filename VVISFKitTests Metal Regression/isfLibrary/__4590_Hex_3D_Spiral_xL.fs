/*
	{
	"DESCRIPTION": "Hex 3D Spiral_xL",
	"CATEGORIES": 
		[
		"generator",
		"Optimized for xLights"
		],
	"ISFVSN": "2",
	"CREDIT": "Modified and optimized for xLights by: Old Salt",
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
			"LABEL": "Twist: ",
			"NAME": "twst",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Outline: ",
			"NAME": "outline",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
			},
			{
			"LABEL": "Reverse: ",
			"NAME": "reverse",
			"TYPE": "bool",
			"DEFAULT": 0
			},
			{
			"LABEL": "Color Mode: ",
			"LABELS":
				[
				"Shader Default                  ",
				"xLights Color Pallete (2 used)  "
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
// original: http://www.glslsandbox.com/e#72103.0
// ISF conversion by: Old Salt


#ifdef GL_ES
precision mediump float;
#endif

#define clock TIME
#define rotate2D(a) mat2(cos(a),-sin(a),sin(a),cos(a))

void main()
	{
	vec2 p;
	float c = 0.0;
	vec3 color;
	float twist = twst * 4.0;
	float outthick = 1.0-(0.78 * outline + 0.1);
	
	if (reverse) twist = -1.0 * twist;
	vec2 OffsetCoord = (gl_FragCoord.xy/RENDERSIZE.xy - offset + 0.5)*RENDERSIZE.xy;
	vec2 uv=(OffsetCoord.xy-.5*RENDERSIZE)/RENDERSIZE.y;
	float i,g,d=1.;
	for(float j=0.;j<128.;j++) 
		{
		++i;
		if (d<=.001) break;
		p=uv*g+vec2(.3)*rotate2D(g*twist);
		g+=d=-(length(p)-2.+g/9.)/2.;
		}
	p=vec2(atan(p.x,p.y),g)*8.28+clock*2.;
	p=abs(fract(p+vec2(0,.5*ceil(p.x)))-.5);
	c+=30./i-.5/smoothstep(outthick, 0.9, 1.0 -abs(max(p.x*1.5+p.y,p.y*2.)-1.));
	if (colorMode == 0) color = vec3(c/4.0);
	else color = (c * color1.rgb) + ((1.0-c)* color2.rgb);
	color = clamp((color * 8.0 * intensity), 0.0, 1.0);
	gl_FragColor = vec4(color,1.0);
	}