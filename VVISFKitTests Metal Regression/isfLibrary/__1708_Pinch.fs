/*{
	"DESCRIPTION": "Pinch FX",
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
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "radius",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "center",
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
    vec2 textureCoordinate = vv_FragNormCoord;
	float aspectRatio = RENDERSIZE.y/RENDERSIZE.x;
     
    vec2 textureCoordinateToUse = vec2(textureCoordinate.x, (textureCoordinate.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
     float dist = distance(center, textureCoordinateToUse);
     textureCoordinateToUse = textureCoordinate;
     
     if (dist < radius)
     {
         textureCoordinateToUse -= center;
         float percent = 1.0 + ((0.5 - dist) / 0.5) * scale;
         textureCoordinateToUse = textureCoordinateToUse * percent;
         textureCoordinateToUse += center;
         
         gl_FragColor = IMG_NORM_PIXEL(inputImage, textureCoordinateToUse );
     }
     else
     {
         gl_FragColor = IMG_NORM_PIXEL(inputImage, textureCoordinate );
     }
     
  
}
