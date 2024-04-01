/*{
  "CREDIT": "by Anomes",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "color",
      "TYPE": "color",
      "DEFAULT": [0.0,0.0,0.0,1.0]
    },
    {
      "NAME": "tolerance",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.0,
      "MAX": 1.0
    },
    {
      "NAME": "feather",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0.0,
      "MAX": 100.0,
    }
  ]
}*/


	

void main()
{
	vec4 inputColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	float distance = length(inputColor.rgb-color.rgb);
	if( distance < tolerance*sqrt(3.) )
	{
        		if( 0. < tolerance && 0. < feather )
        		{
           		inputColor.a *= pow(distance/tolerance, 1./feather);
        		}
        		else
        		{
            		inputColor.a = 0.;
        		}
	}
	gl_FragColor = inputColor;
}




