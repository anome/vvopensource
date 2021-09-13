/*
{   "CREDIT": "by mojovideotech",
    "ISFVSN": "2",
    "CATEGORIES": [
        "generator",
        "truchet"
    ],
    "DESCRIPTION": "toroidal segments in random order at the vertex positions of triangular grid cells",
    "INPUTS": [
    {
        "NAME": "shape",
        "TYPE": "long",
        "VALUES": [     0,
                        1,
                        2   
                ],
        "LABELS": [     "circle",
                        "hex",
                        "dodeca"
                ],
        "DEFAULT":      0
    },
    {
        "NAME" :        "scale",
        "TYPE" :        "float",
        "DEFAULT" :     1.75,
        "MIN" :         0.5,
        "MAX" :         2.0
    },
    {
        "NAME" :        "rot",
        "TYPE" :        "float",
        "DEFAULT" :     0,
        "MIN" :         0,
        "MAX" :         360
    },
    {
        "NAME" :        "thickness",
        "TYPE" :        "float",
        "DEFAULT" :     0.2125,
        "MIN" :         0.1,
        "MAX" :         0.25
    },
    {
        "NAME" :        "scroll",
        "TYPE" :        "point2D",
        "DEFAULT" :     [ 0.0, -0.25 ],
        "MAX" :         [ 0.5, 0.5 ],
        "MIN" :         [ -0.5, -0.5 ]
    },
    {
        "NAME" :        "gradient",
        "TYPE" :        "point2D",
        "DEFAULT" :     [ -0.33, 0.33 ],
        "MAX" :         [ 0.5, 0.5 ],
        "MIN" :         [ -0.5, -0.5 ]
    },
    {
        "NAME" :        "shadow",
        "TYPE" :        "float",
        "DEFAULT" :     0.5,
        "MIN" :         0.0,
        "MAX" :         1.0
    },
    {
        "NAME" :        "rate",
        "TYPE" :        "float",
        "DEFAULT" :     2.0,
        "MIN" :         0.0,
        "MAX" :         5.0
    },
   {
        "NAME" :        "delta",
        "TYPE" :        "float",
        "DEFAULT" :     1.0,
        "MIN" :         0.0,
        "MAX" :         2.0
    },
   {
        "NAME" :        "seed",
        "TYPE" :        "float",
        "DEFAULT" :     113.0,
        "MIN" :         3.0,
        "MAX" :         359.0
    }
  ]
}

*/


////////////////////////////////////////////////////////////
// SimplexTruchetWeave  by mojovideotech
//
// based on :
// shadertoy.com/4ltyRn  by Shane
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define     twpi    6.283185307179586   // two pi, 2*pi
#define     twthpi  0.261799387799149    // twelfth pi, pi/12, 15dfeg


float T = TIME * rate;

