/*{
	"CREDIT": "Silvia Fabiani",
  	"CATEGORIES" : [
  		"generator",
    	"spiral",
    	"logarithmic",
    	"coordinatetransform"
  ],
  "DESCRIPTION" : "Transformation from screen-coordinates to logarithmic spiral",
  "INPUTS" : [
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "col3",
			"TYPE": "float",
			"DEFAULT": 0.8,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bend",
			"TYPE": "float",
			"DEFAULT": -1.11,
			"MIN": -6.0,
			"MAX": 6.0
		},
		{
			"NAME": "split",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 1.0,
			"MAX": 4.0
		}
  ]
}
*/

////////////////////////////////////////////////////////////
// based on:LogTransSpiral  by mojovideotech
//
// based on :
// shadertoy.com/Msd3Dn
// Logarithmic Spiral Transform - 2015-12-02 by Jakob Thomsen
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi  	3.14
#define		fifi	(split * 1.2)	

void main() {
	float T = TIME * rate;
    vec2 p = (gl_FragCoord.xy+gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
	p = vec2(0.0, T - fract(log(exp2(length(p.xy)*bend)))) + atan(p.y, p.x) / pi; 
   	p.x = ceil(p.y) - p.x;
    p.x *= fifi;
    float col1 = fract(p.x-T);
    float col2 = fract(p.y+T);
    gl_FragColor = vec4(col2,col1,col2,1.0);
}
