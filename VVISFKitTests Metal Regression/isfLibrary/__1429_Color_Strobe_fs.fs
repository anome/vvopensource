/*{
  "DESCRIPTION": "Strobes between input and a specified color",
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Color Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "strobeRate",
      "LABEL": "Strobe Rate",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0
    },
    {
      "NAME": "strobeColor",
      "LABEL": "Strobe Color",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ]
    }
  ],
  "PASSES": [
    {
      "TARGET": "lastState",
      "WIDTH:": 1,
      "HEIGHT": 1,
      "DESCRIPTION": "",
      "persistent": true
    },
    {}
  ]
}*/




void main()
{
	//	if this is the first pass, i'm going to read the position from the "lastState" image, and write a new position based on this and the hold variables
	if (PASSINDEX == 0)	{
		vec4		srcPixel = IMG_PIXEL(lastState,vec2(0.5));
		//	if the strobe rate is 0, strobe as fast as possible
		if (strobeRate == 0.0)	{
			srcPixel.g = 0.0;
			srcPixel.r = (srcPixel.r == 0.0) ? 1.0 : 0.0;
		}
		//	otherwise strobe for the specified strobeRate
		else	{
			srcPixel.r = (mod(TIME, 1.0) < strobeRate) ? 1.0 : 0.0;
		}
		gl_FragColor = srcPixel;
	}
	//	else this isn't the first pass- read the position value from the buffer which stores it
	else	{
		vec4 lastStateVector = IMG_PIXEL(lastState,vec2(0.5));
		vec4 srcPixel = IMG_THIS_PIXEL(inputImage);
		srcPixel = (lastStateVector.r == 0.0) ? srcPixel : strobeColor;
		gl_FragColor = srcPixel;
	}
}
