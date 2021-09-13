/*{
	"DESCRIPTION": "Based on Conway Game of Life",
	"CREDIT": "VIDVOX",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
		{
			"NAME": "restartNow",
			"TYPE": "event"
		},
		{
			"NAME": "startThresh",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.01,
			"MAX": 0.99
		},
		{
			"NAME": "randomRegrowth",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.0,
			"MAX": 0.1
		},
		{
			"NAME": "randomDeath",
			"TYPE": "float",
			"DEFAULT": 0.0525,
			"MIN": 0.0,
			"MAX": 0.1
		}
	],
	"PERSISTENT_BUFFERS": [
		"lastData"
	],
	"PASSES": [
		{
			"TARGET":"lastData"
		}
	]
	
}*/


/*

Any live cell with fewer than two live neighbours dies, as if caused by under-population.
Any live cell with two or three live neighbours lives on to the next generation.
Any live cell with more than three live neighbours dies, as if by over-population.
Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

*/

// Life+ by mojovidotech
// Remix of 
// Life by DavidLublin
// http://www.interactiveshaderformat.com/sketches/744 by DavidLublin


varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

//---------------------------------
// noise functions by mojovideotech

#define Q floor(TIME+100.)/(8320.51*sqrt(1213.93/startThresh))

float mash(float x)
	{
	return mod(mod(x, Q)*x, startThresh);		
	}
float hash(float x)
	{
	return mod(mod(x, Q)*x, randomRegrowth);		
	}
float bash(float x)
	{
	return mod(mod(x, Q)*x,randomDeath);		
	}
//---------------------------------	
	
float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}


void main()	{
	float c1 = fract(hash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y)));
	float c2 = fract(bash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y))); 
	float c3 = fract(mash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y))); 
	vec4 col = vec4(c1,c2,c3,0.2+(startThresh*0.5));
	vec4		inputPixelColor = vec4(col);
	vec2		loc = gl_FragCoord.xy;
	
	if ((TIME < 0.5)||(restartNow))	{
		//	randomize the start conditions
		float	alive = rand(vec2(mash(TIME)+1.0,2.1*mash(TIME)+0.1)*loc);
		if (alive > 1.0 - startThresh)	{
			inputPixelColor = vec4(1.0);
		}
	}
	else	{
		vec4	color = IMG_PIXEL(lastData, loc);
		vec4	colorL = IMG_PIXEL(lastData, left_coord);
		vec4	colorR = IMG_PIXEL(lastData, right_coord);
		vec4	colorA = IMG_PIXEL(lastData, above_coord);
		vec4	colorB = IMG_PIXEL(lastData, below_coord);

		vec4	colorLA = IMG_PIXEL(lastData, lefta_coord);
		vec4	colorRA = IMG_PIXEL(lastData, righta_coord);
		vec4	colorLB = IMG_PIXEL(lastData, leftb_coord);
		vec4	colorRB = IMG_PIXEL(lastData, rightb_coord);
		
		float	neighborSum = gray(colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB);
		float	state = gray(color);
		
		//	live cell
		if (state > 0.0)	{
			if (neighborSum < 3.0)	{
				//	under population
				inputPixelColor = vec4(0.5,0.0,0.0,0.5);
			}
			else if (neighborSum < 5.0)	{
				//	status quo
				inputPixelColor = vec4(0.9,0.9,1.0,1.0);
				
				//	spontaneous death?
				float	alive = rand(vec2(bash(TIME)+1.0,2.1*hash(TIME)+0.1)*loc);
				if (alive > 1.0 - randomDeath)	{
					inputPixelColor = vec4(0.0,0.5,0.0,0.5);
				}
			}
			else	{
				//	over population
				inputPixelColor = vec4(0.0,0.0,0.5,0.5);
			}
		}
		//	dead cell
		else	{
			if ((neighborSum > 3.0)&&(neighborSum < 5.0))	{
				//	reproduction
				inputPixelColor = vec4(1.0,0.0,1.0,1.0);
			}
			else if (neighborSum < 3.0)	{
				//	spontaneous reproduction
				float	alive = rand(vec2(hash(TIME)+1.0,2.1*bash(TIME)+0.1)*loc);
				if (alive > 1.0 - randomRegrowth)	{
					inputPixelColor = vec4(1.0,0.0,1.0,1.0);
				}
			}
		}
	}
	
	gl_FragColor = inputPixelColor+col;
}
