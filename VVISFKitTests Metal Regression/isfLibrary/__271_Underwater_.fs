/*{
	"CREDIT": "by echophons",
	"DESCRIPTION": "port from http://glslsandbox.com/e#20906.0",
	"CATEGORIES": [ "Generator"
	],
	"INPUTS": [
		{
			"NAME": "RAYCOLOR",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.3,
				0.8,
				1.0
		]
		},
		{
			"NAME": "py",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 2.0
		},		
		
		{
			"NAME": "px",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 2.0
		}
	]
}*/

#ifdef GL_ES
precision highp float;
#endif
vec3   resolution = vec3(RENDERSIZE, 1.0);
float  time = TIME;

// the audience is now drowning

// Remixed to add selectable ray color.

//uniform float time;
uniform vec2 mouse;
//uniform vec2 resolution;

float rand(int seed, float ray) {
	return mod(sin(float(seed)*363.5346+ray*674.2454)*6743.4365, 1.0);
	
}

void main( void ) {
	float pi = 3.14159265359;
	vec2 position = ( gl_FragCoord.xy / resolution.xy ) - vec2(px, py);
	position.y *= resolution.y/resolution.x;
	float ang = atan(position.y, position.x);
	float dist = length(position);
	gl_FragColor.rgb = vec3(RAYCOLOR) * (pow(dist, -1.0) * 0.04);
	for (float ray = 0.0; ray < 10.0; ray += 0.095) {
		//float rayang = rand(5234, ray)*6.2+time*5.0*(rand(2534, ray)-rand(3545, ray));
		float rayang = rand(5, ray)*6.2+(time*0.05)*10.0*(rand(2546, ray)-rand(5785, ray))-(rand(3545, ray)-rand(5467, ray));
		rayang = mod(rayang, pi*2.0);
		if (rayang < ang - pi) {rayang += pi*2.0;}
		if (rayang > ang + pi) {rayang -= pi*2.0;}
		float brite = .5 - abs(ang - rayang);
		brite -= dist * 0.1;
		if (brite > 0.0) {
			gl_FragColor.rgb += vec3((RAYCOLOR.x*0.5)+0.5*rand(8644, ray), (RAYCOLOR.y*0.5)+0.5*rand(4567, ray), (RAYCOLOR.z*0.5)+0.5*rand(7354, ray)) * brite * 0.1;
		}
	}
	gl_FragColor.a = 1.0;
}