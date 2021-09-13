/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
  	"generator",
    "raymarching"
  ],
  "DESCRIPTION" : "based on https://www.shadertoy.com/view/lslcWj by LukeRissacher",
  "INPUTS" : [
    {
      "NAME": "offset",
      "TYPE": "point2D",
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ]
    },
	{
		"NAME" : 		"R1",
		"TYPE" : 		"float",
		"DEFAULT" : 	3.0,
		"MIN" : 		0.0,
		"MAX" : 		 9.0
	},
	{
		"NAME" : 		"R2",
		"TYPE" : 		"float",
		"DEFAULT" : 	17.0,
		"MIN" : 		9.0,
		"MAX" : 		19.0
	},
	{
		"NAME" : 		"Z",
		"TYPE" : 		"float",
		"DEFAULT" : 	2.5,
		"MIN" : 		1.0,
		"MAX" : 		3.5
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	-0.5,
		"MIN" : 		-3.0,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"WHOA",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		-3.0,
		"MAX" : 		3.0
	}
  ]
}
*/


///////////////////////////////////////////
// SpaceSpore  by mojovideotech
//
// based on :
//
// shadertoy.com/view/lslcWj by LukeRissacher
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
///////////////////////////////////////////

#define 	twpi  	6.283185307179586  	
#define R(p, a) p = p * cos(a) + vec2(-p.y, p.x) * sin(a)

float Swave(float t) {
    return 0.5 + 0.5 * sin(twpi * t);
}

float Sdef(vec3 p) {
    return WHOA - abs(sin(p.x) + sin(p.y) + sin(p.z)) / 3.0;
}

float Smap(vec3 p, float s) {
    float dSphere = length(p) - 1.0;
    return max(dSphere, (0.95 - Sdef(s * p)) / s);
}

vec3 Scol(vec3 p) {
    float a = clamp((2.0 - length(p)) / 2.0, 0.0, 1.0);
    vec3 col = 0.5 + 0.5 * cos(twpi * cross(vec3(1.0, 0.0, 0.5), vec3(Swave(-a) * vec3(0.5, 1.0, 1.0))));
    return col * a;
}

void main() {
	float TT = TIME * rate; 
	vec3 rd = normalize(vec3(2.0 * gl_FragCoord.xy - RENDERSIZE.xy, -RENDERSIZE.y));
    vec3 ro = vec3(offset, 5.0-Z); 
    R(rd.xz, 0.5 * TT);
    R(ro.xz, 0.5 * TT);
    R(rd.yz, 0.1 * TT);
    R(ro.yz, 0.1 * TT);
    float t = 0.0;
    gl_FragColor.rgb = vec3(0.0);
    float scale = mix(R1, R2, Swave(0.068 * TIME));
    for (int i = 0; i < 64; i++) {
        vec3 p = ro + t * rd;
        float d = Smap(p, scale);
        if (t > 5.0 || d < 0.001) {
            break;
        }
        t += 0.8 * d;
        gl_FragColor.rgb += 0.05 * Scol(p);
    }
}
