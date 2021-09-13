/*
{
  "CATEGORIES" : [
    "Generator",
    "Example"
  ],
  "DESCRIPTION" : "Morphs between a circle and a square",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "mixPosition",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "shapeSize",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "shapeCenterColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        1,
        1
      ]
    },
    {
      "NAME" : "shapeEdgeColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0.9,
        0,
        0.08,
        1.0
      ]
    }
  ],
  "CREDIT" : "VIDVOX"
}
*/

void main()	{
	vec4		inputPixelColor = vec4(0.0,0.0,0.0,0.0);
	vec2		pos = gl_FragCoord.xy / min(RENDERSIZE.x,RENDERSIZE.y);
	vec2		center = (RENDERSIZE.xy / 2.0) / min(RENDERSIZE.x,RENDERSIZE.y);
	float		maxSize = max(RENDERSIZE.x,RENDERSIZE.y) / (2.0*min(RENDERSIZE.x,RENDERSIZE.y)); 
	float		circleDistance = distance(pos,center);
	float		squareDistance = max(abs(pos.x-center.x),abs(pos.y-center.y));
	float		mixDistance = mix(circleDistance,squareDistance,mixPosition);

	if (mixDistance < shapeSize * maxSize)
		inputPixelColor = mix(shapeCenterColor,shapeEdgeColor,mixDistance / (shapeSize * maxSize));

	gl_FragColor = inputPixelColor;
}
