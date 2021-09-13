/*{
  "DESCRIPTION": "RGB GLitchMod",
  "CREDIT": "by dantheman",
  "CATEGORIES": [
    "Distortion Effect"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "offset",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        1
      ]
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT":5,
      "MIN": 1,
      "MAX": 20
    },
        {
      "NAME": "offsetScale",
      "TYPE": "float",
      "DEFAULT":0.02,
      "MIN": -0.1,
      "MAX": 0.1
    },
    {
      "NAME": "mix_var",
      "TYPE": "float",
      "DEFAULT":0.29,
      "MIN": 0,
      "MAX": 1
    }
  ],
  "PASSES": [
    {
      "TARGET": "one",
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "buffer",
      "persistent": true
    },
     {
      "TARGET": "two",
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "buffer",
      "persistent": true
    },
         {
      "TARGET": "three",
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "buffer",
      "persistent": true
    },
         {
      "TARGET": "four",
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "buffer",
      "persistent": true
    }
  ]
}*/



void main() {
  
  vec2 pos = isf_FragNormCoord;
  vec4 old = IMG_NORM_PIXEL(one, pos);
  vec4 oldold = IMG_NORM_PIXEL(two, pos);
  vec4 oldoldold = IMG_NORM_PIXEL(three, pos);
  vec4 new = IMG_NORM_PIXEL(inputImage, pos);
  vec4 U = vec4(0);
  
   U = (IMG_NORM_PIXEL(two, isf_FragNormCoord+offset*offsetScale)*mix_var + IMG_NORM_PIXEL(one, isf_FragNormCoord+offset*offsetScale)*mix_var
   +IMG_NORM_PIXEL(three, isf_FragNormCoord+offset*offsetScale)*mix_var+IMG_NORM_PIXEL(four, isf_FragNormCoord+offset*offsetScale)*mix_var);


  gl_FragColor =(new)+(U-(old+oldold+oldoldold)/scale);
}