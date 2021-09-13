/*{
  "CREDIT": "by INKA",
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
      "LABEL": "Color",
      "NAME": "basecolor",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ]
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



vec3 rgb2hsv(vec3 c)	{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	//vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	//vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	vec4 p = c.g < c.b ? vec4(c.bg, K.wz) : vec4(c.gb, K.xy);
	vec4 q = c.r < p.x ? vec4(p.xyw, c.r) : vec4(c.r, p.yzx);
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)	{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec4 color_tone(vec4 outColor, float index) {
	float divisor = 3.0;
	float variation = 0.3236;
	float ratio = 0.45;
	
	// r = h 
	// g = s
	// b = v
	
	outColor.r = outColor.r - (ratio * index);
	
	outColor.b = (1.0 - index / 3.);
	
	//if (outColor.b < 0.1)	{
	//	outColor.b = outColor.b + variation * floor(index / (divisor));
	//	outColor.b = outColor.b - floor(outColor.b);
	//}
	
	return outColor;
}
void main () {
	
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
	lum = pow(lum,1.4);
	

	int ix = 0;
	float range = 1.0 / 5.0;

	vec4 hsvbasecolor = basecolor;
	hsvbasecolor.rgb = rgb2hsv(hsvbasecolor.rgb);
	
	vec4 color_1 = hsvbasecolor;
	vec4 color_2 = color_tone(hsvbasecolor, 3.);
	vec4 color_3 = color_tone(hsvbasecolor, 2.);
	vec4 color_4 = color_tone(hsvbasecolor, 1.);
	vec4 color_5 = color_tone(hsvbasecolor, 0.);

	//	orange to red
	vec4 startColor;
	vec4 endColor;
	
	//	cyan to green
	if (lum > range * 3.0)	{
		startColor = color_4;
		endColor = color_5;
		ix = 3;
	}
	//	blue to cyan
	else if (lum > range * 2.0)	{
		startColor = color_3;
		endColor = color_4;
		ix = 2;
	}
	// purple to blue
	else if (lum > range)	{
		startColor = color_2;
		endColor = color_3;
		ix = 1;
	}
	else	{
		startColor = color_1;
		endColor = color_2;
	}
	
	startColor.rgb = hsv2rgb(startColor.rgb);
	endColor.rgb = hsv2rgb(endColor.rgb);
	
	vec4 thermal = mix(startColor, endColor, (lum - float(ix) * range)/range);
	gl_FragColor = thermal;

}