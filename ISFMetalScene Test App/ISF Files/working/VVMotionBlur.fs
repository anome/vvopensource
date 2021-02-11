/*{
  "DESCRIPTION": "this is basically identical to the demonstration of a persistent buffer",
  "CREDIT": "by zoidberg",
  "CATEGORIES": [
    "Blur"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "blurAmount",
      "TYPE": "float",
      "DEFAULT": 0.75
    }
  ],
  "PASSES": [
    {
      "TARGET": "bufferVariableNameA",
      "PERSISTENT": true
    },
    {
    }
  ]
}*/

void main()
{
	vec4		freshPixel = IMG_THIS_PIXEL(inputImage);
	vec4		stalePixel = IMG_THIS_PIXEL(bufferVariableNameA);
	gl_FragColor = mix(freshPixel,stalePixel,blurAmount);
}
