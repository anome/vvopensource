/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"LABEL": "PLANET RADIUS",
			"NAME": "PLANET_RADIUS",
			"TYPE": "float",
			"DEFAULT": 0.28,
			"MIN": 0.1,
			"MAX": 1.0
		},
		{
			"NAME": "PLANETCOLOR",
			"TYPE": "color",
			"DEFAULT": [
			1.0,
			0.0,
			0.0,
			1.0
			]
		},
		{
			"LABEL": "PLANETDAY",
			"NAME": "PLANETDAY",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "PLANETSHADOW",
			"NAME": "PLANETSHADOW",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "SOLARMASS",
			"NAME": "SOLARMASS",
			"TYPE": "float",
			"DEFAULT": 0.96,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "REDNESS",
			"NAME": "REDNESS",
			"TYPE": "float",
			"DEFAULT": 0.07,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "GREENNESS",
			"NAME": "GREENNESS",
			"TYPE": "float",
			"DEFAULT": 0.06,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "BLUENESS",
			"NAME": "BLUENESS",
			"TYPE": "float",
			"DEFAULT": 0.34,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "CORONALAYERED",
			"NAME": "CORONALAYERED",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "CORONASPIKED",
			"NAME": "CORONASPIKED",
			"TYPE": "float",
			"DEFAULT": 0.19,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "ACTIVENESS",
			"NAME": "ACTIVENESS",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "PUFFY",
			"TYPE": "color",
			"DEFAULT": [
			0.1,
			0.1,
			0.3,
			1.0
			]
		},
		{
			"LABEL": "PUFFYNESS",
			"NAME": "PUFFYNESS",
			"TYPE": "float",
			"DEFAULT": 0.24,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "PUFFYSCALE",
			"NAME": "PUFFYSCALE",
			"TYPE": "float",
			"DEFAULT": 0.12,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "STREAKY",
			"TYPE": "color",
			"DEFAULT": [
			0.0,
			0.5,
			0.5,
			1.0
			]
		},
		{
			"LABEL": "STREAKYNESS",
			"NAME": "STREAKYNESS",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "STREAKYSCALE",
			"NAME": "STREAKYSCALE",
			"TYPE": "float",
			"DEFAULT": 0.14,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "STARGAMMA",
			"NAME": "STARGAMMA",
			"TYPE": "float",
			"DEFAULT": 0.33,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "BIGSTARSCALE",
			"NAME": "BIGSTARSCALE",
			"TYPE": "float",
			"DEFAULT": 0.51,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "BIGSTARS",
			"NAME": "BIGSTARS",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "SMALLSTARS",
			"NAME": "SMALLSTARS",
			"TYPE": "float",
			"DEFAULT": 0.48,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Mouse X",
			"NAME": "mX",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "Mouse Y",
			"NAME": "mY",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"LABEL": "FOV",
			"NAME": "FOV",
			"TYPE": "float",
			"DEFAULT": 60.0,
			"MIN": 30.0,
			"MAX": 120.0
		}
	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;
vec2 iMouse = vec2(mX*RENDERSIZE.x, mY*RENDERSIZE.y);

// Star map shader...procedural space background.
// Original code "Star Map 1" by morgan3D: https://www.shadertoy.com/view/4sBXzG
// Tweaked, added controls and animated the cloud map on the planet.

#define SHOW_PLANET

const float pi = 3.1415927;
const float deg = pi / 180.0;

// See derivation of noise functions by Morgan McGuire at https://www.shadertoy.com/view/4dS3Wd
const int NUM_OCTAVES = 6;

