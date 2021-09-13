/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
    {
      "MAX": 150,
      "MIN": 1,
      "DEFAULT":15,
      "NAME": "SCALE",
      "TYPE": "float"
},
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT":0.8,
      "NAME": "topColor",
      "TYPE": "float"
},
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT":0.6,
      "NAME": "leftColor",
      "TYPE": "float"
},
    {
      "MAX": 1.0,
      "MIN": 0.0,
      "DEFAULT":0.4,
      "NAME": "rightColor",
      "TYPE": "float"
},
    {
      "MAX": 2.0,
      "MIN": 0.0,
      "DEFAULT":0.5,
      "NAME": "r",
      "TYPE": "float"
},
    {
      "MAX": 2.0,
      "MIN": 0.0,
      "DEFAULT":0.5,
      "NAME": "g",
      "TYPE": "float"
},
    {
      "MAX": 2.0,
      "MIN": 0.0,
      "DEFAULT":0.5,
      "NAME": "b",
      "TYPE": "float"
}
	]
}*/

// CubeTiles by mojovideotech
// Source : http://glslsandbox.com/e#1610.3
// Rhombille tiling by @ko_si_nus
//

const float TAN30 = 0.5773502691896256;
const float COS30 = 0.8660254037844387;
const float SIN30 = 0.5;
const float XPERIOD = 2.0 * COS30;
const float YPERIOD = 2.0 + 2.0 * SIN30;
const float HALFXPERIOD = XPERIOD / 2.0;
const float HALFYPERIOD = YPERIOD / 2.0;


void main(void) {
		vec2 position = gl_FragCoord.xy / RENDERSIZE.y * SCALE;

	float x;
	float y = mod(position.y, YPERIOD);
	if (y < HALFYPERIOD) {
		x = mod(position.x, XPERIOD);
	}
	else {
		x = mod(position.x + HALFXPERIOD, XPERIOD);
		y -= HALFYPERIOD;
	}

	float color, opp;
	if (x < COS30) {
		color = leftColor;
		opp = TAN30 * (COS30 - x);
	}
	else {
		color = rightColor;
		opp = TAN30 * (x - COS30);
	}
	if (y < opp || opp < y-1.0) {
		color = topColor;
	}

	gl_FragColor = vec4((color/2.)*r, (color/2.)*g, (color/2.)*b,  1.0);
}