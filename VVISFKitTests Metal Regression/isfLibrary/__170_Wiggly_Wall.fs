// License: MIT 2021 by NERDDISCO (Tim Pietrusky)

/*{
	"DESCRIPTION": "Wiggly Wall",
	"CREDIT": "NERDDISCO",
	"ISFVSN": "2",
	"VSN": "1.0.0",
	"CATEGORIES": [
		"Wall",
		"Pixel Spirit Deck",
		"Wiggly",
		"Base"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "position_x",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 2.0
		},
		{
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
		    "NAME": "auto_movement",
		    "TYPE": "bool",
		    "DEFAULT": 1.0
		},
		{
			"NAME": "frequency",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "magnitude",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "spread",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "wiggly_x",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "wiggly_y",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
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
 * WIGGLY
 * from https://www.curiouslyminded.xyz/til/wiggly-noise
 * - authored with the math help by @mimetikmusic -
 * (cx) centerPoint in the x axis
 * (cy) centerPoint in the y axis
 * (freq) frequency of the wobble
 * (mag) magnitude of the wobble
 * (sp) spread of the wobble 
 * returns => wiggly noise
 */
float wiggly(float cx, float cy, float mag, float freq, float sp) {
    float w = sin(cx * mag * freq * PI) * cos(cy * mag * freq * PI) * sp;
    return w;
}

void main()	{
    vec4 color = IMG_THIS_PIXEL(inputImage);
    vec2 st = isf_FragNormCoord.xy;
    
    float _position_x = position_x;
    
    if (auto_movement) {
        _position_x -= wiggly(
            st.x + TIME * 0.05,
            st.x - TIME * 0.05,
            frequency,
            magnitude,
            spread
        ); 
    } else {
        _position_x -= wiggly(
            st.x + wiggly_x * 0.05,
            st.x + wiggly_y * 0.05,
            frequency,
            magnitude,
            spread
        ); 
    }
    
    color += stroke(st.x, _position_x, width);
    
	gl_FragColor = color;
}