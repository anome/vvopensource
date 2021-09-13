/*{
  "DESCRIPTION": "",
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Film"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "uBias",
      "LABEL": "Bias",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 0,
      "DEFAULT": -0.5
    },
    {
      "NAME": "uScale",
      "LABEL": "Scale",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.5
    },
    {
      "NAME": "uGhosts",
      "LABEL": "Ghosts",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 5,
      "DEFAULT": 5
    },
    {
      "NAME": "uGhostDispersal",
      "LABEL": "Ghost Dispersal",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.1,
      "DEFAULT": 0.0125
    },
    {
      "NAME": "uAdditive",
      "LABEL": "Additive Mode",
      "TYPE": "bool",
      "DEFAULT": 1
    },
    {
      "NAME": "uDirection",
      "LABEL": "Direction",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ]
    },
    {
      "NAME": "uLensColor",
      "LABEL": "Lens Color",
      "TYPE": "color",
      "DEFAULT": [
        0.9,
        0.8,
        0.7,
        1
      ]
    }
  ],
  "PASSES": [
    {
      "TARGET": "downsampleAndThresholdImage",
      "WIDTH": "floor($WIDTH/1.0)",
      "HEIGHT": "floor($HEIGHT/1.0)",
      "DESCRIPTION": "Downsample and threshold"
    },
    {}
  ]
}*/




void main()
{

	if (PASSINDEX == 0)	{
		vec2 loc = isf_FragNormCoord;
		gl_FragColor = max(vec4(0.0), IMG_NORM_PIXEL(inputImage,loc) + uBias) * uScale;
	}
	else if (PASSINDEX == 1)	{
		vec2 texcoord = isf_FragNormCoord;
		vec2 texelSize = 1.0 / RENDERSIZE;
		vec2 direction = vec2(1.0) - uDirection;
		vec2 ghostVec = (direction - texcoord) * uGhostDispersal;
		//vec2 direction = vec2(0.5,0.5);
		vec4 result = vec4(0.0);
		for (int i = 0; i < 5; ++i) { 
			if (float(i)>uGhosts)
				break;
			vec2 offset = fract(texcoord + ghostVec * float(i));

			result += IMG_NORM_PIXEL(downsampleAndThresholdImage, offset) * uLensColor;
		}
		//	apply the alpha
		result.rgb = result.rgb * uLensColor.a;
		if (uAdditive)	{
			result = result + IMG_NORM_PIXEL(inputImage, texcoord);
		}
		else	{
			result = result * IMG_NORM_PIXEL(inputImage, texcoord);
		}
		gl_FragColor = result;
	}

}