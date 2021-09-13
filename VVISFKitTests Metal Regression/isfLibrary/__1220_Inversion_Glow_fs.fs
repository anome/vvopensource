/*{
  "DESCRIPTION": "performs a 3d rotation",
  "CREDIT": "by zoidberg",
  "CATEGORIES": [
    "Color Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "gain",
      "LABEL": "Gain",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 1,
      "DEFAULT": 1
    },
    {
      "NAME": "variance",
      "LABEL": "Original Level",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/


varying vec2		texOffsets[6];


float gray(vec4 color)	{
	return (color.r + color.g + color.b) / 3.0;
}


void main()
{
	vec4 original = IMG_THIS_PIXEL(inputImage);
	
	//	Using an Atkinson-like kernel!
	float errorSum = 0.0;
	
	//	accumulate the distances from 0.5;
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[0])));
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[1])));
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[2])));
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[3])));
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[4])));
	errorSum += (0.5-gray(IMG_NORM_PIXEL(inputImage,texOffsets[5])));
	errorSum = errorSum / 3.0;
	
	vec4 returnMe = abs(original - variance);
	returnMe.rgb += gain * vec3(errorSum);
	returnMe.a = original.a;
	
	gl_FragColor = returnMe;
}