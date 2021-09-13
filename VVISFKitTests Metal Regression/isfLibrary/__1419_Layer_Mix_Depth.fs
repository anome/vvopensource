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
      "NAME": "maskSize",
      "TYPE": "float",
      "DEFAULT": 1
    },
    {
      "NAME": "fill",
      "TYPE": "bool"
    },
	{
		"NAME": "centerpoint",
		"TYPE": "point2D",
		"DEFAULT": [
			0.5,
			0.5
		]
	}
  ]
}*/



vec4 mixLuma(vec4 original, vec4 mask) {
	float lumaSrc = (original.r + original.g + original.b) / 3.;
	float lumaMask = (mask.r + mask.g + mask.b) / 3.;
	vec4 color;
	
	if(lumaSrc < treshold && lumaMask > treshold)
		return mask;
	else
		return original;
}

void main()
{
	vec4 original = IMG_THIS_PIXEL(inputImage);
	vec4 _mask = IMG_THIS_PIXEL(maskImage);
	
	vec2 normMaskSrcCoord;
	
	normMaskSrcCoord.x = (gl_FragCoord.x-_mask.x)/RENDERSIZE.x;
	normMaskSrcCoord.y = (gl_FragCoord.y-_mask.y)/RENDERSIZE.y;
	
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
	
	vec2 modifiedCenter = centerpoint;
	vec4 mask;
	
	normMaskSrcCoord.x = (normMaskSrcCoord.x - modifiedCenter.x) * (1.0/maskSize) + modifiedCenter.x;
	normMaskSrcCoord.y = (normMaskSrcCoord.y - modifiedCenter.y) * (1.0/maskSize) + modifiedCenter.y;
	
	if ((normMaskSrcCoord.x < 0.0) || (normMaskSrcCoord.y < 0.0) || (normMaskSrcCoord.x > 1.0) || (normMaskSrcCoord.y > 1.0))	{
		mask = vec4(0.0);
	} else {
		mask = IMG_NORM_PIXEL(maskImage, normMaskSrcCoord);
	}
	
	vec4 color = mixLuma(original, mask);
	
	gl_FragColor = color;
}
