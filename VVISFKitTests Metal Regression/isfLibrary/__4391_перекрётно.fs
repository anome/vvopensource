/*{
  "CREDIT": "by ",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.6
    },
    {
      "NAME": "xx",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "s",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/
// kernel Convolution Filters алгоритмы

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


void main()	{
	vec4 inputPixelColor;
	
	vec4 color = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
	vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
	vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
	vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

	vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
	vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
	vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
	vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);

	//vec4 avg = intensity * (color + colorLA + colorRA + colorLB + colorRB) / 5.0;
	vec4 avg = intensity * (
	    colorLA * -1.0 + colorA * 0.0 + colorRA * -1.0 + 
	    colorL * 0.0 + color * 4.0 + colorR * 0.0 + 
	    colorLB * -1.0 + colorB * 0.0 + colorRB * -1.0
    ) / 1.0;
    
    float x = isf_FragNormCoord.x;
    float i = intensity * 100.;
    float d = xx;
    vec3 hsv = rgb2hsv(color.rgb);
	
	if(x > .9 + d) {
	    float g = 1. + ((avg.r + avg.g + avg.b) / 3.) * i * (hsv.z);
	    inputPixelColor = vec4(g, g, g, 1.);
	} else if (x > .8 - s + d) {
	    float g = 1. + ((avg.r + avg.g + avg.b) / 3.) * i;
	    inputPixelColor = color * g;
	} else if (x > .5 + d) {
	    float g = 1. + ((avg.r + avg.g + avg.b) / 3.) * i * (1. - hsv.z);
	    inputPixelColor = color * g;
	} else if (x > .3 + d) {
	    float g = 1. + ((avg.r + avg.g + avg.b) / 3.) * i * (hsv.z);
        inputPixelColor = color * g;
	} else {
    	inputPixelColor = color;
	}
	
	gl_FragColor = inputPixelColor;
}
