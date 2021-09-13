/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"fire"
	],
	"INPUTS": [
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 6,
			"MIN": 2,
			"MAX": 16
		},
		{
			"NAME": "loops",
			"TYPE": "float",
			"DEFAULT": 5,
			"MIN": 4,
			"MAX": 12
		},
		{
			"NAME": "intensity",
			"TYPE": "float",
			"DEFAULT": 0.04,
			"MIN": 0.025,
			"MAX": 0.075
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 3.3,
			"MIN": 2.0,
			"MAX": 5.0
		},
			{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 2,
			"MIN": -5,
			"MAX": 5
		},
		{
			"NAME": "shift",
			"TYPE": "point2D",
			"DEFAULT": [
				20,
				20
			],
			  "MAX" : [
        		30,
        		30
      		],
      		"MIN" : [
        		10,
        		10
      		]
		}
	]
}*/

#ifdef GL_ES
precision mediump float;
#endif

// TurbulentFlames by mojovideotech

void main() 
{
	vec2 p = (gl_FragCoord.xy / RENDERSIZE * scale) - shift;
	vec2 i = p;
	float c = 1.0;
	float bc = 0.0;
	for (int n = 0; n < 16; n++)
	{
		int bpc = int(floor(loops*2.5));
        bpc -= n;
        if (bpc<1) break;
		float t = -TIME * (1.5 - (2.0 / float(n+1))) *rate;
		i = p + vec2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(vec2(p.x / (2.*sin(i.x+t)/intensity),p.y / (cos(i.y+t)/intensity)));
	}
	c /= float(18.-loops);
	c = 1.5-sqrt(pow(c,brightness));
	float col = c*c*c*c;
	gl_FragColor = vec4(vec3(col * 1.2, col * 0.5, col * 0.1), 1.0);
}