/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/WdXGRj by kaneta.  I used the volume render technique in this article\nhttps://shaderbits.com/blog/creating-volumetric-ray-marcher",
  "INPUTS" : [
    {
      "NAME" : "Cloud1",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.5,
      "LABEL" : "CloudLength",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud2",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "LABEL" : "CloudWith",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud3",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.5,
      "LABEL" : "CloudDepth",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud4",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 2.2000000000000002,
      "LABEL" : "Density",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud5",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 1,
      "LABEL" : "CloudRotateSpeed",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud6",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 0.5,
      "LABEL" : "Grow",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud7",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.25,
      "LABEL" : "Repeat",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud8",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 10,
      "LABEL" : "Zoom Tilt",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud9",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 0,
      "LABEL" : "Zoom",
      "MIN" : 0
    },
    {
      "NAME" : "Cloud10",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "LABEL" : "Softness",
      "MIN" : 0
    },
    {
      "NAME" : "S1naarS3",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "LABEL" : "S1naarS3",
      "MIN" : 0
    },
    {
      "NAME" : "DnaarTorus",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "LABEL" : "DnaarTorus",
      "MIN" : 0
    },
    {
      "NAME" : "Angle",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "LABEL" : "Angle",
      "MIN" : 0
    },
    {
      "NAME" : "Bolletje1X",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 0,
      "LABEL" : "Bolletje1X",
      "MIN" : -10
    },
    {
      "NAME" : "Bolletje1Y",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 0,
      "LABEL" : "Bolletje1Y",
      "MIN" : -10
    },
    {
      "NAME" : "Bolletje1Z",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 0,
      "LABEL" : "Bolletje1Z",
      "MIN" : -10
    },
    {
      "NAME" : "Bolletje2X",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 1,
      "LABEL" : "Bolletje2X",
      "MIN" : -10
    },
    {
      "NAME" : "Bolletje2Y",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 1,
      "LABEL" : "Bolletje2Y",
      "MIN" : -10
    },
    {
      "NAME" : "Bolletje2Z",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 1,
      "LABEL" : "Bolletje2Z",
      "MIN" : -10
    }
  ],
  "ISFVSN" : "2"
}
*/


// noise
// Volume raycasting by XT95
// https://www.shadertoy.com/view/lss3zr
mat3 m = mat3( 0.00,  0.80,  0.60,
              -0.80,  0.36, -0.48,
              -0.60, -0.48,  0.64 );
float hash( float n )
{
    return fract(sin(n)*43758.5453);
}

