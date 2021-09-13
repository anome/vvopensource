// Storm in a teacup
//
// This started with me trying to work out how to make a cloud,
// and then kinda developed from there...
// The sea and plane are deliberately voxel-y, partly
// to keep the frame rate up and partly coz I just like it. :)
//
// Thanks to Evvvvil, Flopine, Nusan, BigWings, Iq, Shane
// and a bunch of others for sharing their knowledge!

#define MIN_DIST   0.0015
#define MAX_DIST   55.0
#define MAX_STEPS  120.0
#define SHADOW_STEPS  32.0
#define MAX_SHADOW_DIST 3.0
#define CLOUD_STEPS  20.0

float time, flash, glow;

//#define AA  // Enable this line if your GPU can take it!

struct MarchData {
    float d;
    vec3 mat;        // RGB
    bool isCloud;
};

// Thanks Shane - https://www.shadertoy.com/view/lstGRB
float noise(vec3 p) {
	const vec3 s = vec3(7.0, 157.0, 113.0);
	vec3 ip = floor(p);
    vec4 h = vec4(0.0, s.yz, s.y + s.z) + dot(ip, s);
	p -= ip;
	
    h = mix(fract(sin(h) * 43758.5453), fract(sin(h + s.x) * 43758.5453), p.x);
	
    h.xy = mix(h.xz, h.yw, p.y);
    return mix(h.x, h.y, p.z);
}

float noise(float n) {
    float flr = floor(n);
    vec2 rndRange = fract(sin(vec2(flr, flr + 1.0) * 12.9898) * 43758.5453);
    return mix(rndRange.x, rndRange.y, fract(n));
}

float smin(float a, float b, float k) {
    float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
    return mix(b, a, h) - k * h * (1.0 - h);
}

MarchData minResult(MarchData a, MarchData b) {
    if (a.d < b.d) return a;
    return b;
}

mat2 rot(float a) {
    float c = cos(a);
    float s = sin(a);
    return mat2(c, s, -s, c);
}

float sdBox(vec3 p, vec3 b) {
    vec3 q = abs(p) - b;
    return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0);
}

