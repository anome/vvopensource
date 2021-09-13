/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "Simple filter",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "threshold",
      "TYPE" : "float",
      "MAX" : 2.5,
      "DEFAULT" : 1.15,
      "MIN" : 0.5,
      "IDENTITY" : 0
    }
  ],
  "CREDIT" : "Eliot Lash"
}
*/

void main()	{
	vec4		inputPixelColor;

	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	
	if(length(inputPixelColor) < threshold) {
		inputPixelColor.r += 1.0;
		inputPixelColor.b += 0.5;
	}
	
	gl_FragColor = inputPixelColor;
}
