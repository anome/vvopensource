/*{
  "CREDIT": "by Tim Gerritsen for EboStudio/EboSuite",
  "CATEGORIES": [
  ],
  "DESCRIPTION": "Ebosuite tunnel",
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "LABEL": "Fov",
    "NAME": "uFov",
    "TYPE": "float",
    "DEFAULT": 40.0,
    "MIN": 0.0,
    "MAX": 180.0
  },
  {
    "LABEL": "Camera speed",
    "NAME": "uCameraSpeed",
    "TYPE": "float",
    "DEFAULT": 0.135,
    "MIN": -2.0,
    "MAX": 2.0
  },  
  {
    "LABEL": "Segment Angle",
    "NAME": "uAngle",
    "TYPE": "float",
    "DEFAULT": 142.0,
    "MIN": -180.0,
    "MAX": 180.0
  },  
  {
    "LABEL": "Segment length",
    "NAME": "uSegmentLength",
    "TYPE": "float",
    "DEFAULT": 0.569,
    "MIN": 0.001,
    "MAX": 1.0
  },
  {
    "LABEL": "Wall thickness",
    "NAME": "uThickness",
    "TYPE": "float",
    "DEFAULT": 0.052,
    "MIN": 0.0001,
    "MAX": 0.25
  },  
  {
    "LABEL": "Wall color",
    "NAME": "uWallColor",
    "TYPE": "color",
    "DEFAULT": [0.0,0.0,0.5,1.0]
  },  
  {
    "LABEL": "Wall texture",
    "NAME": "uWallTexture",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 1.0
  },  
  {
    "LABEL": "Wall hole",
    "NAME": "uHole",
    "TYPE": "bool",
    "DEFAULT": 1
  },
  {
    "LABEL": "Object texturing",
    "NAME": "uObjectTexturing",
    "TYPE": "long",
    "DEFAULT": 2,
    "VALUES": [0,1,2],
    "LABELS": ["Object", "Relative wall", "Relative object"]
  }, 
  {
    "LABEL": "Object color",
    "NAME": "uObjectColor",
    "TYPE": "color",
    "DEFAULT": [0.0,1.0,0.5,1.0]
  },  
  {
    "LABEL": "Object texture",
    "NAME": "uObjectTexture",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 1.0
  },  
  {
    "LABEL": "Object size",
    "NAME": "uObjectSize",
    "TYPE": "float",
    "DEFAULT": 0.03,
    "MIN": 0.0,
    "MAX": 0.1
  },  
  {
    "LABEL": "Object speed",
    "NAME": "uObjectSpeed",
    "TYPE": "float",
    "DEFAULT": 0.555,
    "MIN": 0.0,
    "MAX": 2.0
  },  
  {
    "LABEL": "Object shape",
    "NAME": "uObjectSphere",
    "TYPE": "float",
    "DEFAULT": 1.733,
    "MIN": 0.0,
    "MAX": 3.0
  },
  {
    "LABEL": "Object repeat",
    "NAME": "uObjectRepeat",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 32.0
  },

  {
    "LABEL": "Light distance",
    "NAME": "uLightDistance",
    "TYPE": "float",
    "DEFAULT": -0.937,
    "MIN": -2.0,
    "MAX": 3.0
  },
  {
    "LABEL": "Light ambient",
    "NAME": "uLightAmbient",
    "TYPE": "float",
    "DEFAULT": 0.5,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "LABEL": "Fog",
    "NAME": "uFog",
    "TYPE": "float",
    "DEFAULT": 8.583,
    "MIN": 0.0,
    "MAX": 10.0
  },
  {
    "LABEL": "Use alpha",
    "NAME": "uAlpha",
    "TYPE": "bool",
    "DEFAULT": 0
  }
  ]
}
*/
#define MAX_MARCH_STEPS 128

#ifdef GL_ES
precision highp float;
#endif

#define PI 3.1415926536

#define mag(x) dot(x,x)

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
  vec3 local;
  float distance;
  vec2 uv;
  vec4 color;
};

