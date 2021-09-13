/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "fold_iter",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 0.1
		},
		{
			"NAME": "pal_iter",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "rot_offset",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "rot_speed",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.01,
			"MAX": 0.5
		}
	]
}*/

//https://www.shadertoy.com/view/lsGSzW


mat2 rot(float angle) {
    return mat2(cos(angle), -sin(angle),
                sin(angle), cos(angle));
}

// http://iquilezles.org/www/articles/palettes/palettes.htm
vec3 palette( in float t )
{
    vec3 a = vec3(0.5);
    vec3 b= vec3(0.5);
    vec3 c= vec3(1.0, 1.0, 0.5);
    vec3 d= vec3(0.8, 0.9, 0.3);
    return a + b*cos( 6.28318*(c*t+d) );
}

void main() {
	//vec2 uv = isf_FragNormCoord.xy;
	vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    for(int i = 0; i < 32; i++) {
        uv = abs(uv);
        uv *= rot(rot_offset+(rot_speed*TIME));
        uv += -vec2(0.5,0.5);
        uv *= (1.03 + fold_iter);
    }
    uv = pow(abs(sin(uv)),vec2(0.3));
    vec3 col = palette(uv.x*uv.y*(1.9 + pal_iter));
	gl_FragColor = vec4(vec3(col.r),1.0);
}