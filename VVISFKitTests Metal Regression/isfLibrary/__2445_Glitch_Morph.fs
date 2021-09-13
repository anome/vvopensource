/*
{
  "CATEGORIES" : [
    "Glitch"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "amount",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "strength",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.1,
      "MIN" : 0
    },
    {
      "NAME" : "xGain",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "MIN" : -2
    },
    {
      "NAME" : "yGain",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "MIN" : -2
    }
  ],
  "CREDIT" : "VIDVOX"
}
*/

//	inspired by paniq
//	https://gl-transitions.com/editor/morph

void main()	{
	vec4		ca = IMG_THIS_PIXEL(inputImage);
	vec2		oa = vec2(0.0);
	oa.x = ((ca.r + ca.b) - 1.0) * xGain;
	oa.y = ((ca.g + ca.b) - 1.0) * yGain;
	oa = oa * strength;
	vec4		ra = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy + oa * amount);
	
	gl_FragColor = mix(ca,ra,amount);
}
