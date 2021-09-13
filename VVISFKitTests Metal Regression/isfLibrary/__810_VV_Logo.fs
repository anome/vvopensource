/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Makes the VIDVOX logo",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "bgColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        0,
        0
      ]
    },
    {
      "NAME" : "fillColor",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        1,
        1,
        1
      ]
    },
    {
      "NAME" : "splitPos",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "MIN" : 0
    }
  ],
  "CREDIT" : "VIDVOX"
}
*/



float sign(vec2 p1, vec2 p2, vec2 p3)
{
	return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
}


bool PointInTriangle(vec2 pt, vec2 v1, vec2 v2, vec2 v3)
{
	bool b1, b2, b3;

	b1 = sign(pt, v1, v2) < 0.0;
	b2 = sign(pt, v2, v3) < 0.0;
	b3 = sign(pt, v3, v1) < 0.0;

	return ((b1 == b2) && (b2 == b3));
}


//	bot legs
vec2 pt0 = vec2(0.0,0.582);
vec2 pt1 = vec2(0.0,0.463);
vec2 pt2 = vec2(0.748,0.247);
vec2 pt3 = vec2(0.748,0.335);

vec2 pt4 = vec2(1.0,0.582);
vec2 pt5 = vec2(1.0,0.463);
vec2 pt6 = vec2(0.252,0.247);
vec2 pt7 = vec2(0.252,0.335);

// top arms
// re-use pt2,pt3,pt6,p7
vec2 pt8 = vec2(0.0,0.0);
vec2 pt9 = vec2(0.0,0.12);

vec2 pt10 = vec2(1.0,0.0);
vec2 pt11 = vec2(1.0,0.12);


void main()	{
	vec4		inputPixelColor = bgColor;
	vec2		pt = isf_FragNormCoord;
	
	//	sorry these pt coords are flipped and not centered and I'm lazy right now
	//	so fix it with math!
	float		modSplitPos = (splitPos < 1.0) ? splitPos : -2.0+splitPos;
	pt -= vec2(0.5);
	pt.y *= RENDERSIZE.y/RENDERSIZE.x;
	pt.y *= 1.0418;
	pt += vec2(0.5);
	pt.y = 1.0 - pt.y;
	pt.y -= 0.418 / 2.0;
	
	// bottom
	if (PointInTriangle(pt-vec2(modSplitPos,0.0),pt0,pt1,pt2))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt-vec2(modSplitPos,0.0),pt0,pt2,pt3))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt+vec2(modSplitPos,0.0),pt5,pt7,pt4))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt+vec2(modSplitPos,0.0),pt6,pt7,pt5))	{
		inputPixelColor = fillColor;
	}
	// top
	else if (PointInTriangle(pt-vec2(modSplitPos,0.0),pt8,pt9,pt2))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt-vec2(modSplitPos,0.0),pt9,pt2,pt3))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt+vec2(modSplitPos,0.0),pt10,pt7,pt11))	{
		inputPixelColor = fillColor;
	}
	else if (PointInTriangle(pt+vec2(modSplitPos,0.0),pt6,pt7,pt10))	{
		inputPixelColor = fillColor;
	}
	
	gl_FragColor = inputPixelColor;
}
