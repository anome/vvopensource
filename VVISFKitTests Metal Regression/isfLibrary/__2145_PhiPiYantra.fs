/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [
   {
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 3.89,
			"MIN": -16,
			"MAX": 16
		}
  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#29138.6"
}
*/

// PhiPiYantra by mojovideotech 

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
#define 	cupi  	1.464591887561523        // cube root of pi
#define 	prpi 	1.439619495847591 	// pi root of pi
#define 	rcpipi 	0.027425693123298 	// reciprocal of pipi  , 1/pipi

void main( void ) {
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy ) - vec2(0.5,0.5);
	float r = (position.x*position.x+position.y*position.y);
	position = vec2(position.x/r, position.y/r);
	float T = (TIME + 1000.) / rate;
	float color = 0.0;
	float t2 = (T * rcpipi)/9973.0;
	float t2e = phiphi*exp(t2-floor(t2));
	color += (sin( position.x*t2e * cos( T /pepi ) * 79.0 ) + cos( position.y*t2e * cos( T / twpi ) * 13.0 ))*(1.0-cos(t2*2.0*pi));
	t2 /= prpi;
	t2e -= prphi*exp(t2-floor(t2));
	color /= (sin( position.y*t2e * sin( T / shpi ) * 43.0 ) + sin( position.x*t2e * sin( T / prpi ) * 37.0 ))*(1.0-cos(t2*2.0*pi));
    t2 /= cupi;
	t2e -= phicu*exp(t2-floor(t2));
	color += (sin( position.x*t2e * sin( T / chpi ) * 101.0 ) + sin( position.y*t2e * sin( T / pisq ) * 83.0 ))*(1.0-cos(t2*2.0*pi));
	r *= 61.0;
	color *= r/log2(200.0-r/inversesqrt(r));
	gl_FragColor.ragb = vec4( vec3( 1.0-color, color / 1.25, sin( color + (T/601.0) * picu ) * 1.25 ), 1.0 );
}