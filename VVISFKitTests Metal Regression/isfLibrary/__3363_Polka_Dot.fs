/*{
  "DESCRIPTION": "Polda dot FX",
  "CREDIT": "by IMIMOT (ported from https://github.com/BradLarson/GPUImage)",
  "CATEGORIES": [
    "Halftone Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "fractionalWidthOfPixel",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": 0,
      "MAX": 0.1
    }
  ]
}*/


void main()
{
	float aspectRatio = RENDERSIZE.y/RENDERSIZE.x;
	vec2 textureCoordinate = isf_FragNormCoord;
	float dotScaling = 0.9;

    vec2 sampleDivisor = vec2(fractionalWidthOfPixel, fractionalWidthOfPixel / aspectRatio);
     
    vec2 samplePos = textureCoordinate - mod(textureCoordinate, sampleDivisor) + 0.5 * sampleDivisor;
    vec2 textureCoordinateToUse = vec2(textureCoordinate.x, (textureCoordinate.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
    vec2 adjustedSamplePos = vec2(samplePos.x, (samplePos.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
    float distanceFromSamplePoint = distance(adjustedSamplePos, textureCoordinateToUse);
    float checkForPresenceWithinDot = step(distanceFromSamplePoint, (fractionalWidthOfPixel * 0.5) * dotScaling);
     
    vec4 inputColor = IMG_NORM_PIXEL(inputImage, samplePos);

    gl_FragColor = vec4(inputColor.rgb * checkForPresenceWithinDot, inputColor.a);
  
}
