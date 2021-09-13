/*{
  "DESCRIPTION": "performs a 3d rotation",
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "xrot",
      "LABEL": "X rotate",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "yrot",
      "LABEL": "Y rotate",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "zrot",
      "LABEL": "Z rotate",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 1
    },
    {
      "NAME": "zoom",
      "LABEL": "Zoom Level",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 1
    }
  ]
}*/


void main()
{
	gl_FragColor = IMG_THIS_PIXEL(inputImage);
}
