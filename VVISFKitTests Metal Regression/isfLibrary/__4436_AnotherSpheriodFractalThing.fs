/*{
	"CREDIT": "by eshack94",
  "CATEGORIES" : [
    "generator",
    "fractal"
  ],
  "INPUTS" : [
  	{
		"NAME" : 		"range",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.23,
		"MIN" : 		0.05,
		"MAX" : 		1.0
	},
  	{
		"NAME" : 		"cycle",
		"TYPE" : 		"float",
		"DEFAULT" : 	-1.07,
		"MIN" : 		-3.0,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.86,
		"MIN" : 		-5.0,
		"MAX" : 		5.0
	},
	{
      	"NAME" : 		"CX",
      	"TYPE" : 		"float",
      	"DEFAULT" : 	-0.6,
      	"MIN" : 		-1.0,
      	"MAX" :  		1.0
    },
	{
      	"NAME" : 		"CY",
      	"TYPE" : 		"float",
      	"DEFAULT" : 	-0.8,
      	"MIN" : 		-1.0,
      	"MAX" :  		1.0
    },
   	{
      	"NAME" : 		"CZ",
      	"TYPE" : 		"float",
      	"DEFAULT" : 	0.1,
      	"MIN" : 		-1.0,
      	"MAX" :  		1.0
    },
	{
    	"NAME" : 		"RX",
     	"LABEL" : 		"X rotate",
     	"TYPE" :		"float",
      		"MIN" :		-6.2831853,
      		"MAX" :		6.2831853,
      	"DEFAULT" : 	-3.14159
    },
   	{
     	"NAME" : 		"RY",
      	"LABEL" : 		"Y rotate",
      	"TYPE" : 		"float",
      		"MIN" :		-6.2831853,
      		"MAX" :		6.2831853,
      	"DEFAULT" : 	1.5707963
   	},
   	{
      	"NAME" : 		"RZ",
      	"LABEL" : 		"Z rotate",
      	"TYPE" : 		"float",
      		"MIN" :		-6.2831853,
      		"MAX" :		6.2831853,
      	"DEFAULT" : 	-1.5707963
    },
    {
		"NAME" : 		"morph",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.76,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	}
  ],
  "DESCRIPTION" : ""
}
*/


////////////////////////////////////////////////////////////
// AnotherSpheriodFractalThing  by eshack94
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi   	3.141592653589793  // pi
#define 	twpi  	6.283185307179586  	// two pi, 2*pi


float t=TIME*rate;

vec2 B(vec2 a) { return vec2(log(length(a)),atan(a.y,a.x)-twpi); }

vec3 rotate(vec3 vec, vec3 axis, float ang) {
	vec3 N = vec3(dot(vec2(cos(ang), dot(axis, vec)), vec2(sin(ang), dot(axis, vec))));
	vec3 M = cross(axis, vec) * vec3(sin(ang), dot(axis, vec), 1.0 - cos(ang));
    return vec * mix(N,M,morph);
}

vec3 rot(vec3 vec, vec3 axis, float ang) {
	vec3 N = vec3(dot(vec2(cos(ang), dot(axis, vec)), vec2(sin(ang), dot(axis, vec))));
	vec3 M = cross(axis, vec) * vec3(sin(ang));
    return vec * mix(N,M,morph);
}

vec3 spin(vec3 v) {
    for(int i = 1; i <5; i++) {
		vec3 q = (vec3(sin(v.x),sin(v.y+CY/pi),sin(v.z+CZ/pi))*0.5+0.5).zxy;
    	v=(3.145*rot((v),q,float(i*i))*float(i)+q);
		v=(vec3(sin(v.x+CX/pi),sin(v.y+CY/pi),sin(v.z))*0.5+0.5).yzx;
    }
    return (v.xyz);
}

vec3 F(vec2 E, float G) {
	vec2 e_=E;
	float c=0.;
	const int i_max=40;
	for(int i=0; i<i_max; i++) {
		e_=B(vec2(e_.x,abs(e_.y)))+vec2(.1*sin(t/3.)-.1,5.+.1*-cos(t/5.));
		c += length(e_);
	}
	float d = log2(log2(c*.25))*9.;
	return vec3(.5+.75*cos(d),.3+.67*cos(d-TIME*cycle),.1+.95*cos(G));
}

void main(void)
{	
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	float th =  uv.t * pi, ph = uv.s * twpi;
	vec3 p = vec3(sin(th) * cos(ph),  sin(th) * sin(ph), cos(th));
	float sax = sin(RX);
	float cax = cos(RX);
  	float say = sin(RY);
  	float cay = cos(RY);
  	float saz = sin(RZ);
  	float caz = cos(RZ);
    vec3 H = vec3((cay * p.x + say * p.z)+(caz * p.x - saz * p.y),(cax * p.y - sax * p.z)+(saz * p.x + caz * p.y),(sax * p.y + cax * p.z)+(-say * p.x + cay * p.z));
gl_FragColor=vec4(spin(range*F(H.zx,H.y)),1.);
}

