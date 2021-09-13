/*{
	"DESCRIPTION": "Chroma Key + Edge Blur",
	"CREDIT": "ported from http://www.memo.tv, IMIMOT and ZOIDBERG)",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "thresholdSensitivity",
			"LABEL" : "threshold",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.00,
			"MAX": 1.0
		},
		{
			"NAME": "smoothing",
			"LABEL" : "smoothing",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.00,
			"MAX": 1.0
		},
		{
			"NAME": "colorToReplace",
			"LABEL" : "color",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.61,
				0.0,
				1.0
			]
		},
		{
			"NAME": "blurAmount",
			"LABEL" : "edge blur",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 12.0,
			"DEFAULT": 2.0
		},
		{
			"NAME": "erode",
			"LABEL" : "erode",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.4
		}
		
	],
	"PASSES": [
		{
			"TARGET": "chroma"
		},
		{
			"TARGET": "edges"
		},
		{
			"TARGET": "halfSizeBaseRender",
			"WIDTH": "floor($WIDTH/2.0)",
			"HEIGHT": "floor($HEIGHT/2.0)"
		},
		{
			"TARGET": "quarterSizeBaseRender",
			"WIDTH": "floor($WIDTH/4.0)",
			"HEIGHT": "floor($HEIGHT/4.0)"
		},
		{
			"TARGET": "eighthSizeBaseRender",
			"WIDTH": "floor($WIDTH/8.0)",
			"HEIGHT": "floor($HEIGHT/8.0)"
		},
		{
			"TARGET": "quarterGaussA",
			"WIDTH": "floor($WIDTH/4.0)",
			"HEIGHT": "floor($HEIGHT/4.0)"
		},
		{
			"TARGET": "quarterGaussB",
			"WIDTH": "floor($WIDTH/4.0)",
			"HEIGHT": "floor($HEIGHT/4.0)"
		},
		{
			"TARGET": "fullGaussA"
		},
		{
			"TARGET": "fullGaussB"
		},
		{
			"TARGET": "compositing"
		}
	]
}*/



varying vec2 texOffsets[5];



void main()
{
	int blurLevel = int(floor(blurAmount/6.0));
	float blurLevelModulus = mod(blurAmount, 6.0);
	if( PASSINDEX == 0 )
	{
		vec4 textureColor = IMG_THIS_NORM_PIXEL(inputImage);

		float maskY = 0.2989 * colorToReplace.r + 0.5866 * colorToReplace.g + 0.1145 * colorToReplace.b;
		float maskCr = 0.7132 * (colorToReplace.r - maskY);
		float maskCb = 0.5647 * (colorToReplace.b - maskY);

		float Y = 0.2989 * textureColor.r + 0.5866 * textureColor.g + 0.1145 * textureColor.b;
		float Cr = 0.7132 * (textureColor.r - Y);
		float Cb = 0.5647 * (textureColor.b - Y);

		float blendValue = smoothstep(thresholdSensitivity, thresholdSensitivity + smoothing, distance(vec2(Cr, Cb), vec2(maskCr, maskCb)));
		vec4 blendImage = vec4(textureColor.rgb, textureColor.a * blendValue);

		gl_FragColor = mix(vec4(0.0),blendImage,blendImage.a);
	}
	else if( PASSINDEX == 1 )
	{
		vec4 textureColor = IMG_THIS_NORM_PIXEL(chroma);
		gl_FragColor = vec4(textureColor.a, textureColor.a, textureColor.a, 1.);
	}
	else if (PASSINDEX==2)
	{
		gl_FragColor = IMG_NORM_PIXEL(edges, vv_FragNormCoord);
	}
	else if (PASSINDEX==3)
	{
		gl_FragColor = IMG_NORM_PIXEL(halfSizeBaseRender, vv_FragNormCoord);
	}
	else if (PASSINDEX==4)
	{
		gl_FragColor = IMG_NORM_PIXEL(quarterSizeBaseRender, vv_FragNormCoord);
	}
	else if (PASSINDEX == 5)
	{
		vec4 sample0 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[0]);
		vec4 sample1 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[1]);
		vec4 sample2 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[2]);
		vec4 sample3 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[3]);
		vec4 sample4 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[4]);
		gl_FragColor = vec4((sample0 + sample1 + sample2 + sample3 + sample4).rgb / (5.0), 1.0);
	}
	else if (PASSINDEX == 6)
	{
		vec4 sample0 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[0]);
		vec4 sample1 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[1]);
		vec4 sample2 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[2]);
		vec4 sample3 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[3]);
		vec4 sample4 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[4]);
		gl_FragColor = vec4((sample0 + sample1 + sample2 + sample3 + sample4).rgb / (5.0), 1.0);
	}
	else if (PASSINDEX == 7)
	{
		vec4 sample0 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[0]);
		vec4 sample1 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[1]);
		vec4 sample2 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[2]);
		gl_FragColor =  vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0);
	}
	else if (PASSINDEX == 8)
	{
		vec4 sample0 = IMG_NORM_PIXEL(fullGaussA,texOffsets[0]);
		vec4 sample1 = IMG_NORM_PIXEL(fullGaussA,texOffsets[1]);
		vec4 sample2 = IMG_NORM_PIXEL(fullGaussA,texOffsets[2]);
		vec4 blurredImg =  vec4((sample0 + sample1 + sample2).rgb / (3.0), 1.0);
		if (blurLevel == 0)
		{
			gl_FragColor = mix(IMG_NORM_PIXEL(edges,vv_FragNormCoord), blurredImg, (blurLevelModulus/6.1));
		}
		else
		{
			gl_FragColor = blurredImg;
		}
	}
	else if( PASSINDEX == 9 )
	{
		vec4 textureColor = IMG_THIS_NORM_PIXEL(inputImage);
		float alpha = IMG_THIS_NORM_PIXEL(fullGaussB).r;
		if( alpha < 1. )
		{
			float gray = dot(textureColor.rgb, vec3(0.299, 0.587, 0.114));
			textureColor.rgb = mix(textureColor.rgb, vec3(gray), 1.-alpha);
		}
		float shadow = 0.;
		float highlight = 1.;
		float gamma = max(   (1.-erode)/2.  ,  0.001);
        		alpha = pow(    min( max(alpha-shadow, 0.) / (highlight-shadow) , 1. )    ,    1./gamma    );
		textureColor.a = alpha;
		gl_FragColor = textureColor;
	}
}
