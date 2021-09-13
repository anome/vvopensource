/*{
  "CREDIT": "by carter rosenberg",
  "CATEGORIES": [
    "Color Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "exposure",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "treshold",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/



void main() {
	//	based on
	//	https://developer.apple.com/library/mac/documentation/graphicsimaging/reference/CoreImageFilterReference/Reference/reference.html#//apple_ref/doc/filter/ci/CIExposureAdjust
	vec4 color = IMG_THIS_PIXEL(inputImage);
	float luma = (color.r + color.g + color.b) / 3.0;


	color.rgb = color.rgb * (1.0 + max(0., (luma * 2. - treshold)) * pow(2., exposure * 3.)); //pow(1.0 + (0.25 * power), exposure * 2.);
	gl_FragColor = color;
}
