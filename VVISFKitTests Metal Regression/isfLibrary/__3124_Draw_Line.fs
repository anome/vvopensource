/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "point1",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.25,
        0.25
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "point2",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.75,
        0.75
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "lineWidth",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 3,
      "MIN" : 0
    }
  ],
  "CREDIT" : ""
}
*/

//	returns the distance from pt0 to the line defined by pt1 and pt2
float distancePtToLine(vec2 pt1, vec2 pt2, vec2 pt0)	{
	return ((pt2.y-pt1.y)*pt0.x-(pt2.x-pt1.x)*pt0.y+pt2.x*pt1.y-pt2.y*pt1.x)/(sqrt(pow(pt2.y-pt1.y,2.0)+pow(pt2.x-pt1.x,2.0)));
}

float isPtOnLine(vec2 pt1, vec2 pt2, vec2 pt0)	{
	float	returnMe = 0.0;
	float	lw = lineWidth / min(RENDERSIZE.x,RENDERSIZE.y);
	if ((pt0.x >= min(pt1.x,pt2.x) - lw/2.0)&&(pt0.x <= max(pt1.x,pt2.x) + lw/2.0)&&(pt0.y >= min(pt1.y,pt2.y) - lw/2.0)&&(pt0.y <= max(pt1.y,pt2.y) + lw/2.0))	{
		if (abs(distancePtToLine(pt1,pt2,pt0)) < lw)	{
			returnMe = 1.0;
		}
	}

	return returnMe;
}

void main()	{
	vec4		inputPixelColor = vec4(0.0);
	vec2		loc = isf_FragNormCoord;
	inputPixelColor = vec4(isPtOnLine(point1,point2,loc));
	
	gl_FragColor = inputPixelColor;
}
