/*
{
  "CATEGORIES" : [
    "Special"
  ],
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "showOriginal",
      "TYPE" : "bool",
      "DEFAULT" : 1,
      "LABEL" : "show original"
    }
  ],
  "CREDIT" : "Imimot"
}
*/

void main()	{
	vec4 srcImage = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 bgImage = vec4(0.0);
	vec2 destloc = isf_FragNormCoord;
	vec2 outloc = isf_FragNormCoord;
	
	if (outloc.x<0.3333) {
		
		float posx_norm = outloc.x/0.33333;
		
		destloc.x = posx_norm; 
		
		if (outloc.y>0.3333 && outloc.y < 0.66666) {
			
			destloc.y = (outloc.y-0.33333)/0.3333;
			
			bgImage = IMG_NORM_PIXEL(inputImage, destloc);

		}
		

	} else if (outloc.x>0.6666) {
		
		float posx_norm = (outloc.x-0.6666)/0.33333;
		
		destloc.x = 1.0-posx_norm; 
		
		if (outloc.y>0.3333 && outloc.y < 0.66666) {
			
			destloc.y = (outloc.y-0.33333)/0.3333;
			
			//	both of these are also the same
			bgImage = IMG_NORM_PIXEL(inputImage, destloc);

		}
		

	}
	
	if (showOriginal) {
	
		gl_FragColor = mix(srcImage, bgImage, bgImage.a);
	
	} else {
		
		gl_FragColor = bgImage;
	}	
	
}
