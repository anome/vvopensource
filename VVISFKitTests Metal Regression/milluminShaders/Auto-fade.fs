/*{
	"CREDIT": "by Anomes",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "mode",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2,
				3,
				4,
				5,
				6
			],
			"LABELS": [
				"Sin",
				"Cos",
				"Triangle",
				"Square",
				"Sawtooth Up",
				"Sawtooth Down",
				"Random"
			],
			"DEFAULT": 0,
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 50.0,
			"DEFAULT": 10.0
		}
	],
}*/


#define PI 3.14159


float random(float seed)
{
	return fract(  sin(seed)*43758.5453  +  seed  );
}


void main()
{
	vec2 pos = gl_FragCoord.xy / RENDERSIZE;
	vec4 color = IMG_NORM_PIXEL(inputImage, pos);
	float factor = 0.;
	if( mode == 0 ) // sin
	{
	 	factor = (  sin(mod(TIME*speed/10.,1.)*2.*PI)  +1.)/2.;
	}
	else if( mode == 1 ) // cos
	{
	 	factor = (  cos(mod(TIME*speed/10.,1.)*2.*PI)  +1.)/2.;
	}
	else if( mode == 2 ) // triangle
	{
		factor = mod(TIME*speed/10.,1.);
		factor = (  factor < 0.5  ?  factor*2.  :  (1.-factor)*2.  );
	}
	else if( mode == 3 ) // square
	{
		factor = mod(TIME*speed/10.,1.);
		factor = (  factor < 0.5  ?  0.  :  1.  );
	}
	else if( mode == 4 ) // sawtooth up
	{
		factor = fract(  mod(TIME*speed/10.,1.)  );
	}
	else if( mode == 5 ) // sawtooth down
	{
		factor = fract(  -mod(TIME*speed/10.,1.)  );
	}
	else if( mode == 6 ) // random
	{
		factor = random(  floor(TIME*speed)  );
	}
	color.a *= factor;
	gl_FragColor = color;
}

