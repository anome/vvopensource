/*{
	"CREDIT": "by mojovideotech",
  	"CATEGORIES" : [
  		"generator",
    	"spiral",
    	"logarithmic",
    	"coordinatetransform"
  ],
  "DESCRIPTION" : "Transformation from screen-coordinates to logarithmic spiral with golden angle.",
  "INPUTS" : [
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 1.0
		}
  ]
}
*/

////////////////////////////////////////////////////////////
// LogTransSpiral  by mojovideotech
//
// based on :
// shadertoy.com/Msd3Dn
// Logarithmic Spiral Transform - 2015-12-02 by Jakob Thomsen
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define		piphi	2.39996322972865	// pi*(3-sqrt(5))

void main() {
	float T = TIME * rate;
    vec2 p = (gl_FragCoord.xy+gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
	p = vec2(0.0, T - log2(length(p.xy))) + atan(p.y, p.x) / twpi; 
   	p.x = ceil(p.y) - p.x;
    p.x *= piphi;
    float r = fract(p.x+T);
    float b = fract(p.y+T);
    gl_FragColor = vec4(r,0.0,b,1.0);
}
