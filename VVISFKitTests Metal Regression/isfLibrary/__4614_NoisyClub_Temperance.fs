// License: MIT 2021 by NERDDISCO (Tim Pietrusky)

/*{
	"DESCRIPTION": "NoisyClub Temperance",
	"CREDIT": "NERDDISCO",
	"ISFVSN": "2",
	"CATEGORIES": [
		"underground",
		"noise",
		"club",
		"rainbow",
		"pixelspirit",
		"nerddisco"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "offset_factor",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": -1.5,
			"MAX": 1.5
		},
		{
			"NAME": "offset_multiplier",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.001,
			"MAX": 4.0
		},
		{
		    "NAME": "lines_aligned",
		    "TYPE": "bool",
		    "DEFAULT": 1.0
		},
		{
			"NAME": "line_1_position",
			"TYPE": "float",
			"DEFAULT": 0.28,
			"MIN": -0.5,
			"MAX": 1.5
		},
		{
			"NAME": "line_2_position",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -0.5,
			"MAX": 1.5
		},
		{
			"NAME": "line_3_position",
			"TYPE": "float",
			"DEFAULT": 0.72,
			"MIN": -0.5,
			"MAX": 1.5
		},
		{
		    "NAME": "make_some_noise",
		    "TYPE": "bool",
		    "DEFAULT": 0.0
		},
		{
		    "NAME": "noise_mix",
		    "TYPE": "float",
		    "DEFAULT": 0.0,
		    "MIN": -1.0,
		    "MAX": 1.0
		},
		{
		    "NAME": "auto_noise",
		    "TYPE": "bool",
		    "DEFAULT": 0.0
		},
		{
		    "NAME": "auto_noise_multiplier",
		    "TYPE": "float",
		    "DEFAULT": 1.0,
		    "MIN": 0.001,
		    "MAX": 5.0
		},
		{
		    "NAME": "manual_noise",
		    "TYPE": "float",
		    "DEFAULT": 0.0,
		    "MIN": -1.0,
		    "MAX": 2.0
		},
		{
		    "NAME": "like_in_the_club",
		    "TYPE": "bool",
		    "DEFAULT": 0.0
		},
		{
		    "NAME": "like_in_the_club_multiplier",
		    "TYPE": "float",
		    "DEFAULT": 1.0,
		    "MIN": 0.001,
		    "MAX": 5.0
		}
	]
}*/

#define PI 3.1415926535897932384626433832795

/*
 * The Wall
 * from https://github.com/patriciogonzalezvivo/PixelSpiritDeck/blob/master/00-elements/004-the_wall.frag
 */
float stroke(float x, float s, float w) {
    float d = step(s, x + w * .5) - step(s, x - w * .5);
    return clamp(d, 0., 1.);
}

/*
 * cheapNoise by Lea Rosema
 * from https://codepen.io/terabaud/pen/bGgOpjb
 */
float cheapNoise(vec3 stp, float mixVal) {
    vec3 p = vec3(stp.st * .1, stp.p);
    
    return mix(sin(p.z + p.x * 20. + cos(p.y * 20. - p.z)) * 
        cos(p.y * 10. + p.z + cos(p.y * 20. + p.z)), 
        sin(2. + p.x * 16. - p.z) * cos(2. + p.y*17. + p.z + cos(p.x * 20. + p.z)), mixVal);
}

/*
 * pallete by Inigo Quilez
 * from https://iquilezles.org/www/articles/palettes/palettes.htm
 */
vec3 palette(in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d) {
    return a + b * cos(6.28318 * (c * t + d));
}

void main()	{
    vec2 st = isf_FragNormCoord.xy;
    vec4 inputColor = IMG_THIS_PIXEL(inputImage);
    vec3 color = inputColor.rgb;
    //vec3 color = vec3(0.);
    
    if (make_some_noise) {
        if (auto_noise) {
            st.x = cheapNoise(vec3(st.xy, TIME * auto_noise_multiplier), noise_mix);
        } else {
            st.x = cheapNoise(vec3(st.xy, manual_noise), noise_mix);
        }
        
    }
    
    float offset = cos(st.y * PI) * (offset_factor * offset_multiplier);
    
    if (lines_aligned) {
        color += stroke(st.x, .28 + offset, .1);
        color += stroke(st.x, .5 + offset, .1);
        color += stroke(st.x, .72 + offset, .1);
    } else {
        color += stroke(st.x, line_1_position + offset, .1);
        color += stroke(st.x, line_2_position + offset, .1);
        color += stroke(st.x, line_3_position + offset, .1);
    }
    
    if (like_in_the_club) {
        vec3 rainbow = palette(
            st.y + (TIME * like_in_the_club_multiplier), 
            vec3(0.5, 0.5, 0.5),
            vec3(0.5, 0.5, 0.5),
            vec3(1.0, 1.0, 1.0),
            vec3(0.0, 0.33, 0.67)
        );
        
        // Check for the white stripe
        if (color.r + color.g + color.b >= 3.0) {
            color.rgb *= rainbow;
        }
    }

	gl_FragColor = vec4(color, 1.0);
}