float hash(float n) { return fract(sin(n) * 1e4); }
float hash(vec2 p) { return fract(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }
// 1 octave value noise
float noise(float x) { float i = floor(x); float f = fract(x); float u = f * f * (3.0 - 2.0 * f); return mix(hash(i), hash(i + 1.0), u); }
float noise(vec2 x) { vec2 i = floor(x); vec2 f = fract(x);	float a = hash(i); float b = hash(i + vec2(1.0, 0.0)); float c = hash(i + vec2(0.0, 1.0)); float d = hash(i + vec2(1.0, 1.0)); vec2 u = f * f * (3.0 - 2.0 * f); return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y; }
float noise(vec3 x) { const vec3 step = vec3(110, 241, 171); vec3 i = floor(x); vec3 f = fract(x); float n = dot(i, step); vec3 u = f * f * (3.0 - 2.0 * f); return mix(mix(mix( hash(n + dot(step, vec3(0, 0, 0))), hash(n + dot(step, vec3(1, 0, 0))), u.x), mix( hash(n + dot(step, vec3(0, 1, 0))), hash(n + dot(step, vec3(1, 1, 0))), u.x), u.y), mix(mix( hash(n + dot(step, vec3(0, 0, 1))), hash(n + dot(step, vec3(1, 0, 1))), u.x), mix( hash(n + dot(step, vec3(0, 1, 1))), hash(n + dot(step, vec3(1, 1, 1))), u.x), u.y), u.z); }
// Multi-octave value noise
float NOISE(float x) { float v = 0.0; float a = 0.5; float shift = float(100); for (int i = 0; i < NUM_OCTAVES; ++i) { v += a * noise(x); x = x * 2.0 + shift; a *= 0.5; } return v; }
float NOISE(vec2 x) { float v = 0.0; float a = 0.5; vec2 shift = vec2(100); mat2 rot = mat2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50)); for (int i = 0; i < NUM_OCTAVES; ++i) { v += a * noise(x); x = rot * x * 2.0 + shift; a *= 0.5; } return v; }
// Fast hash2 from https://www.shadertoy.com/view/lsfGWH
float hash2(vec2 co) { return fract(sin(dot(co.xy, vec2(12.9898,78.233))) * 43758.5453); }
float maxComponent(vec2 v) { return max(v.x, v.y); }
float maxComponent(vec3 v) { return max(max(v.x, v.y), v.z); }
float minComponent(vec2 v) { return min(v.x, v.y); }
mat3 rotation(float yaw, float pitch) { return mat3(cos(yaw), 0, -sin(yaw), 0, 1, 0, sin(yaw), 0, cos(yaw)) * mat3(1, 0, 0, 0, cos(pitch), sin(pitch), 0, -sin(pitch), cos(pitch)); }
float square(float x) { return x * x; }

///////////////////////////////////////////////////////////////////////

// Only globals needed for the actual spheremap
float screenscale = 1.0 / iResolution.x;
float time = iGlobalTime;

// starplane was derived from https://www.shadertoy.com/view/lsfGWH
float starplane(vec3 dir) { 
    // Project to a cube-map plane and scale with the resolution of the display
    vec2 basePos = dir.xy * (0.5 / screenscale) / max(1e-3, abs(dir.z));
         
	float largeStarSizePixels = BIGSTARSCALE*100.;
    
    // Probability that a pixel is NOT on a large star. Must change with largeStarSizePixels
	float prob = 1.0-(BIGSTARSCALE*100.)/(BIGSTARS*5000.);
    	
	float color = 0.0;
	vec2 pos = floor(basePos / largeStarSizePixels);
	float starValue = hash2(pos);
    
    // Big stars
	if (starValue > prob) {

        // Sphere blobs
		vec2 delta = basePos - largeStarSizePixels * (pos + vec2(.5));
		color = max(1.0 - length(delta) / (.5 * largeStarSizePixels), 0.0);
		
        // Star shapes
        color *= BIGSTARSCALE*100./(101. - BIGSTARSCALE*100.) / max(1e-3, abs(delta.x) * abs(delta.y));
        
        // Avoid triplanar seams where star distort and clump
        color *= pow(abs(dir.z), 100.*STARGAMMA / 1.5);
    } 

    // Small stars

    // Stabilize stars under motion by locking to a grid
    basePos = floor(basePos);

    if (hash2(basePos.xy * screenscale) > (1.0-SMALLSTARS*.002)) {
        float r = hash2(basePos.xy * 0.5);
        color += r * (.5 * sin(time * (r * 5.0) + r) + 0.7) * 1.5;
    }
	
    // Weight by the z-plane
    return color * abs(dir.z)*(10.0*STARGAMMA); //FACTOR is gamma
}


float starbox(vec3 dir) {
	return starplane(dir.xyz) + starplane(dir.yzx) + starplane(dir.zxy);
}    


float starfield(vec3 dir) {
    return starbox(dir) + starbox(rotation(45.0 * deg, 45.0 * deg) * dir);
}


vec3 nebula(vec3 dir) {
    float purple = abs(dir.x);
    float yellow = noise(dir.z);
    vec3 streakyHue = vec3(STREAKY.r, STREAKY.g, STREAKY.b);
    // vec3 streakyHue = vec3(STREAKY.r,STREAKY.g, STREAKY.b);
    vec3 puffyHue = vec3(PUFFY.r,PUFFY.g, PUFFY.b);

    float streaky = min(1.0, 4.0 * pow(NOISE(dir.yz * square(dir.x) * STREAKYSCALE*100. + dir.xy * square(dir.z) * 7.0 + vec2(150.0, 2.0)), 10.+20.*(1.-STREAKYNESS)));
    float puffy = square(NOISE(dir.xz * PUFFYSCALE *100. + vec2(30, 10)) * dir.y);

    return clamp(puffyHue * puffy * (PUFFYNESS*10. - streaky) + streaky/3. * streakyHue, 0.0, 1.0);
}


