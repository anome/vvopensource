/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{	
			"NAME": "LAYERS",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{	
			"NAME": "LAYER_MULTIPLIER",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 1.0,
			"MAX": 100.0
		},
	{	
			"NAME": "SHIFTX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
	{
			"NAME": "SHIFTY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{	
			"NAME": "SCALE",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 1.0,
			"MAX": 100.0
		},
		{	
			"NAME": "MODE",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 1.0,
			"MAX": 6.0
		}
	]
}*/

// Ported from "Quasicrystals" by charmless: https://www.shadertoy.com/view/Msc3Rn

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

// try changing LAYERS - 3,5,7 are nice.
// try changing the wrap function (wrap1, wrap2, wrap3) used at the end of quasicrystal()

#define PI 3.14159265359

float wave(float theta, vec2 p) {
	return (cos(dot(p,vec2(cos(theta),sin(theta)))) + 1.) / 2.;
}


// triangular wrapping
float wrap1(float val) {
    float v_int = floor(val), 
          v_frac = fract(val);

    bool even = fract(v_int / 2.) < 1e-4;
	return even ? v_frac : 1. - v_frac;
}

// sawtooth wrapping
float wrap2(float val) {
    return fract(val);
}

// sinusoidal wrapping
float wrap3(float val) {
    return (1. + sin(val)) / 2.;
}

// TAN function wrapping
float wrap4(float val) {
    return (1. + tan(val));
}
// TAN/SIN function wrapping
float wrap5(float val) {
    return (1. + tan(val)/sin(val));
}

// COS/SIN function wrapping
float wrap6(float val) {
    return (1./cos(val)*sin(val))*fract(val);
}

float quasicrystal(vec2 p) {
    float sum = 0.;
    for(float theta = 0.; theta < PI; theta += .15) 
        sum += wave(theta+theta*PI*LAYERS*LAYER_MULTIPLIER, p);
	if (int (MODE) == 1) {return wrap1(sum);}
	if (int (MODE) == 2) {return wrap2(sum);}
	if (int (MODE) == 3) {return wrap3(sum);}
	if (int (MODE) == 4) {return wrap4(sum);}
	if (int (MODE) == 5) {return wrap5(sum);}
	if (int (MODE) == 6) {return wrap6(sum);}
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
	float ratio = iResolution.x/iResolution.y;
	vec2 uv = fragCoord.xy/iResolution.y;
    vec2 target = vec2(mix(-.5, .5, .5),
                       mix(.5, .5, .5));
    uv -= vec2(SHIFTX+target.x * ratio+.6,SHIFTY+target.y);
    uv *= mix(RENDERSIZE.y/SCALE, RENDERSIZE.x/SCALE, .5);

    fragColor = vec4(quasicrystal(uv/.5));  
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}