/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "cam_x",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "LABEL" : "Camera X",
      "MIN" : -2
    },
    {
      "NAME" : "cam_y",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 2,
      "LABEL" : "Camera Y",
      "MIN" : 0.1
    },
    {
      "NAME" : "cam_z",
      "TYPE" : "float",
      "MAX" : 3,
      "DEFAULT" : 0.5,
      "LABEL" : "Camera Z",
      "MIN" : 0.1
    },
    {
      "NAME" : "light_pos_x",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "LABEL" : "Light Pos X",
      "MIN" : -10.0
    },
    {
      "NAME" : "light_pos_y",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 9,
      "LABEL" : "Light Pos Y",
      "MIN" : 1.0
    },
    {
      "NAME" : "light_pos_z",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "LABEL" : "Light Pos Z",
      "MIN" : -10.0
    },
    {
      "NAME" : "light_lfo_amp",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 0.0,
      "LABEL" : "Light LFO Amp",
      "MIN" : 0.0
    },
    {
      "NAME" : "light_lfo_speed",
      "TYPE" : "float",
      "MAX" : 3,
      "DEFAULT" : 1,
      "LABEL" : "Light LFO Speed",
      "MIN" : 0.0
    }
  ],
  "PASSES" : [
    {
      "TARGET" : "bufferVariableNameA",
      "WIDTH" : "$WIDTH/16.0",
      "HEIGHT" : "$HEIGHT/16.0"
    },
    {
      "DESCRIPTION" : "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
    }
  ],
  "CREDIT" : ""
}
*/

// "ShaderToy Tutorial - Ray Marching for Dummies!" 
// by Martijn Steinrucken aka BigWings/CountFrolic - 2018
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
//
// This shader is part of a tutorial on YouTube
// https://youtu.be/PGtv-dBi2wE

#define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST .01

#define WALL_LIGHT_AMP 2.0
#define LIGHTA_AMP 3.0

#define WALL_X 7.0

const int MODE_WALL_LIGHT_FLICKER = 0;
const int MODE_WALL_LIGHT_PHASE = 1;
const int MODE_OVERHEAD_LIGHT_PHASE = 2;

const float PI = 3.1457;

float bpm() { return 128.5; }
float beats_per_bar() { return 4.; }
float bar() { return TIME * bpm() / 60.0 / beats_per_bar(); }
float bar_phase() { return mod(bar(), 1.0); }
float bar2_phase() { return mod(bar(), 2.0) / 2.0; }
float bar4_phase() { return mod(bar(), 4.0) / 4.0; }
float bar8_phase() { return mod(bar(), 8.0) / 8.0; }
float bar16_phase() { return mod(bar(), 16.0) / 16.0; }
float beat() { return TIME * bpm() / 60.0; }
float beat_phase() { return mod(beat(), 1.0); }
float minim_phase() { return mod(beat()*0.5, 1.0); }
float quaver_phase() { return mod(beat()*2.0, 1.0); }
float semiquaver_phase() { return mod(beat()*4.0, 1.0); }

float smin( float a, float b, float k ) {
    float h = clamp( 0.5+0.5*(b-a)/k, 0., 1. );
    return mix( b, a, h ) - k*h*(1.0-h);
}

float sqr(float phase) {
    return float(int(phase * 2.0))*0.5;
}

float rand(float n){return fract(sin(n) * 43758.5453123);}

float noise(float p){
	float fl = floor(p);
    float fc = fract(p);
	return mix(rand(fl), rand(fl + 1.0), fc);
}

float flicker() {
    return 1.0 - 0.35*noise(beat()*16.);
}

float light_mode_phase() {
    return bar8_phase();
}

int light_mode() {
    int i = int(light_mode_phase() * 4.0);
    if (i == 0) {
        return MODE_WALL_LIGHT_FLICKER;
    } else if (i == 1) {
        return MODE_WALL_LIGHT_PHASE;
    } else if (i == 2) {
        return MODE_WALL_LIGHT_FLICKER;
    } else if (i == 3) {
        return MODE_OVERHEAD_LIGHT_PHASE;
    } else {
        return MODE_WALL_LIGHT_FLICKER;
    }
}

