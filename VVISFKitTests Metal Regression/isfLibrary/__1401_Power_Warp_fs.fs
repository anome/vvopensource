/*{
	"DESCRIPTION": "Power curves distortions with shifting + Side Scroller And Flip",
	"CREDIT": "Vidvox + Brian Chasalow",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "power_x",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.25,
			"MAX": 4.0
		},
		{
			"NAME": "power_y",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.25,
			"MAX": 4.0
		},
		{
			"NAME": "shift_x",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "shift_y",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "mode_x",
			"TYPE": "long",
			"VALUES": [
				0,
				1
			],
			"LABELS": [
				"Style 1",
				"Style 2"
			],
			"DEFAULT": 0
		},
		{
			"NAME": "mode_y",
			"TYPE": "long",
			"VALUES": [
				0,
				1
			],
			"LABELS": [
				"Style 1",
				"Style 2"
			],
			"DEFAULT": 0
		},
		 {
 "NAME": "mirrorHorizontal",
 "TYPE": "bool",
 "MIN": false,
 "MAX": true,
 "DEFAULT": true
 },
 {
 "NAME": "mirrorVertical",
 "TYPE": "bool",
 "MIN": false,
 "MAX": true,
 "DEFAULT": true
 }
	]
}*/


const float pi = 3.14159265359;



void main()	{
	vec4		inputPixelColor;
	vec2		pos = vv_FragNormCoord.xy;
	
	if (mode_x == 0)	{
		pos.x = pow(pos.x, power_x);
	}
	else	{
		if (pos.x > 0.5)
			pos.x = 0.5 + pow(2.0*(pos.x - 0.5), power_x) / 2.0;
		else	{
			pos.x = pow(1.0 - 2.0*pos.x, power_x) / 2.0;
			pos.x = 0.5 - pos.x;
		}
	}
	// pos.x = mod(pos.x + shift_x, 1.0);
	
	if (mode_y == 0)	{
		pos.y = pow(pos.y, power_y);
	}
	else	{
		if (pos.y > 0.5)
			pos.y = 0.5 + pow(2.0*(pos.y - 0.5), power_y) / 2.0;
		else	{
			pos.y = pow(1.0 - 2.0*pos.y, power_y) / 2.0;
			pos.y = 0.5 - pos.y;
		}
	}
	// pos.y = mod(pos.y + shift_y, 1.0);
	

	
	
	pos.x += shift_x;
	pos.y += shift_y;
	vec2 modded = mod(pos,1.0);
	
	if(mirrorHorizontal && pos.x >= 1.0 && pos.x <= 2.0)
		modded = vec2(1.0-modded.x, modded.y);
	if(mirrorVertical && pos.y >= 1.0 && pos.y <= 2.0)
		modded = vec2(modded.x, 1.0-modded.y);
	
	
	inputPixelColor = IMG_NORM_PIXEL(inputImage, modded);
	
	gl_FragColor = inputPixelColor;
}
