/*{
  "CREDIT": "by wilstonoreo",
  "DESCRIPTION": "",
  "CATEGORIES": ["LiCHTPiRATEN"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
        {
            "NAME": "num_horz",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "num_vert",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 1.0,
            "MAX": 100.0
        },
        {
            "NAME": "offset",
            "TYPE": "point2D",
              "DEFAULT": [0.0,0.0],
            "MIN": [0.0,0.0],
            "MAX": [1.0,1.0]
        }
  ]
}*/

void main() {
  vec2 size = vec2(num_horz,num_vert);
  vec2 v = floor(size* fract(vv_FragNormCoord)) / size ;
  gl_FragColor = IMG_NORM_PIXEL(inputImage,fract(v + offset) );
  
}