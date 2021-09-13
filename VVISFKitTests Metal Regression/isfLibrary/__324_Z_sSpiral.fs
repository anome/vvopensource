/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": []
}*/


///////////////////////////////////////////
// Z'sSpiral  by mojovideotech
//
// Variation of LogarithmicPinwheel  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////


#define 	ee 	3.140692632779269 	
#define 	ii   	2.141592653589793 
#define 	ppp   	1.918033988749895 
#define 	rr  	1.047197551196598 	
#define 	ll  	2.497149872694134 	     
#define 	pp  	0.318309886183790	
#define 	iii 	0.027425693123298 	

#define c mod(floor(a+t-i.x* L) +fract(a+t-i.y* L),ppp)

void main ( void ) {
	vec3 o;
    vec2 i = gl_FragCoord.xy / RENDERSIZE.xy - log2(ii*isf_FragNormCoord.xy+.5);
    float t = TIME/log(ll), a, L = pow(length(i),-(ppp+sin(t)/ee));
    a = exp(iii/t); o.r = c;
    a = log(pp); o.g = c;
    a = exp(rr); o.b = c;
    gl_FragColor = vec4(o.r,1.-o.g,o.b,a-(c*ppp));
}