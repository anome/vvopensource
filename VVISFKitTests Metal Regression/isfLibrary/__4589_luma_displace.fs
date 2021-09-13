
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
			"NAME": "inputImage2",
			"TYPE": "image"
		}
]
	
}*/

float lum(vec3 color) {
    return color.r*.2+color.g*.7+color.b*.1;
}

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	//inputPixelColor = IMG_THIS_PIXEL(inputImage);
	//inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	
	vec2 st = isf_FragNormCoord.xy;
    
    float displace = lum(IMG_NORM_PIXEL(inputImage2,st).rgb);	
	//	both of these are also the same
	st.y += displace/30.;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, st);
	//inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