float dBox(vec3 p, vec3 s) {
	return length(max(abs(p)-s, 0.));
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
	vec3 ab = b-a;
    vec3 ap = p-a;
    float t = dot(ab, ap) / dot(ab, ab);
    t = clamp(t, 0., 1.);
    vec3 c = a + t*ab;
    return length(p-c)-r;
}

float sdCylinder(vec3 p, vec3 a, vec3 b, float r) {
	vec3 ab = b-a;
    vec3 ap = p-a;
    float t = dot(ab, ap) / dot(ab, ab);
    vec3 c = a + t*ab;
    float x = length(p-c)-r;
    float y = (abs(t-.5)-.5)*length(ab);
    float e = length(max(vec2(x, y), 0.));
    float i = min(max(x, y), 0.);
    return e+i;
}

float sdTorus(vec3 p, vec2 r) {
	float x = length(p.xz)-r.x;
    return length(vec2(x, p.y))-r.y;
}

float tunnel_dist(vec3 p) {
    float wall_x = WALL_X;
    vec3 box_pos = vec3(wall_x, 3.0, 15.0);
    vec3 box_dim = vec3(1.0, 1000.0, 1000.0);
    float right_dist = dBox(p-box_pos, box_dim);
    box_pos.x = -wall_x;
    float left_dist = dBox(p-box_pos, box_dim);
    return min(right_dist, left_dist);
}

float seat_dist(vec3 p) {
    vec3 box_dim = vec3(1.0, 1.0, 2.0);
    vec3 box_pos = vec3(WALL_X-box_dim.x, 0.0, 10.0);
    return dBox(p-box_pos, box_dim);
}

float wall_barrel_dist(vec3 p, float bz) {
    vec3 a = vec3(-WALL_X+2.0, 0.0, bz);
    vec3 b = vec3(a.x, a.y+2.0, a.z);
    float r = 0.5;
    return sdCylinder(p, a, b, r);
}

float bin_dist(vec3 p) {
    vec3 a = vec3(WALL_X-1., 0.0, 14.0);
    vec3 b = vec3(a.x, a.y+1.0, a.z);
    float r = 0.65;
    return sdCapsule(p, a, b, r);
}

float sphere_dist(vec3 p, float r) {
    return length(p)-r;
}

float metablob_dist(vec3 p) {
    float spread = 1.3;
    float y = 3.0;
    float z = 8.0;
    float smth = 3.0;
    float br = bar();
    float s1 = sphere_dist(p-vec3(cos(br*0.8)*spread, y + sin(br*1.0)*spread, z + cos(br*1.3)*spread), 0.2);
    float s2 = sphere_dist(p-vec3(sin(br*1.3)*spread, y + cos(br*1.1)*spread, z + sin(br*1.4)*spread), 0.25);
    float s3 = sphere_dist(p-vec3(sin(br*0.9)*spread, y + sin(br*0.8)*spread, z + sin(br*1.7)*spread), 0.3);
    float d = s1;
    d = smin(d, s2, smth);
    d = smin(d, s3, smth);
    return d;
}

float GetDist(vec3 p) {
    float plane_d = p.y;
    float tunnel_d = tunnel_dist(p);
    float seat_d = seat_dist(p);
    float barrel1_z = 10.0;
    float barrel_sep = 1.5;
    float barrel1_d = wall_barrel_dist(p, barrel1_z);
    float barrel2_d = wall_barrel_dist(p, barrel1_z+barrel_sep);
    float barrel3_d = wall_barrel_dist(p, barrel1_z+barrel_sep*2.);
    float barrel4_d = wall_barrel_dist(p, barrel1_z+barrel_sep*16.);
    float barrel5_d = wall_barrel_dist(p, barrel1_z+barrel_sep*17.);
    float barrel6_d = wall_barrel_dist(p, barrel1_z+barrel_sep*18.);
	float bin_d = bin_dist(p);
	float metablob_d = metablob_dist(p);

    float d = plane_d;
    d = min(d, tunnel_d);
    d = min(d, seat_d);
    d = min(d, barrel1_d);
    d = min(d, barrel2_d);
    d = min(d, barrel3_d);
    d = min(d, barrel4_d);
    d = min(d, barrel5_d);
    d = min(d, barrel6_d);
    d = min(d, bin_d);
    d = min(d, metablob_d);


    return d;
}

