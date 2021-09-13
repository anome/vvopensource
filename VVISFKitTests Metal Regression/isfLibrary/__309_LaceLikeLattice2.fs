/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "2d",
    "iterations",
    "fractal",
    "lines",
    "lace-like"
  ],
  "DESCRIPTION": "",
  "ISFVSN" : "2",
  "VSN" : "2",
  "INPUTS": [
    {
      "NAME": "center",
      "TYPE": "point2D",
      "DEFAULT": [
        0.0,
        0.0
      ],
      "MAX": [
        1.0,
        1.0
      ],
      "MIN": [
        -1.0,
        -1.0
      ]
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "vectorX",
      "TYPE": "float",
      "DEFAULT": 3.141592653,
      "MIN": 0,
      "MAX": 6.283185307
    },
    {
      "NAME": "vectorY",
      "TYPE": "float",
      "DEFAULT": 0.523598775,
      "MIN": -0.785398163,
      "MAX": 0.785398163
    },
    {
      "NAME": "rot",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 6.283185307
    },
    {
      "NAME": "width",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.015,
      "MAX": 1.5
    },
    {
      "NAME": "push",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": -2.0,
      "MAX": 2.0
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 52.0,
      "MIN": 1.0,
      "MAX": 200.0
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 11,
      "MIN": 0,
      "MAX": 19.9
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 31,
      "MIN": 7,
      "MAX": 36
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": 0.025,
      "MAX": 1
    },
    {
      "NAME": "hue",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 1,
      "MAX": 10
    }
  ]
}*/

////////////////////////////////////////////////////////////
// LaceLikeLattice by mojovideotech
// v2.0 optimized code 2/2020
//
// based on :
// String Theory by nimitz 
// www.shadertoy.com/XdSSz1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


float T = TIME * rate;

mat2 mm2(float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float f(vec2 q) {
  q.y = sin(q.y*1.0+T*1.23)*cos(T+q.y*0.05); 
  q += sin(q.y*0.05)*0.01;
  return smoothstep(-0.01,width,abs(q.x));
}

void main() 
{
  vec2 uv = (2.0*gl_FragCoord.xy - RENDERSIZE.xy)/max(RENDERSIZE.x,RENDERSIZE.y); 
  vec2 p = uv.yx-center.yx;
  p *= mm2(rot)*zoom; 
  vec3 col = vec3(0.0), col2 = vec3(0.0);
  vec2 pa, pb;
  float counter = depth;
  for(float i=0.0;i<36.0;i++) {
    p.y -= 20.0 - detail;
    p.x -= sin(T*0.125+push)*1.5+1.5;
    p*= mm2(i*vectorY+vectorX);
    pa = vec2(abs(p.y-1.5),abs(p.x));
    pb = vec2(p.x,abs(p.y));
    p = mix(pa,pb,smoothstep(0.5,0.1,abs(sin(T*0.05)+0.5)));
    col2 = (sin(vec3(3.0+hue,21.0,0.02941*hue)+i*color)*color+(1.0-color)+0.125)*(1.0-f(p));
    col = max(col,col2);
    counter -= 1.0;
    if (counter<1.0) break; 
  }
  
  gl_FragColor = vec4(col,1.0);
}