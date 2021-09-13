/*
{
  "INPUTS" : [
    {
      "NAME" : "restartNow",
      "TYPE" : "event"
    },
    {
      "MAX" : 1,
      "NAME" : "startThresh",
      "TYPE" : "float",
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "minDelta",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.10000000000000001,
      "MIN" : 0
    },
    {
      "NAME" : "deltaRand",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "smoothCurves",
      "TYPE" : "bool",
      "DEFAULT" : 0
    },
    {
      "NAME" : "boundarySplitGrowth",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "boundarySplitDeath",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "MAX" : 0.10000000000000001,
      "NAME" : "randomRegrowth",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "MAX" : 0.10000000000000001,
      "NAME" : "randomDeath",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2",
  "PASSES" : [
    {
      "TARGET" : "lastData",
      "PERSISTENT" : true
    }
  ],
  "CREDIT" : "VIDVOX",
  "VSN" : null,
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Based on Conway Game of Life"
}
*/


/*

Any live cell with fewer than two live neighbours dies, as if caused by under-population.
Any live cell with two or three live neighbours lives on to the next generation.
Any live cell with more than three live neighbours dies, as if by over-population.
Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

*/






float gray(vec4 n)
{
	return n.a*(n.r + n.g + n.b)/3.0;
}

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(11.9898,79.233))) * 43757.5453);
}


void main()	{
	vec4		inputPixelColor = vec4(0.0);
	vec2		loc = gl_FragCoord.xy;
	if ((TIME < 0.1)||(restartNow))	{
		//	randomize the start conditions
		float	alive = rand(vec2(TIME+1.0,2.1*TIME+0.1)*isf_FragNormCoord);
		if (alive > 1.0 - startThresh)	{
			inputPixelColor = vec4(1.0);
		}
	}
	else	{
		vec4	color = IMG_THIS_PIXEL(lastData);
		vec2	texc = loc;
		vec4	colorL = (texc.x <= 1.0) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(-1.0 , 0));
		vec4	colorR = (texc.x >= RENDERSIZE.x - 2.0) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(1.0 , 0));
		vec4	colorA = (texc.y >= RENDERSIZE.y - 2.0) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(0,1.0));
		vec4	colorB = (texc.y <= 1.0) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(0,-1.0));

		vec4	colorLA = ((texc.x >= RENDERSIZE.x - 2.0)||(texc.y >= RENDERSIZE.y - 2.0)) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(-1.0 , 1.0));
		vec4	colorRA = ((texc.x >= RENDERSIZE.x - 2.0)||(texc.y >= RENDERSIZE.y - 2.0)) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(1.0 , 1.0));
		vec4	colorLB = ((texc.y <= 1.0)||(texc.x <= 1.0)) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(-1.0 , -1.0));
		vec4	colorRB = ((texc.y <= 1.0)||(texc.x <= 1.0)) ? vec4(0.0) : IMG_PIXEL(lastData, texc.xy + vec2(1.0 , -1.0));
		
		float	neighborSum = gray(colorL) + gray(colorR) + gray(colorA) + gray(colorB) + gray(colorLA) + gray(colorRA) + gray(colorLB) + gray(colorRB);
		float	state = gray(color);
		
		inputPixelColor = color;

		float deltaVal = minDelta;
		float dr = (deltaRand > 0.0) ? deltaRand * rand(vec2(TIME+1.14,7.13*TIME+0.134)*loc) : 0.0;
		deltaVal += dr;
		if (deltaVal > 1.0)
			deltaVal = 1.0;
		float dv = 0.0;
		//	live cell
		if (state > 0.0)	{
			
			if (neighborSum < 3.0)	{
				//inputPixelColor -= (4.0 - neighborSum) * vec4(0.1);
				dv = (smoothCurves) ? (boundarySplitDeath + 3.0 - neighborSum) * (deltaVal) : (deltaVal);
				inputPixelColor -= vec4(dv);
			}
			else if (neighborSum > 5.0)	{
				//inputPixelColor -= (neighborSum - 5.0) * vec4(0.1);
				dv = (smoothCurves) ? (neighborSum - (5.0 - boundarySplitDeath)) * (deltaVal) : (deltaVal);
				inputPixelColor -= vec4(dv);
			}
			else	{
				dv = (smoothCurves) ? ((boundarySplitGrowth + 1.0 - abs(4.0-neighborSum))*(deltaVal)) : (deltaVal);
				inputPixelColor += vec4(dv);
				//inputPixelColor += vec4(0.1);
			}
			//inputPixelColor += ((2.0 - abs(3.0-neighborSum))*vec4(0.05));
			//	spontaneous death?
			float	alive = (randomDeath > 0.0) ? rand(vec2(TIME+1.0,2.1*TIME+0.1)*loc) : 0.0;
			if (alive > 1.0 - randomDeath)	{
				inputPixelColor -= vec4(deltaVal);
			}
		}
		//	dead cell
		else	{
			if ((neighborSum >= 3.0)&&(neighborSum <= 5.0))	{
				//	reproduction
				//inputPixelColor = vec4(0.1);
				dv = (smoothCurves) ? ((boundarySplitGrowth + 1.0 - abs(4.0-neighborSum))*(deltaVal)) : (deltaVal);
				inputPixelColor += vec4(dv);
			}
			else if (randomRegrowth > 0.0) {
				//	spontaneous reproduction
				float	alive = (randomRegrowth > 0.0) ? rand(vec2(TIME+1.0,2.1*TIME+0.1)*gl_FragCoord.xy) : 0.0;
				if (alive > 1.0 - randomRegrowth)	{
					inputPixelColor += ((boundarySplitGrowth + 5.0 - abs(4.0-neighborSum))*vec4(deltaVal));
				}	
			}
		}
	}
	//	clip the output (needed if using a float buffer!)
	if (inputPixelColor.r < 0.0)
		inputPixelColor = vec4(0.0);
	else if (inputPixelColor.r > 1.0)
		inputPixelColor = vec4(1.0);
	
	gl_FragColor = inputPixelColor;
}
