/*{
  "CREDIT": "by isak.burstrom",
  "CATEGORIES": [
    "Color Effect", "INKA"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "color",
      "TYPE": "color",
      "DEFAULT": [
        0.0,
        1.0,
        0.9,
        1
      ]
    },
    {
      "NAME": "colorscale",
      "TYPE": "float",
      "DEFAULT": 0.23
    },
    {
      "NAME": "retainLuma",
      "TYPE": "float",
      "DEFAULT": 0.45
    }
    
  ]
}*/

// WIP
// TODO: hue shift for each color w/ ratio = 0.45; and use only basecolor as color input

const vec4 lumcoeff = vec4(0.299, 0.587, 0.114, 0.0);

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

vec4 shiftHue(vec4 color, float n, float luma) {
	color.rgb = rgb2hsv(color.rgb);
	color.r = color.r + colorscale * n;
	color.b -= (1.0 - luma) * retainLuma;
	color.rgb = hsv2rgb(color.rgb);
	
	return color;
}

void main() {
	vec4 srcPixel = IMG_THIS_PIXEL(inputImage);
	float luma = dot(srcPixel,lumcoeff);
	float range = 1. / 3.;
	float index;
	vec4 color_dark, color_light;
	
	if(luma < 0.33) {
		index = 0.;
		color_dark = shiftHue(color, 0., luma);
		color_light = shiftHue(color, 1., luma);
	} else if(luma < 0.66) {
		index = 1.;
		color_dark = shiftHue(color, 1., luma);
		color_light = shiftHue(color, 2., luma);
	} else {
		index = 2.;
		color_dark = shiftHue(color, 2., luma);
		color_light = shiftHue(color, 3., luma);
	}
	
	gl_FragColor = mix(color_dark, color_light, (luma - index * range) / range);
}
