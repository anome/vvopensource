/*{
  "CREDIT": "by s23b & mojovideotech, remixed by Tim Gerritsen for EboStudio/EboSuite",
  "CATEGORIES" : [
  ],
  "DESCRIPTION" : "mod of https://www.shadertoy.com/view/XltXWN by s23b",
  "INPUTS" : [
    {
        "NAME":     "inputImage",
        "TYPE":     "image"
    },  
    {
        "NAME":     "textureRotate",
        "TYPE":     "float",
        "DEFAULT":  0.5,
        "MIN":      -5.0,
        "MAX":      5.0
    },
    {
        "NAME":     "rate",
        "TYPE":     "float",
        "DEFAULT":  0.5,
        "MIN":      -3.0,
        "MAX":      3.0
    },
    {
        "NAME":     "FOV",
        "TYPE":     "float",
        "DEFAULT":  1.25,
        "MIN":      -2.5,
        "MAX":      2.5
    },
    {
        "NAME":     "twist",
        "TYPE":     "float",
        "DEFAULT":  0.2,
        "MIN":      -1.0,
        "MAX":      1.0
    },
    {
        "LABEL":    "Wall color",
        "NAME":     "uWallColor",
        "TYPE":     "color",
        "DEFAULT":  [0.6,1.0,0.8,1.0]
    },    
    {
        "NAME":     "ambient",
        "TYPE":     "float",
        "DEFAULT":  0.584,
        "MIN":      0.0,
        "MAX":      1.0
    },
    {
        "NAME" :    "fog",
        "TYPE" :    "float",
        "DEFAULT" : 0.01,
        "MIN" :     0.0025,
        "MAX" :     0.05
    },
    {
        "LABEL":    "Pilar radius",
        "NAME" :    "uPilarRadius",
        "TYPE" :    "float",
        "DEFAULT" : 0.131,
        "MIN" :     0.0,
        "MAX" :     0.5
    },
    {
        "LABEL":    "Arc",
        "NAME" :    "uArc",
        "TYPE" :    "float",
        "DEFAULT" : 0.543,
        "MIN" :     0.0,
        "MAX" :     1.0
    },
    {
        "LABEL":    "Wall",
        "NAME" :    "uWall",
        "TYPE" :    "float",
        "DEFAULT" : 0.317,
        "MIN" :     -0.5,
        "MAX" :     1.0
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

////////////////////////////////////////////////////////////
// CollapsingArchitecture  by mojovideotech
// 
// mod of shadertoy.com/XltXWN  by s23b
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define MAX_STEPS 80
#define EPS .0001
#define RENDER_DIST 16.
#define AO_SAMPLES 4.
#define AO_RANGE 10.
#define PI 3.141592653589793    // pi
#define mag(x) dot(x,x)
#define saturate(x) clamp(x, 0., 1.)

float hash(vec3 uv) {
  float f = fract(sin(dot(uv ,vec3(.009123898,.00231233, .00532234)))* 111111.5452313);
    return f;
}

float noise(vec3 uv) {
    vec3 fuv = floor(uv);
    vec4 cell0 = vec4(
        hash(fuv + vec3(0, 0, 0)),
        hash(fuv + vec3(0, 1, 0)),
        hash(fuv + vec3(1, 0, 0)),
        hash(fuv + vec3(1, 1, 0))
    );
    vec2 axis0 = mix(cell0.xz, cell0.yw, fract(uv.y));
    float val0 = mix(axis0.x, axis0.y, fract(uv.x));
    vec4 cell1 = vec4(
        hash(fuv + vec3(0, 0, 1)),
        hash(fuv + vec3(0, 1, 1)),
        hash(fuv + vec3(1, 0, 1)),
        hash(fuv + vec3(1, 1, 1))
    );
    vec2 axis1 = mix(cell1.xz, cell1.yw, fract(uv.y));
    float val1 = mix(axis1.x, axis1.y, fract(uv.x));
    return mix(val0, val1, fract(uv.z));
}

float fbm(vec3 uv) {
    float f = 0.;
    float r = 1.;
    for (int i = 0; i < 5; ++i) {
        f += noise((uv + 10.) * r) / (r *= 2.);
    }
    return f / (1. - 1. / r);
}

void tRotate(inout vec2 p, float angel) {
    float s = sin(angel), c = cos(angel);
    p *= mat2(c, -s, s, c);
}

void tTwist(inout vec3 p, float a) {
    tRotate(p.xy, p.z * a);
}

float tRepeat1(inout float p, float r) {
    float id = floor((p + r * .5) / r);
    p = mod(p + r * .5, r) - r * .5;
    return id;
}

vec2 tRepeat2(inout vec2 p, vec2 r) {
    vec2 id = floor((p + r * .5) / r);
    p = mod(p + r * .5, r) - r * .5;
    return id;
}

float sdRect(vec2 p, vec2 r) {
    p = abs(p) - r;
    return min(max(p.x, p.y), 0.) + length(max(p, 0.));
}

float sdCircle(vec2 p, float r) {
    return length(p) - r;
}

float opU(float a, float b) {
    return min(a, b);
}

float opS(float a, float b) {
    return max(a, -b);
}

float map(vec3 p, out vec2 uv) {
    tTwist(p, twist);
    tRepeat2(p.xz, vec2(.7, 1.));
    int n = int(floor((p.x-.0)/.7-0.5));
    vec3 realp = p;
    p.x = abs(p.x);
    p.y += .5;
    float d = abs(p.z) - .15 + abs(twist*0.1);
    float w = opU(sdCircle(p.xy - vec2(0, .75), 0.6*uArc+0.01), sdRect(p.xy - vec2(0, .375), vec2(.15, .375)));
    d = opS(d, w);
    d = opS(d, sdRect(p.xy - vec2(0,.35), vec2(.45,.3)));
    vec2 pilarPos = p.xz - vec2(0.35, 0.);
    float pilar = sdCircle(pilarPos, uPilarRadius);
    uv = vec2(0);
    if (pilar < d) {
        pilarPos = normalize(pilarPos);
        float a = acos(pilarPos.y)/(2.0*PI);
        uv = fract((vec2(sign(realp.x)*a + TIME*textureRotate,p.y*1.5)));
    }
    d = mix(opU(d, pilar), pilar, uWall);
    p.z = abs(p.z);
    d = opS(d, sdRect(p.yz - vec2(.5, .5), vec2(.6,.4 )));
    d = opU(d, -abs(p.y - .5) + .8);
    return d;
}

float trace(vec3 ro, vec3 rd, float maxDist, out float steps, out vec2 uv) {
    float total = 0.;
    steps = 0.;
    for (int i = 0; i < MAX_STEPS; ++i) {
        ++steps;
        float d = map(ro + rd * total,uv);
        total += d;
        if (d < EPS || maxDist < total) break;
    }
    return total;
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(.001, 0);
    vec2 uv;
    return normalize(vec3(
        map(p + e.xyy,uv) - map(p - e.xyy,uv),
        map(p + e.yxy,uv) - map(p - e.yxy,uv),
        map(p + e.yyx,uv) - map(p - e.yyx,uv)
    ));
}

float calculateAO(vec3 p, vec3 n) {
    float r = 0., w = 1., d;
    vec2 uv;
    for (float i = 1.; i <= AO_SAMPLES; i++){
        d = i / AO_SAMPLES / AO_RANGE;
        r += w * (d - map(p + n * d, uv));
        w *= .5;
    }
    return 1.-saturate(r * AO_RANGE);
}

bool isWall(vec3 p) {
    p.x += .35;
    tRepeat2(p.xz, vec2(.7, 1));
    return .375 < abs(p.y + .15) + length(p.xz);
}

vec4 tex(vec3 p, vec2 uv) {
    vec4 t;
    tTwist(p, twist);
    bool wall = isWall(p);
    
    if (mag(uv) > 0.) {
        uv = fract(uv);
        uv -= 0.5;
        if (RENDERSIZE.x > RENDERSIZE.y) {
            uv.x *= RENDERSIZE.y/RENDERSIZE.x;
        } else {
            uv.y *= RENDERSIZE.x/RENDERSIZE.y;
        }
        uv += 0.5;
        t = IMG_NORM_PIXEL(inputImage,uv);
    } else {
        vec4 wc = vec4(uWallColor.rgb,1.0);
        t = fbm((p + (wall ? 0. : .1 + .9 * fbm(p * 5.))) * vec3(5., 20., 5.)) * uWallColor * vec4(1., .7, .4, 1.) * .75
            + fbm(p * vec3(2., 10., 2.)) * uWallColor * vec4(1., .8, .5, 1.) * .25;
        if (wall) t = vec4(mix(t.rgb, wc.rgb, .5), uWallColor.a);
    }
    return saturate(t);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2. - 1.;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    float time = TIME * rate;
    vec3 ro = vec3(sin(time * PI / 2. + 1.) * 1., 0, time);
    vec3 rd = normalize(vec3(uv, FOV));
    time += 2.;
    vec3 light = vec3(sin(time * PI / 2. + 1.) * 1., 0, time);
    time -= 2.;
    tRotate(rd.xz, -cos(time * PI / 2. + 1.) * .5);
    tRotate(ro.xy, -ro.z * twist);
    tRotate(light.xy, -light.z * twist);
    tRotate(rd.xy, -ro.z * twist);
    vec2 tuv;
    float steps, dist = trace(ro, rd, RENDER_DIST, steps, tuv); 
    vec3 p = ro + rd * dist;
    vec3 normal = getNormal(p);
    vec3 l = normalize(light - p);
    vec3 shadowStart = p + normal * EPS * 10.;
    float shadowDistance = distance(shadowStart,light);
    vec2 shadowUV;
    float shadowSteps, shadow = float(trace(shadowStart, l, shadowDistance, shadowSteps, shadowUV) > shadowDistance);
    shadow *= 1. - sqrt(shadowSteps / float(MAX_STEPS));
    float diffuse = max(0., dot(l, normal));
    float specular = pow(max(0., dot(reflect(-l, normal), -rd)), 8.);
    float ao = calculateAO(p, normal);        
    vec4 t = tex(p,tuv);
    gl_FragColor.rgb = (ao * t.rgb) * (ambient + (specular + diffuse) * shadow);
    gl_FragColor = mix(gl_FragColor, vec4(.6, .5, .7, 1.), saturate(dist * dist * fog ));
    gl_FragColor = pow(gl_FragColor, vec4(1. / gamma));
    float a = float(uAlpha);
    gl_FragColor.a = (a*t.a)+(1.0-a);
}