vec3 sun(vec3 d) {
    float angle = atan(d.x, d.y);    
    float falloff = pow(max(d.z, 0.2), 1001.0-SOLARMASS*1000.);
	vec3 core = vec3(REDNESS*50. + 0.5 * noise(time * 0.25 + d.xy * 5.0 * REDNESS*50.), GREENNESS*50. + 0.5 * noise(time * 0.25 + d.xy * 5.0 * GREENNESS*50.), BLUENESS*50. + 0.5 * noise(time * 0.25 + d.xy * 5.0 * BLUENESS*50.)) * falloff; 
    float corona = NOISE(vec2(d.z * CORONALAYERED*1000. + time, time * 1.0 + angle * CORONASPIKED*300.)) * smoothstep(0.95, 0.92, d.z) * (falloff/(1.0-ACTIVENESS)) * square(d.z);
    
    return core * (.5 - corona);
}


vec4 planet(vec3 view) {

#ifdef SHOW_PLANET

    if (view.y > -PLANET_RADIUS) {
        return vec4(0.0);
    } 
    
    // Compute the point on the planet sphere
    float angle  = atan(view.x, view.z);
    float radius = sqrt((1.0 + view.y) / (1.0 - PLANET_RADIUS));
    
    vec3 s = vec3(radius * normalize(view.xz), sqrt(1.0 - square(radius)));
    
    
    vec3 dir = s;
    dir = rotation(0.0, time * 0.15) * dir;
    float latLongLine = 0.0;// (1.0 - pow(smoothstep(0.0, 0.04, min(abs(fract(atan(dir.y, length(dir.xz)) / (15.0 * deg)) - 0.5), abs(fract(atan(dir.x, dir.z) / (15.0 * deg)) - 0.5)) * 2.0), 10.0));
    
    // Antialias the edge of the planet
    vec4 surface = vec4(1.2 * vec3(PLANETCOLOR.r,PLANETCOLOR.g,PLANETCOLOR.b) * 
  			(noise(dir * 39.0 + 3.5) * 1.0 + noise(dir * 26.0) + 2.0 * noise(dir * 13.0 + 10.0)) *
         vec3(s.yx * 0.5 + 0.5, 0.5).rbg, smoothstep(0.992, 0.988, radius));

    // Keep the clouds above the planet
    vec4 cloud = vec4(vec3(1.5),
                      smoothstep(155.0, 0.995, radius) * 
                      square(NOISE(vec2(time * 0.1, 3.0) + dir.xy*3. * sin(time/19.)*21.0 * square(dir.x/2.) + dir.yz * cos(time/13.)*3.0 + dir.zy * sin(time/9.)*1.2)));
    
    return vec4(
        mix(surface.rgb, cloud.rgb*2., cloud.a) * (max(PLANETSHADOW/10., s.y) * vec3(PLANETDAY*5. - latLongLine)),
        max(surface.a, cloud.a));
#else
    return vec4(0.0);
#endif
}


vec3 sphereColor(vec3 dir) {
    vec3 n = nebula(dir);
    vec4 p = planet(dir);
    vec3 color = 
        sun(dir) + 
        mix(vec3(starfield(dir)) * (1.0 - maxComponent(n)) +  // Nebula holds out star
    	    n, // nebula
            p.rgb, p.a); // planet
    
	return color;
}


////////////////////////////////////////////////////////////////////////////////////////////////////
// Spheremap visualization code from https://www.shadertoy.com/view/4sSXzG

void mainImage( out vec4 fragColor, in vec2 fragCoord ) {
    float scale = 1.0 / min(iResolution.x, iResolution.y);
	// Of the background
	float verticalFieldOfView = FOV * deg;
	const float insetSphereRadius = .22;

    float yaw   = -((iMouse.x / iResolution.x) * 6.2832);
    float pitch = ((iMouse.y / iResolution.y) * 6.2832);
    
	vec3 dir = rotation(yaw, pitch) * normalize(vec3(fragCoord.xy - iResolution.xy / 2.0, iResolution.y / ( -2.0 * tan(verticalFieldOfView / 2.0))));
    
    fragColor.rgb = sphereColor(dir);
    
    fragColor.rgb = sqrt(fragColor.rgb);
    fragColor.a = 1.0;
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}