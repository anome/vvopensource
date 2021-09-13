/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Polar Coordinates",
  "ISFVSN" : "2",
  "CREDIT" : "VIDVOX",
  "INPUTS" : [
      {
      "NAME" : "rotate",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    }
  ]
}
*/


const float pi = 3.14159265359;


vec2 transformfunction (float a, float r)	{
	vec2	returnMe = vec2(0.0);
	returnMe.x = cos(a);
	returnMe.y = sin(a);
	return returnMe;
}


void main()	{
	vec4		inputPixelColor = vec4(0.0);
	
	//	'loc' is the location in pixels of this vertex.  we're going to convert this to polar coordinates (radius/angle)
	vec2		loc = RENDERSIZE * isf_FragNormCoord;
	//	'r' is the radius- the distance in pixels from 'loc' to the center of the rendering space
	float		r = distance(RENDERSIZE/2.0, loc);
	//	'a' is the angle of the line segment from the center to loc is rotated
	float		a = 2.0 * pi * rotate + atan ((loc.y-RENDERSIZE.y/2.0),(loc.x-RENDERSIZE.x/2.0));
	
	vec2		pt = transformfunction(a,r) * 0.5 + 0.5;
	float		dist = distance(pt,isf_FragNormCoord);
	inputPixelColor = vec4(pt.x,pt.y,r,1.0);
	
	gl_FragColor = inputPixelColor;
}
