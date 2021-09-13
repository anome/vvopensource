/*{
	"CREDIT": "splenooname - Andrea Bovo <spleen666@gmail.com>",
    "DESCRIPTION": "spiral stuff.",
	"CATEGORIES": [
		"spiral",
		"twist"
	],
	"INPUTS": [
		{
			"NAME": "rays",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 5.0,
			"MAX": 20.0
		},
		{
			"NAME": "twist",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 1.0,
			"MAX": 10.0
		},
		{
			"NAME": "rOffset",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 1.25
		},
		{
			"NAME": "gOffset",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 1.25
		},
		{
			"NAME": "bOffset",
			"TYPE": "float",
			"DEFAULT": 1,
			"MIN": 0.1,
			"MAX": 1.25
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.2,
			"MAX": 1.0
		},
		{
			"NAME": "period",
			"TYPE": "float",
			"DEFAULT": 80.0,
			"MIN": 10.0,
			"MAX": 100.0
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

#define R RENDERSIZE
#define t TIME

mat2 rotate (float a) {
	float c = cos (a);
	float s = sin (a);	
	return mat2 (-s, c, c, s);	
}

float getCol(float dist, float angle) {
	float col = (
            (
            	cos( (angle * 10.) + pow( dist * 1.0, sin( t * speed) * twist ) * .5) + sin(dist * period )
        	) + 0.25
        ) / 0.25;
    return col * col;
}

void main() {
	// normalize uv coords [-1/2,1/2] vertically
	vec2 uv = (gl_FragCoord.xy -.5 * R.xy) / R.y;
	// rotate coords
	uv *= rotate ( t * speed);
	//determine the vector length of the center position
    float dist = length(uv);
    // get angle from coords
    float angle = atan(uv.x, uv.y);
	// get colors
	float r = getCol(dist * rOffset, angle);
	float g = getCol(dist * gOffset, angle);
	float b = getCol(dist * bOffset, angle);
	//use smoothstep to create a smooth vignette
	float vignette = smoothstep(0.5, 0.4, dist);
	// out color
	gl_FragColor = vec4( vec3(r,g,b)*vignette, 1.);
}