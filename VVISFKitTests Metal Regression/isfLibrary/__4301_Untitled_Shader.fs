
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
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
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

vec2 rotate(vec2 p, float ang) {
    return vec2(cos(ang)*p.x+sin(ang)*p.y, -sin(ang)*p.x+cos(ang)*p.y);
}
vec4 getID(vec2 pos, mat4 centers) {
    float d1 = length(pos - centers[0].xy);
    float d2 = length(pos - centers[1].xy);
    float d3 = length(pos - centers[2].xy);
    float d4 = length(pos - centers[3].xy);
    
    float temp = 100.0;
    vec4 weights = exp(-temp*vec4(d1,d2,d3,d4));
    weights = weights / (weights.x+weights.y+weights.z+weights.w);
    
    float d12, d34;
    float i12, i34, i;
    weights = vec4(0.);
    
    if (d1 < d2) { d12 = d1; i12 = 1.; } else { d12 = d2; i12 = 2.; }
    if (d3 < d4) { d34 = d3; i34 = 3.; } else { d34 = d4; i34 = 4.; }
    if (d12 < d34) { i = i12; } else { i = i34; };
    if (i==1.)
        weights.x=1.;
    else if (i==2.)
        weights.y = 1.;
    else if (i==3.)
        weights.z = 1.;
    else
        weights.w = 1.;
        
    return weights;
}

void transform(inout vec2 pos, mat4 c, vec4 r, vec4 s, vec4 w) {
    vec2 center = vec2(0.5, 0.5);
    
    vec2 new_center = w.x*c[0].xy + w.y*c[1].xy + w.z*c[2].xy + w.w*c[3].xy;
    float rot = w.x*r.x+w.y*r.y+w.z*r.z+w.w*r.w;
    float scale = w.x*s.x+w.y*s.y+w.z*s.z+w.w*s.w;
    pos = (scale * rotate(pos - new_center, rot) + center);
    pos = mod(abs(pos) ,1.);
}

void transform_seed(inout vec2 pos, float seed, vec4 r, vec4 s) {
    mat4 centers;
    centers[0] = vec4(0.5, 0.5, 0., 0.);
    centers[1].xy = rand2(seed);
    centers[2].xy = rand2(seed);
    centers[3].xy = rand2(seed);
    vec4 w = getID(pos, centers);
    transform(pos, centers, r, s, w);
}

void main()	{
	vec4 inputPixelColor;
	vec2 pos = isf_FragNormCoord.xy;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
	vec4 color = vec4(0.,0.,0.,1.0);
	
	mat4 centers;
	centers[0] = vec4(0.5,0.25, 0.,0.);
	centers[1] = vec4(0.25,0.75, 0.,0.);
	centers[2] = vec4(0.75,0.75, 0.,0.);
	centers[3] = vec4(0.5,0.5, 0,0.);
	float ang = 0.01 * TIME;
	vec2 center = vec2(0.5, 0.5);
	centers[0].xy = rotate(centers[0].xy-center, 1.*ang) + center;
	centers[1].xy = rotate(centers[1].xy-center, 1.*ang) + center;
	centers[2].xy = rotate(centers[2].xy-center, 1.*ang) + center;
	centers[3].xy = rotate(centers[3].xy-center, 1.*ang) + center;
	
	vec4 rots = vec4(ang, -20.*ang, 5.*ang, -20.*ang);
	vec4 scale = 2.0 + vec4(0.1*cos(TIME), -0.01*cos(TIME), -0.1*sin(TIME), 0.);
	
	float seed = 1970.;
	transform_seed(pos, seed * 1.123, rots, scale);
	transform_seed(pos, seed * 0.654, rots, scale);
	transform_seed(pos, seed * 2.654, rots, scale);
	transform_seed(pos, seed * 1.123, rots, scale);

	
	gl_FragColor = IMG_NORM_PIXEL(inputImage, pos);
}