float shadow_march(vec3 ro, vec3 rd, float min_light, float light_dist, float k) {
    float res = 1.0;
    float ph = 1e20;
    float maxt = min(MAX_DIST, light_dist);
    for (float t=min_light; t < maxt;) {
        vec3 p = ro + rd*t;
        float h = GetDist(p);
        if (h<SURF_DIST) {
            return 0.0;
        }
        float y = h*h/(2.0*ph);
        float d = sqrt(h*h-y*y);
        res = min( res, k*d/max(0.0,t-y) );
        ph = h;
        t += h;
    }
    return res;
}

float RayMarch(vec3 ro, vec3 rd) {
	float dO=0.;
    
    for(int i=0; i<MAX_STEPS; i++) {
    	vec3 p = ro + rd*dO;
        float dS = GetDist(p);
        dO += dS;
        if(dO>MAX_DIST || dS<SURF_DIST) break;
    }
    
    return dO;
}

vec3 GetNormal(vec3 p) {
	float d = GetDist(p);
    vec2 e = vec2(.01, 0);
    
    vec3 n = d - vec3(
        GetDist(p-e.xyy),
        GetDist(p-e.yxy),
        GetDist(p-e.yyx));
    
    return normalize(n);
}

vec3 wall_light_pos() {
    return vec3(WALL_X-2.5, 5, 8);
}

vec3 lighta_pos() {
    return vec3(0.0, 20, beat_phase()*MAX_DIST*2.0);
}

float GetLight(vec3 p, vec3 lp) {
    vec3 l = normalize(lp-p);
    vec3 n = GetNormal(p);
    
    float dif = clamp(dot(n, l), 0., 1.);
    float d = RayMarch(p+n*SURF_DIST*2., l);
    if(d<length(lp-p)) dif *= .1;
    float min_light = 0.2;
    float light_dist = 1.0;
    float k = 0.5;
    float s = shadow_march(p+n*SURF_DIST*2., l, min_light, light_dist, k);

    
    return dif*s;
}



void main()	{
	vec2 uv = (gl_FragCoord.xy-0.5 * RENDERSIZE.xy)/RENDERSIZE.y;

 vec3 col = vec3(0);
    
    vec3 ro = vec3(0, 1, 0);
    vec3 rd = normalize(vec3(uv.x, uv.y, 1));

    float d = RayMarch(ro, rd);
    
    vec3 p = ro + rd * d;
    
    float light = 1.0;
    
    const int MODE_WALL_LIGHT_FLICKER = 0;
	const int MODE_WALL_LIGHT_PHASE = 1;
	const int MODE_OVERHEAD_LIGHT_PHASE = 2;

    
    if (light_mode() == MODE_WALL_LIGHT_FLICKER) {
    	light = pow(GetLight(p, wall_light_pos()), 2.0) * flicker() * WALL_LIGHT_AMP;
    } else if (light_mode() == MODE_WALL_LIGHT_PHASE) {
        light = pow(GetLight(p, wall_light_pos()), 2.0) * (1.0 - quaver_phase()) * WALL_LIGHT_AMP;
    } else if (light_mode() == MODE_OVERHEAD_LIGHT_PHASE) {
        light = pow(GetLight(p, lighta_pos()), 1.0) * LIGHTA_AMP;
    }
    
    float fade_dist = 1.0 - pow(d / MAX_DIST, 2.);
    float dif = light * fade_dist;
    col = vec3(dif);
    
        
    gl_FragColor = vec4(col,1.0);
}