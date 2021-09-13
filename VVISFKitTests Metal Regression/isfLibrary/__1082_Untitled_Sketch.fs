/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	]
}*/
vec3 iResolution = vec3(RENDERSIZE, 1.);

// Created by SHAU - 2017
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

/*
 * Apollonian based on similar by Shane, 
 * https://www.shadertoy.com/view/4d2BW1
 * IQ
 * https://www.shadertoy.com/view/4ds3zn
 * https://www.shadertoy.com/view/llKXzh
 * & Greg Rostami
 * https://www.shadertoy.com/view/Xtlyzl
 */

#define EPS 0.001
#define FAR 50.0 
#define PI 3.1415
#define T TIME

const float freqA = 0.15;
const float freqB = 0.25;
const float ampA = 2.4;
const float ampB = 1.7;

vec2 path(in float z) {return vec2(ampA * sin(z * freqA), ampB * cos(z * freqB)); }
vec2 path2(in float z) {return vec2(ampB * sin(z * freqB * 1.5 + PI), ampA * cos(z * freqA * 1.3)); }

mat2 rot(float x) {return mat2(cos(x), sin(x), -sin(x), cos(x));}

vec3 lp1 = vec3(0.0);
vec3 lp2 = vec3(0.0);

// IQ - cosine based palette
//http://iquilezles.org/www/articles/palettes/palettes.htm
vec3 palette(in float t) {
    vec3 CP1A = vec3(0.5, 0.5, 0.5);
    vec3 CP1B = vec3(0.5, 0.5, 0.5);
    vec3 CP1C = vec3(2.0, 1.0, 0.0);
    vec3 CP1D = vec3(0.50, 0.20, 0.25);
    return CP1A + CP1B * cos(6.28318 * (CP1C * t + CP1D));
}

float map(vec3 rp) {

    float scale = 1.0;
    vec3 q = rp;

	for(int i = 0; i < 8; i++) {
        
        q = mod(q - 1.0, 2.0) - 1.0;
        q -= sign(q) * 0.1; //trick from IQ
        
		float r2 = dot(q, q);		
		float k = (1.1 - (sin(T * 0.1) + 1.0) * 0.125) / r2;
		q *= k;
		scale *= k;
	}
	
    return 0.125 * length(q) / scale;
}

vec3 normal(vec3 rp, float t) {
    float e = EPS * t;
    return normalize(vec3(map(rp + vec3(e, 0.0, 0.0)) - map(rp - vec3(e, 0.0, 0.0)),
                          map(rp + vec3(0.0, e, 0.0)) - map(rp - vec3(0.0, e, 0.0)),
                          map(rp + vec3(0.0, 0.0, e)) - map(rp - vec3(0.0, 0.0, e))));
}

float occlusion(vec3 rp, vec3 n) {
	
    float fac = 2.5;
    float occ = 0.0;
    
    for (int i = 0; i < 5; i ++) {
    
        float hr = 0.01 + float(i) * 0.35 / 4.0;        
        float dd = map(n * hr + rp);
        occ += (hr - dd) * fac;
        fac *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);    
}

float shadow(vec3 ro, vec3 rd, float tmax) {
    
	float shadow = 1.0;
    float t = 0.0;
    
    for(int i = 0; i < 12; i++) {
        float ns = map(ro + rd * t);
        shadow = min(shadow, 8.0 * ns / t);
        t += clamp(ns, 0.02, 0.10);
        if (ns < EPS || t > tmax) break;
    }
    return clamp(shadow, 0.0, 1.0);
}


vec2 march(vec3 ro, vec3 rd) {
    
    float t = 0.0;
    
    for (int i = 0; i < 200; i++) {
        vec3 rp = ro + rd * t;
        float ns = map(rp);       
        if (ns < EPS || t > FAR) break;
        
        t += ns;
    }
    
    return vec2(t, FAR);
}

void setupCamera(vec2 fragCoord, out vec3 ro, out vec3 rd) {
    
    //coordinate system
    vec2 uv = fragCoord.xy / iResolution.xy;
    uv = uv * 2.0 - 1.0;
    uv.x *= iResolution.x / iResolution.y;

    lp1 = vec3(1.0, 1.0, T * 0.5 + sin(T * 0.2) * 2.2); //look at and light 1
    ro = vec3(1.0, 1.0, T * 0.5);
    lp2 = ro + vec3(0.1, 0.1, 0.1); //light 2 in front camera
    
    lp1.xy += path2(lp1.z) + 0.2;
    
    ro.xy += path(ro.z) * 0.2;
    lp2.xy += path(lp2.z) * 0.2;
    
    // Using the above to produce the unit ray-direction vector.
    float FOV = PI / 4.; // FOV - Field of view.
    vec3 forward = normalize(lp1.xyz - ro.xyz);
    vec3 right = normalize(vec3(forward.z, 0., -forward.x )); 
    vec3 up = cross(forward, right);    
    rd = normalize(forward + FOV * uv.x * right + FOV * uv.y * up);
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {

    vec3 pc = vec3(0.0);
    vec3 gc = palette(T * 0.1) * 4.0;
    
    vec3 ro, rd;
    setupCamera(fragCoord, ro, rd);
    
    vec2 t = march(ro, rd);
    if (t.x > 0.0 && t.x < FAR) {
        
        vec3 rp = ro + rd * t.x;
        vec3 rpb = ro + rd * (t.x - EPS);
        vec3 n = normal(rp, t.x);
        vec3 rrd = reflect(rd, n);
        float ao = occlusion(rp, n);
        
        vec3 ld1 = normalize(lp1 - rp);vec4(sqrt(clamp(pc, 0.0, 1.0)),1.0);
        float lt1 = length(lp1 - rp);
        float diff1 = dot(n, ld1);
        float shad1 = shadow(rpb, ld1, lt1);

        vec3 ld2 = normalize(lp2 - rp);
        float lt2 = length(lp2 - rp);
        float diff2 = dot(n, ld2);
        //try and save some cycles? Nah
        float shad2 = shadow(rpb, ld2, lt2);
        
        
        float spec1 = pow(clamp(dot(rrd, ld1), 0.0, 1.0), 32.0);
        float spec2 = pow(clamp(dot(rrd, ld2), 0.0, 1.0), 16.0);
        float fres = pow(clamp(1.0 + dot(n, rd), 0.0, 1.0), 16.0);        
        
        pc = vec3(0.4, 0.0, 0.05) * clamp(n.y, 0.0, 1.0) * 0.03;
        
        pc += vec3(1.0) * diff2 * exp(lt2 * -lt2) * shad2 * 0.005 + gc * diff1 * exp(lt1 * -lt1) * shad1;
        pc *= ao;
        pc += gc * spec1 * exp(lt1 * -lt1) * shad1;
        pc += vec3(1.0) * spec2 * exp(lt2 * -lt2) * shad2;

        gc *= step(0.39, mod(rp.y - T * 0.2, 0.4)) * 4.0;
        pc += gc * clamp(diff1 * exp(-t.x), 0.0, 1.0);
        
    }
    
    
    float fog = 1.0 - exp(-t.x * t.x * 6. / FAR);
    pc = mix(pc, vec3(0.0), fog);
   
    fragColor = vec4(sqrt(clamp(pc, 0.0, 1.0)),1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}