/*{
  "CREDIT": "BrianChasalow, enhanced by zerbzman",
  "CATEGORIES": [
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "imageScale",
      "TYPE": "float",
      "MIN": 0.01,
      "MAX": 3,
      "DEFAULT": 1.0
    },
    {
      "NAME": "slide",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "slideSpeed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.1,
      "DEFAULT": 0.05
    },
    {
      "NAME": "shift",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0
    },
    {
      "NAME": "shiftSpeed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0.0
    },
    {
      "NAME": "mirrorHorizontal",
      "TYPE": "bool",
      "MIN": false,
      "MAX": true,
      "DEFAULT": true
    },
    {
      "NAME": "mirrorVertical",
      "TYPE": "bool",
      "MIN": false,
      "MAX": true,
      "DEFAULT": true
    }
  ]
}*/

void main(void)
{
	vec2 coord = isf_FragNormCoord;
	vec2 imageSize = IMG_SIZE(inputImage);
	
	// first we need to scale the image to fit the RENDERSIZE
	vec2 scaledImage;
	scaledImage.x = (imageSize.x * RENDERSIZE.y) / imageSize.y;
	scaledImage.y = RENDERSIZE.y;
	scaledImage *= imageScale;
	
	// now we need to normalize the scaled image with the RENDERSIZE
	coord *= RENDERSIZE / scaledImage;
    
	coord.x += slide + (TIME * slideSpeed); // Adjust speed of slide and shift
	coord.y += shift + (TIME * shiftSpeed);
	vec2 moddedCoord = fract(coord); // reset the coordinates to what will fit in the screen no matter what the size
	
	// make sure only the even images are mirrored
	if(mirrorHorizontal && mod(coord.x, 2.0) >= 1.0 && mod(coord.x, 2.0) <= 2.0)
		moddedCoord = vec2(1.0 - moddedCoord.x, moddedCoord.y);
	if(mirrorVertical && mod(coord.y, 2.0) >= 1.0 && mod(coord.y, 2.0) <= 2.0)
		moddedCoord = vec2(moddedCoord.x, 1.0-moddedCoord.y);
	
	vec4 pixel = IMG_NORM_PIXEL(inputImage, moddedCoord);
	gl_FragColor = pixel;
}