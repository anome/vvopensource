/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "based on https://www.shadertoy.com/view/ltBfWt",
	"CATEGORIES": [
		"equirectangular"
	],
	"INPUTS": [
	]
}*/

////////////////////////////////////////////////////////////
// Equirec_WeirdTerrain  by mojovideotech
//
// based on :
// shadertoy.com/ltBfWt
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	EPSILON .01	
#define 	twpi  	6.283185307 	// two pi, 2*pi
#define 	pi   	3.141592653 	// pi


vec3 eye;

vec3 skyColor(vec3 pos) { return mix(vec3(1., 1., 0.), vec3(1., 0., 0.), pos.y); }

vec2 hash2(vec2 p) {
  	vec3 p3 = fract(vec3(p.xyx) * vec3(0.1271, 0.3117, 0.1833));
 	p3 *= dot(p3, p3.yzx+19.19);
	vec2 o = fract(vec2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
 	o = 0.5 + sin(o*twpi);
	return o;
}

float noise(vec2 st) {
    vec2 i = floor(st), f = fract(st);
    vec2 u = f*f*(3.0-2.0*f);
    return mix( mix( dot( hash2(i + vec2(0.0,0.0) ), f - vec2(0.0,0.0) ), 
                     dot( hash2(i + vec2(1.0,0.0) ), f - vec2(1.0,0.0) ), u.x),
                mix( dot( hash2(i + vec2(0.0,1.0) ), f - vec2(0.0,1.0) ), 
                     dot( hash2(i + vec2(1.0,1.0) ), f - vec2(1.0,1.0) ), u.x), u.y);
}

float f(vec2 pos) { return noise(pos.yx * .1) * 50. * (pos.y - (eye.z + 3.))/ 40.; }

vec3 getNormal(vec3 p) {
    vec3 n = vec3(f(vec2(p.x - EPSILON, p.z)) - f(vec2(p.x + EPSILON, p.z)),
        		2.0 * EPSILON,
        		f(vec2(p.x, p.z - EPSILON)) - f(vec2(p.x, p.z + EPSILON)));
    return normalize(n);  
}

mat4 lookAt(vec3 eye, vec3 target, vec3 up) {
    vec3 f = normalize(target - eye);
    vec3 r = normalize(cross(f, up));
    vec3 u = normalize(cross(r, f));
    return mat4(
        vec4(r, 0.),
        vec4(u, 0.),
        vec4(-f, 0.),
        vec4(0., 0., 0., 1.));
}

bool castRay(vec3 ro, vec3 rd, out float resT) {
    const float mint = 0.001;
    const float maxt = 80.0;
    const float dt = 0.5;
    float lh = 0.0, ly = 0.0;
    float t = mint;
    for (float t = mint; t < maxt; t += dt) {
        vec3 p = ro + rd * t;
        float h = f(vec2(p.x, p.z));
        if (p.y < h) {
            resT = t - dt + dt * (lh - ly) / (p.y - ly - h + lh);
            return true;
        }
        lh = h;
        ly = p.y;
    }
    return false;
}

vec3 getShading(vec3 p, vec3 normal, vec3 light) {
	vec3 diffuseColor = vec3(0.0, 0.7, 0.3);
    return max(0., dot(normal, light)) * diffuseColor;
}

vec3 terrainColor(vec3 pos, vec3 eye) { return getShading(pos, getNormal(pos), vec3(0., 1., 0.)) *  (1.5 - (pos.z - eye.z) / 30.); }

void main() {
	vec2 uv = (2. * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
	vec2 sph = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5) * vec2(twpi, pi);
   	vec3 rd = vec3(sin(sph.x)*cos(sph.y), sin(sph.y), cos(sph.x)*cos(sph.y));
	eye = vec3(1., 12., 1.0-TIME*3.0);
    
    vec3 dir = normalize(lookAt(eye, vec3(0., 11.,  1.0+TIME), vec3(0., 1., 0.)) * vec4(rd, 0.)).xyz;
    float resT;
    
    if (castRay(eye, dir, resT)) 
    {
        gl_FragColor = vec4(terrainColor(eye + dir * resT, eye),1.0);
    }
    else 
    {
        gl_FragColor = vec4( skyColor(vec3(uv, 0.0)),1.0);
    }
}
