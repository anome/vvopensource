
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
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
	],
	"PASSES": [
		{
			"TARGET": "oldBuffer",
			"PERSISTENT": true,
			"FLOAT": false
		}
	]
	
}*/

const float pi = 3.14159265359;
const vec4 seed = vec4(12.9898,78.233, 45.666,   43758.5453123);

float random (vec2 vec) {
    return abs(fract(sin(dot(vec.xy, seed.xy)) * seed.w));
}

float rand1(float x) {
    return random(vec2(x, 2.123*x));
}

vec2 rand2(float x) {
    return vec2(rand1(x), rand1(1.17856*x));
}

vec3 rand3(float x) {
    return vec3(rand1(0.7264*x), rand1(1.11*x), rand1(4.981*x));
}

void rotateCenter(inout vec2 pos, float theta) {
    vec2 center = vec2(0.5, 0.5);
    pos = pos-center;
    pos = vec2(cos(theta)*pos.x+sin(theta)*pos.y, -sin(theta)*pos.x+cos(theta)*pos.y);
    pos = pos + center;
}

void main()	{
	vec2 pos = isf_FragNormCoord.xy;
	vec2 center = vec2(0.5, 0.5);
    vec2 oldpos = 0.99 * (pos - center) + center;
    
    float FPS = 60.;
    float time_scale = FPS;
	float t = time_scale * TIME - fract(time_scale * TIME);
	
	vec2 A = rand2(t);
	vec2 B = rand2(t+32.562);
	float xmin = min(A.x, B.x);
	float xmax = max(A.x, B.x);
	float ymin = min(A.y, B.y);
	float ymax = max(A.y, B.y);
	float alpha = 1.0;
	alpha *= step(xmin, pos.x) * (1.-step(xmax, pos.x));
	alpha *= step(ymin, pos.y) * (1.-step(ymax, pos.y));

    vec2 offset = 0.01 * (rand2(t)-0.5);
    
	vec4 color = IMG_NORM_PIXEL(inputImage, pos + offset);
	vec4 oldPixel = IMG_NORM_PIXEL(oldBuffer, oldpos);
	
	color = vec4(alpha * color.xyz, 1.0);
	oldPixel = vec4(oldPixel.xyz, 1.0);
	float oldPixel_mean = (oldPixel.x + oldPixel.y + oldPixel.z) / 3.;
	oldPixel = mix(oldPixel, vec4(oldPixel_mean,oldPixel_mean,oldPixel_mean, 1.0), 0.25);
	
	gl_FragColor = max(color, 0.95 * (color/FPS + oldPixel));
	
}
