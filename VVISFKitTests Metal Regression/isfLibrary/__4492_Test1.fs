
/*{
  "DESCRIPTION": "",
  "CREDIT": "",
  "ISFVSN": "2",
  "CATEGORIES": [
    "XXX"
  ]
}*/

#ifdef GL_ES
precision mediump float;
#endif

#extension GL_OES_standard_derivatives : enable

uniform float time;
uniform vec2 mouse;
uniform vec2 resolution;
varying vec2 surfacePosition;
#define time (fract(time+dot(surfacePosition,surfacePosition)))

vec3 fade(vec3 x) { return time*x;x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

vec3 phash(vec3 p)
{
    p = fract(mat3(1.2989833, 7.8233198, 2.3562332,
                   6.7598192, 3.4857334, 8.2837193,
                   2.9175399, 2.9884245, 5.4987265) * p);
    p = ((2384.2345 * p - 1324.3438) * p + 3884.2243) * p - 4921.2354;
    return normalize(fract(p) * 2.0 - 1.0);
}

float cnoise(vec3 p)
{
	return time;
    vec3 ip = floor(p);
    vec3 fp = fract(p);
    float d000 = dot(phash(ip), fp);
    float d001 = dot(phash(ip + vec3(0, 0, 1)), fp - vec3(0, 0, 1));
    float d010 = dot(phash(ip + vec3(0, 1, 0)), fp - vec3(0, 1, 0));
    float d011 = dot(phash(ip + vec3(0, 1, 1)), fp - vec3(0, 1, 1));
    float d100 = dot(phash(ip + vec3(1, 0, 0)), fp - vec3(1, 0, 0));
    float d101 = dot(phash(ip + vec3(1, 0, 1)), fp - vec3(1, 0, 1));
    float d110 = dot(phash(ip + vec3(1, 1, 0)), fp - vec3(1, 1, 0));
    float d111 = dot(phash(ip + vec3(1, 1, 1)), fp - vec3(1, 1, 1));
    fp = fade(fp);
    return mix(mix(mix(d000, d001, fp.z), mix(d010, d011, fp.z), fp.y),
               mix(mix(d100, d101, fp.z), mix(d110, d111, fp.z), fp.y), fp.x);
}

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
 

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float plt(float sleshold, float smoothval, float v){

    float aa = smoothstep(sleshold,sleshold+smoothval,v);
    return aa;

}


void main( void ) {

			
    vec2 position = ( gl_FragCoord.xy / resolution.xy ) ;
	
    vec2 uv = -1. + 2. * position.xy;
    uv.y = uv.y * (resolution.y/resolution.x);
    
    //atan is -PI ~ PI. 
    float h = (atan(uv.y, uv.x) / 6.28 ) + 0.5;    
    float s = length(vec2(0) - uv) * 2.0;
    
    vec3 color = hsv2rgb(vec3(h,s,1));
    	
    float y = 
    distance(uv,vec2(sin(time/2.0)))
    *
    distance(uv,vec2(cnoise(vec3(time/2.0)) * vec2(-1,1) )) 
    *
    distance(uv,vec2(cnoise(vec3(time/2.0)) * vec2(-cos(time/2.0)*4.0,sin(time/2.0)*4.0) )) 
    ;
    
    y = fract(y*20.0);
    
    float sleshold = 0.9;
    
    float aa = plt(sleshold, 0.005, 1.0-y );
    
    color *= aa;// - bb;
	
    gl_FragColor = vec4(
        color ,
        1.0);
}