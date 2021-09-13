/*{
  "CREDIT": "Modulo-Pi",
  "ISFVSN": "2.0",
  "CATEGORIES" : [
    "invert"
  ],
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "mirrorX",
      "TYPE" : "float",
      "MIN" : 0,
      "MAX" : 1,
      "DEFAULT" : 0.5
    },
    {
      "NAME" : "mirrorY",
      "TYPE" : "float",
      "MIN" : 0,
      "MAX" : 1,
      "DEFAULT" : 0.5
    },
    {
      "NAME" : "inOut",
      "TYPE" : "float",
      "MIN" : 0,
      "MAX" : 0.2,
      "DEFAULT" : 0.0
    }
  ],
  "DESCRIPTION" : "Mirror"
}*/


void main()  {
    float mX = mirrorX;
    float mY = mirrorY;
    vec2 flipLoc = isf_FragNormCoord;
    if ( mX > 0.5) {
        flipLoc.x = (flipLoc.x < mX) ? flipLoc.x + inOut : mX - (flipLoc.x - mX) + inOut;
    } else {
        flipLoc.x = (flipLoc.x > mX) ? flipLoc.x + inOut : mX - (flipLoc.x - mX) + inOut;  
    }
    if ( mY > 0.5) {
        flipLoc.y = (flipLoc.y < mY) ? flipLoc.y + inOut : mY - (flipLoc.y - mY) + inOut;
    } else {
        flipLoc.y = (flipLoc.y > mY) ? flipLoc.y + inOut : mY - (flipLoc.y - mY) + inOut;  
    }

    gl_FragColor = IMG_NORM_PIXEL(inputImage,flipLoc);
}