/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	]
}*/

// LogarithmicPinwheel by mojovideotech

#define 	pepi 	23.140692632779269005729086367949 	// powe(pi)
#define 	pi   	3.1415926535897932384626433832795 	// pi
#define 	phi   	1.6180339887498948482045868343656 	// golden ratio
#define 	hfpi  	1.5707963267948966192313216916398 	// half pi
#define 	trpi  	1.0471975511965977461542144610932 	// one third of pi
#define 	lgpi  	0.4971498726941338543512682882909 	// log(pi)      
#define 	rcpi  	0.31830988618379067153776752674503	// reciprocal of pi 
#define 	rcpipi 	0.0274256931232981061195562708591 	// reciprocal of pipi

#define c mod(floor(a+t-i.x* L) +floor(a+t-i.y* L),hfpi)

void main ( void )
{
	vec3 o;
    vec2 i = gl_FragCoord.xy / RENDERSIZE.xy - log2(pi*vv_FragNormCoord.xy);
    float t = TIME*log(lgpi), a, L = pow(length(i),-(phi+sin(t)/pepi));
    a = exp(rcpipi); o.r = c;
    a = exp(rcpi); o.g = c;
    a = exp(trpi); o.b = c;
    gl_FragColor = vec4(o.r,o.g,o.b,a-(c*phi));
}