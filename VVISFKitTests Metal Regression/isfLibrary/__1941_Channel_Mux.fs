/*{
  "CREDIT": "by wilstonoreo",
  "DESCRIPTION": "",
  "CATEGORIES": ["LiCHTPiRATEN"
  ],
  "INPUTS": [
    {
      "NAME": "redImage",
      "TYPE": "image"
    },
    {
      "NAME": "blueImage",
      "TYPE": "image"
    },
    {
      "NAME": "greenImage",
      "TYPE": "image"
    },
    {
      "NAME": "alphaImage",
      "TYPE": "image"
    }
  ]
}*/

float grayscale(vec4 c)
{
  return 0.299 *c.r + 0.587 *c.g + 0.114*c.b;
}

void main() 
{
  vec4 rP = IMG_THIS_PIXEL(redImage);
  vec4 gP = IMG_THIS_PIXEL(greenImage);
  vec4 bP = IMG_THIS_PIXEL(blueImage);
  vec4 aP = IMG_THIS_PIXEL(alphaImage);

  gl_FragColor = vec4(grayscale(rP),grayscale(gP),grayscale(bP),grayscale(aP));
}