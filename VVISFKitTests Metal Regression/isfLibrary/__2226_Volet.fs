/*{
	"CREDIT": "by isadoratelesdecastro",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}, 
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
		"NAME" : 		"division",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	}
	]
}*/


vec4 myStep( vec4 img, float pix, float lala )
{
    if( pix<lala )
    {
    	return colorA;
    }
    else 
    {
    	return img;
    }
}

void main() 
{
	vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
	vec4 srcPixel = IMG_THIS_PIXEL(inputImage);
	vec4 colorImage = vec4(srcPixel.rgb, 1.0);
    	vec4 y = myStep(colorImage, uv.x, division);
    	vec4 resultColor = vec4(0.0);
    	resultColor = y;
    
    	gl_FragColor = resultColor;
}







