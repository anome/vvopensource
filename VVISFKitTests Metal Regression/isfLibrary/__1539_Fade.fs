/*{
  "DESCRIPTION": "RGB Average",
  "CREDIT": "by dantheman",
  "CATEGORIES": [
    "stylized Effect"
  ],
  "INPUTS": [
    {
      "NAME": "image0",
      "TYPE": "image"
    },
    {
      "NAME": "image1",
      "TYPE": "image"
    },
    {
      "NAME": "count",
      "TYPE": "float",
      "MIN": 1,
      "MAX": 10,
      "DEFAULT": 1
    }
  ]
}*/

void main(void) {
	vec2 uv = isf_FragNormCoord.xy;
	vec3	final = vec3(0.0);
	vec3	srcPixel0 = IMG_NORM_PIXEL(image0,uv).xyz;
	final =srcPixel0;
	vec3	srcPixel1 = IMG_NORM_PIXEL(image1,uv).xyz;
	final+=srcPixel1/count;
gl_FragColor = vec4((final), 1.0);

}
