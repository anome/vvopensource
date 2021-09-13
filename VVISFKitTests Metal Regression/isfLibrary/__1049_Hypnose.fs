/*
{
  "CATEGORIES" : [
    "generator"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "Radius",
      "TYPE" : "float",
      "MAX" : 5.0,
      "DEFAULT" : 0.5,
      "MIN" : 0.2
    },
    {
      "NAME" : "Radius2",
      "TYPE" : "float",
      "MAX" : 5.0,
      "DEFAULT" : 0.5,
      "MIN" : 0.3
    }
  ],
  "CREDIT" : "SilviaFabiani"
}
*/
#ifdef GL_ES
precision mediump float;
#endif
#define PI 3.14159265
void main(){
  vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
  st.x *= RENDERSIZE.x/RENDERSIZE.x;
    vec3 color = vec3(0.);
    
  float d = 0.0;

  // Remap the space to -1. to 1.
  st = st * 2.0-1.0;

  // Make the distance field
  d = length( (abs(st)-(cos(Radius*TIME -1.0)))/(sin(Radius2*TIME -1.0)) );

  //d = length( max(abs(st)-.3,0.3) );

  // Visualize the distance field
 gl_FragColor = vec4(vec3(fract(d*3.0)),1.0);

  // Drawing with the distance field
  //gl_FragColor = vec4(vec3( step(.3,d) ),1.0);
  //gl_FragColor = vec4(vec3( step(.3,d) * step(d,.4)),1.0);
 //gl_FragColor = vec4(vec3( smoothstep( Radius*2.0,Radius2,d)* smoothstep( Radius,Radius2,d)) ,1.0);
}