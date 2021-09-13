/*{
	"CREDIT": "by INKA",
	"CATEGORIES": [
		"Tile Effect, INKA"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "size",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "sides",
			"TYPE": "float",
			"MIN": 0.5,
			"MAX": 2.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "patternmod",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 1.25,
			"DEFAULT": 1.0
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "angle",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "luma",
			"TYPE": "float",
			"MIN": 0.5,
			"MAX": 1.0,
			"DEFAULT": 0.8
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 500.0,
			"DEFAULT": 400.0
		},
		{
			"NAME": "perspective",
			"TYPE": "float",
			"MIN": 0.8,
			"MAX": 1.0,
			"DEFAULT": 0.9
		},

		{
			"NAME": "shift",
			"TYPE": "point2D",
			"DEFAULT": [
				0.0,
				0.0
			]
		}
	]
}*/

/*
 based on kaleidoscope tile by vidvox
*/

const float tau = 6.28318530718;



void main() {
	float movement = TIME * speed;
	float rot = floor(rotation * 4.) / 4.; 
	float s = sin(tau * rot);
	float c = cos(tau * rot);
	vec2 tex = vv_FragNormCoord * RENDERSIZE;
	float scale = 1.0 / max(size,0.001);
	float _luma = 1.0;
	float _patternmod = 1.0;
	vec2 point = vec2( c * tex.x - s * tex.y, s * tex.x + c * tex.y ) * scale;
	point = (point - shift) / RENDERSIZE;
	//	do this to repeat
	point = mod(point,1.0);
	if (point.x < 0.5)	{
		point.y = mod(point.y + 0.0/RENDERSIZE.y, 1.0);
	}
	else {
		point.y = mod(point.y + 0.0/RENDERSIZE.y, 1.0);
	}
	if (point.y < 0.5)	{
		point.x = mod(point.x + movement/RENDERSIZE.x, 1.0);
	}
	else {
		point.x = mod(point.x + movement * perspective/RENDERSIZE.x, 1.0);
		_luma = luma;
		_patternmod = patternmod;
	}
	//	do this for relection
	point = 1.0-abs(1.0-2.0*point);
	
	//	Now let's do a squish based on angle
	//	convert to polar coordinates
	vec2 center = vec2(0.5,0.5);
	float r = distance(center, point);
	float a = atan ((point.y-center.y),(point.x-center.x));
	
	// now do the kaleidoscope

	a = mod(a, tau/(sides * _patternmod));
	a = abs(a - tau/(sides * _patternmod)/2.);
	
	s = sin(a + tau * angle);
	c = cos(a + tau * angle);
	
	float zoom = RENDERSIZE.x / RENDERSIZE.y;
	
	point.x = (r * c)/zoom + 0.5;
	point.y = (r * s)/zoom + 0.5;

	gl_FragColor = IMG_NORM_PIXEL(inputImage,point) * _luma;
}