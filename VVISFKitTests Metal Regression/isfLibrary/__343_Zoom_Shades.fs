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
			"MAX": 1.0,
			"MIN": 0.0
		},
		{
			"NAME": "LAYERS",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MAX": 10.0,
			"MIN": 0.0
		},
		{
			"NAME": "DIR",
			"TYPE": "float",
			"DEFAULT": -1.0,
			"MAX": 1.0,
			"MIN": -1.0
		},
		{
			"NAME": "FADE",
			"TYPE": "float",
			"DEFAULT": -1.0,
			"MAX": 10.0,
			"MIN": 0.0
		},
		{
			"NAME": "GAIN",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MAX": 1.0,
			"MIN": 0.0
		}

      	]
}*/

// Based off https://www.interactiveshaderforma.com/sketches/1019  but converted to work with VDMX
// Takes input texture and layers it to create a volumetric zoom.

#ifdef GL_ES
#define LOWP lowp
precision mediump float;
#else
#define LOWP
#endif

#define PI 3.14159
#define NLAYERS 10
//#define MAXZ 1.0

void main(void) {
		float n = 1.0 / float(LAYERS);
		vec4 color = IMG_THIS_PIXEL(inputImage);
		vec2 texcoord = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5);
		
		for (int i=0; i<int(NLAYERS); i++)
		{
			if (float(i) > LAYERS) 
				break;
				
			float p = fract(fract(TIME * SPEED * 0.5) + float(i) * n);
			float c = sin(p * PI);
			float z = (1.0 + p * DIR);
			vec2 uv = 0.5 + texcoord * z;
			vec4 pixel = IMG_NORM_PIXEL(inputImage, uv);
			float pixelLuma = (pixel.r + pixel.g + pixel.b) / 3.0;
			
			if (pixelLuma > 1.0 - GAIN && pixelLuma > (color.r + color.g + color.b) / 3.0)
				color = pixel / (1.0 + p * FADE);
			
		}
		
		gl_FragColor = color;
}
