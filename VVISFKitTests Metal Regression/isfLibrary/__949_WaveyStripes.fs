/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
  	"generator",
    "stripes",
    "contours",
    "iterations",
    "waves"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ],
      "DEFAULT": [
        0.0,
        0.0
      ],
      "NAME":   "center",
      "TYPE":   "point2D"
    },
    {
      "NAME":   "zoom",
      "TYPE":   "float",
      "DEFAULT": 2.0,
      "MIN":    0.0,
      "MAX":    4.0
    },
    {
      "NAME":   "density",
      "TYPE":   "float",
      "DEFAULT": 8.0,
      "MIN":    0.0,
      "MAX":    20.0
    },
    {
      "NAME":   "seed",
      "TYPE":   "float",
      "DEFAULT": 57.0,
      "MIN":    1.0,
      "MAX":    100.0
    },
    {
      "NAME":   "rate",
      "TYPE":   "float",
      "DEFAULT": 0.25,
      "MIN":    0.01,
      "MAX":    1.0
    },
    {
      "NAME":   "vertical",
      "TYPE":   "bool",
      "DEFAULT":  true
    },
    {
      "NAME":   "autopan",
      "TYPE":   "bool",
      "DEFAULT":  false
    }    
  ],
    "ISFVSN" : "2"
}*/



////////////////////////////////////////////////////////////
// WaveyStripes  by mojovideotech
//
// based on :
// shadertoy.com/wlsfRn  by Inigo Quilez (IQ) 
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define   ag     0.4142135623730951  // silver ratio
#define   twpi   6.283185307179586    // two pi, 2*pi

#define   T     TIME*rate

float hash( in float n ) { return fract(sin(n)*43758.5453); }

float noise( in vec2 p ) {
    float S1 = floor(seed), S2 = S1+1.0;
    vec2 i = floor(p), f = fract(p);
    f = f*f*(3.0-2.0*f);
    float n = i.x + i.y*S1;
    return mix(mix( hash(n+ 0.0), hash(n+ 1.0),f.x),
               mix( hash(n+S1), hash(n+S2),f.x),f.y);
}

vec2 map( in vec2 p, in float time ) {
    for( int i=0; i<4; i++ ){
      float a = noise(p*ag)*twpi + T;
      p += 0.1*vec2(cos(a), sin(a));
    }
    return p;
}

float height( in vec2 p, in vec2 q ) {
    float h = dot(p-q,p-q);
    h += 0.005*noise(p);
    return h;
}
 

void main() 
{
    vec3 col = vec3(0.0);
    float asp = max(RENDERSIZE.x,RENDERSIZE.y);
    vec2 uv = (gl_FragCoord.xy-RENDERSIZE.xy)/asp;
    float t = T * 0.25;
    if (autopan) uv -= normalize(vec2(sin(t),cos(t)))-0.5;
      else uv -= center;
    uv *= 6.0-zoom;
    if (vertical) uv.xy = uv.yx;  
    vec2 p = uv.yx;
    vec2 q = map(p,T);
    float w = (density+10.0)*q.x;
    float u = floor(w), f = fract(w);
    col = vec3(0.7,0.55,0.5) + 0.3*sin(3.0*u+vec3(0.0,1.5,2.0));
    float sha = smoothstep(0.0,0.5,f)-smoothstep(0.8,1.0,f);
    vec2  eps = vec2(asp*0.5,0.0);
    float l2c = height(q,p);
    float l2x = height(map(p+eps.xy,T),p) - l2c;
    float l2y = height(map(p+eps.yx,T),p) - l2c;
    vec3  nor = normalize(vec3(l2x, eps.x, l2y));   
    col *= 0.3+0.7*sha;
    col *= 0.8+0.2*vec3(1.0,0.9,0.3)*dot(nor,vec3(0.7,0.3,0.7));
    col += 0.3*pow(nor.y,8.0)*sha;

  gl_FragColor = vec4(col, 1.0 );
}
