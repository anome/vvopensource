/*{
  "CREDIT": "by zoidberg",
  "CATEGORIES": [
    "Color Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
     {
			"NAME" :	"VaryR",
			"TYPE" :	"float",
			"DEFAULT" :	1.0,
			"MIN" :	0.1,
			"MAX" :	1.0
	},
	{
			"NAME" :	"VaryG",
			"TYPE" :	"float",
			"DEFAULT" :	0.5,
			"MIN" :	0.1,
			"MAX" :	1.0
	},
	{
			"NAME" :	"VaryB",
			"TYPE" :	"float",
				"DEFAULT" :	0.1,
			"MIN" :	0.1,
			"MAX" :	1.0
	},
	 {
			"NAME" :	"Pulsation",
			"TYPE" :	"float",
			"DEFAULT" :	1.0,
			"MIN" :	1.0,
			"MAX" :	32.0
	}
  ]
}*/

void main() {
	vec4		srcPixel = IMG_THIS_PIXEL(inputImage);
	float Pulse = (cos (TIME*Pulsation));
	gl_FragColor = vec4 (((VaryR*Pulse)-srcPixel.r), ((VaryG-srcPixel.g)*Pulse), ((VaryB-srcPixel.b)*Pulse), srcPixel.a);
}