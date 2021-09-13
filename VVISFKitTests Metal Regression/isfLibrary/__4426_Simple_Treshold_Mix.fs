/*{
  "CREDIT": "by isak.burstrom",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "INKA",
    "Glitch"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "maskImage",
      "TYPE": "image"
    },
    {
      "NAME": "treshold",
      "TYPE": "float",
      "DEFAULT": 1
    }
  ]
}*/

const vec2 direction = vec2(1.0,1.0);
const float smoothness = 0.25;
const float SQRT_2 = 1.414213562373;

vec4 mixLuma(vec4 layerA, vec4 layerB) {
	float lumaA = (layerA.r + layerA.g + layerA.b) / 3.;
	float lumaB = (layerB.r + layerB.g + layerB.b) / 3.;
	// treshold from light
	return (lumaB >= lumaA - (1.0 - treshold) && lumaB > treshold) ? layerB : layerA;
}

void main()
{
	vec4 original = IMG_THIS_PIXEL(inputImage);
	vec4 _mask = IMG_THIS_PIXEL(maskImage);
	
	vec2 normMaskSrcCoord;
	
	normMaskSrcCoord.x = (gl_FragCoord.x-_mask.x) / RENDERSIZE.x;
	normMaskSrcCoord.y = (gl_FragCoord.y-_mask.y) / RENDERSIZE.y;
	
	
	vec4 mask = IMG_NORM_PIXEL(maskImage, normMaskSrcCoord);
	float gradientMask = 1.0;
	
	vec4 color = mixLuma(original, mask);

	gl_FragColor = mix(original, color, gradientMask);
}
