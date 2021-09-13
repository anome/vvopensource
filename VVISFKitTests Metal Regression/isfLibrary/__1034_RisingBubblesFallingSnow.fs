/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator",
    "bubbles",
    "circles",
    "snow"
  ],
  "DESCRIPTION": "",
  "ISFVSN" : "2",
    "INPUTS": [
    {
        "NAME" :    "numbubs",
        "TYPE" :    "float",
        "DEFAULT" : 0.4,
        "MIN" :     0.05,
        "MAX" :     0.5
    },
    {
        "NAME" :    "size",
        "TYPE" :    "float",
        "DEFAULT" : 0.33,
        "MIN" :     0.125,
        "MAX" :     0.5
    },
    {
        "NAME" :    "rate",
        "TYPE" :    "float",
        "DEFAULT" : 0.575,
        "MIN" :     -1.5,
        "MAX" :     1.5
    },
    {
        "NAME" :     "edge",
        "TYPE" :     "float",
        "DEFAULT" :  0.67,
        "MIN" :      0.01,
        "MAX" :      0.99
    },
    {
        "NAME" :    "soft",
        "TYPE" :    "float",
        "DEFAULT" : 0.5,
        "MIN" :     0.333,
        "MAX" :     0.667
    },
    {
        "NAME" :    "density",
        "TYPE" :    "float",
        "DEFAULT" : 0.33,
        "MIN" :     0.0,
        "MAX" :     0.333  
    },
    {
        "NAME" :    "offset",
        "TYPE" :    "float",
        "DEFAULT" : 9.0,
        "MIN" :     -100.0,
        "MAX" :     100.0
    },
    {
        "NAME" :    "skew",
        "TYPE":     "float",
        "DEFAULT" : 1.5,
        "MIN" :     0.0,
        "MAX" :     6.0
    },
    {
        "NAME" :    "zoom",
        "TYPE":     "float",
        "DEFAULT" : 54.0,
        "MIN" :     0.0,
        "MAX" :     64.0
    },
    {
        "NAME" :    "bgcolor",
        "TYPE" :    "color",
        "DEFAULT" : [ 0.0, 0.3, 0.5, 1.0 ]
    }
    ]
}*/


////////////////////////////////////////////////////////////
// RisingBubblesFallingSnow   by mojovideotech
//
// based on : 
// Rising Bubbles  by Avin
// shadertoy.com/WllGRn
//
// License: 
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


float H12(vec2 p) { return fract(sin(dot(p.xy, vec2(12.9898, 78.233)))*43758.5453); } 

float bubbles( vec2 uv, float si, float sp, float tf, float bl, float ti) {       
    vec2 ruv = uv*si  + 0.25;    
    vec2 id = ceil(ruv) + sp;       
    float t = (ti + tf)*sp;
    ruv.y -= t*(H12(vec2(id.x))*0.5 + 0.5)*0.1;
    vec2 guv = fract(ruv) - 0.5;
    ruv = ceil(ruv);    
    float g = length(guv);
    float v = H12(ruv)*size;
    v *= step(v, numbubs);
    float m = smoothstep(v,v - bl, g);    
    v*= edge;
    m -= smoothstep(v,v - 0.1, g);
    g = length(guv - vec2(v*0.332, v*0.334));
    float h = v*0.75;
    m += smoothstep(h, 0.0, g)*0.75;
    return m;        
}

void main() 
{
    vec2 uv = (gl_FragCoord.xy - 0.5*RENDERSIZE.xy)/RENDERSIZE.y;
    float m = 0.0, z = 67.0 - zoom, T = TIME*rate;          
    float i = -1.0, f = (0.333 - density) + 0.125;
    i += f;
    for(float j=0.0; j<=10.0; j+=0.01){
        vec2 iuv = uv + vec2(cos(uv.y*skew + i*20.0 + sin(T*0.05))*0.095, 0.0);
        float s = (i*0.5 + 0.5)*z + z*0.25;
        m += bubbles(iuv + vec2(i*0.1, 0.0), s, abs(rate*200.0) + i*5.0, i*offset, soft + i*0.25, T)*abs(i);
        i += f;
        if (i>=0.0) { break; }
    }     
    vec3 col = bgcolor.rgb + m*0.45;   
    gl_FragColor = vec4(col,bgcolor.a);
}
