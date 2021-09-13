/*{
  "CREDIT": "by ",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 1
    }
  ]
}*/
// kernel Convolution Filters алгоритмы

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

void main()	{
	vec4 inputPixelColor;
	
	vec4 color = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
	vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
	vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
	vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

	vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
	vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
	vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
	vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);

	//vec4 avg = intensity * (color + colorLA + colorRA + colorLB + colorRB) / 5.0;
	vec4 avg = intensity * (
	    colorLA * 0.0 + colorA * -1.0 + colorRA * 0.0 + 
	    colorL * -1.0 + color * 4.0 + colorR * -1.0 + 
	    colorLB * 0.0 + colorB * -1.0 + colorRB * 0.0
    ) / 1.0;
	
	if(isf_FragNormCoord.x > .4) {
	    if(isf_FragNormCoord.x > .6) {
	        float g = (avg.r + avg.g + avg.b) / 3.;
    	    inputPixelColor = vec4(g, g, g, 1.);
    	} else {
    	    // inputPixelColor = avg;
    	    float g = 1. + ((avg.r + avg.g + avg.b) / 3.);
    	    inputPixelColor = color * g;
    	}
	} else {
    	inputPixelColor = color;
	}
	
	gl_FragColor = inputPixelColor;
}
