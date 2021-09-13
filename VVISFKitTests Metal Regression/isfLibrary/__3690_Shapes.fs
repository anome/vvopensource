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
			"DEFAULT": 3.0,
			"MIN": 3.0,
			"MAX": 50.0
		},
				{
			"NAME": "perspect",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
				{
			"NAME": "para1",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 3.0,
			"MAX": 50.0
		}
	]
			
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

void main(){
	
  vec2 st = gl_FragCoord.xy/RENDERSIZE;
  st.x *= RENDERSIZE.x/RENDERSIZE.y;
  vec3 color = vec3(0.0);
  float d = 0.0;

  // Remap the space to -1. to 1.
  st = st *2.-1.;

  // Number of sides of your shape
  int N = int(sides);

  // Angle and radius from the current pixel
  float a = atan(st.x,st.y)+PI;
  float r = TWO_PI/float(N);
  
  // Shaping function that modulate the distance
  d = cos(floor(.5 + a/r) * r - a )*length(st);

	//It's easier to see what's going on when you uncomment the second line. 
	color = vec3(1.0-smoothstep(.4,0.51,cos(atan(d*perspect)*para1)));
	//color = vec3(d);

  gl_FragColor = vec4(color,1.0);
}