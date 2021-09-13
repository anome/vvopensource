/*{
	"CREDIT": "by Tim Gerritsen for EboStudio/EboSuite",
	"DESCRIPTION": "Ebosuite blending boxes",
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
			"DEFAULT": 36.0,
			"MIN": 0.0,
			"MAX": 180.0
		},
		{
			"LABEL": "Scene rotate X",
			"NAME": "uSceneRotateX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -180.0,
			"MAX": 180.0
		},
		{
			"LABEL": "Scene rotate Y",
			"NAME": "uSceneRotateY",
			"TYPE": "float",
			"DEFAULT": -43.0,
			"MIN": -180.0,
			"MAX": 180.0
		},
		{
			"LABEL": "Scene rotate Z",
			"NAME": "uSceneRotateZ",
			"TYPE": "float",
			"DEFAULT": 24.0,
			"MIN": -180.0,
			"MAX": 180.0
		},
		{
			"LABEL": "Box1 size",
			"NAME": "uMainBoxSize",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0001,
			"MAX": 10.0
		},
		{
			"LABEL": "Box1 color",
			"NAME": "uMainBoxColor",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Box1 texture",
			"NAME": "uMainBoxTextureBlend",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},

		{
			"LABEL": "Box2+3 dist",
			"NAME": "uBoxDistance",
			"TYPE": "float",
			"DEFAULT": 0.377,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Box2+3 speed",
			"NAME": "uBoxSpeed",
			"TYPE": "float",
			"DEFAULT": 0.581,
			"MIN": -5.0,
			"MAX": 5.0
		},
		{
			"LABEL": "Box rot speed",
			"NAME": "uBoxRotationSpeed",
			"TYPE": "float",
			"DEFAULT": 0.497,
			"MIN": -5.0,
			"MAX": 5.0
		},
		{
			"LABEL": "Box2 size",
			"NAME": "uBoxASize",
			"TYPE": "float",
			"DEFAULT": 0.427,
			"MIN": 0.0001,
			"MAX": 1.0
		},
		{
			"LABEL": "Box2 color",
			"NAME": "uBoxAColor",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Box2 texture",
			"NAME": "uBoxATextureBlend",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},

		{
			"LABEL": "Box3 size",
			"NAME": "uBoxBSize",
			"TYPE": "float",
			"DEFAULT": 0.472,
			"MIN": 0.0001,
			"MAX": 1.0
		},
		{
			"LABEL": "Box3 color",
			"NAME": "uBoxBColor",
			"TYPE": "color",
			"DEFAULT": [1.0,1.0,1.0,1.0]
		},
		{
			"LABEL": "Box3 texture",
			"NAME": "uBoxBTextureBlend",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},

		{
			"LABEL": "Blend",
			"NAME": "uBlend",
			"TYPE": "float",
			"DEFAULT": 0.21,
			"MIN": 0.0001,
			"MAX": 1.0
		},
		{
			"LABEL": "Deform strength",
			"NAME": "uDeformStrength",
			"TYPE": "float",
			"DEFAULT": -0.43,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Deform pos X",
			"NAME": "uDeformCenterX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Deform pos Y",
			"NAME": "uDeformCenterY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Deform twist",
			"NAME": "uDeformTwist",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Texture divide",
			"NAME": "uTextureDivide",
			"TYPE": "bool",
			"DEFAULT": 0
		},
		{
		    "LABEL": "Ambient light",
		    "NAME": "uAmbient",
		    "TYPE": "float",
		    "DEFAULT": 0.5,
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
#define MAX_MARCH_STEPS 128
#define PI 3.1415926536

#define mag(x) dot(x,x)

#define LIGHTPOSITION vec3(-1.,1.,3.)
#define FOG vec2(2.0, 4.0)
#define SPECULAR 0.5


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

mat2 Rot(float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

vec3 Rotate(vec3 p, vec3 a)
{
	p.xy = Rot(a.z)*p.xy;
	p.xz = Rot(a.y)*p.xz;
	p.yz = Rot(a.x)*p.yz;
	return p;
}

float SdfBox(vec3 p, vec3 s, float r)
{
	vec3 ap = abs(p) - s/2.0;
	return min(max(ap.x, max(ap.y, ap.z)), 0.0) - r + length(max(ap, 0.));
}

float Smin(float a, float b, float t)
{
	float c = max(t-abs(a-b),0.0)/t;
    return min(a,b)-c*c*t/4.0;
}

vec2 BoxUV(vec3 boxPos, vec3 boxSize, float divide, float offset)
{
	vec3 uvw = boxPos / boxSize;
	vec3 ap = abs(uvw)+0.5;
	vec2 uv = (mix(mix(uvw.xy, uvw.xz, 1.-step(1., ap.z)),uvw.zy,step(1., ap.x))+0.5);

	vec2 res = RENDERSIZE;
	if (divide > 0.0) {
		res.x /= 3.0;
	}
	if (res.x > res.y) {
		uv.x = (uv.x-0.5)*(res.y/res.x)+0.5;
	} else {
		uv.y = (uv.y-0.5)*(res.x/res.y)+0.5;
	}
	uv.x *= divide*1.0/3.0 + (1.0-divide);
	uv.x += offset;

	return uv;
}

Hit Scene(Ray ray, vec3 p)
{
	Hit hit;
	vec3 deformP = p - vec3(uDeformCenterX,uDeformCenterY,uDeformTwist);
	vec3 centerDeform = vec3(0,0,-1.)*(0.5*uDeformStrength*(sin(atan(deformP.x,deformP.y))*deformP.z*10.*uDeformStrength));
	p = Rotate(p, radians(vec3(uSceneRotateX, uSceneRotateY, uSceneRotateZ)));
	centerDeform = Rotate(centerDeform, radians(vec3(uSceneRotateX, uSceneRotateY, uSceneRotateZ)));
	vec3 rotatedPosition = Rotate(p, centerDeform);

 	hit.index = -1;

	float sdf = 1e4;

	vec3 mainBP = vec3(0);
	vec3 mainBox = rotatedPosition - mainBP;
	mainBox = Rotate(mainBox, vec3(0.9,1.0,1.1)*TIME*uBoxRotationSpeed);

	vec3 boxAP = vec3(-uBoxDistance, 0.0, 0.0);
	vec3 boxBP = vec3(uBoxDistance, 0.0, 0.0);
	boxAP.xz = Rot(TIME*uBoxSpeed)*boxAP.xz;
	boxBP.xz = Rot(TIME*uBoxSpeed)*boxBP.xz;
	vec3 boxAR = vec3(TIME*uBoxRotationSpeed)*vec3(0.9,1.0,1.1);
	boxAR.z *= 0.0;
	vec3 boxBR = vec3(-TIME*uBoxRotationSpeed)*vec3(0.9,1.0,1.1);
	boxBR.y *= 0.0;
	vec3 boxAS = vec3(uBoxASize);
	vec3 boxBS = vec3(uBoxBSize);
	vec3 boxA = Rotate(p - boxAP, boxAR + centerDeform);
	vec3 boxB = Rotate(p - boxBP, boxBR + centerDeform);

	vec3 mainBS = vec3(uMainBoxSize);
	float b1 = SdfBox(mainBox, mainBS, 0.01);
	float b2 = SdfBox(boxA, boxAS, 0.01);
	float b3 = SdfBox(boxB, boxBS, 0.01);

	float blend = uBlend;
	
	float divide = float(uTextureDivide);
	vec2 buv = fract(BoxUV(mainBox, mainBS, divide, divide/3.0));
	hit.color = mix(vec4(uMainBoxColor.rgb,1.0), IMG_NORM_PIXEL(InputImage, buv), uMainBoxTextureBlend);
	buv = fract(BoxUV(boxA, boxAS, divide, 0.0));
	vec4 boxAColor = mix(vec4(uBoxAColor.rgb,1.0), IMG_NORM_PIXEL(InputImage, buv), uBoxATextureBlend);
	buv = fract(BoxUV(boxB, boxBS, divide, 2.0*divide/3.0));
	vec4 boxBColor = mix(vec4(uBoxBColor.rgb,1.0), IMG_NORM_PIXEL(InputImage, buv), uBoxBTextureBlend);

	float c = max(blend-abs(b2-b3),0.0)/blend;
	vec4 color = mix(boxAColor, boxBColor, max(step(b3,b2)-c*c,0.0));
	if (b1 < min(sdf, min(b2,b3))) {
		hit.index = 1;
		c = 1.0;
	} else if (b2 < min(sdf, min(b1,b3))) {
		hit.index = 2;
		c = max(blend-abs(b1-b2),0.0)/blend;
	} else if (b3 < min(sdf, min(b1,b2))) {
		hit.index = 3;
		c = max(blend-abs(b1-b3),0.0)/blend;
	}
	c = mix(clamp(c*10.0 - 9.0, 0.0, 1.0), c, 0.8);
	float t = 1.0-c*c;
	hit.color = mix(hit.color,color, t);

	sdf = min(sdf, Smin(b1,Smin(b2,b3,blend),blend));

	hit.distance = sdf;
	return hit;
}

vec3 CalculateNormal(Ray ray, vec3 p)
{
	vec2 o = vec2(0.0, 0.001);
	return normalize(vec3(
		Scene(ray, p + o.yxx).distance - Scene(ray, p - o.yxx).distance,
		Scene(ray, p + o.xyx).distance - Scene(ray, p - o.xyx).distance,
		Scene(ray, p + o.xxy).distance - Scene(ray, p - o.xxy).distance
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
		Hit h = Scene(ray, p);

		if (h.distance < 0.0001) {
			hit = h;
			hit.distance = depth+h.distance;
			hit.position = ray.origin + ray.direction * hit.distance;
			hit.normal = CalculateNormal(ray, hit.position);
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
	float specular = max(0., dot(-ray.direction, reflect(lightDirection, hit.normal)));
	float shininess = 20.;
	specular = pow(specular, shininess);
	float fog = 1.-smoothstep(FOG.x,FOG.x+FOG.y,hit.distance);
    float alpha = float(uAlpha);
	return vec4(((uAmbient + diffuse)*hit.color.rgb + SPECULAR*specular*vec3(0.5))*fog, alpha*hit.color.a+(1.0-alpha));
}

void main()
{
	vec2 xy = gl_FragCoord.xy;
	vec2 res = RENDERSIZE.xy;

	float aspect = res.x/res.y;
	vec2 uv = xy/res;
	uv -= 0.5;

	float z = 3.;
	Ray camRay = Ray(
		vec3(0,0,z),
		normalize(vec3(uv * res, -res.x/(2.0*(tan(radians(uFov*0.5))))))
	);
	Hit hit = March(camRay);

	vec4 color = Render(hit, camRay, LIGHTPOSITION);
    gl_FragColor = color;
}