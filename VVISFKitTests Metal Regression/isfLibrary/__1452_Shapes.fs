/*{
  "DESCRIPTION": "Shapes",
  "CREDIT": "Patricio Gonzalez Vivo translated by @colin_movecraft",
  "CATEGORIES": [
    "TEST"
  ],
  "INPUTS": [
    {
      "NAME": "sides",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": 3,
      "MAX": 50
    },
    {
      "NAME": "angle",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 360,
      "DEFAULT": 0
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "line",
      "TYPE": "float",
      "DEFAULT": 0.9,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "with",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -2,
      "MAX": 2
    },
    {
      "NAME": "pos",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        0,
        0
      ],
      "MAX": [
        1,
        1
      ]
    },
    {
      "NAME": "para1",
      "TYPE": "float",
      "MIN": -2,
      "MAX": 2,
      "DEFAULT": 0.5
    },
    {
      "NAME": "para2",
      "TYPE": "float",
      "MIN": -2,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "para3",
      "TYPE": "float",
      "MIN": -2,
      "MAX": 2,
      "DEFAULT": 0
    },
    {
      "NAME": "para4",
      "TYPE": "float",
      "MIN": -2,
      "MAX": 2,
      "DEFAULT": 1
    }
  ]
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}


void main(){

 vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st -= vec2(pos);
	st.x *= RENDERSIZE.x/RENDERSIZE.y*with; // 1:1 ratio

	st = rotate2d(angle*0.01745) * st; 


  vec3 color = vec3(0.0);
  float d = 0.0;

  // Remap the space to -1. to 1.
  //st = st *2.-1.;

  // Number of sides of your shape
  int N = int(sides);

  // Angle and radius from the current pixel
  float a = atan(abs(st.x*para4),mod(st.y*para2,para3))+PI;
  float r = TWO_PI/float(N);
  
  // Shaping function that modulate the distance
  d = cos(floor(para1 + a/r) * r - a)*length(st*1.0);

	//It's easier to see what's going on when you uncomment the second line. 
	color = vec3(1.0-smoothstep(.4,.41,d));
	//color = vec3(d);

 gl_FragColor = vec4(vec3( step(zoom*line,d) * step(d,zoom)),1.0);
}