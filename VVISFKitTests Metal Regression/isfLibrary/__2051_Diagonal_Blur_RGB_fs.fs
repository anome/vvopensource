/*{
	"CREDIT": "by VIDVOX",
	"CATEGORIES": [
		"Blur"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "amounts",
			"LABEL": "Amount",
			"TYPE": "color",
			"DEFAULT": [
				0.1,
				0.2,
				0.3,
				1.0
			]
		},
		{
			"NAME": "angles",
			"LABEL": "Angles",
			"TYPE": "color",
			"DEFAULT": [
				0.25,
				0.5,
				0.75,
				1.0
			]
		},
		{
			"NAME": "quality",
			"LABEL": "Quality",
			"VALUES": [
				32,
				24,
				16,
				8,
				2
			],
			"LABELS": [
				"Very Low",
				"Low",
				"Mid",
				"High",
				"Best"
			],
			"DEFAULT": 24,
			"TYPE": "long"
		}
	]
}*/


const float pi = 3.14159265359;


void main() {
	vec2 loc = vv_FragNormCoord * RENDERSIZE;

	vec2 p1 = vec2(0.0);
	vec2 p2 = vec2(1.0);
	vec2 vectorRed = vec2(cos(pi * angles.r),sin(pi * angles.r));
	vec2 vectorGreen = vec2(cos(pi * angles.g),sin(pi * angles.g));
	vec2 vectorBlue = vec2(cos(pi * angles.b),sin(pi * angles.b));
	
	vec4 returnMe;
	
	if ((amounts.r + amounts.g + amounts.b) > 0.0)	{
		p1 = loc - amounts.r * RENDERSIZE * vectorRed;
		p2 = loc + amounts.r * RENDERSIZE * vectorRed;
		
		//	now we have the two points to smear between,
		float i;
		float count = clamp(amounts.r * max(RENDERSIZE.x,RENDERSIZE.y) / float(quality), 5.0, 50.0);
		vec2 diff = p2 - p1;
		for (i = 0.0; i < count; ++i)	{
			returnMe.r = returnMe.r + IMG_PIXEL(inputImage, p1 + diff * (i / (count - 1.0))).r / count;
		}
		
		p1 = loc - amounts.g * RENDERSIZE * vectorGreen;
		p2 = loc + amounts.g * RENDERSIZE * vectorGreen;
		
		//	now we have the two points to smear between,
		count = clamp(amounts.g * max(RENDERSIZE.x,RENDERSIZE.y) / float(quality), 5.0, 50.0);
		diff = p2 - p1;
		for (i = 0.0; i < count; ++i)	{
			returnMe.g = returnMe.g + IMG_PIXEL(inputImage, p1 + diff * (i / (count - 1.0))).g / count;
		}
		
		
		p1 = loc - amounts.b * RENDERSIZE * vectorBlue;
		p2 = loc + amounts.b * RENDERSIZE * vectorBlue;
		
		//	now we have the two points to smear between,
		diff = p2 - p1;
		count = clamp(amounts.b * max(RENDERSIZE.x,RENDERSIZE.y) / float(quality), 5.0, 50.0);
		for (i = 0.0; i < count; ++i)	{
			returnMe.b = returnMe.b + IMG_PIXEL(inputImage, p1 + diff * (i / (count - 1.0))).b / count;
		}
		
		returnMe.a = IMG_THIS_PIXEL(inputImage).a;	
	}
	else	{
		returnMe = IMG_THIS_PIXEL(inputImage);
	}
	gl_FragColor = returnMe;
}