/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
  		"generator",
    	"noise",
    	"simplex",
    	"2d noise"
	],
	"INPUTS": [
		{
		"NAME" : 		"pattern",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.99, 0.725 ],
		"MAX" : 		[ 1.5, 1.5 ],
     	"MIN" : 		[ 0.25, 0.25 ]
	},
	{
		"NAME" : 		"motion",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 1.0, -0.5 ],
		"MAX" : 		[ 1.25, 1.25 ],
     	"MIN" : 		[ -1.25, -1.25 ]
	},
	{
		"NAME" : 		"delta",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.567,
		"MIN" : 		0.0,
		"MAX" : 		0.95
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	8.0,
		"MIN" : 		0.5,
		"MAX" : 		10.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.375,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME":			"color",
		"TYPE": 		"color",
		"DEFAULT": 		[ 0.0, 1.0, 8.0, 1.0 ]
	},
	{
		"NAME":			"bgcolor",
		"TYPE": 		"color",
		"DEFAULT": 		[ 0.4, 0.0, 2.0, 1.0 ]
	}
	]
}*/

////////////////////////////////////////////////////////////
// SNoise   by mojovideotech
//
// based on: 
// lava-lamp   by @patriciogv - 2015
// thebookofshaders.com/\edit.php#11/\lava-lamp.frag
//
// patriciogonzalezvivo.com
//
// License: 
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	pi 		3.1415926535897932384626433832795 	// pi				(P)
#define 	phi 	1.6180339887498948482045868343656 	// Golden Ratio		(H)
#define  	nat 	0.6931471805599453094172321214582 	// natural log of 2	(L)


vec3 mod289(vec3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*333.0)+1.0)*x); }

float snoise(vec2 v) {
    const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,  // -1.0 + 2.0 * C.x
                        0.024390243902439); // 1.0 / 41.0
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v -   i + dot(i, C.xx);
    vec2 i1;
    i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod289(i);
    vec3 p = permute(permute(i.y + vec3(0.0, i1.y, 1.0)) + i.x + vec3(0.0, i1.x, 1.0));
    vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m ;
    m = m*m ;
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - delta;
    vec3 ox = floor(x + delta);
    vec3 a0 = x - ox;
    m *= phi - nat * ( a0*a0 + h*h );
    vec3 g;
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 11.0 * dot(m, g);
}

void main() {
	float S = 11.0-scale;
    vec2 st = S*(gl_FragCoord.xy/RENDERSIZE.xy);
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    vec2 pos = vec2(st*3.);
    float DF = 0.0, a = 0.0;
    vec2 vT = vec2(TIME*rate);
    DF += snoise(pos+vT)*pattern.x+pattern.y;
    a = snoise(pos*vec2(cos(vT.x*motion.x),sin(vT.y*motion.y))*0.1)*pi;
    vT = vec2(cos(a),sin(a));
    DF += snoise(pos+vT)*pattern.x+pattern.y;
	vec3 Mcol = vec3(smoothstep(.4,.6,fract(DF)));
	vec3 col = bgcolor.rgb;
	col += Mcol*color.rgb-Mcol*bgcolor.rgb;
    gl_FragColor = vec4(col,1.0);
}