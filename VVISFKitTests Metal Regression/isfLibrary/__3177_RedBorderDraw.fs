/*
{
  "CREDIT": "by mojovideotech",
  "ISFVSN": "2",
  "CATEGORIES" : [
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  {
    "NAME" :      "size",
    "TYPE" :      "float",
    "DEFAULT" :   0.075,
    "MIN" :       0.01,
    "MAX" :       0.5
  },
  {
    "NAME" :      "rate",
    "TYPE" :      "float",
    "DEFAULT" :   0.333,
    "MIN" :       0.125,
    "MAX" :       1.0
  },
  {
   	"NAME" : 		"auto",
     "TYPE" : 		"bool",
     "DEFAULT" : 	true
   },
   {
    "NAME" :      "draw",
    "TYPE" :      "float",
    "DEFAULT" :   1.0,
    "MIN" :       0.0,
    "MAX" :       1.0
  }
  ]
}
*/

////////////////////////////////////////////////////////////////////
// RedBorderDraw  by mojovideotech
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////


#ifdef GL_ES
precision mediump float;
#endif


void main() 
{
    float TT = (4.0*(sin(TIME*rate)-1.0)+2.0)+5.0;
    float T;
    if (auto) T = clamp(TT,0.0,4.0);
    else T = draw*4.0;
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
    vec2 ss = vec2(size*min(RENDERSIZE.x,RENDERSIZE.y))/RENDERSIZE.xy;
    float top = step(clamp(T-uv.x,0.0,1.0), step(ss.y, 1.0-uv.y));
    float right = step(uv.y-(1.0-clamp(T,1.0,2.0))-1.0, step(ss.x, 1.0-uv.x));
    float bottom = step(uv.x-(2.0-clamp(T,2.0,3.0))-1.0, step(ss.y, uv.y));
    float left = step(clamp(T-uv.y,3.0,4.0)-3.0, step(ss.x, uv.x));
    float c = float (top*right*bottom*left);
    gl_FragColor = vec4(0.8941,0.0,0.1176,1.0-c);
}