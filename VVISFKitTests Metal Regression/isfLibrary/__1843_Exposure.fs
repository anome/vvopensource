/*{
  "CREDIT": "by carter rosenberg",
  "CATEGORIES": [
    "Color Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "exposure",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/

void main() {
	vec4 color = IMG_THIS_PIXEL(inputImage);
	color.rgb = color.rgb * pow(2.0, exposure);
	gl_FragColor = color;
}
