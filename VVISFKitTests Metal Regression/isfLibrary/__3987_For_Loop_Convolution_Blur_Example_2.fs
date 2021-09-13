/*
{
  "CATEGORIES" : [
    "Blur", "Examples"
  ],
  "TAGS" : [
    "Convolution"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "blurLevel",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "iterationCount",
      "TYPE" : "float",
      "MAX" : 10.0,
      "DEFAULT" : 2.0,
      "MIN" : 1.0
    }
  ],
  "CREDIT" : "by VIDVOX"
}
*/

void main()	{
	float mWeight = 1.0 - blurLevel;
	float nWeight = blurLevel / (pow(2.0*iterationCount,2.0)-1.0);
	int iterationCountInt = int(iterationCount);
	vec4 result = vec4(0.0);
	
	if (blurLevel > 0.0)	{
		//	goes iterationCountInt pixels in each direction, for a total of 25 pixels
		for (int i = -10;i <= 10;++i)	{
			if (abs(i)<abs(iterationCountInt))	{
				for (int j = -10;j <= 10;++j)	{
					if (abs(j)<abs(iterationCountInt))	{
						vec2 loc = gl_FragCoord.xy + vec2(i,j);
						vec4 color = IMG_PIXEL(inputImage,loc);
						//	if this is the center pixel...
						if ((i == 0)&&(j == 0))	{
							result.rgb += mWeight * color.rgb;
							result.a = color.a;
						}
						else	{
							result.rgb += nWeight * color.rgb;
						}
					}
				}
			}
		}
	}
	else	{
		result = IMG_THIS_NORM_PIXEL(inputImage);	
	}
	
	gl_FragColor = result;
}