mat2 Rot(float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float SdfBox(vec3 p, vec3 s, float r)
{
  vec3 ap = abs(p) - s/2.0;
  return min(max(ap.x, max(ap.y, ap.z)), 0.0) + length(max(ap, 0.0)) - r;
}

float Smin(float a, float b, float t)
{
  float c = max(t-abs(a-b),0.0)/t;
  return min(a,b)-c*c*t/4.0;
}

Hit Scene(vec3 p)
{
  Hit hit;

  float oldZ = p.z;
  float z = p.z - uCameraSpeed*TIME;
  p.z = z;
  float n = floor(z*2.0)/2.0;
  float f = fract(z*2.0)/2.0;
  p.xy = Rot(oldZ*radians(uAngle))*p.xy;

  vec3 looped = p;
  looped.xy = abs(p.xy) * vec2(-1.,1.);
  looped.xy = min(looped.xy, looped.yx*vec2(-1.,1.));
  looped.x += 0.1;
  looped.z = f;

  float sdf = 1e4;
  vec3 boxSize = vec3(uThickness,0.5, uSegmentLength);
  float roundness = 0.0001;

  float box = SdfBox(looped, boxSize, roundness);
  float hole = SdfBox(looped-vec3(0,0,0.25)*boxSize, boxSize*vec3(1.0,boxSize.yz*0.5), roundness+0.001);
  if (uHole) {
    box = max(-hole, box);
  }

  sdf = box;
  hit.index = int(1.0-step(0.001,sdf))*2-1;
  hit.uv = p.xy*4.0+0.5;
  if (step(boxSize.z*0.5,f) <= 0.) {
    hit.uv.y = 1.-f/(uSegmentLength*0.5);
    if (step(0.15,-abs(p.y)*4.0+0.5) > 0.) {
      hit.uv.x = p.y*4.0+0.5;
    }
  }
  hit.color = mix(uWallColor, IMG_NORM_PIXEL(inputImage, hit.uv), uWallTexture);

  float repeat = floor(uObjectRepeat);
  if (repeat > 0. && step(0.00001, uObjectSize) * (1.-step(0.00001,mod(floor(z*2.0+0.5),repeat))) > 0.) {
    
    vec3 toObject = p - vec3(sin(TIME*uObjectSpeed + n)*0.1,0.0,0.125 + n);
    float sphere = mix(SdfBox(toObject, vec3(uObjectSize), 0.0), length(toObject)-uObjectSize, uObjectSphere);
    hit.index += 1+int(1.-step(sdf, sphere));

    float onSphere = step(sphere,sdf);
    sdf = Smin(sdf, sphere, 0.05);
    float t = 0.05;
    float c = max(t-abs(sdf-sphere),0.0)/t;
    hit.uv = normalize(toObject).xy*0.5+0.5;

    hit.uv -= 0.5;
    if (RENDERSIZE.x > RENDERSIZE.y) {
      hit.uv.x *= RENDERSIZE.y/RENDERSIZE.x;
    } else {
      hit.uv.y *= RENDERSIZE.x/RENDERSIZE.y;
    }
    hit.uv += 0.5;
    if (uObjectTexturing == 0) {
      vec4 objectColor = mix(uObjectColor, IMG_NORM_PIXEL(inputImage, hit.uv), uObjectTexture);
      float mixColor = 1.-smoothstep(0.002,0.006+sin(hit.uv.x*hit.uv.y*20.)*0.001,c*c*t/4.0);   
      hit.color = mix(objectColor, hit.color, mixColor);
    } else if (uObjectTexturing == 2) {
      hit.color = mix(uWallColor, IMG_NORM_PIXEL(inputImage, hit.uv), uWallTexture);
    }
  }
  hit.distance = sdf;
  return hit;
}

vec3 CalculateNormal(vec3 p)
{
  vec2 o = vec2(0.0, 0.001);
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
  float maxDepth = min(uFog+2.0, uFog*2.0);
  for (int i = 0; i < MAX_MARCH_STEPS; i++) {
    vec3 p = ray.origin + ray.direction * depth;
    Hit h = Scene(p);

    if (h.distance < 0.0001) {
      hit = h;
      hit.distance = depth+h.distance;
      hit.position = ray.origin + ray.direction * hit.distance;
      hit.normal = CalculateNormal(hit.position);
      return hit;
    }

    depth += max(minStep, h.distance);
    if (depth > maxDepth) {
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
  float specular = 0.7*max(0., dot(-ray.direction, reflect(lightDirection, hit.normal)));
  float shininess = 5.;
  specular = pow(specular, shininess);
  float fog = 1.0-smoothstep(uFog, min(uFog+2.0, uFog*2.0),hit.distance);
  diffuse = uLightAmbient + (1.-uLightAmbient)*diffuse;
  float alpha = float(uAlpha);
  float a = alpha*hit.color.a+(1.0-alpha);
  return vec4((diffuse*hit.color.rgb + specular*vec3(0.5))*fog*a, a);
}

void main()
{
  vec2 xy = gl_FragCoord.xy;
  vec2 res = RENDERSIZE;
  vec2 uv = xy/res;
  uv -= 0.5;

  Ray camRay = Ray(
    vec3(0,0,0),
    normalize(vec3(uv * res, -res.x/(2.0*(tan(radians(uFov*0.5))))))
  );
  vec3 lightPosition = camRay.origin+vec3(0.0,0.0,-uLightDistance);
  Hit hit = March(camRay);
  gl_FragColor = Render(hit, camRay, lightPosition);
}