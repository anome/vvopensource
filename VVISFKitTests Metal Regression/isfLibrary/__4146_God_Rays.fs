/*
{
  "CATEGORIES" : [
    "Stylize"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "lightDirDOTviewDir",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "lightPositionOnScreen",
      "TYPE" : "point2D"
    },
    {
      "VALUES" : [
        5,
        10,
        25
      ],
      "NAME" : "quality",
      "TYPE" : "long",
      "DEFAULT" : 10,
      "LABELS" : [
        "Low",
        "Mid",
        "High"
      ]
    },
    {
      "NAME" : "Positie",
      "TYPE" : "float",
      "MAX" : 1000,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "by VIDVOX"
}
*/


void main(void)
{
	vec4 origColor = IMG_THIS_PIXEL(inputImage);
	vec4 raysColor = IMG_THIS_PIXEL(inputImage);
	int NUM_SAMPLES = quality;

	if (lightDirDOTviewDir>0.0){
		float exposure	= 0.1/float(NUM_SAMPLES);
		float decay		= 1.0 ;
		float density	= 0.5;
		float weight	= 6.0;
		float illuminationDecay = 1.0;
		vec2		normSrcCoord;

		normSrcCoord.x = isf_FragNormCoord[0];
		normSrcCoord.y = isf_FragNormCoord[1];

		vec2 deltaTextCoord = vec2(normSrcCoord.st - lightPositionOnScreen*Positie/RENDERSIZE);
		vec2 textCoo = normSrcCoord;
		deltaTextCoord *= 1.0 / float(NUM_SAMPLES) * density;

		for(float i=0.0; i < 25.0 ; i++)	{
			if (i > float(NUM_SAMPLES))
				break;
			textCoo -= deltaTextCoord;
			vec4 tsample = IMG_NORM_PIXEL(inputImage, textCoo);
			tsample *= illuminationDecay * weight;
			raysColor += tsample;
			illuminationDecay *= decay;
		}
		raysColor *= exposure * lightDirDOTviewDir;
		float p = 0.3 *raysColor.g + 0.59*raysColor.r + 0.11*raysColor.b;
		gl_FragColor = origColor + p;
	}
	else {
		gl_FragColor = origColor;
	}
}