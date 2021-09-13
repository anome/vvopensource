/*{
	"DESCRIPTION": "Halftone FX",
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
			"MIN": 0.0,
			"MAX": 0.1
		}
		
	]
}*/

const vec3 W = vec3(0.2125, 0.7154, 0.0721);

void main()
{
	float aspectRatio = RENDERSIZE.y/RENDERSIZE.x;
	
	vec2 textureCoordinate = vv_FragNormCoord;

    vec2 sampleDivisor = vec2(fractionalWidthOfPixel, fractionalWidthOfPixel / aspectRatio);
     
     vec2 samplePos = textureCoordinate - mod(textureCoordinate, sampleDivisor) + 0.5 * sampleDivisor;
     vec2 textureCoordinateToUse = vec2(textureCoordinate.x, (textureCoordinate.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
     vec2 adjustedSamplePos = vec2(samplePos.x, (samplePos.y * aspectRatio + 0.5 - 0.5 * aspectRatio));
     float distanceFromSamplePoint = distance(adjustedSamplePos, textureCoordinateToUse);
     
     vec3 sampledColor = IMG_NORM_PIXEL(inputImage, samplePos ).rgb;
     float dotScaling = 1.0 - dot(sampledColor, W);
     
     float checkForPresenceWithinDot = 1.0 - step(distanceFromSamplePoint, (fractionalWidthOfPixel * 0.5) * dotScaling);
     
     gl_FragColor = vec4(vec3(checkForPresenceWithinDot), 1.0);
  
}
