/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted",
    "generator"
  ],
  "DESCRIPTION" : "based on https://www.shadertoy.com/view/lljfDd by dlsym",
  "INPUTS" : [
	{
		"NAME" : 		"K",
		"TYPE" : 		"float",
		"DEFAULT" :		7.0,
		"MIN" : 		1.0,
		"MAX" : 		24.0
	},
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.75,
		"MIN" : 		1.0,
		"MAX" : 		2.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	3.0,
		"MIN" : 		0.0,
		"MAX" : 		5.0
	},
	{
      	"NAME" : 		"c1",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.4, 0.33, 0.5, 1.0 ]
   	},
    {
      	"NAME" : 		"c2",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.9, 0.9, 0.9, 1.0 ]
    },
    {
      	"NAME" : 		"c3",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.9, 0.5, 0.33, 1.0 ]
    },
    {
      	"NAME" : 		"c4",
      	"TYPE" : 		"color",
      	"DEFAULT" :	[ 0.8, 0.6, 0.9, 1.0 ]
    }
  ]
}
*/


////////////////////////////////////////////////////////////
// QuaziCrystal  by mojovideotech
//
// based on :
// shadertoy.com/lljfDd by dlsym
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define 	pi   	3.141592653589793 	// pi

// http://www.iquilezles.org/www/articles/palettes/palettes.htm
vec4 colorize(float t, vec3 a, vec3 b, vec3 c, vec3 d) {
    vec3 col = 2.5 * a * b * (cos(0.4*pi*(c*t+d))); 
    return vec4(col, 1.0);
}

float v(vec2 coord, float k, float s, float rot) {
    float cx = cos(rot), sy = sin(rot);
    return 0.0 + 0.5 * cos((cx * coord.x + sy * coord.y) * k + s);
}

void main() {
   	float T = rate * TIME;
    vec2 uv = gl_FragCoord.xy - (0.5 * RENDERSIZE.xy); // center
    float vt = 0.0, j = floor(K);
    for(int i = 0; i < 24; i++) {
    	if (float(i) >= j) break;
        float s = float(i) * pi / j;
    	float w = v(uv, 2.1-scale, T, s);
        vt += w / 0.5;
    }
	gl_FragColor = colorize(vt, c1.rgb, c2.rgb, c3.rgb, c4.rgb);
}
