/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES" : [
    "generator"
  ],
    "DESCRIPTION" : "based on http://glslsandbox.com/e#42256.0",
 	"INPUTS" : [
    {
		"NAME" : 		"freq1",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"freq2",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.25,
		"MIN" : 		0.0,
		"MAX" : 		2.0
	},
	{
		"NAME" : 		"loop1",
		"TYPE" : 		"float",
		"DEFAULT" : 	5,
		"MIN" : 		3,
		"MAX" : 		7
	},
	{
		"NAME" : 		"loop2",
		"TYPE" : 		"float",
		"DEFAULT" : 	16,
		"MIN" : 		6,
		"MAX" : 		25
	},
	{
		"NAME" : 		"radius1",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.0,
		"MIN" : 		-0.5,
		"MAX" : 		1.5
	},
	{
		"NAME" : 		"radius2",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.5,
		"MAX" : 		2.5
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.5,
		"MIN" : 		0.25,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-2.0,
		"MAX" : 		2.0
	},
	{
		"NAME" : 		"thickness",
		"TYPE" : 		"float",
		"DEFAULT" :     0.001,
		"MIN" : 		0.0001,
		"MAX" : 		0.005
	}
  ]
}
*/

////////////////////////////////////////////////////////////
// StringArt  by mojovideotech
//
// based on :
// glslsandbox.com/e#42256.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define 	pi   	3.141592653589793 	// pi
#define 	twpi  	6.283185307179586  	// two pi, 2*pi

float T = 0.01*(TIME*rate);

float cyc(float a) { 
	float c = cos(a), s = sin(a);
	vec2 g = vec2(c,s), h = vec2(s,-c); 
	return dot(g,h); 
}

vec2 rotate(vec2 st, float a) {
	float b = cyc(a);
	a = cyc(mix(a,b,freq1));
	st *= mat2( cos(a) , -sin(a), sin(a), cos(a) ) ;
	return st ;
}

float segment(vec2 p, vec2 a, vec2 b) {
    vec2 ab = b - a, ap = p - a;
    float rb = radius1, rp = radius2;
    vec2 rr = rotate(vec2(rb,rp),T*pi);
    float k = clamp(dot(ap, ab)/dot(ab, ab), rr.x, rr.y);
    return smoothstep(0.0, 5.0/RENDERSIZE.y, length(ap - k*ab) - thickness);
}

float shape(float u,vec2 p, float angle) {
    float d = 1.0;
    vec2 a = vec2(scale, 0.0), b;
    vec2 rot = vec2(cos(angle), sin(angle)*u);
    for (int i = 0; i < 7; ++i) {
        b = a;
        for (int j = 0; j < 25; ++j) {
        	b = vec2(b.x*rot.x - b.y*rot.y, b.x*rot.y + b.y*rot.x);
        	d = min(d, segment(p,  a, b));
        	if (float(j)>floor(loop2)) break;
        }
        a = vec2(a.x*rot.x - a.y*rot.y, a.x*rot.y + a.y*rot.x);
        if (float(i)>floor(loop1)) break;
    }
    return d;
}
void main(void)
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 cc = (-RENDERSIZE.xy + 2.0*gl_FragCoord.xy) / RENDERSIZE.y;
    float col = shape(freq2,abs(cc),freq1+cos(T)*twpi);
   
	gl_FragColor = vec4(vec3(pow(1.0-col, 2.0)),1.0);
}
