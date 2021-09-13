/*
{   "CREDIT": "by mojovideotech",
    "CATEGORIES": [
        "generator"
    ],
    "DESCRIPTION": "",
    "ISFVSN": "2",
    "INPUTS": [
        {
            "NAME" :    "rate",
            "TYPE" :    "float",
            "DEFAULT":  0.5,
            "MIN" :     0.0,
            "MAX" :     2.0
        }
    ]
}

*/


////////////////////////////////////////////////////////////////////
// AnthropomorphicVirons  by mojovideotech
//
// based on :
// shadertoy.com//XdtyRr  by ShadowX
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////


#define S(a, b) smoothstep(-aa, aa, b-(a))
#define si(a) (sin(a)+1.0)*2.0
#define sh(a) (sin(a)+1.0)*1.5


mat2 r2D(float a) { return mat2(sin(a), cos(a), -cos(a), sin(a)); }

float BG(vec2 uv) { return 1.5-length(uv); }

float Eye(vec2 uv, float s, float r, float a, float t, float aa) {
    uv *= r2D(r);
    uv.x *= a;
    return 1.0-S(s, length(uv));
}

float Creature(vec2 uv, float s, float t, float aa){
    vec2 st = uv;
    st.x += cos(uv.y * 13.0 + t) * 0.008;
    st.y += sin(uv.x * 15.0 - t) * 0.006;
    aa = 0.025;
    float ss = floor(s*50.0)+9.0;
    float body = S(s, length(st)+abs(sin(atan(st.y,st.x)*ss)*0.35*s));
    s *= 0.85;
    float x1pos =  0.20*s + sh(t)*0.01;
    float x2pos = -0.27*s + sh(t)*0.01;
    float ypos = 0.01;
    float an = -1.2; 
    aa = 0.002;
    float ea = si(t*2.2);
    float eye1  = Eye(uv-vec2(x1pos, ypos), 0.21*s+ea*0.0016, an, 1.3, t, aa);
    eye1 -= Eye(uv-vec2(x1pos+cos(t*1.1)*0.12*s, -0.015+ypos+sin(t*1.2)*0.01), 0.07*s+ea*0.0011, an, 1.0, t, aa);
    float eye2  = Eye(uv-vec2(x2pos, ypos), 0.22*s+ea*0.0017, -an, 1.3, t, aa);
    eye2 -= Eye(uv-vec2(x2pos+cos(t*1.1+12.0)*0.12*s, -0.015+ypos+sin(t*1.2+12.0)*0.01), 0.07*s+ea*0.0012, an, 1.0, t, aa);
    body += eye1 + eye2;
    return body;
}

void main() {
    vec2 uv = (2.0*gl_FragCoord.xy - RENDERSIZE)/max(RENDERSIZE.x,RENDERSIZE.y);
    float t = TIME*rate;
    float aa;
    float creatures = Creature(uv-vec2(-0.05+sin(-t)*0.02, cos(t)*0.02), 0.33, t, aa);
    creatures *= Creature(uv+vec2(0.55+sin(t)*0.04, 0.255+cos(-t)*0.03), 0.25, t*0.8, aa);
    creatures *= Creature(uv+vec2(-0.425+sin(-t)*0.03, -0.3+sin(t)*0.02), 0.23, t*1.2, aa);
    creatures *= Creature(uv+vec2(0.5+sin(t)*0.02, -0.325-cos(t)*0.03), 0.18, t*0.6, aa);
    creatures *= Creature(uv+vec2(-0.7+cos(-t)*0.03, 0.233+cos(t)*0.04), 0.19, t*0.9, aa);
    creatures *= Creature(uv+vec2(0.8+sin(t)*0.04, -0.1+cos(t)*0.04), 0.16, t*0.7, aa);
    creatures *= Creature(uv+vec2(-0.35+cos(t)*0.03, 0.35+cos(-t)*0.04), 0.15, t*1.1, aa);
    creatures *= Creature(uv+vec2(-0.8+sin(-t)*0.05, -0.15+cos(t)*0.04), 0.14, t*1.0, aa);
    float col = BG(uv)*creatures;
    vec3 color = vec3(col);
    gl_FragColor = vec4(color,1.0);
}
