/*{
	"CREDIT": "by Dave Malcolm",
	"DESCRIPTION": "Sea, Land and Snow generator based on red values from input image",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
				{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		    {
      "NAME" : "Beachdepth",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.2,
      "MIN" : 0
    },
		    {
      "NAME" : "sealevel",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
		    {
      "NAME" : "snowline",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.7,
      "MIN" : 0
    },
		    {
      "NAME" : "snowfade",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.1,
      "MIN" : 0
    }
	],
  "IMPORTED": {
    "Land": {
      "PATH": "land.jpg"
    },
     "Water": {
      "PATH": "water.jpg"
    },
     "Snow": {
      "PATH": "snow.jpg"
    }
	
	}
}*/

void main() {
	

	float depth = ((IMG_THIS_NORM_PIXEL(inputImage).r - sealevel + Beachdepth) / Beachdepth);
	float snowdepth = ((IMG_THIS_NORM_PIXEL(inputImage).r - snowline + snowfade) / snowfade);

if (IMG_THIS_NORM_PIXEL(inputImage).r > snowline) {
	gl_FragColor= IMG_THIS_NORM_PIXEL(Snow);
}
else if ((IMG_THIS_NORM_PIXEL(inputImage).r < snowline)&&(IMG_THIS_NORM_PIXEL(inputImage).r > (snowline - snowfade))) {
	gl_FragColor= (IMG_THIS_NORM_PIXEL(Snow) * snowdepth) + (IMG_THIS_NORM_PIXEL(Land) * (1.0 - snowdepth));
}
else if ((IMG_THIS_NORM_PIXEL(inputImage).r < (snowline - snowfade))&&(IMG_THIS_NORM_PIXEL(inputImage).r > sealevel)) {
	gl_FragColor= IMG_THIS_NORM_PIXEL(Land);
}
else if ((IMG_THIS_NORM_PIXEL(inputImage).r < sealevel)&&(IMG_THIS_NORM_PIXEL(inputImage).r > (sealevel - Beachdepth))) {
	gl_FragColor= (IMG_THIS_NORM_PIXEL(Land) * depth) + (IMG_THIS_NORM_PIXEL(Water) * (1.0 - depth));
}
else {
	gl_FragColor= IMG_THIS_NORM_PIXEL(Water);
	
}
}