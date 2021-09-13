
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

float rand2 (vec2 vec) {
    return fract(sin(dot(vec.xy, seed.xy)) * seed.w);
}

vec2 random_vec2(vec2 vec) {
    return vec2(rand2(vec), rand2(seed.z * vec));
}

vec2 getThetas(float s) {
    float s2 = seed.z * s;
    vec2 r = random_vec2(vec2(s,s2));
    
    float theta1 = r.x;
    float theta2 = theta1 + pi/2. + r.y * (-pi/10.);
    
    return vec2(theta1, theta2);
}

vec2 getCenter(float s) {
    float s2 = seed.z * s;
    vec2 r = random_vec2(vec2(s,s2));
    
    float cx = 0.5 + (r.x-0.5) / 2.;
    float cy = 0.5 + (r.y-0.5) / 2.;
    
    return vec2(cx, cy);
}

vec2 rotateCenter(vec2 pos, float theta) {
    float c = cos(theta);
    float s = sin(theta);
    float cx = 0.5;
    float cy = 0.5;
    return vec2(c*(pos.x-cx)+s*(pos.y-cy)+cx, -s*(pos.x-cx)+c*(pos.y-cy)+cy);
}

vec2 changeThetas(vec2 theta, float t) {
    return vec2(theta.x + t, theta.y + t);
}

vec4 getUV(float theta_u, float theta_v) {
    return vec4(cos(theta_u),sin(theta_u),cos(theta_v),sin(theta_v));
}

vec4 getGH(vec4 uv) {
    /*
    det_m = u[0]*v[1]-u[1]*v[0]
    h_est = -np.array([u[1],-u[0]]) / det_m
    g_est = np.array([v[1],-v[0]]) / det_m
    */
    float det = uv[0]*uv[3]-uv[1]*uv[2];
    return vec4(uv[1],-uv[0],uv[3],-uv[2]) / det;
}

vec2 applyFracture(vec2 pos, vec2 c, vec4 gh) {
    vec2 pos_c = pos - c;
    vec2 pos_a = vec2(gh.x*pos_c.x + gh.y*pos_c.y, gh.z*pos_c.x + gh.w*pos_c.y);
    return pos_a;
}

vec2 iteration(vec2 pos, vec2 theta, vec2 center, float scale) {
    vec4 uv = getUV(theta.x, theta.y);
    vec4 gh = getGH(uv) * scale;
    vec2 pos_t = applyFracture(pos, center, gh);
    //pos = mod(pos_t, vec2(1.,1.));
    //pos = mod(abs(pos_t),1.);
    
    vec2 g1 = vec2(1.0, 1.0);
    vec2 g2 = vec2(1.0, 1.0);
    vec2 g3 = vec2(1.0, 1.0);
    vec2 g4 = vec2(1.0, 1.0);
    
    float wx = 2.*pos_t.x;
    float wy = 2.*pos_t.y;
    float a = (1.-wx)*(1.-wy);
    float b = wx*(1.-wy);
    float c = (1.-wx)*wy;
    float d = wx*wy;
    
    vec2 g12 = mix(g1,g2,step(0.,pos_t.x));
    vec2 g34 = mix(g3,g4,step(0.,pos_t.x));
    vec2 g = mix(g12,g34,step(0.,pos_t.y));
    
    pos_t = a*vec2(0.,0.) + b*vec2(0.5,0.) + c*vec2(0.,0.5) + d*vec2(0.5,0.5)*g;
    
    pos_t = pos_t + center;
    pos_t = mod((pos_t), 1.);
    
    return pos_t;
}

float getBorderGain(vec2 pos, float scale) {
    float rw = 1. - max(pos.x, 1.-pos.x);
    float rh = 1. - max(pos.y, 1.-pos.y);
    
    float edge = 0.05;
    
    float g;
    float gw = smoothstep(0.0, scale * edge, rw);
    float gh = smoothstep(0.0, scale * edge, rh);
    g = min(gw,gh);
    
    float g2;
    gw = smoothstep(0.0, scale * edge, 1. - rw);
    gh = smoothstep(0.0, scale * edge, 1. - rh);
    g2 = min(gw,gh);
    
    g = min(g,g2);
    
    return g;
}

void main()	{
	vec4		inputPixelColor;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	
	vec2 pos = isf_FragNormCoord.xy;
	
	vec2 theta1 = getThetas(1.0);
	vec2 center1 = vec2(0.5, 0.5); //getCenter(1.144);
	float gain1;
	vec2 theta2 = getThetas(1.9);
	vec2 center2 = getCenter(1.144);
	float gain2;
	vec2 theta3 = getThetas(1.459);
	vec2 center3 = getCenter(1.2144);
	float gain3;
	
	float t = TIME / 10.;
	center1 = rotateCenter(center1, t);
	center2 = rotateCenter(center2, t);
	center3 = rotateCenter(center3, t);
	
	theta1 = changeThetas(theta1, t);
	theta2 = changeThetas(theta2, t * 1.5);
	theta3 = changeThetas(theta3, t * 3.13);
	
	float scale = 1.5;
	float scale_variation = 0.2*cos(TIME);
	float edge_scale = 0.15;
	pos = iteration(pos, theta1, center1, scale + scale_variation);
	gain1 = getBorderGain(pos, edge_scale);
	pos = iteration(pos, theta2, center2, scale - scale_variation);
	gain2 = getBorderGain(pos, scale*edge_scale);
	pos = iteration(pos, theta3, center3, scale + scale_variation);
	gain3 = getBorderGain(pos, scale*scale*edge_scale);
	pos = iteration(pos, theta3, center1, scale + scale_variation);
	gain3 = getBorderGain(pos, scale*scale*edge_scale);
	
	float test = rand2 (vec2(TIME, TIME));
	float x = mix(pos.x/2., 0.5+pos.x/2., step(0.95, test) );
	vec2 pos_simple = vec2(x, pos.y);
	
	vec4 color = IMG_NORM_PIXEL(inputImage, pos_simple);
	float gain = gain1 * gain2 * gain3;
	vec4 outputColor = vec4(gain * color.x, gain * color.y, gain * color.y, 1.0);
	
	vec4 oldcolor = IMG_NORM_PIXEL(oldBuffer, isf_FragNormCoord.xy);
	
	gl_FragColor = mix(outputColor, oldcolor, 0.75);
	
}
