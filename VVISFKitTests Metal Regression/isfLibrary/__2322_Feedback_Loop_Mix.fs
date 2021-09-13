/*{
  "DESCRIPTION": "trails",
  "CREDIT": "INKA",
  "CATEGORIES": [
    "Distortion Effect",
    "INKA"
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
      "DEFAULT": 0
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
    },
    {
      "NAME": "invert",
      "TYPE": "bool"
    }
  ],
  "PASSES": [
    {
      "TARGET": "buffer",
      "persistent": true
    },
    {}
  ]
}*/


mat2 rotmat( float deg ) {
	float theta = radians(deg)*16.0;
	float s = sin(theta);
	float c = cos(theta);

	return mat2(c, -s, s, c);
}

void main()
{
	vec2 uv =  gl_FragCoord.xy / RENDERSIZE.xy;
	vec4 color;

	if (PASSINDEX == 0)	{
		vec4 original = IMG_THIS_PIXEL(inputImage);
 		vec2 warp = (uv - 0.5) * zoom;
 		float _feedback = feedback;
 		
		warp *= rotmat(rotation);
		warp += 0.5;
		
 		color = IMG_NORM_PIXEL(buffer, warp); 
		
		if(passThrough > 0.) {
			float feedbackLuma = (color.r + color.g + color.b) / 3.;	
			float luma = (original.r + original.g + original.b) / 3.;
			
			if(luma > 1.0 - passThrough && luma > feedbackLuma)
				_feedback = 0.;
		}
		
		gl_FragColor = mix(original, color, _feedback);
	}
	else if (PASSINDEX == 1)	{	
		color = IMG_THIS_PIXEL(buffer);
		if ( invert )
			color = 1. - color;
			
		gl_FragColor = color;
	}
}
