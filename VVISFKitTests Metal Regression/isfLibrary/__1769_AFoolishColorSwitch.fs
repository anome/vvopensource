/*{
  "CATEGORIES": [
    "XXX"
  ],
  "DESCRIPTION": "Color Filter my own way",
  "ISFVSN": "2",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "darkness",
      "TYPE": "bool",
      "DEFAULT": 1
    },
    {
      "NAME": "POSITION",
      "TYPE": "float",
      "MAX": 1,
      "DEFAULT": 0,
      "MIN": 0
    },
    {
      "NAME": "ColorR",
      "TYPE": "float",
      "MAX": 1,
      "DEFAULT": 0.5,
      "MIN": 0
    },
    {
      "NAME": "ColorG",
      "TYPE": "float",
      "MAX": 1,
      "DEFAULT": 0.5,
      "MIN": 0
    },
    {
      "NAME": "ColorB",
      "TYPE": "float",
      "MAX": 1,
      "DEFAULT": 0.5,
      "MIN": 0
    },
    {
      "NAME": "Lightness",
      "TYPE": "float",
      "MAX": 0.2,
      "DEFAULT": 0,
      "MIN": -0.1
    }
  ],
  "CREDIT": "Silvia Fabiani"
}*/

void main()	{
	vec4		inputPixelColor;
	vec2 		loc = isf_FragNormCoord.xy + POSITION ;
	loc = mod(loc, 1.0);
			inputPixelColor = IMG_NORM_PIXEL(inputImage, loc);
		
		if (darkness) {
		inputPixelColor.rgb = (0.1 - inputPixelColor.rgb + Lightness);
		}	
		else {
		inputPixelColor.rgb =  (inputPixelColor.rgb - sin (Lightness * 0.5));
		}			
	gl_FragColor = inputPixelColor;
	gl_FragColor.b  = (inputPixelColor.b  + ColorB);
	gl_FragColor.g = (inputPixelColor.g   + ColorG);
	gl_FragColor.r  = (inputPixelColor.r  + ColorR);	
	}