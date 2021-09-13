
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "scaleInput",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 1.0,
			"MAX": 9.0
		},
		{
			"NAME": "powInput",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 1.0,
			"MAX": 5.0
		},
		{
			"NAME": "styleInput",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"one",
				"two",
				"three"
			],
			"DEFAULT": 0
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

#define M_PI 3.1415926535897932384626433832795
#define M_2PI  	6.283185307179586  	// two pi, 2*pi
#define M_R2PI  0.159154943091895	// reciprocal of twpi, 1/twpi


float hue2rgb(float f1, float f2, float hue) {
    if (hue < 0.0)
        hue += 1.0;
    else if (hue > 1.0)
        hue -= 1.0;
    float res;
    if ((6.0 * hue) < 1.0)
        res = f1 + (f2 - f1) * 6.0 * hue;
    else if ((2.0 * hue) < 1.0)
        res = f2;
    else if ((3.0 * hue) < 2.0)
        res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
    else
        res = f1;
    return res;
}

vec3 hsl2rgb(vec3 hsl) {
    vec3 rgb;
    if (hsl.y == 0.0) {
        rgb = vec3(hsl.z); // Luminance
    } else {
        float f2;
        if (hsl.z < 0.5)
            f2 = hsl.z * (1.0 + hsl.y);
        else
            f2 = hsl.z + hsl.y - hsl.y * hsl.z;
        float f1 = 2.0 * hsl.z - f2;
        rgb.r = hue2rgb(f1, f2, hsl.x + (1.0/3.0));
        rgb.g = hue2rgb(f1, f2, hsl.x);
        rgb.b = hue2rgb(f1, f2, hsl.x - (1.0/3.0));
    }   
    return rgb;
}

vec3 hsl2rgb(float h, float s, float l) {
    return hsl2rgb(vec3(h, s, l));
}

float smin( float a, float b, float k )
{
    float res = exp( -k*a ) + exp( -k*b );
    return -log( res )/k;
}


vec3 hsv2rgb(vec3 c) {
  vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float hypot (vec2 z) {
  float t;
  float x = abs(z.x);
  float y = abs(z.y);
  t = min(x, y);
  x = max(x, y);
  t = t / x;
  return (z.x == 0.0 && z.y == 0.0) ? 0.0 : x * sqrt(1.0 + t * t);
}

vec4 domainColoring (vec2 z, vec2 gridSpacing, float saturation, float gridStrength, float magStrength, float linePower) {
  float carg = atan(z.y, z.x);
  float cmod = hypot(z);
  float rebrt = (fract(z.x / gridSpacing.x) - 0.5) * 2.0;
  rebrt *= rebrt;
  float imbrt = (fract(z.y / gridSpacing.y) - 0.5) * 2.0;
  imbrt *= imbrt;
  float grid = 1.0 - (1.0 - rebrt) * (1.0 - imbrt);
  grid = pow(abs(grid), linePower);
  float circ = (fract(log2(cmod)) - 0.5) * 2.0;
  circ = pow(abs(circ), linePower);
  circ *= magStrength;
  vec3 rgb = hsv2rgb(vec3(carg * 0.5 / M_PI, saturation, 0.5 + 0.5 * saturation - gridStrength * grid));
  rgb *= (1.0 - circ);
  rgb += circ * vec3(1.0);
  return vec4(rgb, 1.0);
}


#define cx_mul(a, b) vec2(a.x*b.x-a.y*b.y, a.x*b.y+a.y*b.x)
#define cx_div(a, b) vec2(((a.x*b.x+a.y*b.y)/(b.x*b.x+b.y*b.y)),((a.y*b.x-a.x*b.y)/(b.x*b.x+b.y*b.y)))
#define cx_modulus(a) length(a)
#define cx_conj(a) vec2(a.x,-a.y)
#define cx_arg(a) atan(a.y,a.x)
#define cx_sin(a) vec2(sin(a.x) * cosh(a.y), cos(a.x) * sinh(a.y))
#define cx_cos(a) vec2(cos(a.x) * cosh(a.y), -sin(a.x) * sinh(a.y))
#define cx_frompolar(r, arg) r*vec2(cos(arg), sin(arg))
vec2 cx_sqrt(vec2 a) {
    float r = sqrt(a.x*a.x+a.y*a.y);
    float rpart = sqrt(0.5*(r+a.x));
    float ipart = sqrt(0.5*(r-a.x));
    if (a.y < 0.0) ipart = -ipart;
    return vec2(rpart,ipart);
}
vec2 cx_pow(vec2 z, float e) {
    float new_arg = e * cx_arg(z);
    float new_r = pow(hypot(z), e);
    return cx_frompolar(new_r, new_arg);
}

vec2 complexWarp(vec2 z, int style) {
    if(style == 0) {
        //z^2- 1/(z^2)
        vec2 z2 = cx_pow(z, powInput);
        return z2 - cx_div(vec2(2.5,1.0), z2);
    } else if(style == 1) {
        // (z−2)2(z+ 1−2i)(z+ 2 + 2i)/z3
        vec2 a = cx_pow(z -pointInput, 2.);
        vec2 b = z + vec2(1., -2.);
        vec2 c = z + vec2(2., 2.);
        vec2 z3 = cx_mul(z, cx_mul(z, z));
        vec2 abc = cx_mul(a, cx_mul(b, c));
        return cx_div(abc, z3);
    } else if(style == 2) {
        return cx_pow(z, powInput);
    }
}

vec4 myColor(vec2 xy) {
    vec2 center = vec2(0, 0);
    float theta = atan(xy.y, xy.x);
    float radius = 2.0 + 0.1*sin(TIME);
    radius += 0.1*cos(TIME)*sin(theta*12.0);
    radius *= 1.0;
    float f = mod(theta, M_2PI) / M_2PI;
    float hue = f;
    vec3 rgb = hsl2rgb(hue, 1.0, 0.75);
    
    float dist = length(xy - center) - radius;
    if(dist > 0.0) {
        float w = 1.0 - 3.0*smin(1.0, dist, 5.0);
        return vec4(w*rgb, 1.0);
    } else {
	    return vec4(0., 0., 0., 1.0);
    }

}

void main()	{
    float aspect = RENDERSIZE.x / RENDERSIZE.y;
    vec2 xy = isf_FragNormCoord - 0.5; 
    xy *= vec2(aspect, 1.0);
    xy *= scaleInput;
    xy = complexWarp(xy, styleInput);
    xy += vec2(sin(TIME), 0.);
    
    //gl_FragColor = myColor(xy);
    //gl_FragColor = vec4(0., isf_FragNormCoord.x, isf_FragNormCoord.y, 1.0);
    gl_FragColor = domainColoring(xy, vec2(1.0), 0.9, 0.6, 0., 3.0);
}
