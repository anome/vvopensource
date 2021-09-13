
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		},
		{
			"NAME": "sigma",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 0.1,
			"MAX": 20.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

float normpdf(in float x, in float sigma)
{
	return 0.39894*exp(-0.5*x*x/(sigma*sigma))/sigma;
}

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
// 	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
// 	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	
	vec3 c = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy).rgb;

	//declare stuff
	const int mSize = 11;
	const int kSize = (mSize-1)/2;
	float kernel[mSize];
	vec3 final_colour = vec3(0.0);
	
	//create the 1-D kernel
// 	float sigma = 7.0;
	float Z = 0.0;
	for (int j = 0; j <= kSize; ++j)
	{
		kernel[kSize+j] = kernel[kSize-j] = normpdf(float(j), sigma);
	}
	
	//get the normalization factor (as the gaussian has been clamped)
	for (int j = 0; j < mSize; ++j)
	{
		Z += kernel[j];
	}
	
	//read out the texels
	for (int i=-kSize; i <= kSize; ++i)
	{
		for (int j=-kSize; j <= kSize; ++j)
		{
		    float x = float(i) / RENDERSIZE.x;
		    float y = float(j) / RENDERSIZE.y;
			final_colour += kernel[kSize+j]*kernel[kSize+i]*IMG_NORM_PIXEL(inputImage, (isf_FragNormCoord.xy+vec2(x,y))).rgb;

		}
	}
	
	
	gl_FragColor = vec4(final_colour/(Z*Z), 1.0);
	
}
