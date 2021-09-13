/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [
	 {
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 6,
			"MIN": -100,
			"MAX": 100
		},
			{
			"NAME": "radius",
			"TYPE": "float",
			"DEFAULT": -40,
			"MIN": -60,
			"MAX": 60
		},
		{
			"NAME": "multiplier",
			"TYPE": "float",
			"DEFAULT": 9,
			"MIN": 1,
			"MAX": 30
		},
		    {
      "MAX": [
        2.0,
        2.0
      ],
      "MIN": [
        -2.0,
        -2.0
      ],
      "DEFAULT":[0.0,0.0],
      "NAME": "offset",
      "TYPE": "point2D"
    },
     {
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 3.3,
			"MIN": 0.1,
			"MAX": 3.5
		},
     {
			"NAME": "zoomwarp",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": 0.0,
			"MAX": 0.25
		},
    {
			"NAME": "rotozoom",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -3,
			"MAX": 3
		},
		{
		     "NAME": "R",
            "TYPE": "float",
           "DEFAULT": 0.31,
            "MIN": 0.0,
            "MAX": 1.5
        },
         {
            "NAME": "G",
            "TYPE": "float",
           "DEFAULT": 0.61,
            "MIN": 0.0,
            "MAX": 1.5
        },
         {
            "NAME": "B",
            "TYPE": "float",
           "DEFAULT": 1.1,
            "MIN": 0.0,
            "MAX": 1.33
        }
  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#29138.9"
}
*/


// PhiPiPlanet by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif

#define     sqfv    2.23606797749979    // sq root of 5
#define 	pepi 	23.140692632779269 	// powe(pi);
#define 	pisq  	9.869604401089359	// pi squared, pi^2
#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	e     	2.718281828459045 	// eulers number
#define 	phisq  	2.6180339887499		// phi squared, phi^2
#define 	sqpi 	1.772453850905516	// square root of pi
#define 	phi   	1.618033988749895 	// golden ratio
#define		erpi   	1.523671054858932   // e root of pi
#define 	cupi  	1.464591887561523   // cube root of pi
#define 	prpi 	1.439619495847591 	// pi root of pi
#define 	phrphi 	1.34636082003487	// phi root of phi
#define 	thpi  	0.996272076220750	// tanh(pi)
#define 	rcphi  	0.61803398874989	// reciprocal of pi  , 1/phi 
#define 	rcpipi 	0.027425693123298 	// reciprocal of pipi  , 1/pipi

void main( void ) {
	vec2 pos = ( gl_FragCoord.xy / RENDERSIZE.xy)* 2.0 - 1.0;
	pos.x *=RENDERSIZE.x/RENDERSIZE.y;
	pos += vec2(offset);
	vec2 position = vec2(pos)*(4.0-zoom);	
	float r = (position.x*position.x+position.y*position.y)+zoomwarp;
	position = vec2((position.x/r)+rotozoom, (position.y/r)+rotozoom);
	vec3 col = vec3 (0.0);
	float T = TIME * 0.25 + (10000. * multiplier);
	float t2 = (atan(T * rcpipi)/9973.0)/rate;
	float t2e = phrphi*exp(t2-floor(t2)/rate);
	float color = (sin( position.x*t2e * cos( T /sqpi ) * 79.0 ) + cos( position.y*t2e * cos( T / twpi ) * 13.0 ))*(1.0-cos(t2*2.0*pi));
	t2 /= prpi;
	t2e -= thpi*exp(t2-floor(t2)/rate);
	color /= (sin( position.y*t2e * sin( T / phisq ) * 43.0 ) + cos( position.x*t2e * sin( T / e ) * 37.0 ))*(1.0-cos(t2*2.0*pi));
    t2 /= cupi;
	t2e -= rcphi*exp(t2-floor(t2)/rate);
	color += (sin( position.x*t2e * sin( T / erpi ) * 101.0 ) + sin( position.y*t2e * sin( T / pisq ) * 83.0 ))*(1.0-cos(t2*2.0*pi));
	r *= pow(phi,sqfv);
	color -= r/log2(pepi-r/inversesqrt(r)*radius);
	color /= 1.0-color;
	r += zoom;
	col.x -= color + sin(T/pi); 
	col.y += color + cos(T/pi); 
	col.z -= color - cos(T/pi);
	col += vec3 (R-(G+B),G-(R+B),B-(R+G));
	gl_FragColor = vec4(vec3(col), 1.0 );
}