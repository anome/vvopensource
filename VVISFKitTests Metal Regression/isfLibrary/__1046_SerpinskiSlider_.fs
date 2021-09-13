/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
  	"generator",
  	"serpinski"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
  	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.25,
		"MAX" : 		5.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		-3.0,
		"MAX" : 		3.0
	},
	{
     	"NAME" :		"cycle1",
     	"TYPE" : 		"float",
     	"DEFAULT" :		1.5,
     	"MIN" : 		0.1,
     	"MAX" :			5.0
	},
    {
      	"NAME" :		"cycle2",
      	"TYPE" :		"float",
     	"DEFAULT" :		2.25,
     	"MIN" : 		0.1,
     	"MAX" :			5.0
	},
	{
		"NAME" : 		"freq",
		"TYPE" : 		"float",
		"DEFAULT" : 	40.0,
		"MIN" : 		5.0,
		"MAX" : 		60.0
	},
	{
		"NAME" : 		"depth",
		"TYPE" : 		"float",
		"DEFAULT" : 	50.0,
		"MIN" : 		2.0,
		"MAX" : 		80.0
	},
	{
		"NAME" : 		"fractdim",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.667,
		"MIN" : 		0.01,
		"MAX" : 		0.99
	}
  ]
}*/



////////////////////////////////////////////////////////////
// SerpinskiSlider+  by mojovideotech
//
// based on :
// glslsandbox/e#26868.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


vec3 tri(vec2 p, float r)
{
	p *= r;
	vec2 x = fract(p);
	vec2 y = p - TIME * cycle1;
	y -= p - x - TIME * cycle2;
	vec3 c = vec3(sin(y.x*2.0), cos(y.y*2.0), 0.0)*0.2+0.2;
	return ((x.y < x.x) ? 1.0 : 0.0) * c;
	 x /= TIME*fractdim;
	return (((x.x < x.y) == (x.x < 1.0 - x.y)) ? 1.0 : 0.0) * c;
}

void main( void ) 
{
	vec2 pos = (gl_FragCoord.xy / RENDERSIZE.xx);
	pos = (pos / scale);
	pos -= 0.025 * TIME * rate;
	vec2 s = fract(pos*depth);
	vec2 i = pos*depth - s;
	float l = clamp(0.0, 1.0, (0.5 - abs(s.x-0.5))*freq);
	l *= clamp(0.0, 1.0, (0.5 - abs(s.y-0.5))*freq);
	l *= clamp(0.0, 1.0, abs(s.x - s.y)*freq);
	l *= clamp(0.0, 1.0, abs(1.0 - s.x - s.y)*freq);
	vec3 c = vec3(sin(i.x*0.3), cos(i.y*0.3), 0.0)*0.5+0.5;
	vec3 c2 = vec3(cos(i.x*0.3), sin(i.y*0.3), 0.0)*0.5+0.5;
	vec3 tc = mix(c, c2, s.x > s.y ? 1.0 : 0.0);	
    tc = tri(pos, depth);
	tc += tri(pos, depth*0.25);
	tc += tri(pos, depth*0.125);
	tc += tri(pos, depth*0.5);
	gl_FragColor = vec4(c, 1.0);
	gl_FragColor *= vec4(tc*l, 1.0);
}
