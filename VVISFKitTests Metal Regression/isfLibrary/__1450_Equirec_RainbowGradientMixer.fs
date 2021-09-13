/*{
 "CREDIT": "by mojovideotech",
 "CATEGORIES" : [
    "color",
    "gradient",
    "rainbow",
    "equirectangular"
  ],
  "DESCRIPTION" : "equirectangular version of RainbowGradientMixer",
  "INPUTS" : [
   	{
		"NAME": 	"blend",
		"TYPE": 	"float",
		"DEFAULT":	1.0,
		"MIN": 		-0.5,
		"MAX": 		1.5
	},
	{
      	"NAME": 	"Rx",
      	"TYPE": 	"float",
      	"MIN": 		-6.2831853,
      	"MAX": 		6.2831853,
      	"DEFAULT":	3.1415926
    },
    {
      	"NAME": 	"Gx",
      	"TYPE": 	"float",
      	"MIN": 		-6.2831853,
      	"MAX": 		6.2831853,
      	"DEFAULT":	-1.0
    },
    {
      	"NAME": 	"Bx",
      	"TYPE": 	"float",
      	"MIN": 		-6.2831853,
      	"MAX": 		6.2831853,
      	"DEFAULT":	-6.2831853
    },
    {
   		"NAME": 	"flip",
     	"TYPE": 	"bool",
     	"DEFAULT": 	false
   	}
  ]
}
*/

////////////////////////////////////////////////////////////
// Equirec_RainbowGradientMixer   by mojovideotech
//
// mod of 
// shadertoy.com/ltVXW3  by Loeizd
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	hfpi  	1.570796326794897 	// half pi, pi/2


void main() 
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float th = uv.y * pi, ph = uv.x * twpi;
	vec3 s = vec3(sin(th) * cos(ph), cos(th), sin(th) * sin(ph));
	if (flip) s.x = s.z;
    float a = blend * pi - hfpi;
    vec3 m = abs(s.x * twpi - vec3(Rx,Gx,Bx));
    m = pi - m;
    m += a;
    m = smoothstep(0.0, pi, m);
    
    gl_FragColor = sqrt(vec4(m, 1.0));
}
