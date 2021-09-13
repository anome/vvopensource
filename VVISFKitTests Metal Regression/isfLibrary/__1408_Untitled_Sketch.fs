/*{
  "CREDIT": "by artabasestudio",
  "DESCRIPTION": "FREEZE FLIP V",
  "ISFVSN": "2.0",
  "CATEGORIES": [
    "filter"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "freeze",
      "TYPE": "bool",
      "DEFAULT": 0
    }
  ],
  "PASSES": [
    {
      "TARGET": "freezeBuffer",
      "persistent": true
    }
  ]
}*/
void main() {
	vec2		normSrcCoord;

	normSrcCoord.x = isf_FragNormCoord[0];
	normSrcCoord.y = isf_FragNormCoord[1];

	normSrcCoord.y = (1.0-normSrcCoord.y);

	gl_FragColor = IMG_NORM_PIXEL(inputImage, normSrcCoord);
},
{

	if (freeze)	{
		gl_FragColor = IMG_THIS_PIXEL(freezeBuffer);
	}
	else	{
		gl_FragColor = IMG_THIS_PIXEL(inputImage);
	}

}
