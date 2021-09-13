/*{
	"CREDIT": "by Carter Rosenberg, inspired by Dave Jones",
	"CATEGORIES": [
		"Color Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "additiveMode",
			"LABEL": "Additive Mode",
			"TYPE": "bool",
			"DEFAULT": 0.0
		},
		{
			"NAME": "image1Min",
			"LABEL": "image1 Min",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.8
		},
		{
			"NAME": "image1Gain",
			"LABEL": "image1 Gain",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},	
		{
			"NAME": "image2Min",
			"LABEL": "image2 Min",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.6
		},
		{
			"NAME": "image2Gain",
			"LABEL": "image2 Gain",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "image3Min",
			"LABEL": "image3 Min",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.4
		},
		{
			"NAME": "image3Gain",
			"LABEL": "image3 Gain",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "image4Min",
			"LABEL": "image4 Min",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.2
		},
		{
			"NAME": "image4Gain",
			"LABEL": "image4 Gain",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [
				0.75,
				0.1,
				0.25,
				1.0
			]
		},
		{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [
				0.125,
				0.75,
				0.44,
				1.0
			]
		},
		{
			"NAME": "color3",
			"TYPE": "color",
			"DEFAULT": [
				0.78,
				0.25,
				0.75,
				1.0
			]
		},
		{
			"NAME": "color4",
			"TYPE": "color",
			"DEFAULT": [
				0.333,
				0.0,
				0.75,
				1.0
			]
		}
	]
}*/


//	Inspired by.. http://www.experimentaltvcenter.org/jones-colorizer-history-design
//	The original idea was 4 or 6 channels of different inputs.. this uses 1 re-used 4 times as an FX
//	each channel has brightness adjustment and a clip min
//	and is then colorized based based on an input RGBA color
//	


float gray(vec4 n)
{
	return (n.r + n.g + n.b)/3.0;
}


void main() {
	vec4 out_color = vec4(0.0);
	vec2 coord = vv_FragNormCoord;
	
	//	do these in reverse order.. image1 is in front of image2, etc..
	
	//	starting with image4, draw it 
	vec4 pixel = IMG_NORM_PIXEL(inputImage,coord);
	
	float brightness = gray(pixel) + image4Gain;
	if (brightness > image4Min)	{
		if (additiveMode)	{
			out_color += mix(out_color, brightness * color4, color4.a);
		}
		else	{
			out_color = mix(out_color, brightness * color4, color4.a);
		}
	}
	//	now use an overlay blending to put image3 on top if that
	pixel = IMG_NORM_PIXEL(inputImage,coord);
	brightness = gray(pixel) + image3Gain;
	if (brightness > image3Min)	{
		if (additiveMode)	{
			out_color += mix(out_color, brightness * color3, color3.a);
		}
		else	{
			out_color = mix(out_color, brightness * color3, color3.a);
		}
	}
	//	now use an overlay blending to put image2 on top if that
	pixel = IMG_NORM_PIXEL(inputImage,coord);
	brightness = gray(pixel) + image2Gain;
	if (brightness > image2Min)	{
		if (additiveMode)	{
			out_color += mix(out_color, brightness * color2, color2.a);
		}
		else	{
			out_color = mix(out_color, brightness * color2, color2.a);
		}
	}
	//	now use an overlay blending to put image1 on top if that
	pixel = IMG_NORM_PIXEL(inputImage,coord);
	brightness = gray(pixel) + image1Gain;
	if (brightness > image1Min)	{
		if (additiveMode)	{
			out_color += mix(out_color, brightness * color1, color1.a);
		}
		else	{
			out_color = mix(out_color, brightness * color1, color1.a);
		}
	}
	
	gl_FragColor = out_color;
}