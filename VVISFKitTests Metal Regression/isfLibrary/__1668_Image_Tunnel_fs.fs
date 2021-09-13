/*{
  "DESCRIPTION": "Based of ZoomTex",
  "CREDIT": "by INKA",
  "CATEGORIES": [
    "XXX",
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "SPEED",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MAX": 1,
      "MIN": 0
    },
    {
      "NAME": "LAYERS",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MAX": 10,
      "MIN": 0
    },
    {
      "NAME": "DIR",
      "TYPE": "float",
      "DEFAULT": -1,
      "MAX": 1,
      "MIN": -1
    },
    {
      "NAME": "FADE",
      "TYPE": "float",
      "DEFAULT": 0,
      "MAX": 1,
      "MIN": 0
    },
    {
      "NAME": "GAIN",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MAX": 1,
      "MIN": 0
    },
    {
      "NAME": "MOD",
      "TYPE": "float",
      "DEFAULT": 0.25
    },
    {
      "NAME": "MOD1",
      "TYPE": "float",
      "DEFAULT": 0.25
    }
  ]
}*/

// Based off https://www.interactiveshaderforma.com/sketches/1019  but converted to work with VDMX
// Takes input texture and layers it to create a volumetric zoom.


#define PI 3.14159
#define NLAYERS 10
#define TAU 6.2831853071795865

//#define MAXZ 1.0

void main(void) {
		float n = 1.0 / floor(LAYERS);
		vec4 color = vec4(0.0);// = IMG_THIS_PIXEL(inputImage);
		vec2 texcoord = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5);
		
		for (int i=0; i<int(NLAYERS); i++)
		{
			if (float(i) > LAYERS) 
				break;
				
			float p = fract(fract(SPEED) * floor(DIR) + float(i) * n);
			float c = sin(p * PI);
			float z = (p * MOD * 10.);
			vec2 uv = 0.5 + texcoord * z;
			uv.y = uv.y - p * MOD1;
    
	    //offs.y = 0.15 * TAU * x *  MOVE_Y; //cos(TAU * x * 0.3) + 0.2 * cos(TAU * x * 0.1);
			vec4 pixel = IMG_NORM_PIXEL(inputImage, uv);
			
			if ((uv.x < 0.0) || (uv.y < 0.0) || (uv.x > 1.0) || (uv.y > 1.0)) {
				pixel = vec4(0.0);
			}
			
			float pixelLuma = (pixel.r + pixel.g + pixel.b) / 3.0;
			pixel = pixel / max(1., pow(10. * FADE, p));
			if(pixelLuma > 1.0 - GAIN && (pixel.r + pixel.g + pixel.b) / 3.0 > (color.r + color.g + color.b) / 3.0)
				color = pixel;
			
		}
		
		gl_FragColor = color;
}
