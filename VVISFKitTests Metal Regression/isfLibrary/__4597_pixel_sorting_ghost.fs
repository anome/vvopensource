/*
{
  "DESCRIPTION": "a pixel sorting exploration.",
  "CREDIT": "Andrea Bovo <spleen666@gmail.com>",
  "CATEGORIES": [
    "feedback",
    "trails",
    "pixelsort",
    "video"
  ],
  "INPUTS": [
      {
        "NAME": "inputImage",
        "TYPE": "image"
      },
      {
	    "NAME": "stepx",
	    "TYPE": "float",
	    "MIN": 1,
	    "MAX": 5,
	    "DEFAULT": 2
	  },
      {
	    "NAME": "stepy",
	    "TYPE": "float",
	    "MIN": 1,
	    "MAX": 5,
	    "DEFAULT": 2
	  },
      {
        "NAME": "stepFade",
        "TYPE": "float",
        "MIN": 1,
        "MAX": 100,
        "DEFAULT": 50
      },
      {
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0.0,
				0.0
			]
		}
  ],
  "PASSES": [
    {
      "PERSISTENT": true,
      "TARGET": "BufferA",
      "WIDTH": "$WIDTH/1.0",
	  "HEIGHT": "$HEIGHT/1.0"
    },
    {
    }
  ]
}
*/

#define R RENDERSIZE
#define t TIME

#define GAMMA 2.0
// sRGB -> linear
#define degamma( rgba ) ( pow(max(rgba, 0.), vec4(GAMMA)) )
// linear -> sRGB
#define gamma( rgba ) ( pow(max(rgba, 0.), vec4(1./GAMMA)) )

// sRGB -> linear / linear -> sRGB - optimized
#define degammao( rgba ) ( rgba*rgba )
#define gammao( rgba ) ( sqrt(rgba) )

#define HOLYGREY vec4(0.2126, 0.7152, 0.0722, 0.)
#define luma( rgba ) ( dot(rgba, HOLYGREY) )

void main() {
    vec2 uv = gl_FragCoord.xy / R.xy;
    vec4 col = vec4(0.0);
    //
    if (PASSINDEX == 0)	{
        
	   vec2 texel = 1. / R.xy;

      float step_y = texel.y;
      float step_x = texel.x;

	   vec2 s = vec2(0., -step_y);
	   vec2 n = vec2(0., step_y);

	   vec4 im_n = IMG_NORM_PIXEL(BufferA, uv + n);
	   vec4 im = IMG_NORM_PIXEL(BufferA, uv);
	   vec4 im_s = IMG_NORM_PIXEL(BufferA, uv + s);
	   
	   im = degammao(im);
	   im_s = degammao(im_s);
	   im_n = degammao(im_n);

	   float len_n = luma(im_n);
	   float len = luma(im);
	   float len_s = luma(im_s);
	   
	   int frame = int( mod( float(FRAMEINDEX) + gl_FragCoord.y, stepy  ));
        /**if (len_s > len) {
	       im = im_s;
	    } else if (len_n < len) {
	       im = im_n;
	    }*/
	   if( frame == 0 ) {
	    if (len_s > len) {
	       im = im_s;
	    }
	   } else if (len_n <= len) {
	       im = im_n;
	   }
	   
	   col = IMG_NORM_PIXEL(inputImage, uv);
       col = degammao(col);
	    
       col = (col + im * (stepFade - 1.0) ) / stepFade;
       col = gammao(col);
       
       gl_FragColor = col;
       
	} else if (PASSINDEX == 1){
	    col = IMG_NORM_PIXEL(BufferA, uv);
        gl_FragColor = col;
    }
}