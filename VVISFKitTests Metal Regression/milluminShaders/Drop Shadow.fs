/*{
	"CREDIT": "original implementation as v002.blur in QC by anton marini and tom butterworth, ported by zoidberg, modified by anomes",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "shadowColor",
			"LABEL" : "shadow color",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "offset",
			"TYPE": "point2D",
			"DEFAULT": [
				12.0,
				-12.0
			]
		}, 
		{
			"NAME": "blurAmount",
			"LABEL" : "blur amount",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 24.0,
			"DEFAULT": 7.0
		}, 
		{
			"NAME": "opacity",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		}, 
		{
			"NAME": "premultiplied",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET": "halfSizeBaseRender",
			"WIDTH": "floor($WIDTH/3.0)",
			"HEIGHT": "floor($HEIGHT/3.0)",
			"DESCRIPTION": 0
		},
		{
			"TARGET": "quarterSizeBaseRender",
			"WIDTH": "floor($WIDTH/6.0)",
			"HEIGHT": "floor($HEIGHT/6.0)",
			"DESCRIPTION": 1
		},
		{
			"TARGET": "eighthSizeBaseRender",
			"WIDTH": "floor($WIDTH/12.0)",
			"HEIGHT": "floor($HEIGHT/12.0)",
			"DESCRIPTION": 2
		},
		{
			"TARGET": "eighthGaussA",
			"WIDTH": "floor($WIDTH/12.0)",
			"HEIGHT": "floor($HEIGHT/12.0)",
			"DESCRIPTION": 3
		},
		{
			"TARGET": "eighthGaussB",
			"WIDTH": "floor($WIDTH/12.0)",
			"HEIGHT": "floor($HEIGHT/12.0)",
			"DESCRIPTION": 4
		},
		{
			"TARGET": "quarterGaussA",
			"WIDTH": "floor($WIDTH/6.0)",
			"HEIGHT": "floor($HEIGHT/6.0)",
			"DESCRIPTION": 5
		},
		{
			"TARGET": "quarterGaussB",
			"WIDTH": "floor($WIDTH/6.0)",
			"HEIGHT": "floor($HEIGHT/6.0)",
			"DESCRIPTION": 6
		},
		{
			"TARGET": "halfGaussA",
			"WIDTH": "floor($WIDTH/3.0)",
			"HEIGHT": "floor($HEIGHT/3.0)",
			"DESCRIPTION": 7
		},
		{
			"TARGET": "halfGaussB",
			"WIDTH": "floor($WIDTH/3.0)",
			"HEIGHT": "floor($HEIGHT/3.0)",
			"DESCRIPTION": 8
		},
		{
			"TARGET": "fullGaussA",
			"DESCRIPTION": 9
		},
		{
			"TARGET": "fullGaussB",
			"DESCRIPTION": 10
		}
	]
}*/



varying vec2 texOffsets[5];


void main() {
	int blurLevel = int(floor(blurAmount/6.0));
	float blurLevelModulus = mod(blurAmount, 6.0);
	//	first three passes are just copying the input image into the buffer at varying sizes
	if (PASSINDEX==0)	{
		gl_FragColor = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord);
	}
	else if (PASSINDEX==1)	{
		gl_FragColor = IMG_NORM_PIXEL(halfSizeBaseRender, vv_FragNormCoord);
	}
	else if (PASSINDEX==2)	{
		gl_FragColor = IMG_NORM_PIXEL(quarterSizeBaseRender, vv_FragNormCoord);
	}
	//	start reading from the previous stage- each two passes completes a gaussian blur, then we increase the resolution & blur again...
	else if (PASSINDEX == 3)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[2]);
		vec4		sample3 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[3]);
		vec4		sample4 = IMG_NORM_PIXEL(eighthSizeBaseRender,texOffsets[4]);
		//gl_FragColor = (sample0 + sample1 + sample2) / 3.0;
		gl_FragColor = (sample0 + sample1 + sample2 + sample3 + sample4) / 5.0;
	}
	else if (PASSINDEX == 4)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[2]);
		vec4		sample3 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[3]);
		vec4		sample4 = IMG_NORM_PIXEL(eighthGaussA,texOffsets[4]);
		//gl_FragColor = (sample0 + sample1 + sample2) / 3.0;
		gl_FragColor = (sample0 + sample1 + sample2 + sample3 + sample4) / 5.0;
	}
	else if (PASSINDEX == 5)	{
		vec4		sample0 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(eighthGaussB,texOffsets[2]);
		gl_FragColor =  (sample0 + sample1 + sample2) / 3.0;
	}
	else if (PASSINDEX == 6)	{
		vec4		sample0 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(quarterGaussA,texOffsets[2]);
		gl_FragColor =  (sample0 + sample1 + sample2) / 3.0;
	}
	else if (PASSINDEX == 7)	{
		vec4		sample0 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(quarterGaussB,texOffsets[2]);
		gl_FragColor =  (sample0 + sample1 + sample2) / 3.0;
	}
	else if (PASSINDEX == 8)	{
		vec4		sample0 = IMG_NORM_PIXEL(halfGaussA,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(halfGaussA,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(halfGaussA,texOffsets[2]);
		gl_FragColor =  (sample0 + sample1 + sample2) / 3.0;
	}
	else if (PASSINDEX == 9)	{
		vec4		sample0 = IMG_NORM_PIXEL(halfGaussB,texOffsets[0]);
		vec4		sample1 = IMG_NORM_PIXEL(halfGaussB,texOffsets[1]);
		vec4		sample2 = IMG_NORM_PIXEL(halfGaussB,texOffsets[2]);
		gl_FragColor =  (sample0 + sample1 + sample2) / 3.0;
	}
	else if (PASSINDEX == 10) {
		//	this is the last pass- calculate the blurred image as i have in previous passes, then mix it in with the full-size input image using the blur amount so i get a smooth transition into the blur at low blur levels
		vec4		sample0 = IMG_NORM_PIXEL(fullGaussA,texOffsets[0]-offset/RENDERSIZE);
		vec4		sample1 = IMG_NORM_PIXEL(fullGaussA,texOffsets[1]-offset/RENDERSIZE);
		vec4		sample2 = IMG_NORM_PIXEL(fullGaussA,texOffsets[2]-offset/RENDERSIZE);
		vec4		blurredImg =  (sample0 + sample1 + sample2) / 3.0;
		blurredImg *= opacity;
		vec4 finalColor;
		if (blurLevel == 0)
			finalColor = mix(IMG_NORM_PIXEL(inputImage,vv_FragNormCoord-offset/RENDERSIZE)*opacity, blurredImg, (blurLevelModulus/6.0));
		else
			finalColor = blurredImg;
		finalColor = vec4(shadowColor.rgb, finalColor.a*shadowColor.a);
		if( premultiplied )
			finalColor.rgb *= finalColor.a;
		vec4 originalColor = IMG_NORM_PIXEL(inputImage,vv_FragNormCoord);
		gl_FragColor = mix(originalColor, finalColor, 1.0-originalColor.a);
	}
}
