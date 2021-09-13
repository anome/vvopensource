/*
{
  "CATEGORIES" : [
    "Special"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "panel1",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.0,
      "MIN" : 0.0
    },
    {
      "NAME" : "panel2",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.2,
      "MIN" : 0.0
    },
    {
      "NAME" : "panel3",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.4,
      "MIN" : 0.0
    },
    {
      "NAME" : "panel4",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.6,
      "MIN" : 0.0
    },
    {
      "NAME" : "panel5",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.8,
      "MIN" : 0.0
    }
  ],
  "VSN" : "2",
  "CREDIT" : "Imimot"
}
*/

void main()	{
	vec2 current; 
	
	current.x = isf_FragNormCoord.x;
	current.y = isf_FragNormCoord.y;
	
	float newy = current.y+panel5;
	
	if (current.x < 0.2) {
		
		newy = current.y+panel1;
		
	} else if (current.x < 0.4) {
		
		newy = current.y+panel2;
		
	} else if (current.x < 0.6) {
		
		newy = current.y+panel3;
		
	} else if (current.x < 0.8) {
		
		newy = current.y+panel4;
		
	}
	
	if (newy>1.0) {	
		newy -= 1.0;
	}
	
	current.y = newy;

		
	gl_FragColor = IMG_NORM_PIXEL(inputImage, current);
}
