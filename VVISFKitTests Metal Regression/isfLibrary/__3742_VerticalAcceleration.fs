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
       10,
       10
       
      ],
      "NAME": "grid",
      "TYPE": "point2D"
    },
    
    {
      "NAME": "Variant",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 1.02,
      "MAX": 2.17
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
    return fract(sin(dot(st.xy, vec2(Variant,(Variant*2.0))))*(Variant*4.0));
}

float pattern(vec2 st, vec2 v, float t) {
    vec2 p = floor(st+v);
    return step(t, rant(100.+p*.000001)+ranf(p.x)*0.5 );
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    st *= (grid*Variant);
    
    vec2 ipos = floor(st);  
    vec2 fpos = fract(st);  
    vec2 vel = vec2(TIME*(Variant*8.0)*max((Variant*16.0),(Variant*32.0))); 
    vel *= vec2(-1.,0.0) * ranf(1.0+ipos.x); 
    //  vel *= vec2(-1.,0.0) * ranf(1.0+ipos.y); 
    //à la ligne précédente, en remplaçant x pyr y on obtient le sens vertical
    vec2 off1 = vec2((Variant*64.0),0.);
    vec2 off2 = vec2((Variant*128.0),0.);
    vec3 color = vec3(0.);
    color.r = pattern(st+off2,vel,1.0+((Variant*256.0))/RENDERSIZE.x);
    color.g = pattern(st+off2,vel,1.0+((Variant*256.0))/RENDERSIZE.x);
    color.b = pattern (st-off2,vel,1.0+(Variant*256.0)/RENDERSIZE.x); 
    color *= step(0.2,fpos.y);
//  color *= step(0.2,fpos.x); //pour obtenir des lignes ininterrompues
    gl_FragColor = vec4(color,1.0);
}