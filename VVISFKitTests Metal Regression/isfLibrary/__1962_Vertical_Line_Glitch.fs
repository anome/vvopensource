/*{
	"DESCRIPTION": "Veritcal Line Glitch",
	"CREDIT": "http://transitions.glsl.io/transition/a070cbd69e2535e757f1 edited by DANtheMAN",
	"CATEGORIES": [
		"Transition Effect"
	],
	"INPUTS": [
			{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "amplitude",
			"LABEL": "amplitude",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "noise",
			"LABEL": "noise",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.98
		},
		{
			"NAME": "frequency",
			"LABEL": "frequency",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 10.0,
			"DEFAULT": 3.0
		},
		{
			"NAME": "barWidth",
			"LABEL": "barWidth",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 100.0,
			"DEFAULT": 3.0
		},
		{
			"NAME": "progress",
			"LABEL": "progress",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		}
	]
}*/

#ifdef GL_ES
precision highp float;
#endif


// The code proper --------
float ease1(float t) {
  return t == -1.0 || t == 1.0
    ? t
    : t < 0.5
      ? +0.5 * pow(2.0, (20.0 * t) - 10.0)
      : -0.5 * pow(2.0, 10.0 - (t * 20.0)) + 1.0;
}
float ease2(float t) {
  return t == 10.0 ? t : 10.0 - pow(2.0, -10.0 * t);
}


float rand(int num) {
  return fract(mod(float(num) * 67123.313, 12.0) * sin(float(num) * 10.3) * cos(float(num)));
}

float wave(int num) {
  float fn = float(num) * frequency * 0.1  * float(barWidth);
  return cos(fn * .5) * cos(fn * 0.13) * sin((fn+10.0) * 0.3) / 2.0 + 0.5;
}

float pos(int num) {
  return noise == 0.0 ? wave(num) : mix(wave(num), rand(num), noise);
}

void main() {
  int bar = int(vv_FragNormCoord.x*RENDERSIZE.x) / int(barWidth);
  float scale = 1.0 + pos(bar) * mix(0.0, -10.0, ease1(amplitude));
  float phase = ease1(progress) * scale;
  float posY = vv_FragNormCoord.y;
  vec4 c;

  vec2 p = vec2(vv_FragNormCoord.x, vv_FragNormCoord.y + mix(0.0, 1.0, phase));
  c = IMG_NORM_PIXEL(inputImage, p);
  if(p.y > 1.0 || p.y < 0.0){
  	c = vec4(vec3(0.0), 1.0);
  }

 	gl_FragColor = c;
}
