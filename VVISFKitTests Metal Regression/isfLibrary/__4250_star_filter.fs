/*{
	"CREDIT": "by VIDVOX",
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
			"NAME": "size",
			"LABEL": "Width",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.01
		}
	]
}*/


const float pi = 3.14159265359;
const float angle = 0.0;

vec4 addLuma(vec4 color) {
	float luma = (color.r + color.g + color.b) / 3.0;
	color.rgb = color.rgb * max(0.0, ( -1.5 + (luma * 2.0)) * pow(2.0, 0.5)); 

	return color;
}

void main() {
	vec2 loc = isf_FragNormCoord * RENDERSIZE;

	vec2 vector1 = vec2(cos(pi * angle), sin(pi * angle));
	vec2 vector2 = vec2(cos(pi * angle + pi / 2.), sin(pi * angle + pi / 2.));
	
	vec4 returnMe;
	vec4 original = IMG_THIS_PIXEL(inputImage);
	float width = size * 0.075;

	vec2 p1 = loc - width * RENDERSIZE * vector1;
	vec2 p2 = loc + width * RENDERSIZE * vector1;
	vec2 p3 = loc - width * RENDERSIZE * vector2;
	vec2 p4 = loc + width * RENDERSIZE * vector2;
	
	//	now we have the two points to smear between,
	//float i;
	float count = 15.0;
	//float count = 10.0;
	vec2 diff1 = p2 - p1;
	vec2 diff2 = p4 - p3;
	for (float i = 0.0; i < 15.0; ++i)	{
		if (i > float(count))
			break;
		float tmp = (i / (count - 1.0));
		
		vec4 color1 = IMG_PIXEL(inputImage, p1 + diff1 * tmp);
		vec4 color2 = IMG_PIXEL(inputImage, p3 + diff2 * tmp);
		returnMe = returnMe + addLuma(color1) / count;
		returnMe = returnMe + addLuma(color2) / count;
		
	}
	gl_FragColor = original + returnMe;
}