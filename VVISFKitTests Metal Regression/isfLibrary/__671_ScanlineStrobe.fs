/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
	{
      "NAME": "A",
      "TYPE": "long",
      "VALUES": [
        0.2820947918,     
		0.4886025119,
		1.0925484306,
		0.3153915652,
		0.5462742153,
		0.5900435860,
		2.8906114210,
		0.4570214810,
		0.3731763300,
		1.4453057110
      ],
      "LABELS": [
        "k01",
        "k02",
        "k03",
        "k04",
        "k05",
        "k06",
        "k07",
        "k08",
        "k09",
        "k10"
      ],
      "DEFAULT": 2.8906114210
    },
    {
      "NAME": "B",
      "TYPE": "long",
      "VALUES": [
        0.2820947918,     
		0.4886025119,
		1.0925484306,
		0.3153915652,
		0.5462742153,
		0.5900435860,
		2.8906114210,
		0.4570214810,
		0.3731763300,
		1.4453057110
      ],
      "LABELS": [
        "k01",
        "k02",
        "k03",
        "k04",
        "k05",
        "k06",
        "k07",
        "k08",
        "k09",
        "k10"
      ],
      "DEFAULT": 1.4453057110
    },		
    {
      "NAME": "C",
      "TYPE": "long",
      "VALUES": [
        0.2820947918,     
		0.4886025119,
		1.0925484306,
		0.3153915652,
		0.5462742153,
		0.5900435860,
		2.8906114210,
		0.4570214810,
		0.3731763300,
		1.4453057110
      ],
      "LABELS": [
        "k01",
        "k02",
        "k03",
        "k04",
        "k05",
        "k06",
        "k07",
        "k08",
        "k09",
        "k10"
      ],
      "DEFAULT": 1.0925484306
    },	
    {
			"NAME" : 		"S",
			"TYPE" : 		"float",
			"DEFAULT" : 	38.0,
			"MIN" : 		1.0,
			"MAX" : 		100.0
		},
		{
			"NAME" : 		"N",
			"TYPE" : 		"float",
			"DEFAULT" : 	-281.0,
			"MIN" : 		-1000.0,
			"MAX" : 		1000.0
		},
		{
			"NAME" : 		"M",
			"TYPE" : 		"float",
			"DEFAULT" : 	939.0,
			"MIN" : 		-10000.0,
			"MAX" : 		10000.0
		},
		{
			"NAME" : 		"Q",
			"TYPE" : 		"float",
			"DEFAULT" : 	-51.9,
			"MIN" : 		-100.0,
			"MAX" : 		100.0
		},
		{
			"NAME" : 		"R",
			"TYPE" : 		"float",
			"DEFAULT" : 	2.2,
			"MIN" : 		-12.0,
			"MAX" : 		12.0
		},
		{
			"NAME" : 		"G",
			"TYPE" : 		"float",
			"DEFAULT" : 	0.05,
			"MIN" : 		-0.5,
			"MAX" : 	 	0.5
		},
		{
			"NAME" : 		"Z",
			"TYPE" : 		"float",
			"DEFAULT" : 	-0.45,
			"MIN" : 		-10.0,
			"MAX" : 	 	10.0
		}
	]
}*/

////////////////////////////////////////////////////////////////////
// ScanlineStrobe  by mojovideotech
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////




#define 	pi   	3.141592653589793 	// pi

#define a float(A)
#define b float(B)
#define c float(C)


float psn(vec2 co){
	float dt = dot(co.xy * floor(M/abs(Q))*pi, vec2(floor(N)*a,b))/floor(S);
	float sn = mod(dt,Q*c);
	return fract((sin(sn) * pi)+TIME*R);
}

void main() {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	uv /= Z;
	float n = psn(uv);
	gl_FragColor = sqrt(max(vec4(vec3(n),1.0),0.0)+G);
}