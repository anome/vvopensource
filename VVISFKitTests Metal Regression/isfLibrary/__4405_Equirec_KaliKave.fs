/*
{
    "CREDIT": "by mojovideotech",
    "CATEGORIES" : [
        "generator",
        "equirectangular",
        "Kaliset",
        "fractal"
    ],
    "DESCRIPTION": "",
    "ISFVSN": "2",
    "INPUTS": [
		{
			"NAME" : 	"rot",
			"TYPE" : 	"point2D",
			"DEFAULT" :	[ 0.0, 0.0 ],
			"MAX" : 	[ 1.0, 1.0 ],
     		"MIN" : 	[ -1.0, -1.0 ]
		},
        {
            "NAME":     "rate",
            "TYPE":     "float",
            "DEFAULT":  0.5,
            "MIN":      -2.0,
            "MAX":      2.0
        },
		{
			"NAME" : 	"offset",
			"TYPE" : 	"float",
			"DEFAULT" : 0.0,
			"MIN" : 	0.0,
			"MAX" : 	1.0
		},
        {
            "NAME":     "frag",
            "TYPE":     "float",
            "DEFAULT":  0.0,
            "MIN":      0.0,
            "MAX":      1.0
        },
        {
            "NAME":     "glow",
            "TYPE":     "float",
            "DEFAULT":  0.5,
            "MIN":      0.0,
            "MAX":      1.0
        },
		{
			"NAME" : 	"detail",
			"TYPE" : 	"float",
			"DEFAULT" : 400.0,
			"MIN" : 	0.0,
			"MAX" : 	500.0
		}
    ]
}

*/

////////////////////////////////////////////////////////////////////
// Equirec_KaliKave  by mojovideotech
//
// based on :
// glslsandbox.com/e#63990.0
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307 	// two pi, 2*pi
#define 	pi   	3.141592653 	// pi


float orb;
float de(vec3 p) {
	p.x = mod(p.x + 1.2, 2.4) - 1.2;
	vec4 q = vec4(p, 1.0);
	orb = 10000.0;
	for(int i = 0; i < 16; i++) {
		q = 5.0*abs(q)/dot(q.xyz, q.xyz) - vec4(2.4, 0.9+frag, 2.4, 0.0);
		orb = min(orb, sin(abs(q.x*q.y)));
	}
	return (length(q.xyz))/q.w - 0.01;
}

vec3 rXY(vec3 p, vec2 a) {
    vec2 c = cos(a), s = sin(a);
    p = vec3(p.x, c.x*p.y + s.x*p.z, -s.x*p.y + c.x*p.z);
    return vec3(c.y*p.x + s.y*p.z, p.y, -s.y*p.x + c.y*p.z);
}

void main() 
{
	float t, g, T = TIME * rate;
	vec3 col = vec3(0.0), mat = vec3(0.0);
	float l = 510.0-detail;
	vec2 sph = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5) * vec2(twpi, pi);
   	vec3 uv = vec3(sin(sph.x)*cos(sph.y), sin(sph.y), cos(sph.x)*cos(sph.y));
	vec3 rd = rXY(uv, rot * twpi);
	vec3 ro = vec3(T, 0.0, -3.0+offset);
	for(int i = 0; i < 200; i++) {
		float d = de(ro + rd*t);
		if(d < 0.0001*(1.0 + l*t) || t >= 10.0) break;
		t += d*(0.1 + 0.01*t);
		g += 0.05*(1.0 - d);
	}
	g = clamp(g, 0.0, 1.0);
	if(t < 10.0) {
		vec3 pos = ro + rd*t;
		vec2 h = vec2(0.001, 0.0);
		vec3 nor =  normalize(vec3(
		de(pos + h.xyy) - de(pos - h.xyy),
		de(pos + h.yxy) - de(pos - h.yxy),
		de(pos + h.yyx) - de(pos - h.yyx)));
		vec3 key = normalize(vec3(2.8, 2.7, -0.6));
		vec3 gro = vec3(2.0, -1.0, 2.0);
		col = 0.5*vec3(1.0);
		col += 0.2*clamp(dot(key, nor), 0.0, 0.0);
		col += 0.1*clamp(5.2 + 5.8*dot(-key, nor), 0.0, 0.0);
		col += 0.2*clamp(dot(gro, nor), 2.0, 2.0);
        mat = mix(vec3(0.2, 0.1, 0.1), vec3(1.0, 0.2, 0.1), 2.0*orb);
		col *= mat;
	}
	col = mix(col+(g*mat), vec3(0.5+glow), 0.2 - exp(-0.5*t)); 
	
	gl_FragColor = vec4(sqrt(col), 1.0);
}

