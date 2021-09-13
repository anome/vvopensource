/*{
  "DESCRIPTION": "Creates 'panels'",
  "CREDIT": "IMIMOT",
  "CATEGORIES": [
    "Geometry Adjusment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "panels",
      "TYPE": "long",
      "VALUES": [
        2,
        4
      ],
      "LABELS": [
        "2",
        "4"
      ],
      "DEFAULT": 2
    }
  ]
}*/

void main()	{
	
	vec2	normSrcCoord;

	normSrcCoord.x = isf_FragNormCoord[0];
	normSrcCoord.y = isf_FragNormCoord[1];
		
	if (panels == 2) {
	
		if (normSrcCoord.x<0.5) {
			normSrcCoord.x = (normSrcCoord.x+0.25);
		} else {
			normSrcCoord.x = (normSrcCoord.x-0.25);	
		}
	
	} else if (panels == 4) {
	
		if (normSrcCoord.x<0.25) {
			normSrcCoord.x = (normSrcCoord.x+0.375);
		} else if (normSrcCoord.x<0.5)  {
			normSrcCoord.x = (normSrcCoord.x+0.125);	
		} else if (normSrcCoord.x<0.75)  {
			normSrcCoord.x = (normSrcCoord.x-0.125);	
		} else {
			normSrcCoord.x = (normSrcCoord.x-0.375);	
		}
	
	}
	


	gl_FragColor = IMG_NORM_PIXEL(inputImage, normSrcCoord);
}
