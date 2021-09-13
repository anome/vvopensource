/*
{
  "CATEGORIES" : [
    "Geometry Adjustment"
  ],
  "DESCRIPTION" : "Performs a rotation about a specified point",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "centerPt",
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
      "NAME" : "rotateAngle",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    }
  ],
  "CREDIT" : ""
}
*/


const float pi = 3.14159265359;


vec2 rotatePoint(vec2 pt, float angle, vec2 center)
{
	vec2 returnMe;
	float s = sin(2.0 * angle * pi);
	float c = cos(2.0 * angle * pi);

	returnMe = pt;

	// translate point back to origin:
	returnMe.x -= center.x;
	returnMe.y -= center.y;

	// rotate point
	float xnew = returnMe.x * c - returnMe.y * s;
	float ynew = returnMe.x * s + returnMe.y * c;

	// translate point back:
	returnMe.x = xnew + center.x;
	returnMe.y = ynew + center.y;
	return returnMe;
}


void main()	{
	vec2		loc = gl_FragCoord.xy;
	loc = rotatePoint(loc, rotateAngle, centerPt * RENDERSIZE);
	
	vec4		inputPixelColor = vec4(0.0);
	if ((loc.x < 0.0)||(loc.y < 0.0)||(loc.x > RENDERSIZE.x)||(loc.y > RENDERSIZE.y))	{
		
	}
	else{
		inputPixelColor = IMG_PIXEL(inputImage, loc);
	}
	gl_FragColor = inputPixelColor;
}
