
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

const float fov = radians(90.);
float foc = 1. / tan(fov / 2.);
const float u0 = 0.5;
const float v0 = 0.5;
const float pi = 3.14159265359;

const vec4 seed = vec4(12.9898,78.233, 45.666,   43758.5453123);

float rand2 (vec2 vec) {
    return fract(sin(dot(vec.xy, seed.xy)) * seed.w);
}

mat3 euler_rot(vec3 ori) {
    /*
    R =  [  cos(psi)*cos(theta)         -sin(psi)*cos(phi)+cos(psi)*sin(theta)*sin(phi)     sin(psi)*sin(phi)+cos(psi)*sin(theta)*cos(phi)  ;
            sin(psi)*cos(theta)         cos(psi)*cos(phi)+sin(psi)*sin(theta)*sin(phi)      -cos(psi)*sin(phi)+sin(psi)*sin(theta)*cos(phi) ;
            -sin(theta)                 cos(theta)*sin(phi)                                 cos(theta)*cos(phi)                             ];
    // Here, we compute R*[0;0;1], that's the third column
    */
    float theta = ori[0];
    float phi = ori[1];
    float psi = ori[2];
    float m11 = cos(psi)*cos(theta);
    float m21 = sin(psi)*cos(theta);
    float m31 = -sin(theta);
    float m12 = -sin(psi)*cos(phi)+cos(psi)*sin(theta)*sin(phi);
    float m22 = cos(psi)*cos(phi)+sin(psi)*sin(theta)*sin(phi);
    float m32 = cos(theta)*sin(phi);
    float m13 = sin(psi)*sin(phi)+cos(psi)*sin(theta)*cos(phi);
    float m23 = -cos(psi)*sin(phi)+sin(psi)*sin(theta)*cos(phi);
    float m33 = cos(theta)*cos(phi);
    
    mat3 M = mat3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
    
    return M;
}

vec3 unproject(vec2 pos) {
    float fx = foc;
    float fy = foc * RENDERSIZE.x / RENDERSIZE.y;
    return vec3((pos.x-u0)/fx, (pos.y-v0)/fy,1.0);
}

vec2 project(vec3 M) {
    float fx = foc;
    float fy = foc * RENDERSIZE.x / RENDERSIZE.y;
    return vec2(fx*M.x/M.z+u0, fy*M.y/M.z+v0);
}

vec2 homography(vec2 pos, mat3 R, vec3 td, vec3 n) {
    mat3 H = R - mat3(n.x * td, n.y * td, n.z * td);
    vec3 P = H * unproject(pos);
    vec2 pos_t = project(P);
    
    return pos_t;
}

void main()	{
    vec2 pos = isf_FragNormCoord.xy;
	vec4 inputPixelColor;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
	
	vec3 ang = vec3(cos(TIME), 0., 0.);
	mat3 R2 = euler_rot(ang);
	mat3 R = euler_rot(vec3(0.));
	
	vec3 td = vec3(0.0, 0.0, 1.0);
	
	vec3 n = vec3(0.0, 0.0, -1.0);
	
	vec2 pos2 = homography(pos, R2, td, R2 * n);
	pos2 = mod(pos2, 1.);
	
	vec4 color = IMG_NORM_PIXEL(inputImage, pos2);
	gl_FragColor = color;
}
