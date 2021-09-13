/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Circle Generator",
  "INPUTS" : [
    {
      "NAME" : "RadiusCircle",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.03,
      "MIN" : 0
    },
    {
      "NAME" : "BlurAmount",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.01,
      "MIN" : 0
    },
    {
      "NAME" : "ScreenPosition",
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
      "NAME" : "BgColor",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        0,
        1
      ]
    },
    {
      "NAME" : "FrontColor",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        1,
        1,
        1
      ]
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "Mandria - Recipient.cc based on the example e#45252.0"
}
*/
void main()	{
	vec2 posPixel = ScreenPosition.xy * RENDERSIZE.xy;
	vec2 position = (gl_FragCoord.xy-posPixel.xy)/min(RENDERSIZE.x,RENDERSIZE.y);
	const vec2 center=vec2(0.0);
	float w = smoothstep(0.0, BlurAmount, RadiusCircle-distance(position , center)); 
	gl_FragColor=mix(BgColor, FrontColor, w) ;

	
	
}