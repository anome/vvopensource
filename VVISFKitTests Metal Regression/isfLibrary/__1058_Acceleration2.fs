/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
  		"generator",
  		"2d"
  	],
  "INPUTS": [
    {
      "MAX": [
        300,
        200
      ],
      "MIN": [
        10,
        6
      ],
      "DEFAULT": [
        100,
        50
      ],
      "NAME": "grid",
      "TYPE": "point2D"
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT":800,
      "MIN": -900,
      "MAX": 1800
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN":0.0,
      "MAX": 9
    }
  ]
}*/

///////////////////////////////////////////
// BitStreamer  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
//
// based on :
// www.patriciogonzalezvivo.com/2015/thebookofshaders/10/ikeda-03.frag
//
// from :
// thebookofshaders.com  by Patricio Gonzalez Vivo
///////////////////////////////////////////
 
float ranf(in float x) {
    return fract(sin(x)*1e4);
}

float rant(in vec2 st) { 
    return fract(sin(dot(st.xy, vec2(55.0,89.0)))*514229.0);
}

float pattern(vec2 st, vec2 v, float t) {
    vec2 p = floor(st+v);
    return step(t, rant(100.+p*.000001)+ranf(p.x)*0.5 );
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    st *= grid;
    
    vec2 ipos = floor(st);  
    vec2 fpos = fract(st);  
    vec2 vel = vec2(TIME*rate*max(grid.x,grid.y)); 
    vel *= vec2(-1.,0.0) * ranf(1.0+ipos.y); 
    vec2 off1 = vec2(0.0,0.);
    vec2 off2 = vec2(0.0,0.);
    vec3 color = vec3(0.);
    color.r = pattern(st+off1,vel,0.0+density/RENDERSIZE.x);
    color.g = pattern(st,vel,0.5+density/RENDERSIZE.x);
    color.b = pattern(st-off2,vel,1.0+density/RENDERSIZE.x); 
    color *= step(0.2,fpos.y);

    gl_FragColor = vec4(color,1.0);
}