float noise( in vec3 x )
{
    vec3 p = floor(x);
    vec3 f = fract(x);

    f = f*f*(3.0-2.0*f);

    float n = p.x + p.y*57.0 + 113.0*p.z;

    float res = mix(mix(mix( hash(n+  0.0), hash(n+  1.0),f.x),
                        mix( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y),
                    mix(mix( hash(n+113.0), hash(n+114.0),f.x),
                        mix( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
    return res;
}

float fbm( vec3 p )
{
    float f;
    f  = Cloud6*noise( p ); p = m*p*2.02;
    f += Cloud7*noise( p ); p = m*p*2.03;
    f += 0.12500*noise( p ); p = m*p*2.01;
    f += 0.06250*noise( p );
    return f;
}
/////////////////////////////////////

float stepUp(float t, float len, float smo)
{
  float tt = mod(t += smo, len);
  float stp = floor(t / len) - 1.0;
  return smoothstep(0.0, smo, tt) + stp;
}

// iq's smin
float smin( float d1, float d2, float k ) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return mix( d2, d1, h ) - k*h*(1.0-h); }

float sdTorus( vec3 p, vec2 t)
{
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}


float map( in vec3 p )
{
	vec3 q = p - vec3(0.0,0.5,Cloud5)*TIME;
    float f = fbm(q * Cloud10);
    float s1 = 1.0 - length(p * vec3(Cloud1, Cloud2, Cloud3)) + f * Cloud4 + Cloud9;
    float s2 = 1.0 - length(p * vec3(0.1, 1.0, 0.2)) + f * 2.5;
    float torus = 1. - sdTorus(p * 2.0, vec2(6.0, 0.005)) + f * 3.5;
    
    vec3 positionA = vec3(Bolletje2X, Bolletje2Y, Bolletje2Z);
    vec3 positionB = vec3(Bolletje1X, Bolletje1Y, Bolletje1Z);
    vec3 positionC = vec3(-1.0, 0.0, 1.0);
    float s3 = 1.0 - smin(smin(
                           length(p * 1.0 - positionA),
                           length(p * 2.0 - positionB), 4.0),
                           length(p * 3.0 - positionC), 4.0) + f * 2.5;
                           
                           //length(p * 3.0 - vec3(cos(TIME * 2.0) * 3.0, 0.0, sin(TIME * 3.3) * 7.0)), 4.0) + f * 2.5;
                           
                           //Animation
        float t = 0.0;//mod(stepUp(TIME, 0.0, 0.0), 0.0);
    
	float d = mix(s1, s2, clamp(t, 0.0, 1.0));
    d = mix(d, torus, clamp(t - 1.0, 0.0, 1.0));
    d = mix(d, s3, clamp(t - 2.0, 0.0, 1.0));
    d = mix(d, s1, clamp(t - 3.0, 0.0, 1.0));
    
//    d = min(s1, s2);
	float slider = S1naarS3;//sin(TIME*10.0)*.5+0.5;
	float slider2 = DnaarTorus;//sin(TIME*10.0)*.5+0.5;
	d = mix(s1, s3, slider);
	d = mix(d, torus, slider2);
    
	return min(max(0.0, d), 1.0);
}

float jitter;

#define MAX_STEPS 32
#define SHADOW_STEPS 8
#define VOLUME_LENGTH 15.
#define SHADOW_LENGTH 2.

// Reference
// https://shaderbits.com/blog/creating-volumetric-ray-marcher
vec4 cloudMarch(vec3 p, vec3 ray)
{
    float density = 0.;

    float stepLength = VOLUME_LENGTH / float(MAX_STEPS);
    float shadowStepLength = SHADOW_LENGTH / float(SHADOW_STEPS);
    vec3 light = normalize(vec3(1.0, 2.0, 1.0));

    vec4 sum = vec4(0., 0., 0., 1.);
    
    vec3 pos = p + ray * jitter * stepLength;
    
    for (int i = 0; i < MAX_STEPS; i++)
    {
        if (sum.a < 0.1) {
        	break;
        }
        float d = map(pos);
    
        if( d > 0.001)
        {
            vec3 lpos = pos + light * jitter * shadowStepLength;
            float shadow = 0.;
    
            for (int s = 0; s < SHADOW_STEPS; s++)
            {
                lpos += light * shadowStepLength;
                float lsample = map(lpos);
                shadow += lsample;
            }
    
            density = clamp((d / float(MAX_STEPS)) * 20.0, 0.0, 1.0);
            float s = exp((-shadow / float(SHADOW_STEPS)) * 3.);
            sum.rgb += vec3(s * density) * vec3(1.1, 0.9, .5) * sum.a;
            sum.a *= 1.-density;

            sum.rgb += exp(-map(pos + vec3(0,0.25,0.0)) * .2) * density * vec3(0.15, 0.45, 1.1) * sum.a;
        }
        pos += ray * stepLength;
    }

    return sum;
}

mat3 camera(vec3 ro, vec3 ta, float cr )
{
	vec3 cw = normalize(ta - ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void main() {



    vec2 p = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
    jitter = hash(p.x + p.y * 57.0 + TIME);
    float angle = Angle * TIME * 10.0;
    vec3 ro = vec3(cos(angle) * 8.0, -Cloud8, sin(angle) * 8.0);
    vec3 ta = vec3(0.0, 1., 0.0);
    mat3 c = camera(ro, ta, 0.0);
    vec3 ray = c * normalize(vec3(p, 1.75));
    vec4 col = cloudMarch(ro, ray);
    vec3 result = col.rgb + mix(vec3(0.3, 0.6, 1.0), vec3(0.05, 0.35, 1.0), p.y + 0.75) * (col.a);
    
    float sundot = clamp(dot(vec3(0.0,0.0,-2.0),normalize(vec3(1.0, 2.0, 1.0))),0.0,1.0);
    result += 0.4*vec3(1.0,0.7,0.3)*pow( sundot, 4.0 );
    result = pow(result, vec3(1.0/2.2));
    
    gl_FragColor = vec4(result,1.0);
}