float sdCappedCylinder(vec3 p, float h, float r) {
    vec2 d = abs(vec2(length(p.xy), p.z)) - vec2(h, r);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

MarchData sdSea(vec3 p, const float bowlInner) {
    MarchData result;
    result.isCloud = false;

	mat2 r = rot(23.23);

    vec2 af = vec2(1.0);
    const vec2 afc = vec2(0.5, 1.64);
    float t = time * 0.4;

    float wave = noise(p.x);
    for (int i = 0; i < 8; i++) {
	    wave += (1.0 - abs(sin((p.x + t) * af.y))) * af.x; p.xz *= r; af *= afc; 
    }
    
    result.d = max(p.y + 1.0 - wave * 0.3, bowlInner);
    result.mat = vec3(0.03, 0.09, 0.12) * wave;
    return result;
}

MarchData sdCup(vec3 p) {
    MarchData result;
    result.mat = vec3(1.0);
    result.isCloud = false;
    
    float bowlInner = length(p) + p.y * 0.1 - 2.0;
    
    float h = max(abs(length(p.xy - vec2(2.0, p.x * p.x * 0.1 - 1.1)) - 0.5) - 0.06, abs(p.z) - 0.06);
    result.d = smin(max(abs(bowlInner) - 0.06, p.y), max(h, -bowlInner), 0.1);
    
    return minResult(result, sdSea(p, bowlInner));
}

float sdSaucer(vec3 p) {
    float l = length(p.xz);
    p.y += 1.9 - l * (0.1 + 0.02 * smoothstep(0.0, 0.1, l - 2.05));
    return sdCappedCylinder(p.xzy, 2.6, 0.01) - 0.02;
}

vec3 getRayDir(vec3 ro, vec2 uv) {
    vec3 forward = normalize(-ro);
    vec3 right = normalize(cross(vec3(0.0, 1.0, 0.0), forward));
    vec3 up = cross(forward, right);
    return normalize(forward + right * uv.x + up * uv.y);
}

float sdCloud(vec3 p) {
    float n1 = noise(p * 4.0) + noise(p * 9.292 - vec3(0.0, time, 0.0)) * 0.4;
    
    float n = abs(smoothstep(0.0, 1.0, n1 * 0.3) - 0.4) + 0.55;
    p.y -= 1.3;
    return min(length(p + vec3(0.4, 0.0, 0.0)), length(p - vec3(0.4, 0.0, 0.0))) - n;
}

MarchData sdPlane(vec3 p) {
    MarchData result;
    result.mat = vec3(0.29, 0.33, 0.13);
    result.isCloud = false;

    // Scale, position, rotate.
    p *= 1.5;
    p.xz *= rot(time * 0.6);
    p.xy -= vec2(1.5, 0.4);
    p.xy *= rot(sin(time * 3.0) * 0.1);
    
    // Fuselage.
    vec3 pp = p + vec3(0.0, 0.0, 0.15);
    result.d = sdBox(pp, vec2(0.04 + pp.z * 0.05, 0.3).xxy);
    
    // Prop.
    vec3 ppp = pp;
    ppp.z -= 0.33;
    ppp.xy *= rot(time * 8.0);
    float d = sdBox(ppp, vec3(0.09, 0.01 * sin(length(p.xy) * 34.0), 0.005));
    
    // Tail.
    pp.yz += vec2(-0.05, 0.26);
    result.d = min(result.d, sdBox(pp, vec3(0.01, 0.06 * cos(pp.z * 25.6), 0.03)));
    result.d = min(result.d, sdBox(pp + vec3(0.0, 0.05, 0.0), vec3(0.15 * cos(pp.z * 12.0), 0.01, 0.03)));
    
    // Wings
    p.y = abs(p.y) - 0.08;
    result.d = min(result.d, sdBox(p, vec3(0.3, 0.01, 0.1)));
    
    if (d < result.d) {
        result.d = d;
        result.mat = vec3(0.05);
    }

    result.d = (result.d - 0.005) * 0.4;
    return result;
}

// Map the scene using SDF functions.
bool hideCloud;
MarchData map(vec3 p) {
    MarchData result = sdCup(p);

    result.d = min(result.d, sdSaucer(p));
    result = minResult(result, sdPlane(p));
    
    float d;
    if (flash > 0.0) {
        d = length(p.xz * rot(fract(time) * 3.141) + vec2(noise(p.y * 6.5) * 0.08) - vec2(0.5, 0.0));
        d = max(d, p.y - 0.7);
        glow += 0.001 / (0.01 + 2.0 * d * d);
        if (d < result.d) result.d = d;
    }
    
    float gnd = length(p.y + 1.7);
    if (gnd < result.d) {
        result.d = gnd;
        result.mat = vec3(0.2);
    }

    if (!hideCloud) {
        d = sdCloud(p);
        if (d < result.d) {
            result.d = d * 0.7;
            result.isCloud = true;
        }
    }

    return result;
}

vec3 calcNormal(vec3 p, float t) {
    vec2 e = vec2(0.5773, -0.5773) * t * 0.0001;
    return normalize(e.xyy * map(p + e.xyy).d + 
					   e.yyx * map(p + e.yyx).d + 
					   e.yxy * map(p + e.yxy).d + 
					   e.xxx * map(p + e.xxx).d);
}

vec3 cloudNormal(vec3 p) {
    const vec2 e = vec2(0.5773, -0.5773);
    return normalize(e.xyy * sdCloud(p + e.xyy) + 
					 e.yyx * sdCloud(p + e.yyx) + 
					 e.yxy * sdCloud(p + e.yxy) + 
					 e.xxx * sdCloud(p + e.xxx));
}

float calcShadow(vec3 p, vec3 lightPos) {
    // Thanks iq.
    vec3 rd = normalize(lightPos - p);
    
	float res = 1.0;
    float t = 0.1;
    for (float i = 0.0; i < SHADOW_STEPS; i++)
    {
		float h = map(p + rd * t).d;
        res = min(res, 10.0 * h / t);
        t += h;
        if (res < 0.001 || t > MAX_SHADOW_DIST) break;
    }
    
    return clamp(res, 0.0, 1.0);
}

// Quick ambient occlusion.
float ao(vec3 p, vec3 n, float h) { return map(p + h * n).d / h; }
float cloudAo(vec3 p, vec3 n, float h) { return sdCloud(p + h * n) / h; }

/**********************************************************************************/

vec3 vignette(vec3 col, vec2 fragCoord) {
    vec2 q = fragCoord.xy / iResolution.xy;
    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.4);
    return col;
}

