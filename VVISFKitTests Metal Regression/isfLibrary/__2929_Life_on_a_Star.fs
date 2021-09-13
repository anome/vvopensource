/*{
	"DESCRIPTION": "",
	"CREDIT": "Silvia Fabiani",
	"ISFVSN": "2",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
	{
      "NAME" : "Rouge",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.9,
      "MIN" : 0.0
    },
    {
      "NAME" : "Vert",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.6,
      "MIN" : 0.0
    },
    {
      "NAME" : "b",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.9,
      "MIN" : 0.0
    },
    {
      "NAME" : "angle",
      "TYPE" : "float",
      "MAX" : 12.0,
      "DEFAULT" : 2.5,
      "MIN" : 0.001
    },
    {
      "NAME" : "moving",
      "TYPE" : "float",
      "MAX" : 2.0,
      "DEFAULT" : 1.0,
      "MIN" : 0.001
    },
    {
      "NAME": "spin",
      "TYPE": "bool"
    },
    {
      "NAME": "Red",
      "TYPE": "bool"
    },
    {
      "NAME": "Yellow",
      "TYPE": "bool"
    }	
			]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265359
#define TWO_PI 6.28318530718

// Reference to
// http://thndl.com/square-shaped-shaders.html

void main(){
  vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;

  vec3 color = vec3(0.0);
  float d = 1.0;
  float round = (sin (TIME/2.0))/3.0;

  // Remap the space to -1. to 1.
  st = st * 1.8 - moving;

  // Number of sides of your shape
  int N = 1;

  // Angle and radius from the current pixel
  float a = atan(st.x,st.y)*TWO_PI;
  float Radius = (round / 0.6) + (TWO_PI/float(N));
  // Shaping function that modulate the distance
  if (spin) d = cos(floor(angle  + a-Radius)*Radius-a)*length(st/2.);

	else  d = cos(floor(angle  * a-Radius)*Radius-a)*length(st/2.);

  
  color = vec3(smoothstep((round-0.5),0.1,d));
  // color = vec3(d);

  gl_FragColor = vec4(color,1.0);
	gl_FragColor.b *= b ;
	if (Yellow) gl_FragColor.g  *= Radius ;
	else  gl_FragColor.r *= Vert  ;
	 if (Red) gl_FragColor.r *= Radius ;
	else  gl_FragColor.r *= Rouge ;

	
}


