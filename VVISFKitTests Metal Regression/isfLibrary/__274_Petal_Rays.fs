// #SaturdayShader Week 5 : Petal Rays
// by Joseph Fiola (http://www.joefiola.com)
// 2015-09-12
// Based on Polar Shapes example by Patricio Gonzalez Vivo on http://patriciogonzalezvivo.com/2015/thebookofshaders/07/


/*{
  "CREDIT": "Joseph Fiola",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "Generator",
    "Blobs",
    "Shapes"
  ],
  "INPUTS": [
  {
      "NAME" : "RED",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.8,
      "MIN" : 0.0
          },
    {
      "NAME" : "GREEN",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" : 0.0
          },
    {
      "NAME" : "BLUE",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.1,
      "MIN" : 0.0
          },
    {
      "NAME": "radius",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 20
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 20
    },
    {
      "NAME": "inner",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "outer",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "sinValue",
      "TYPE": "float",
      "DEFAULT": -7.0,
      "MIN": -50,
      "MAX": 50
    },
    {
      "NAME": "cosValue",
      "TYPE": "float",
      "DEFAULT": 19.0,
      "MIN": -50,
      "MAX": 50
    },
    {
      "NAME": "fade",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 2
    },
    {
      "NAME": "pinchPoint",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.4
      ],
      "MIN": [
        -1,
        -1
      ],
      "MAX": [
        1,
        1
      ]
    },
    {
      "NAME": "location",
      "TYPE": "point2D",
      "DEFAULT": [
        0.0,
        0.0
      ]
    }
  ]
}*/

#ifdef GL_ES
precision mediump float;
#endif
#define TWO_PI 6.28318530718

void main(){
	
    vec2 st = gl_FragCoord.xy/RENDERSIZE;
    vec3 color = vec3(0.0);
    
    vec2 pos = location/RENDERSIZE-st;
     vec2 toCenter = vec2 ((cos (TIME *0.1))-st);

    float r = length(pos)*(radius); // radius
    float a = atan(pos.y+pinchPoint.y,pos.x+pinchPoint.x); 
    
    
    float mSpeed = speed * TIME;

    float f = tan(cos(a*cosValue + mSpeed)*sin(a*sinValue+mSpeed)) *outer+inner;
 

    color = vec3( 1.-smoothstep(f,f+fade,r) );
    gl_FragColor = vec4(color, 2.0);
     gl_FragColor = vec4(color, 1.0);
     gl_FragColor = vec4(color, 1.0);
    gl_FragColor.b *= BLUE;
	gl_FragColor.g *= GREEN;
	gl_FragColor.r *= RED;

}
