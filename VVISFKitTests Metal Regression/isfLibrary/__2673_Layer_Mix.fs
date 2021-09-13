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
    },
    {
      "NAME": "shapeMask",
      "TYPE": "bool"
    },
    {
      "NAME": "maskSize",
      "TYPE": "float",
      "DEFAULT": 1
    },
    {
      "NAME": "fill",
      "TYPE": "bool"
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

const vec2 direction = vec2(1.0,1.0);
const float smoothness = 0.25;
const float SQRT_2 = 1.414213562373;

float isPointInShape(vec2 pt, vec4 shapeCoordinates)	{
	float returnMe = distance(pt, vec2(shapeCoordinates.xy + shapeCoordinates.zw / 2.0));
	if (returnMe < min(shapeCoordinates.z,shapeCoordinates.w) / 2.0)	{
		returnMe = 1.0;
	}
	else	{
		returnMe = 0.0;
	}
	return returnMe;
}

vec4 mixLuma(vec4 layerA, vec4 layerB) {
	float lumaA = (layerA.r + layerA.g + layerA.b) / 3.;
	float lumaB = (layerB.r + layerB.g + layerB.b) / 3.;
	
	// treshold from light
	if(lumaB >= lumaA - (1.0 - treshold) && lumaB > treshold)
		return layerB;
	
	// original
	else 
		return layerA;
}

void main()
{
	vec4 original = IMG_THIS_PIXEL(inputImage);
	vec4 _mask = IMG_THIS_PIXEL(maskImage);
	
	vec2 normMaskSrcCoord;
	
	normMaskSrcCoord.x = (gl_FragCoord.x-_mask.x) / RENDERSIZE.x;
	normMaskSrcCoord.y = (gl_FragCoord.y-_mask.y) / RENDERSIZE.y;
	
	// sizing mode fit, otherwise copy
	if (fill) {
		vec4 a = vec4(0.0, 0.0, _maskImage_imgRect.z, _maskImage_imgRect.w);
		vec4 b = vec4(0,0,RENDERSIZE.x,RENDERSIZE.y);
		vec4 rectMask = vec4(0.0);
		
		float bAspect = b.z/b.w;
		float aAspect = a.z/a.w;
		
		if (bAspect > aAspect)	{
			rectMask.z = b.z;
			rectMask.w = rectMask.z / aAspect;
		} else if (bAspect <= aAspect) {
			rectMask.w = b.w;
			rectMask.z = rectMask.w * aAspect;
		}
		
		rectMask.x = (b.z-rectMask.z)/2.0+b.x;
		rectMask.y = (b.w-rectMask.w)/2.0+b.y;
		
		normMaskSrcCoord.x = (gl_FragCoord.x-rectMask.x)/rectMask.z;
		normMaskSrcCoord.y = (gl_FragCoord.y-rectMask.y)/rectMask.w;
	}
	
	vec4 mask = IMG_NORM_PIXEL(maskImage, normMaskSrcCoord);
	float gradientMask = 1.0;
	
	if(shapeMask) {
		vec2 p = center / RENDERSIZE;
		float gradient = distance(p * RENDERSIZE.x / RENDERSIZE.y, isf_FragNormCoord * RENDERSIZE.x / RENDERSIZE.y);
		gradientMask = 1.0 - clamp((gradient - maskSize) / (maskSize), 0., 1.);
	}
	
	vec4 color = mixLuma(original, mask);

	gl_FragColor = mix(original, color, gradientMask);
}
