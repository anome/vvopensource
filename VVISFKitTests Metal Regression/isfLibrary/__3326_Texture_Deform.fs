/*{
	"CREDIT": "by Tim Gerritsen for EboStudio/EboSuite",
	"DESCRIPTION": "Ebosuite texture displace",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "InputImage",
			"TYPE": "image"
	 	},
		{
			"LABEL": "Fov",
			"NAME": "uFov",
			"TYPE": "float",
			"DEFAULT": 73.0,
			"MIN": 0.0,
			"MAX": 180.0
		},
		{
			"LABEL": "Cam rotation X",
			"NAME": "uCameraRotationX",
			"TYPE": "float",
			"DEFAULT": 75.0,
			"MIN": -90.0,
			"MAX": 90.0
		},
		{
			"LABEL": "Cam rotation Y",
			"NAME": "uCameraRotationY",
			"TYPE": "float",
			"DEFAULT": 25.0,
			"MIN": -90.0,
			"MAX": 90.0
		},
		{
			"LABEL": "Cam distance",
			"NAME": "uCameraDistance",
			"TYPE": "float",
			"DEFAULT": 2.043,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{
			"LABEL": "Noise amplitude",
			"NAME": "uNoiseAmp",
			"TYPE": "float",
			"DEFAULT": 1.48,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"LABEL": "Noise freq",
			"NAME": "uNoiseFreq",
			"TYPE": "float",
			"DEFAULT": 4.451,
			"MIN": 0.0,
			"MAX": 20.0
		},
		{
			"LABEL": "Noise offset X",
			"NAME": "uNoiseX",
			"TYPE": "float",
			"DEFAULT": -0.472,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Noise offset Y",
			"NAME": "uNoiseY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Noise offset Z",
			"NAME": "uNoiseZ",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Shadow",
			"NAME": "uShadow",
			"TYPE": "float",
			"DEFAULT": 0.71,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
		    "LABEL": "Fog distance",
		    "NAME": "uFog",
		    "TYPE": "float",
		    "DEFAULT": 8.11,
		    "MIN": 0.0,
		    "MAX": 10.0
		},
		{
			"LABEL": "Noise type",
			"NAME": "uNoiseType",
			"TYPE": "long",
			"DEFAULT": 0,
			"VALUES": [0,1,2,3],
			"LABELS": ["Perlin", "Simplex", "FBM", "Voronoi"]
    
		},
		{
		    "LABEL": "Ambient light",
		    "NAME": "uAmbient",
		    "TYPE": "float",
		    "DEFAULT": 0.1,
		    "MIN": 0.0,
		    "MAX": 1.0
		},
		{
		    "LABEL": "Use alpha",
		    "NAME": "uAlpha",
		    "TYPE": "bool",
		    "DEFAULT": 0
		}
	]
}*/

#define MAX_MARCH_STEPS 32
#define PI 3.1415926536

#define NOISE_PERLIN 0
#define NOISE_SIMPLEX 1
#define NOISE_FBM 2
#define NOISE_VORONOI 3

#define mag(x) dot(x,x)

#define LIGHTPOSITION vec3(0.,0.,3.)

struct Ray
{
	vec3 origin;
	vec3 direction;
};

struct Hit
{
	int index;
	vec3 position;
	vec3 normal;
	float distance;
	vec4 color;
};

float Rnd(vec3 p) { return fract(sin(dot(p, vec3(3734.33638,434.23526,213.12345)))*52346.232422); }

vec3 Rnd3(vec3 p) 
{ 
	float r = 4096.0*sin(dot(p,vec3(17.0, 59.4, 15.0)));
	vec3 result = vec3(fract(512.*r));
	r *= 0.125;
	result.y = fract(512.*r);
	r *= 0.125;
	result.z = fract(512.*r);
	return result-0.5;
}

float NoisePerlin(vec3 p) 
{
	vec3 n = floor(p);
	vec3 f = fract(p);
	vec2 o = vec2(0.0, 1.0);
	f = f*f*(3.0-2.0*f);
	return mix(
		mix(mix(Rnd(n + o.xxx), Rnd(n + o.yxx), f.x), mix(Rnd(n + o.xyx), Rnd(n + o.yyx), f.x), f.y),
		mix(mix(Rnd(n + o.xxy), Rnd(n + o.yxy), f.x), mix(Rnd(n + o.xyy), Rnd(n + o.yyy), f.x), f.y),
		f.z
	);
}

