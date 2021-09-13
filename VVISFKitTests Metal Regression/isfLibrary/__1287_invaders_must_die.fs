/*
 {
  "DESCRIPTION": "hi corona",
  "CATEGORIES": [
    "warping",
    "feedback",
    "trails"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "feedback",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.99
    },
    {
      "NAME": "passThrough",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "MIN": 0.9,
      "MAX": 1.1,
      "DEFAULT": 1
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "MIN": -4,
      "MAX": 4,
      "DEFAULT": 0
    }
  ],
  "PASSES": [
    {
      "TARGET": "buffer",
      "persistent": true,
      "WIDTH": "$WIDTH/1.0",
	  "HEIGHT": "$HEIGHT/1.0"
    },
    {}
  ]
}
*/

#define R RENDERSIZE
#define LUMA_COL vec4(0.299, 0.587, 0.114, 0.0)

float luma(vec4 color) {
  return dot(color, LUMA_COL);
}

mat2 rotmat( float deg ) {
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);
	return mat2(c, -s, s, c);
}

void main() {
	vec2 uv =  gl_FragCoord.xy / R.xy;
	vec4 color;

	if (PASSINDEX == 0)	{
		
		vec4 original = IMG_THIS_PIXEL(inputImage);
 		
 		vec2 warp = (uv - 0.5) * zoom;
 		
 		float _feedback = feedback;
 		
		warp *= rotmat(rotation);
		warp += 0.5;
		
 		color = IMG_NORM_PIXEL(buffer, warp); 
		
		if(passThrough > 0.) {
			
			float feedbackLuma = luma(color);	
			float luma = luma(original);
			
			if(luma > 1. - passThrough && luma > feedbackLuma){
				_feedback = 0.;
			}
		}
		
		gl_FragColor = mix(original, color, _feedback);
	}
	else if (PASSINDEX == 1){
		gl_FragColor = IMG_THIS_PIXEL(buffer);
	}
}
