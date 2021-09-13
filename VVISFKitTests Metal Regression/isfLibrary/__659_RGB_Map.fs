/*{
	"CREDIT": "by INKA",
	"CATEGORIES": [
		"Color Effect",
		"INKA"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "treshold",
			"TYPE": "float",
			"MAX": 0.5,
			"MIN": 0.0,
			"DEFAULT": 0.2
		},
		{
			"LABEL": "Color Black",
			"NAME": "color_black",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"LABEL": "Color White",
			"NAME": "color_white",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.25,
				0.0,
				1.0
			]
		},
		{
			"LABEL": "Color Red",
			"NAME": "color_red",
			"TYPE": "color",
			"DEFAULT": [
				0.54,
				0.07,
				0.0,
				1.0
			]
		},
		{
			"LABEL": "Color Green",
			"NAME": "color_green",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.8,
				0.0,
				1.0
			]
		},
		{
			"LABEL": "Color Blue",
			"NAME": "color_blue",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.5,
				1.0
			]
		}
	]
}*/


void main ()	{
	vec4 pixcol = IMG_THIS_PIXEL(inputImage);
	
	vec4 avg = pixcol;
	float lum = (avg.r+avg.g+avg.b)/3.0;
	//float lum = dot(vec3(0.30, 0.59, 0.11), avg.rgb);
	//lum = pow(lum,1.4);
	
	vec4 startColor;
	
	if (lum < treshold) {
		startColor = color_black;
	} else {
	
		// purple to blue
		if (lum > 1. - treshold)	{
			startColor = color_white;
		}
		//red
		else if (avg.r > avg.b && avg.r > avg.g) {
			startColor = color_red;
		}
		//	green
		else if (avg.g > avg.b)	{
			startColor = color_green;
		}
		// blue
		else {
			startColor = color_blue;
		}
		
	}

	vec4 thermal = startColor;
	gl_FragColor = thermal;

}