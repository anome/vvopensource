
/*{
	"DESCRIPTION": "Radial blur",
	"CREDIT": "SoleneMV",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Blur"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		},
		{
			"NAME": "quality",
			"LABEL": "Quality",
			"VALUES": [
				12,
				8,
				4,
				2
			],
			"LABELS": [
				"Low",
				"Mid",
				"High",
				"Best"
			],
			"DEFAULT": 4,
			"TYPE": "long"
		}
	]
}*/

void main()	{
	vec2 loc = isf_FragNormCoord * RENDERSIZE;

	vec2 p1 = vec2(0.0);
	vec2 p2 = vec2(1.0);
	vec2 dir = isf_FragNormCoord - center;
	float angle = atan(dir.y, dir.x);
	vec2 vector = vec2(cos(angle),sin(angle));
	
	vec4 returnMe;
	
	if (width > 0.0)	{
		p1 = loc - width * RENDERSIZE * vector;
		p2 = loc + width * RENDERSIZE * vector;
		
		//	now we have the two points to smear between,
		//float i;
		float count = clamp(width * max(RENDERSIZE.x,RENDERSIZE.y) / float(quality), 5.0, 125.0);
		//float count = 10.0;
		vec2 diff = p2 - p1;
		for (float i = 0.0; i < 125.0; ++i)	{
			if (i > float(count))
				break;
			float tmp = (i / (count - 1.0));
			returnMe = returnMe + IMG_PIXEL(inputImage, p1 + diff * tmp) / count;
		}
	}
	else	{
		returnMe = IMG_THIS_PIXEL(inputImage);
	}
	gl_FragColor = returnMe;
}
