/*{
	"DESCRIPTION": "Sets the alpha channel of the image",
	"CREDIT": "VIDVOX",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Color Adjustment"
	],
	"INPUTS": [
		{
			"NAME": "inputImage1",
			"TYPE": "image"
		},
		{
			"NAME": "inputImage2",
			"TYPE": "image"
		},	
		{
			"NAME": "inputImage3",
			"TYPE": "image"
		},	
		{
			"NAME": "inputImage4",
			"TYPE": "image"
		},	
		{
			"NAME": "inputImage5",
			"TYPE": "image"
		},			
		{
			"NAME": "newAlpha1",
			"LABEL": "New Alpha",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "newAlpha2",
			"LABEL": "New Alpha",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "newAlpha3",
			"LABEL": "New Alpha",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "newAlpha4",
			"LABEL": "New Alpha",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "newAlpha5",
			"LABEL": "New Alpha",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}		
		
	]

}*/

float overlay( float s, float d )
{
	return (d < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d);
}

vec4 overlay( vec4 s, vec4 d )
{
	vec4 c;
	c.x = overlay(s.x,d.x);
	c.y = overlay(s.y,d.y);
	c.z = overlay(s.z,d.z);
	c.a = overlay(s.a,d.a);
	return c;
}

void main()	{

	vec4	inputPixelColor1= IMG_THIS_NORM_PIXEL(inputImage1);
	inputPixelColor1.a = newAlpha1;
	
	vec4	inputPixelColor2= IMG_THIS_NORM_PIXEL(inputImage2);
	inputPixelColor2.a = newAlpha2;

	vec4	inputPixelColor3= IMG_THIS_NORM_PIXEL(inputImage3);
	inputPixelColor3.a = newAlpha3;	
	
	//vec4	stage1 = mix(inputPixelColor1, inputPixelColor2, inputPixelColor2.a);
	//vec4	stage2 = mix(stage1, inputPixelColor3, inputPixelColor3.a);
	//vec4	mult = inputPixelColor1*inputPixelColor2;
	vec4	ol1 = overlay(inputPixelColor1, inputPixelColor2);
	vec4	ol2 = overlay(inputPixelColor3, ol1);
	gl_FragColor = ol2;

}
