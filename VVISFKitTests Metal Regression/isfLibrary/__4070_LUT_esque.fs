
/*{
  "CREDIT": "by joshuabatty",
  "CATEGORIES": [
    "Color Effect",
    "INKA"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
   	{
		"NAME": "luma",
		"TYPE": "float",
		"DEFAULT": 0.5,
		"MIN": 0.0,
		"MAX": 1.0
	},
 	{
		"NAME": "color_range",
		"TYPE": "float",
		"DEFAULT": 5.0,
		"MIN": 0.0,
		"MAX": 5.0
	},
	{
		"NAME": "drywet",
		"TYPE": "float",
		"DEFAULT": 1.0,
		"MIN": 0.0,
		"MAX": 1.0
	}
  ]
}*/


//	adapted from vidvox's thermal vision effect

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

//--------------------------------------------------------
vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

vec4 lut(float pos){
	return vec4(pal( pos, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.33,0.67) ),1.0);
}

//---------------------------------------------------------
#define PI 3.14159265359
// Cheers to macbooktail for the hue code https://www.shadertoy.com/view/MlSXWd
vec3 hue(vec3 color, float shift, float chroma_amp) {

    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI     = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ     = vec3 (0.212, -0.523, 0.311);

    const vec3  kYIQToR   = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG   = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB   = vec3 (1.0, -1.107, 1.704);

    // Convert to YIQ
    float   YPrime  = dot (color, kRGBToYPrime);
    float   I      = dot (color, kRGBToI);
    float   Q      = dot (color, kRGBToQ);

    // Calculate the hue and chroma
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q) * chroma_amp;

    // Make the user's adjustments
    hue += shift;

    // Convert back to YIQ
    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    // Convert back to RGB
    vec3    yIQ   = vec3 (YPrime, I, Q);
    color.r = dot (yIQ, kYIQToR);
    color.g = dot (yIQ, kYIQToG);
    color.b = dot (yIQ, kYIQToB);

    return color;
}

void main ()	{
	
	vec4 color = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
	vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
	vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
	vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

	vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
	vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
	vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
	vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);

	vec4 avg = (color + colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB) / 9.0;
	
	//float lum = (avg.r+avg.g+avg.b)/3.0;
	float lum = dot(vec3(0.30, 0.59, 0.11), avg.rgb);
	lum = pow(lum,luma);

	int ix = 0;
	float range = 1.0 / color_range;
	
	//	orange to red
	vec4 startColor;
	vec4 endColor;
	
	vec4 col1 = lut(0.0);
	vec4 col2 = lut(0.25);
	vec4 col3 = lut(0.5);
	vec4 col4 = lut(0.75);
	vec4 col5 = lut(1.0);
	
	//	cyan to green
	if (lum > range * 3.0)	{
		startColor = col4;
		endColor = col5;
		ix = 3;
	}
	//	blue to cyan
	else if (lum > range * 2.0)	{
		startColor = col3;
		endColor = col4;
		ix = 2;
	}
	// purple to blue
	else if (lum > range)	{
		startColor = col2;
		endColor = col3;
		ix = 1;
	}
	else	{
		startColor = col1;
		endColor = col2;
	}

	vec4 thermal = mix(startColor,endColor,(lum - float(ix) * range)/range);
	thermal *= color;
	vec4 final = mix(color, thermal, drywet);
	gl_FragColor = final;

}