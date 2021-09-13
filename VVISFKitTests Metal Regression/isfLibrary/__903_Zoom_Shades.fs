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
			"DEFAULT": 4.0,
			"MAX": 10.0,
			"MIN": 0.0
		},
		{
			"NAME": "DIST",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MAX": 5.0,
			"MIN": -5.0
		},
		{
			"NAME": "FADE",
			"TYPE": "float",
			"DEFAULT": 2.0,
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
		float n = 1.0 / floor(LAYERS);
		vec4 color = IMG_THIS_PIXEL(inputImage);
		vec2 texcoord = (gl_FragCoord.xy / RENDERSIZE.xy - 0.5);
	
		for (int i=0; i < int(NLAYERS); i++)
		{
			
			if (float(i) > LAYERS) 
				break;
				
			float p = fract(fract(SPEED) + float(i) * n) / 2.;
			float z = (1.0 + p * DIST);
			float fade = p * FADE * 2.;
			
			vec2 uv = 0.5 + texcoord * z;
			vec4 pixel = IMG_NORM_PIXEL(inputImage, uv) - fade;
			
			
			if ((uv.x < 0.0) || (uv.y < 0.0) || (uv.x > 1.0) || (uv.y > 1.0)) {
				pixel = vec4(0.0);
			}

			float pixelLuma = (pixel.r + pixel.g + pixel.b) / 3.0;
			
			if (pixelLuma > ((color.r + color.g + color.b) / 3.0)) {
				color = mix(color, pixel, 1.0 - fade);
			}
			
		}
		
		gl_FragColor = color;
}