const vec3 sunPos = vec3(6.0, 10.0, -4.0);
const vec3 sunCol = vec3(2.0, 1.6, 1.4);
vec3 applyLighting(vec3 p, vec3 rd, float d, MarchData data) {
    vec3 sunDir = normalize(sunPos - p);
    vec3 n = calcNormal(p, d);
    float amb = dot(vec3(ao(p, n, 0.2), ao(p, n, 0.5), ao(p, n, 2.0)), vec3(0.2, 0.3, 0.5));
    
    // Secondary(/bounce) light.
    float bounce = max(0.0, dot(sunDir * vec3(-1.0, 0.0, -1.0), n)) * 0.3;
    
    float primary = max(0.0, dot(sunDir, n)) * mix(0.4, 1.0, calcShadow(p, sunPos));
    return data.mat * (primary + bounce) * amb * sunCol * exp(-length(p) * 0.14);
}

vec3 cloudLighting(vec3 p, float den) {
    vec3 n = cloudNormal(p);
    vec3 col = sunCol * (1.0 + flash);
    return min(0.75, den) * max(0.1, dot(normalize(sunPos - p), n)) * cloudAo(p, n, 1.0) * col;
}

vec3 getSceneColor(vec3 ro, vec3 rd) {
    // Raymarch.
    vec3 p;
    MarchData h;
    float d = 0.01;
	float den = 0.0;
    hideCloud = false;
    vec3 cloudP;
    float maxCloudD = 0.0;
    for (float steps = 0.0; steps < MAX_STEPS; steps++) {
        p = ro + rd * d;
        h = map(p);
        
        if (h.d < MIN_DIST) {
            if (!h.isCloud)
                break;

            hideCloud = true;
            cloudP = p;
            maxCloudD = 20.0 - sdCloud(p + rd * 20.0);
        }
        
        if (d > MAX_DIST)
            break; // Distance limit reached - Stop.
        
        d += h.d; // No hit, so keep marching.
    }
    
    if (hideCloud) {
        for (float i = 0.0; i < CLOUD_STEPS; i++)
            den += clamp(-sdCloud(cloudP + rd * (maxCloudD * i / CLOUD_STEPS)) * 0.2, 0.0, 1.0);
    }
    
    hideCloud = false;
    
    return applyLighting(p, rd, d, h) + cloudLighting(cloudP, den) + vec3(glow) + flash * 0.05;
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
    time = mod(iTime, 120.0);
    flash = step(0.55, pow(noise(time * 8.0), 5.0));

    vec3 ro = vec3(0.0, 2.0, -5.0);
    ro.xz *= rot(-0.6);
    
    vec3 col = vec3(0.0);
#ifdef AA
    for (float dx = 0.0; dx <= 1.0; dx++) {
        for (float dy = 0.0; dy <= 1.0; dy++) {
            vec2 coord = fragCoord + vec2(dx, dy) * 0.5;
#else
            vec2 coord = fragCoord;
#endif
            vec2 uv = (coord - 0.5 * iResolution.xy) / iResolution.y;

            col += getSceneColor(ro, getRayDir(ro, uv));
#ifdef AA
        }
    }
    col /= 4.0;
#endif
    
    // Output to screen.
    col = vignette(pow(col, vec3(0.4545)), fragCoord);
    fragColor = vec4(col, 1.0);
}

