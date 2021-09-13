/*{
	"DESCRIPTION": "",
	"CREDIT": "Silvia Fabiani",
	"ISFVSN": "2",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
	{
      "NAME" : "r",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.05,
      "MIN" : 0
    },
    {
      "NAME" : "g",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.6,
      "MIN" : 0
    },
    {
      "NAME" : "b",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.8,
      "MIN" : 0
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
  float round = (sin (TIME/4.0))/3.0;

  // Remap the space to -1. to 1.
  st = st *2.-1.;

  // Number of sides of your shape
  int N = 1;

  // Angle and radius from the current pixel
  float a = atan(st.x,st.y)+TWO_PI;
  float r = (round*2.0) * (TWO_PI/float(N));

  // Shaping function that modulate the distance
  d = cos(floor(0.3+a/r)*r-a)*length(st);

  color = vec3(smoothstep((round-0.3),0.5,d));
 color = vec3(d);

  gl_FragColor = vec4(color,1.0);
	gl_FragColor.b *= b ;
	gl_FragColor.g *= g ;
	gl_FragColor.r *= r ;
}


