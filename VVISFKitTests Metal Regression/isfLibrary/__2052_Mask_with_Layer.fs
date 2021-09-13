/*{
	"DESCRIPTION": "Masking FX",
	"CREDIT": "by IMIMOT ",
	"CATEGORIES": [
		"Masking"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "maskImage",
			"TYPE": "image"
		}		
	]
}*/

void main()
{
    vec4 base = IMG_THIS_PIXEL(inputImage);
    vec4 mask = IMG_THIS_PIXEL(maskImage);
     
    gl_FragColor = base*mask;  
}
