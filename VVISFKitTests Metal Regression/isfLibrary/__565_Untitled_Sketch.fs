/*{
	"CREDIT": "by kazikpogoda",
	"DESCRIPTION": "modulo waves",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "segments",
			"TYPE": "long",
			"DEFAULT": "1",
			"MIN": "2"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/

/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	],
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

#define PI 3.14159265359

#define SEGMENTS 8.0

#define RINGS 5.0

#define SPEED -0.3

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec2 delta = (fragCoord -.5 * iResolution.xy) / iResolution.y;
    float dist = length(delta),
    angle = atan(delta.x, delta.y);
    float x = dist * RINGS;
    float y = angle * (SEGMENTS / 8.0);
    float t = iTime * SPEED;
    fragColor = vec4(
          mod(x + mod(y + t, PI / 4.0) + y + mod(x + t, PI / 4.0), PI) / PI,
          0,
          mod(x + mod(y + t, PI / 2.0) + y + mod(x + t, PI / 2.0), PI) / PI,
        1.0
    );
}



void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}