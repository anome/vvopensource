/*{
  "CREDIT": "by Carter Rosenberg",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "NAME": "startImage",
      "TYPE": "image"
    },
    {
      "NAME": "endImage",
      "TYPE": "image"
    },
    {
      "NAME": "width",
      "TYPE": "float",
      "DEFAULT": 0.25
    },
    {
      "NAME": "offset",
      "TYPE": "float",
      "DEFAULT": 0
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "MAX" : 2,
      "DEFAULT" : 0,
      "LABEL" : "Progress",
      "MIN" : 0
    },
    {
      "NAME": "vertical",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "color1",
      "TYPE": "color",
      "DEFAULT": [
        1,
        1,
        1,
        1
      ]
    },
    {
      "NAME": "color2",
      "TYPE": "color",
      "DEFAULT": [
        0,
        0,
        0,
        1
      ]
    }
  ]
}*/



void main() {
	//	determine if we are on an even or odd line
	//	math goes like..
	//	mod(((coord+offset) / width),2)
	
	
	vec4 out_color = IMG_NORM_PIXEL(startImage, isf_FragNormCoord);
	float coord = isf_FragNormCoord[0];

	if (vertical)	{
		coord = isf_FragNormCoord[1];
	}
	if(mod(((coord+offset+TIME*speed) / width),2.0) < 1.0)	{
		out_color =  IMG_NORM_PIXEL(endImage, isf_FragNormCoord);
	}
	
	gl_FragColor = out_color;
}