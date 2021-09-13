/*{
	"DESCRIPTION": "ZoomBlur style FX",
	"CREDIT": "by IMIMOT (ported from https://github.com/BradLarson/GPUImage)",
	"CATEGORIES": [
		"Blur"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "blurSize",
			"TYPE": "float",
			"DEFAULT": 1.00,
			"MIN": 0.000,
			"MAX": 1.0
		},
		{
			"NAME": "blurCenter",
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
	
    // TODO: Do a more intelligent scaling based on resolution here
     vec2 samplingOffset = 1.0/100.0 * (blurCenter - textureCoordinate) * blurSize;
     
     vec4 fragmentColor = IMG_NORM_PIXEL(inputImage, textureCoordinate) * 0.18;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate + samplingOffset) * 0.15;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate + (2.0 * samplingOffset)) *  0.12;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate + (3.0 * samplingOffset)) * 0.09;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate + (4.0 * samplingOffset)) * 0.05;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate - samplingOffset) * 0.15;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate - (2.0 * samplingOffset)) *  0.12;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate - (3.0 * samplingOffset)) * 0.09;
     fragmentColor += IMG_NORM_PIXEL(inputImage, textureCoordinate - (4.0 * samplingOffset)) * 0.05;
     
     gl_FragColor = fragmentColor;
  
}
