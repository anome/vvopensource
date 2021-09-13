/*{
	"CREDIT": "Silvia",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
	{
		"NAME" : 		"rotation",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"Radiant",
		"TYPE" : 		"float",
		"DEFAULT" : 	-0.5,
		"MIN" : 		-1.5,
		"MAX" : 		0.5
	},
	{
		"NAME" : 		"Degrade",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.5,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"Rr",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"Gg",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"Bb",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		3.0
	}					
	]
}
*/

#ifdef GL_ES
precision mediump float;
#endif

#define TWO_PI 6.28318530718


float TT =clamp (TIME, 0.01, 5.0);
vec3 hsb2rgb( in vec3 c ){
    vec3 rgb = clamp(abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),
 6.0)-3.0)-Rr, Gg,Bb );
    rgb = rgb*rgb*(Degrade -4.0*rgb);
    return c.z * mix( vec3(1.0), rgb, c.y);
}
void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE;
    vec3 color = vec3(0.0);
    vec2 toCenter = vec2 (
    //(cos 
    (TT *0.1)
    //)
    -st);
    float angle = atan (
    //(cos 
    (Radiant+TT /10.0)
    //) 
    -toCenter.y,toCenter.x);
    float radius = length(toCenter);    color = hsb2rgb(vec3((angle/TWO_PI)
    + sin (rotation *TIME)
    ,radius,1.0));
    gl_FragColor = vec4(color,1.0);
}

