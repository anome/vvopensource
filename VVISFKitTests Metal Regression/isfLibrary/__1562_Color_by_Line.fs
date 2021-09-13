/*{
  "CREDIT": "by Wilston Oreo, me@wilstonoreo.net",
  "DESCRIPTION": "",
  "CATEGORIES": [
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "colorImage",
      "TYPE": "image"
    },
        {
            "NAME": "spread",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 2.0
        },
        {
            "NAME": "angle",
            "TYPE": "float",
            "DEFAULT": 0.0,
            "MIN": 0.0,
            "MAX": 360.0
        },
        {
            "NAME": "pos",
            "TYPE": "point2D",
              "DEFAULT": [0.0,0.0],
            "MIN": [0.0,0.0],
            "MAX": [1.0,1.0]
        }
  ]
}*/

void main(void)
{
  vec4 color = IMG_THIS_PIXEL(inputImage);
  float theta = angle * 3.14159 / 180.0;
  float grayscale = 0.299*color.r + 0.587*color.g + 0.114*color.b;
  vec4 pickedColor = IMG_NORM_PIXEL(colorImage,fract(pos + vec2(sin(theta),cos(theta)) * spread *  grayscale));
  gl_FragColor = vec4(vec3(grayscale),color.a) * pickedColor;
}

