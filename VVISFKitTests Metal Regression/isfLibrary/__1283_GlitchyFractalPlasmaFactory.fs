/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
  "CATEGORIES" : [
    "Generator",
    "Fractal",
    "Glitch"
  ],
  "INPUTS" : [
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" :    	0.667,
		"MIN" : 		0.005,
		"MAX" : 		1.5
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	2.167,
		"MIN" : 		-3.0,
		"MAX" :   		3.0
	},
	{
		"NAME" : 		"center",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.0, 0.0 ],
		"MAX" : 		[ 10.0, 10.0 ],
     	"MIN" : 		[ -10.0, -10.0 ]
	},
    {
      	"NAME" : 		"c1",
      	"TYPE" : 		"color",
      	"DEFAULT" :		[ 0.1, 0.0, 0.3, 1.0 ]
    },
    {
      	"NAME" : 		"c2",
      	"TYPE" : 		"color",
      	"DEFAULT" :		[ 0.5, 0.6, 0.7, 1.0 ]
   	},
   	{
		"NAME" : 		"loops",
		"TYPE" : 		"float",
		"DEFAULT" :		24.0,
		"MIN" : 		16.0,
		"MAX" : 		30.0
	},
	{
		"NAME" : 		"cycle",
		"TYPE" : 		"float",
		"DEFAULT" : 	6.5,
		"MIN" : 	 	6.0,
		"MAX" : 		8.0
	},
	{
		"NAME" : 		"depth",
		"TYPE" : 		"float",
		"DEFAULT" : 	16.0,
		"MIN" : 	 	3.0,
		"MAX" : 		36.0
	},
	{
      	"NAME" :		"offset1",
      	"TYPE" :		"float",
     	"DEFAULT" :		0.5,
      	"MIN" :			0.1,
      	"MAX" :	 		0.9
   	},
    { 
      	"NAME" :		"offset2",
      	"TYPE":			"float",
      	"DEFAULT" :		35.0,
      	"MIN" :			5.0,
      	"MAX" :			50.0
   	},
	{
		"NAME" : 		"nudge",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.7,
		"MIN" : 		0.5,
		"MAX" : 		0.9
	},
	{
		"NAME" : 		"fudge",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.1,
		"MIN" : 		0.05,
		"MAX" : 		0.125
	}
  ]
}
*/

////////////////////////////////////////////////////////////
// GlitchyFractalPlasmaFactory  by mojovideotech
//
// based on :
// glslsandbox.com/\e#42715.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

float T = (TIME*rate)/66.0;

vec2 B(vec2 a) { return vec2(log(length(a)),atan(a.y,a.x)-cycle); }

vec3 F(vec2 e) {
	float c = depth, o = loops, n = nudge, f = fudge;
	for(int i=0; i<30; i++) {
		e = B(vec2(e.x,abs(e.y)))+vec2(f*sin(T/3.0)-f,(5.0+offset1)+f*cos(T/5.0));
		c += length(e);
		o -= 1.0;
		if (o<=0.0) {break;}
	}	
	float d = log2(log2(c/offset2))*depth;
	return vec3(n+tan(n*cos(d)),0.5+0.5*cos(d-n),n+sin(n*cos(d-n)));
}

void main(void) {
	vec2 uv = (gl_FragCoord.xy /min(RENDERSIZE.x, RENDERSIZE.y))+center.xy;
	uv *= scale;
	vec4 col = c1; 
	col += vec4(F(vec2(dot(F(uv.xy).zx,F(vec2(uv.y,cos(T))).yz),cos(9.0-9.1*sin(-T)))),1.0);
	gl_FragColor = clamp(sqrt(col),vec4(0.0),c2);
}




















