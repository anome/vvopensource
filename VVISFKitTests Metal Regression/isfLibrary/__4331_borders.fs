
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
			"NAME": "element",
			"TYPE": "image"
		},
		{
			"NAME": "distance",
			"TYPE": "image"
		},
		{
			"NAME": "PERIOD",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.6
			]
		}
	],
	"PASSES": [
		{
			"TARGET": "oldImage",
			"PERSISTENT": true,
			"FLOAT": false
		}
	]
	
}*/
const float fov = radians(90.);
float foc = 1. / tan(fov / 2.);
const float u0 = 0.5;
const float v0 = 0.5;
const float pi = 3.14159265359;

const vec4 seed = vec4(12.9898,78.233, 45.666,   43758.5453123);

float rand2 (vec2 vec) {
    return fract(sin(dot(vec.xy, seed.xy)) * seed.w);
}

vec3 wall(vec2 pos, vec3 cam, vec4 plane) {
    float r = RENDERSIZE.x / RENDERSIZE.y;
    vec3 m = vec3((pos.x-u0) / foc, (pos.y - v0) / (foc / r), 1.);
    vec3 N = plane.xyz;
    float n = plane.w;
    float Z = (n-dot(cam, N)) / dot(m, N);
    return Z * m;
}

vec2 change(vec2 M, float t, float th, float scale) {
    
    float f = mod(t, PERIOD) / th;
    if (f < 1.) {
        float rx = (rand2(vec2(TIME + 4.123*M.x, 1.149*M.y)))-0.5;
        float ry = (rand2(vec2(TIME + 0.125*M.y, 3.318*M.x)))-0.5;
        M += 2. * cos(0.5*pi*f) * vec2(rx, ry) * scale;
    }
    
    return M;
}

void addElement(inout vec4 color, vec2 pos) {
    vec4 col = IMG_NORM_PIXEL(element, pos);
    vec4 mask = IMG_NORM_PIXEL(distance, pos);
    
    float f = mask.x;
    
    if (f > 0.99) {
        float str = 0.4;
        color = str * col + (1.-str) * color;
    }
    else {
        float fi = 0.5 * pow(f, 32.);
        vec4 col_new = vec4(1., 0., 0., 1.);
        color = (1.-fi) * color + fi * col_new;
    }
    
}

vec2 turn(vec2 M, float t) {
    vec2 center = vec2(0.5);
    float ang = t;
    M = M - center,
    M = vec2(cos(ang)*M.x + sin(ang)*M.y, -sin(ang)*M.x + cos(ang)*M.y);
    M = M + center;
    
    return M;
}


void main()	{
    vec2 pos = isf_FragNormCoord.xy;
	vec4 inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
	
	float ang = 2.*pi*TIME/2.;
	float zoom = 1. / 0.95;
	float mix = 0.01;
	
	vec2 center = pointInput + 0.03 * vec2(cos(ang), sin(ang));
	vec2 pos_old = zoom * (pos-center) + center;
	pos_old = change(pos_old, 0., 1., 1. / 300.);
	pos_old = turn(pos_old, 0.01 * cos(TIME));
	
	float thresh = 15. / 30.;
	vec2 pos_new = change(pos, TIME, thresh, 1. / 100.);
	
	vec4 color_old = (1.-mix) * IMG_NORM_PIXEL(oldImage, pos_old);
	vec4 color = IMG_NORM_PIXEL(inputImage, pos_new);
	color = max(color, color_old);
	
	addElement(color, pos);
	
	gl_FragColor = color;
}