/* 3d simplex noise by Nikita Miropolskiy (https://www.shadertoy.com/view/XsX3zB) */
float NoiseSimplex(vec3 p) 
{
	float F3 =  0.3333333;
	float G3 =  0.1666667;
	vec3 s = floor(p + dot(p, vec3(F3)));
	vec3 x = p - s + dot(s, vec3(G3));
	vec3 e = step(vec3(0.0), x - x.yzx);
	vec3 i1 = e*(1.0 - e.zxy);
	vec3 i2 = 1.0 - e.zxy*(1.0 - e);
	vec3 x1 = x - i1 + G3;
	vec3 x2 = x - i2 + 2.0*G3;
	vec3 x3 = x - 1.0 + 3.0*G3;
	 
	 vec4 w = max(0.6 - vec4(dot(x, x), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
	 vec4 d = vec4(dot(Rnd3(s), x), dot(Rnd3(s + i1), x1), dot(Rnd3(s + i2), x2), dot(Rnd3(s + 1.0), x3));
	 d *= w*w*w*w;
	 return dot(d, vec4(52.0));
}

float NoiseFbm(vec3 p)
{
	float f = 1.0;
	float a = 1.0;
	mat2 m = mat2(0.8, 0.6, -0.6, 0.8);
	float n = 0.0;
	for (int i = 0; i < 3; i++) {
		n += NoisePerlin(p * f) * a;
		f *= 2.0;
		a *= 0.5;
		p.xy = m*p.xy;
	}
	return n;
}

float NoiseVoronoi(vec3 p)
{
    vec2 n = floor(p.xy);
    vec2 f = fract(p.xy);
	float result = 1.0;
	
    for(int x = -1; x <= 1; x++) {
		for(int y = -1; y <= 1; y++) {
			vec2 o = vec2(x, y);
			
			vec2 r = vec2(Rnd(vec3(n + o, 0)),Rnd(vec3(n + o, 1.0)));
			
			float d = mag((o - f) + .5 + (0.5 * sin((r+p.z*0.01) * 544.12)));
			
			if (d < result) {
				result = d;
			}
		}
	}
	return result;
}

float Noise(vec3 p)
{
	if (uNoiseType == NOISE_PERLIN) {
		return NoisePerlin(p);
	} else if (uNoiseType == NOISE_SIMPLEX) {
		return NoiseSimplex(p);
	} else if (uNoiseType == NOISE_FBM) {
		return NoiseFbm(p);
	} else if (uNoiseType == NOISE_VORONOI) {
		return NoiseVoronoi(p);
	}
	return 0.0;
}

mat2 Rot(float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float Smin(float a, float b, float t)
{
	float c = max(t-abs(a-b),0.0)/t;
    return min(a,b)-c*c*t/4.0;
}

float Smax(float a, float b, float t)
{
	float c = max(t-abs(a-b),0.0)/t;
    return max(a,b)+c*c*t/4.0;
}

Hit Scene(vec3 p)
{
	Hit hit;
 	hit.index = 0;
	hit.distance = p.z;

	vec3 aOffset = vec3(uNoiseX, uNoiseY, uNoiseZ);
	hit.distance = p.z - ((Noise((vec3(p.x,p.y,0.))*uNoiseFreq + aOffset)*0.5+0.5)*uNoiseAmp);
	return hit;
}

vec3 CalculateNormal(vec3 p)
{
	vec2 o = vec2(0.0, 0.01);
	return normalize(vec3(
		Scene(p + o.yxx).distance - Scene(p - o.yxx).distance,
		Scene(p + o.xyx).distance - Scene(p - o.xyx).distance,
		Scene(p + o.xxy).distance - Scene(p - o.xxy).distance
	));
}

Hit March(Ray ray)
{
	float minStep = 0.0001;
	float depth = minStep;

	Hit hit;
	hit.index = -1;
	hit.distance = 1e4;
	for (int i = 0; i < MAX_MARCH_STEPS; i++) {
		vec3 p = ray.origin + ray.direction * depth;
		Hit h = Scene(p);

		if (h.distance < 0.0001) {
			hit = h;
			hit.distance = depth+h.distance;
			hit.position = ray.origin + ray.direction * hit.distance;
			hit.normal = CalculateNormal(hit.position);
			vec2 uv = hit.position.xy+0.5;
			uv = fract(uv);
			uv -= 0.5;
			if (RENDERSIZE.x > RENDERSIZE.y) {
				uv.x *= RENDERSIZE.y/RENDERSIZE.x;
			} else {
				uv.y *= RENDERSIZE.x/RENDERSIZE.y;
			}
			uv += 0.5;
			hit.color = IMG_NORM_PIXEL(InputImage, uv) * (1.0-smoothstep(0.5,1.0,float(i)/float(MAX_MARCH_STEPS)));
			return hit;
		}

		depth += max(minStep, h.distance);
		if (depth > 10.) {
			break;
		}
	}
	return hit;
}

vec4 Render(Hit hit, Ray ray, vec3 lightPosition)
{
	if (hit.index < 0) {
		return vec4(0);
	}
	vec3 lightDirection = normalize(hit.position - lightPosition);
	float diffuse = max(0., dot(-lightDirection, hit.normal));
	
	float fog = 1.0-smoothstep(uFog, min(uFog+2.0, uFog*2.0),hit.distance);
	float alpha = float(uAlpha);
	return vec4(((uAmbient+min(1.0,diffuse+(1.0-uShadow))) * hit.color.rgb) * fog, (alpha*hit.color.a)+(1.0-alpha));
}

void main()
{
	vec2 uv = isf_FragNormCoord.xy;
	vec2 res = RENDERSIZE;

	uv -= 0.5;
	vec3 camPos = vec3(0.,0.0, uCameraDistance);
	vec3 camDir = normalize(vec3(uv * res, -res.x/(2.0*(tan(radians(uFov*0.5))))));
	camDir.yz = Rot(radians(-uCameraRotationX)) * camDir.yz;
	camDir.xy = Rot(radians(uCameraRotationY)) * camDir.xy;
	Ray camRay = Ray(camPos, camDir);

	Hit hit = March(camRay);

	vec4 color = Render(hit, camRay, LIGHTPOSITION);
    gl_FragColor = color;
}