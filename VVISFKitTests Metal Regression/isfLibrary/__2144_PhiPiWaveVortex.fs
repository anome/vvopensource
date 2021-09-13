/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
   "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#29138.6",
  "INPUTS" : [
  		 {
            "NAME": "offsetx",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": -1.0,
            "MAX": 1.0
          },
          {
            "NAME": "offsety",
            "TYPE": "float",
           "DEFAULT": 0.25,
            "MIN": -1.0,
            "MAX": 1.0
          },
  		{
			"NAME": "seed",
			"TYPE": "float",
			"DEFAULT": 26,
			"MIN": -100,
			"MAX": 100
		},
		{
			"NAME": "multiplier",
			"TYPE": "float",
			"DEFAULT": 9,
			"MIN": 1,
			"MAX": 30
		},
		{
			"NAME": "T1",
			"TYPE": "float",
			"DEFAULT": 9,
			"MIN": 1,
			"MAX": 100
		},
		{
			"NAME": "T2",
			"TYPE": "float",
			"DEFAULT": 15,
			"MIN": 1,
			"MAX": 100
		},
		{
			"NAME": "T3",
			"TYPE": "float",
			"DEFAULT": 337,
			"MIN": 100,
			"MAX": 1000
		},
		 {
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 6,
			"MIN": -100,
			"MAX": 100
		},
		 {
            "NAME": "R",
            "TYPE": "float",
           "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 0.9
        },
         {
            "NAME": "G",
            "TYPE": "float",
           "DEFAULT": 0.125,
            "MIN": 0.0,
            "MAX": 0.9
        },
         {
            "NAME": "B",
            "TYPE": "float",
           "DEFAULT": 0.75,
            "MIN": 0.0,
            "MAX": 0.9
        }
    ]
}
*/

// PhiPiWaveVortex by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif

#define 	picu  	31.006276680299820 	// pi cubed, pi^3
#define 	pepi 	23.140692632779269 	// powe(pi);
#define 	chpi 	11.591953275521521  	// cosh(pi)
#define 	shpi 	11.548739357257749 	// sinh(pi)
#define 	pisq  	9.869604401089359	// pi squared, pi^2
#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	phicu  	4.23606797749979 	// phi cubed, phi^3
#define 	pi   	3.141592653589793 	// pi
#define	 	phiphi	2.178457567937601	// phi phied, phi^phi
#define 	prphi	2.028876065463213	// pi root of phi
#define 	hfpi  	1.570796326794897 	// half pi, 1/pi
#define 	cupi  	1.464591887561523        // cube root of pi
#define 	prpi 	1.439619495847591 	// pi root of pi
#define 	rcpipi 	0.027425693123298 	// reciprocal of pipi  , 1/pipi

void main( void ) {
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy ) - vec2(0.5,0.5) + vec2 (offsetx, offsety);
	float r = (position.x*position.x+position.y*position.y);
	position = vec2(position.x/r, position.y/r);
	float color = 0.0;
	float t2 = (TIME / (exp(atan(rcpipi)*seed)*1e1))/rate;
	float t2e = phiphi*exp(t2-floor(t2));
	float M1 = (TIME * T1)/rate;
	color += (sin( position.x*t2e * cos( M1 /pepi ) * 79.0 ) + cos( position.y*t2e * cos( M1 / twpi ) * 13.0 ))*(1.0-cos(t2*2.0*pi));
	t2 /= prpi;
	t2e -= prphi*exp(t2-floor(t2));
	float M2 = (TIME * T2)/rate;
	color /= (sin( position.y*t2e * sin( M2 / shpi ) * 43.0 ) + cos( position.x*t2e * sin( M2 / pisq ) * 37.0 ))*(1.0-cos(t2*2.0*pi));
    t2 /= cupi;
	t2e -= phicu*exp(t2-floor(t2));
	float M3 = (TIME * T3)/rate;
//	color += (sin( position.x*t2e * sin( M3 / chpi ) * 101.0 ) + sin( position.y*t2e * sin( M3 / hfpi ) * 83.0 ))*(1.0-cos(t2*2.0*pi));
    r *= multiplier;
	color *= r/log2(200.0-r/inversesqrt(r));
	vec4 col = vec4( vec3( 1.0-color, color / 1.25, sin( color + (TIME/601.0) * picu ) * 1.25 ), 1.0 );
	col *= vec4(R,G,B,1.0);
	gl_FragColor = vec4 (col);
}