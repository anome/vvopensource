/*{
  "DESCRIPTION": "Swirl FX",
  "CREDIT": "by IMIMOT (ported from https://github.com/BradLarson/GPUImage)",
  "CATEGORIES": [
    "Distortion Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "radius",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "angle",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "swirlCenter",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ]
    }
  ]
}*/

void main()
{
     vec2 textureCoordinate = isf_FragNormCoord;
     vec2 textureCoordinateToUse = textureCoordinate;
     
     vec2 center = swirlCenter;
     
     float dist = distance(center, textureCoordinate);
     if (dist < radius)
     {
         textureCoordinateToUse -= center;
         float percent = (radius - dist) / radius;
         float theta = percent * percent * angle * 8.0;
         float s = sin(theta);
         float c = cos(theta);
         textureCoordinateToUse = vec2(dot(textureCoordinateToUse, vec2(c, -s)), dot(textureCoordinateToUse, vec2(s, c)));
         textureCoordinateToUse += center;
     }
     
     gl_FragColor = IMG_NORM_PIXEL(inputImage, textureCoordinateToUse);
  
}
