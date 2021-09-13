/*
{
  "CATEGORIES" : [
    "Masking"
  ],
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "VALUES" : [
        0,
        1,
        2,
        3
      ],
      "NAME" : "maskType",
      "TYPE" : "long",
      "DEFAULT" : 1,
      "LABEL" : "Horizontal Repeat",
      "LABELS" : [
        "0",
        "1",
        "2",
        "3"
      ]
    }
  ],
  "CREDIT" : "by VIDVOX"
}
*/



const float pi = 3.14159265359;


bool insideMask(int maskType, vec2 textureCoord, vec2 textureSize) {
	
    if(maskType == 0) {
        return true;
    }
    
    if(maskType == 1 || maskType == 2) {
    	
	    vec2 position = ((textureCoord - vec2(0.5)) * textureSize) / min(textureSize.x, textureSize.y);
	    bool insideCircle = (0.5 - distance(position, vec2(0.0))) > 0.01;
	    return maskType == 1 ? insideCircle : !insideCircle;
    }
    
    return true;
    
}

void main() {
	vec4 srcPixel = IMG_THIS_PIXEL(inputImage);
	
	gl_FragColor = insideMask(int(maskType), isf_FragNormCoord, RENDERSIZE) ? vec4(0.0) : srcPixel;
}

