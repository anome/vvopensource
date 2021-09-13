/*{
	"CREDIT": "by mojovideotech",
 "CATEGORIES" : [
    "procedural",
    "2d",
    "noise",
    "sunset",
    "clouds"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  {
      "NAME" : "offset",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "MIN" : [
        -1,
        -1
      ]
    },
    	{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -2,
			"MAX": 2
		},
			{
			"NAME": "sizeX",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 2
		},
			{
			"NAME": "sizeY",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.5,
			"MAX": 2
		},
    		{
			"NAME": "detail",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 1.25
		},
		{
			"NAME": "density",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 2
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 8.5,
			"MIN": 0,
			"MAX": 9
		}
  ]
}
*/

// SkySunClouds by mojovideotech
// based on:
// www.shadertoy.com/view/XlsXDB by miloszmaki

#define saturate(x) clamp(x,0.,1.)
#define rgb(r,g,b) (vec3(r,g,b)/255.)

float frash(float x) { return fract(sin(x) * 71523.5413291); }

float dash(vec2 x) { return frash(dot(x, vec2(13.4251, 15.5128))); }

float noise(vec2 x)
{
    vec2 i = floor(x);
    vec2 f = x - i;
    f *= f*(3.-2.*f);
    return mix(mix(dash(i), dash(i+vec2(1,0)), f.x),mix(dash(i+vec2(0,1)), dash(i+vec2(1,1)), f.x), f.y);
}

float fbm(vec2 x)
{
    float r = 0.0, s = 1.0, w = 1.0;
    for (int i=0; i<5; i++)
    {
        s *= 2.0;
        w *= 0.5;
        r += w * noise(s * x);
    }
    return r;
}

float cloud(vec2 uv, float scalex, float scaley, float _density, float _sharpness, float _speed)
{
    return pow(saturate(fbm(vec2(scalex/sizeX,scaley/sizeY)*(uv+vec2(_speed*rate,0)*TIME))-(1.0-_density*density)), 1.0-_sharpness*detail);
}

vec3 render(vec2 uv)
{
    vec3 color = mix(rgb(255,212,166), rgb(204,235,255), uv.y);
    vec2 spos = uv - vec2(offset);
    float sun = exp(-20.*dot(spos,spos));
    vec3 scol = rgb(255,155,102) * sun * 0.7;
    color += scol;
    vec3 cl1 = mix(rgb(151,138,153), rgb(166,191,224),uv.y);
    float d1 = mix(0.9,0.1,pow(uv.y, 0.7));
    color = mix(color, cl1, cloud(uv,2.,8.,d1,0.4,0.04));
    color = mix(color, vec3(0.9), 8.*cloud(uv,14.,18.,0.9,0.75,0.02) * cloud(uv,2.,5.,0.6,0.15,0.01)*uv.y);
    color = mix(color, vec3(0.8), 5.*cloud(uv,12.,15.,0.9,0.75,0.03) * cloud(uv,2.,8.,0.5,0.0,0.02)*uv.y); 
    color *= vec3(1.0,0.93,0.81)*1.04;
    color = mix(0.75*rgb(255,205,161), color, smoothstep(-0.1,0.3,uv.y));
    color = pow(color,vec3(10.-brightness));
    return color;
}

void main()
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    uv.x -= 0.5;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
	gl_FragColor = vec4(render(uv),1.0);
}