/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
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
			"NAME": "BPM",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "beat",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

#define PI 3.14159265359

float easeInOutQuart(float t, float b, float c, float d) {
	t /= d/2.0;
	if (t < 1.0) return c/2.0*t*t*t*t + b;
	t -= 2.0;
	return -c/2.0 * (t*t*t*t - 2.0) + b;
}

void main( void ) {

	vec2 position = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / min(RENDERSIZE.x, RENDERSIZE.y);
	//position = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / max(RENDERSIZE.x, RENDERSIZE.y);
	
	vec3 destColor = vec3(0.0);
	
	vec2 point = position;
	float number = 10.0;
	
	for(int i = 0; i < 10; i++){
		destColor += dot(vec3(0.05 / length(point)) * vec3(abs(sin(TIME + 2.0)), abs(sin(TIME * 2.0)), abs(sin(TIME * 4.0))), vec3(1.0));
		point = position;
	}
	
	
	gl_FragColor = vec4(destColor, 1.0);
}