mat2 rot2(in float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

vec2 hash22(vec2 p) { 
    float n = sin(dot(p, vec2(1, floor(seed))));
    p = fract(vec2(2097152.0, 262144.0)*n);
    return abs(cos(p*twpi + T))*2.0-1.0; 
}

float hash21(vec2 p){
    float n = dot(p, vec2(127.183, 157.927));
    n = fract(sin(n)*43758.5453);
    return sin(n*twpi + TIME*delta)*0.5 + 0.5;
}
 
float dist(vec2 p){   
    if (shape == 0) return length(p); // Circle.
    else if (shape == 1) { p = abs(p*rot2(twthpi));
        return max(p.y*0.8660254 + p.x*0.5, p.x); // Hexagon.
        }
    else 
        p = abs(p*rot2(twthpi));
        vec2 p2 = p*0.8660254 + p.yx*0.5;
    return max(max(p2.x, p2.y), max(p.x, p.y)); // Dodecahedron.
}

vec3 simplexWeave(vec2 p){    
    vec2 oP = p;
    const float gSc = 5.0, sf = 0.0025, lw = 0.005;;
    p *= gSc;
    vec2 s = floor(p + (p.x + p.y)*0.36602540378); 
    p -= s - (s.x + s.y)*0.211324865; 
    float i = p.x < p.y? 1.0 : 0.0;
    vec2 ioffs = vec2(1.0 - i, i);
    vec2 p1 = p - ioffs + 0.2113248654, p2 = p - 0.577350269; 

    vec3 d = max(0.5 - vec3(dot(p, p), dot(p1, p1), dot(p2, p2)), 0.0);
    vec3 w = vec3(dot(hash22(s), p), dot(hash22((s + ioffs)), p1), dot(hash22(s + 1.0), p2));
    float noise = clamp(0.5 + dot(w, d*d*d)*12.0, 0.0, 1.0);    
    vec3 h = vec3(hash21(s), hash21((s + ioffs)), hash21(s + 1.0));
    vec3 a = vec3(atan(p.y, p.x), atan(p1.y, p1.x), atan(p2.y, p2.x));
    float tw = thickness;
    if (shape == 1) { tw *= 0.8; }
    float mid = dist((p2 - p))*0.5;
    vec3 cir = vec3(dist(p), dist(p1), dist(p2));
    vec3 tor =  abs(cir - mid) - tw;
    tor /= gSc;
    cir /= gSc;
    float dh = hash21((s + s + ioffs + s + 1.0));
    if(dh<0.166666667){ tor = tor.xzy; a = a.xzy; }
    else if(dh<0.333333333){ tor = tor.yxz; a = a.yxz; }
    else if(dh<0.5){ tor = tor.yzx; a = a.yzx; }
    else if(dh<0.666666667){ tor = tor.zxy; a = a.zxy; }
    else if(dh<0.833333333){ tor = tor.zyx; a = a.zyx; }

    vec3 bg = vec3(0.075, 0.125, 0.2)*(shadow*noise+(1.-shadow));
    bg *= clamp(cos((oP.x - oP.y)*twpi*128.0)*1.0, 0.0, 1.0)*0.15 + 0.925;
    vec3 col = bg;
    vec3 rimCol = vec3(1.0, 0.7, 0.5);
    rimCol = mix(rimCol, rimCol*(smoothstep(0.0, 0.75, noise - 0.1) + 0.5), shadow); 
    vec3 torCol = vec3(0.2, 0.4, 1.0);
    a = clamp(cos(a*48.0 + T*0.0)*1.0 + 0.5, 0.0, 1.0)*0.25 + 0.75;
    vec3 cc = max(0.05 - tor*32.0, 0.0);
    cc *= clamp(cos(tor*twpi*80.0)*1.0 + 0.5, 0.0, 1.0)*0.25 + 0.75;

    col = mix(col, vec3(0.0), (1.0 - smoothstep(0.0, sf*4.0, tor.x - 0.0))*0.5);
    col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, tor.x));
    col = mix(col, rimCol*cc.x, 1.0 - smoothstep(0.0, sf, tor.x + lw));
    col = mix(col, torCol*col.x*a.x, 1.0 - smoothstep(0.0, sf, tor.x + 0.015));
    col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, abs(tor.x + 0.015)));
    
    col = mix(col, vec3(0.0), (1.0 - smoothstep(0.0, sf*4.0, tor.y - 0.0))*0.5);
    col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, tor.y));
    col = mix(col, rimCol*cc.y, 1.0 - smoothstep(0.0, sf, tor.y + lw)); 
    col = mix(col, torCol*col.x*a.y, 1.0 - smoothstep(0.0, sf, tor.y + 0.015));
    col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, abs(tor.y + 0.015)));

    col = mix(col, vec3(0.0), (1.0 - smoothstep(0.0, sf*4.0, tor.z - 0.0))*0.5);
	col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, tor.z));
    col = mix(col, rimCol*cc.z, 1.0 - smoothstep(0.0, sf, tor.z + lw));
    col = mix(col, torCol*col.x*a.z, 1. - smoothstep(0., sf, tor.z + 0.015));
    col = mix(col, vec3(0.0), 1.0 - smoothstep(0.0, sf, abs(tor.z + 0.015)));

    return col;
}

void main() 
{
	vec2 uv = (gl_FragCoord.xy/RENDERSIZE.y)-0.5;
    uv *= 3.0-scale;
    vec2 p = uv + vec2(0.8660254, 0.5)*TIME*0.25*-scroll; 
    p *= rot2(radians(rot)-twthpi); 
    vec3 col = simplexWeave(p);
    col = mix(col, col.yzx, -uv.y*gradient.y + gradient.y);
    col = mix(col, col.zxy, -uv.x*gradient.x + gradient.x);

    gl_FragColor = vec4(sqrt(clamp(col, 0.0, 1.5)), 1.0);
}
