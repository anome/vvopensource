/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/NdSGDG by BigWIngs.  Lets make a Newtons Cradle",
    "IMPORTED": {
        "iChannel0": {
            "NAME": "iChannel0",
            "PATH": [
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58.jpg",
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58_1.jpg",
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58_2.jpg",
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58_3.jpg",
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58_4.jpg",
                "585f9546c092f53ded45332b343144396c0b2d70d9965f585ebc172080d8aa58_5.jpg"
            ],
            "TYPE": "cube"
        }
    },
    "INPUTS": [
        {
            "NAME": "iMouse",
            "TYPE": "point2D"
        }
    ]
}

*/


// "Newton's Cradle - Part 2" 
// by Martijn Steinrucken aka The Art of Code/BigWings - 2021
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// Email: countfrolic@gmail.com
// Twitter: @The_ArtOfCode
// YouTube: youtube.com/TheArtOfCodeIsCool
// Facebook: https://www.facebook.com/groups/theartofcode/
// Patreon: https://www.patreon.com/TheArtOfCode
// ShaderToy: https://www.shadertoy.com/user/BigWIngs
// PayPal Donation: https://paypal.me/theartofcode
//
// This shader is the end result of part 1 of my Newton's Cradle tutorial on YouTube
// Watch it here:
// https://youtu.be/nd7Auhb9YN8

#define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST .001

#define S smoothstep
#define T TIME

const int MAT_BASE=1;
const int MAT_BARS=2;
const int MAT_BALL=3;
const int MAT_LINE=4;

mat2 Rot(float a) {
    float s=sin(a), c=cos(a);
    return mat2(c, -s, s, c);
}

float sdBox(vec3 p, vec3 s) {
    p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, max(p.y, p.z)), 0.);
}

float sdBox(vec2 p, vec2 s) {
    p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, p.y), 0.);
}

float sdLineSeg(vec3 p, vec3 a, vec3 b) {
    vec3 ap=p-a, ab=b-a;
    float t = clamp(dot(ap, ab)/dot(ab, ab), 0., 1.);
    vec3 c = a + ab*t;
    return length(p-c);
}

vec2 sdBall(vec3 p, float a) {
    
    p.y-=1.01;
    p.xy *= Rot(a);
    p.y+=1.01;
    
    float ball = length(p)-.15;
    float ring = length(vec2(length(p.xy-vec2(0, .15))-.03, p.z))-.01;
    ball = min(ball, ring);
    
    p.z = abs(p.z);
    float line = sdLineSeg(p, vec3(0,.15,0), vec3(0, 1.01, .4))-.005;
    
    float d = min(ball, line);
    
    return vec2(d, d==ball ? MAT_BALL : MAT_LINE);
}

vec2 Min(vec2 a, vec2 b) {
    return a.x<b.x ? a : b;
}

vec2 GetDist(vec3 p) {
    float base = sdBox(p, vec3(1,.1,.5))-.1;
    float bar = length( vec2(sdBox(p.xy, vec2(.8,1.4))-.15, abs(p.z)-.4) )-.04;
    
    float 
        a = sin(TIME*3.),
        a1 = min(0., a),
        a5 = max(0., a);
    
    vec2 
        b1 = sdBall(p-vec3(.6,.5,0), a1),
        b2 = sdBall(p-vec3(.3,.5,0), (a+a1)*.05),
        b3 = sdBall(p-vec3(0,.5,0), a*.05),
        b4 = sdBall(p-vec3(-.3,.5,0), (a+a5)*.05),
        b5 = sdBall(p-vec3(-.6,.5,0), a5);
    
    vec2 balls = Min(b1, Min(b2, Min(b3, Min(b4, b5))));
    
    float d = min(base, bar);
    d = min(d, balls.x);
    
    base = max(base, -p.y);
    d = max(d, -p.y); // cut off the bottom
    
    int mat = 0;
    
    if(d==base)
        mat = MAT_BASE;
    else if(d==bar)
        mat = MAT_BARS;
    else if(d==balls.x)
        mat = int(balls.y);
    
    return vec2(d, mat);
}

vec2 RayMarch(vec3 ro, vec3 rd) {
	float dO=0.;
    vec2 dSMat = vec2(0);
    
    for(int i=0; i<MAX_STEPS; i++) {
    	vec3 p = ro + rd*dO;
        dSMat = GetDist(p);
   
        dO += dSMat.x;
        if(dO>MAX_DIST || abs(dSMat.x)<SURF_DIST) break;
    }
    
    return vec2(dO, dSMat.y);
}

vec3 GetNormal(vec3 p) {
	float d = GetDist(p).x;
    vec2 e = vec2(.001, 0);
    
    vec3 n = d - vec3(
        GetDist(p-e.xyy).x,
        GetDist(p-e.yxy).x,
        GetDist(p-e.yyx).x);
    
    return normalize(n);
}

vec3 GetRayDir(vec2 uv, vec3 p, vec3 l, float z) {
    vec3 f = normalize(l-p),
        r = normalize(cross(vec3(0,1,0), f)),
        u = cross(f,r),
        c = f*z,
        i = c + uv.x*r + uv.y*u,
        d = normalize(i);
    return d;
}

void main() {



    vec2 uv = (gl_FragCoord.xy-.5*RENDERSIZE.xy)/RENDERSIZE.y;
	vec2 m = iMouse.xy/RENDERSIZE.xy;
    vec3 ro = vec3(0, 3, -3);
    ro.yz *= Rot(-m.y*3.14+1.);
    ro.xz *= Rot(-m.x*6.2831);
    
    vec3 rd = GetRayDir(uv, ro, vec3(0,0.75,0), 2.);
    vec3 col = textureCube(iChannel0,rd).rgb;
   
    vec2 dMat = RayMarch(ro, rd);
    if(dMat.x<MAX_DIST) {
        vec3 p = ro + rd * dMat.x;
        vec3 n = GetNormal(p);
        vec3 r = reflect(rd, n);
        vec3 ref = textureCube(iChannel0,r).rgb;
        
        float dif = dot(n, normalize(vec3(1,2,3)))*.5+.5;
        col = vec3(dif);
        
        int mat = int(dMat.y);//GetMat(p);
        
        if(mat==MAT_BASE)
            col = .1*ref;
        else if(mat==MAT_BARS)
            col = ref;
        else if(mat==MAT_BALL)
            col = ref;
        else if(mat==MAT_LINE)
            col *= .05;
    }
    
    col = pow(col, vec3(.4545));	// gamma correction
    
    gl_FragColor = vec4(col,1.0);
}
