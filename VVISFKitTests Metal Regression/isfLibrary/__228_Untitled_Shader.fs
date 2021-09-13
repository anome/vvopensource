
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "windSpeed",
			"TYPE": "float",
			"DEFAULT": 20.0,
			"MIN": 0.0,
			"MAX": 80.0
		},
		{
			"NAME": "sunPos",
			"TYPE": "point2D",
			"DEFAULT": [
				0.7,
				0.7
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

#define M_PI 3.14159265358979323846

float rand (vec2 co)
{
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float rand (vec2 co, float l)
{
    return rand(vec2(rand(co), l));
}

float rand (vec2 co, float l, float t)
{
    return rand(vec2(rand(co, l), t));
}

float perlin(vec2 p, float dim, float time)
{
	vec2 pos = floor(p * dim);
	vec2 posx = pos + vec2(1.0, 0.0);
	vec2 posy = pos + vec2(0.0, 1.0);
	vec2 posxy = pos + vec2(1.0);
	float c = rand(pos, dim, time);
	float cx = rand(posx, dim, time);
	float cy = rand(posy, dim, time);
	float cxy = rand(posxy, dim, time);
	vec2 d = fract(p * dim);
	d = -0.5 * cos(d * M_PI) + 0.5;
	float ccx = mix(c, cx, d.x);
	float cycxy = mix(cy, cxy, d.x);
	float center = mix(ccx, cycxy, d.y);
	return center * 2.0 - 1.0;
}

void main()
{
    bool hasCloud = false;
    vec2 pos = gl_FragCoord.xy / RENDERSIZE.y;
    float halfTime = TIME / 256. * windSpeed;
	vec4 color = vec4(vec3(0), 1);
	float noise = 16. * perlin((pos + vec2(halfTime)) * 1., 10., 0.) + 8. * perlin((pos + vec2(halfTime)) * 2., 10., 0.) + 4. * perlin((pos + vec2(halfTime)) * 4., 10., 0.) + 2. * perlin((pos + vec2(halfTime)) * 8., 10., 0.) + perlin((pos + vec2(halfTime)) * 16., 10., 0.);
	noise += 32.;
	noise /= 64.;
	if (noise > 0.6)
	{
	    color = vec4(vec3(noise + 0.1), 1);
	    hasCloud = true;
	}
	else
	{
	    color = vec4(vec3(0.8 - max(noise - 0.4, 0.0))*vec3(0.529, 0.808, 0.922), 1);
	}

	if (distance(pos, vec2(sunPos.x * RENDERSIZE.x / RENDERSIZE.y, sunPos.y)) < 0.15)
	{
	    vec3 sun = vec3(0.9765, 0.8431, 0.1098);
	    sun *= (0.15 - distance(pos, vec2(sunPos.x * RENDERSIZE.x / RENDERSIZE.y, sunPos.y))) * 20.;
	    sun.x = min(1., sun.x);
	    sun.y = min(1., sun.y);
	    sun.z = min(1., sun.z);
	    if (hasCloud)
	    {
	        sun *= 0.1;
	    }
	    color += vec4(sun, 0);
	}

	gl_FragColor = color;
}
