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
      "DEFAULT": 1
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

const float tau = 6.28318530718;
float dotScreenPattern(float ang, float sca, vec2 cent) {
	float s = sin( ang * tau ), c = cos( ang * tau );
	vec2 tex = isf_FragNormCoord * RENDERSIZE - cent * RENDERSIZE;
	vec2 point = vec2( c * tex.x - s * tex.y, s * tex.x + c * tex.y ) * max(sca,0.001);
	return ( sin( point.x ) * sin( point.y ) ) * 4.0;
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
	
	if(x > .6 + d) {
	    float g = 1. + (color.r + color.g + color.b) / 10.;
	    g = g * g;
	    
	    inputPixelColor = vec4(vec3(hsv.y) * (1. + avg.rgb * i), 1.);
	    vec3 hsv2 = rgb2hsv(inputPixelColor.rgb);
	    
	     
        float d = dotScreenPattern(0., 0.5, vec2(0.5)); 
        inputPixelColor = vec4(mix(color.rgb, inputPixelColor.rgb, pow(hsv.y, 4.)), 1.);
        float a = pow(hsv2.y, 2.) * pow(hsv.y, 3.) * d / 10.;
        inputPixelColor = vec4(mix(inputPixelColor.rgb, inputPixelColor.rgb * vec3(d), a), 1.);
        
        vec3 hsv3 = rgb2hsv(inputPixelColor.rgb);
        
        hsv3.x = hsv.x;
        hsv3.y = hsv.y;
        // hsv3.z = hsv.z; 
        
        inputPixelColor.rgb = hsv2rgb(hsv3);
	    inputPixelColor = vec4(mix(color.rgb, inputPixelColor.rgb, intensity), 1.);
	} else {
    	inputPixelColor = color;
	}
	
	gl_FragColor = inputPixelColor;
}
