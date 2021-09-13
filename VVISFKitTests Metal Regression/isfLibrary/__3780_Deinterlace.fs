/*{
  "CREDIT": "by zoidberg",
  "CATEGORIES": [
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ]
}*/

const vec2		pointOffset = vec2(0.0, 1.0);

void main() {
	vec4		outColor;
	if (fract((gl_FragCoord.y+0.5)/2.0) == 0.0)	{
		outColor = (IMG_PIXEL(inputImage, gl_FragCoord.xy) + IMG_PIXEL(inputImage, gl_FragCoord.xy - pointOffset))/2.0;
		gl_FragColor = outColor;
	}
	else	{
		outColor = (IMG_PIXEL(inputImage, gl_FragCoord.xy) + IMG_PIXEL(inputImage, gl_FragCoord.xy + pointOffset))/2.0;
		gl_FragColor = outColor;
	}
}