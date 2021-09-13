/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "resetPosition",
      "TYPE" : "event"
    },
    {
      "NAME" : "drawColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME" : "penSize",
      "TYPE" : "float",
      "MAX" : 0.2,
      "DEFAULT" : 0.01,
      "MIN" : 0
    },
    {
      "NAME" : "startPt",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.5,
        0.5
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "penDown",
      "TYPE" : "bool",
      "DEFAULT" : 1
    },
    {
      "NAME" : "eraseMode",
      "TYPE" : "bool",
      "DEFAULT" : 0
    },
    {
      "NAME" : "dirtyTip",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "penRate",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 1,
      "MIN" : 0
    }
  ],
  "PASSES" : [
    {
      "WIDTH" : "1",
      "HEIGHT" : "1",
      "TARGET" : "bufferPosition",
      "PERSISTENT" : true
    },
    {
      "TARGET" : "lastBuffer",
      "PERSISTENT" : true
    }
  ],
  "CREDIT" : ""
}
*/



float seed = 1.239;



float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}


void main()	{
	vec4		inputPixelColor = vec4(0.0);
	bool		doReset = false;
	if ((resetPosition)||(FRAMEINDEX<=1))
		doReset = true;
	
	if (PASSINDEX==0)	{
		if (doReset)	{
			inputPixelColor = vec4(startPt.x,startPt.y,0.0,1.0);
		}
		//	get the last position, move randomly by up to 1 pixel
		else	{
			inputPixelColor = IMG_THIS_PIXEL(bufferPosition);
			float	newDirection = rand(vec2(TIME,seed));
			vec2	shift = vec2(0.0,0.0);
			
			if (newDirection < 0.25)
				shift = vec2(1.0,0.0);
			else if (newDirection < 0.5)
				shift = vec2(-1.0,0.0);
			else if (newDirection < 0.75)
				shift = vec2(0.0,1.0);
			else
				shift = vec2(0.0,-1.0);
			shift = shift * penSize * penRate;
			inputPixelColor.rg += shift;
			
			if (inputPixelColor.r > 1.0)
				inputPixelColor.r = 1.0;
			else if (inputPixelColor.r < 0.0)
				inputPixelColor.r = 0.0;
			if (inputPixelColor.g > 1.0)
				inputPixelColor.g = 1.0;
			else if (inputPixelColor.g < 0.0)
				inputPixelColor.g = 0.0;
		}
	}
	else	{
		if (doReset)	{
			inputPixelColor = vec4(0.0,0.0,0.0,0.0);
		}
		else	{
			vec4	lastPos = IMG_NORM_PIXEL(bufferPosition,vec2(0.5));
			if ((penDown)&&(distance(isf_FragNormCoord,lastPos.rg) < penSize))	{
				inputPixelColor = (eraseMode) ? vec4(0.0) : drawColor;
				if (dirtyTip > 0.0)	{
					float	dirtRand = rand(isf_FragNormCoord*vec2(TIME,1.0+TIME));
					if (dirtRand <= dirtyTip)	{
						inputPixelColor = IMG_NORM_PIXEL(lastBuffer,isf_FragNormCoord);
					}
				}
			}
			else	{
				inputPixelColor = IMG_NORM_PIXEL(lastBuffer,isf_FragNormCoord)
			}

		}
	}
	
	gl_FragColor = inputPixelColor;
}
