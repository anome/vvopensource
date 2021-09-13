/*{
	"CREDIT": "by msfeldstein",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "color",
			"TYPE": "color"
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": -1,
			"MAX": 1,
			"DEFAULT": 0.5
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"MIN": 1,
			"MAX": 10,
			"DEFAULT": 4
		}
	],
	"PERSISTENT_BUFFERS": [
		"timeBuffer"
	],
	"PASSES": [
		{
			"TARGET":"timeBuffer",
			"WIDTH:": 1,
			"HEIGHT": 1,
			"FLOAT": true,
			"DESCRIPTION": "this buffer stores the current time value"
		},
		{}
	]
}*/

void main() {
	float T = IMG_PIXEL(timeBuffer,vec2(0.5)).x;
	if (PASSINDEX == 0) {
		gl_FragColor = vec4(T + 0.1);
	} else {
		vec2 pos = gl_FragCoord.xy / RENDERSIZE;
		pos *= size;
		pos = fract(pos);
		float v = 1.0 - distance(pos, vec2(0.5));
		v = floor(sin(v * 20.0 + T * 1000.0) + 1.0);
		gl_FragColor = v * color;
	}
	
}