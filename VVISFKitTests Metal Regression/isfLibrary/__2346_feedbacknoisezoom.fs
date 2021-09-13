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
      "NAME": "noiseLevel",
      "TYPE": "float",
      "DEFAULT": 0.14,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "invert",
      "TYPE": "bool"
    },
	{
		"NAME": "distortion",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 1.0,
		"DEFAULT": 0.0
	},
	{
		"NAME": "frequency",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 1,
		"DEFAULT": 0.1
	},
	{
		"NAME": "period",
		"TYPE": "float",
		"MIN": 0.0,
		"MAX": 1.0,
		"DEFAULT": 0.3
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
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);

	return mat2(c, -s, s, c);
}


// Hash without Sine
// https://www.shadertoy.com/view/4djSRW
#define NOISEVEC vec3(443.8975,397.2973, 491.1871)
#define PI 3.141592654

//  1 out, 2 in...
float noise(float seed) {
	
	vec2 p = (gl_FragCoord.xy + TIME + seed);
	p = vec2(atan(p.x, p.y), length(p));
    vec3 p3 = fract(vec3(p.xyx) * NOISEVEC);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}


vec2 pb(in vec2 uv, in float per){
    uv.y += (period * PI * 2.) / per;
    vec2 result = (cos(uv.y * per)) * normalize(vec2(1., cos((uv.y) * per)));
    return result;
}

void main()
{
	vec2 uv =  gl_FragCoord.xy / RENDERSIZE.xy;
	vec4 color;

	if (PASSINDEX == 0)	{
		vec4 original = IMG_THIS_PIXEL(inputImage);
 		vec2 warp = (uv - 0.5) * zoom;
 		
		//warp = warp / RENDERSIZE.x - .5;
 		float _feedback = feedback;
 		
		warp *= rotmat(rotation);
		warp += 0.5;
		
		
	    vec2 hpert = pb(warp, frequency * 10.0);
	    warp += hpert * distortion * .01;
		
    	
 		color = IMG_NORM_PIXEL(buffer, warp); 
 		
		vec4 noise1 = vec4(noise(0.)) * noiseLevel;
		vec4 noise2 = vec4(noise(0.5)) * noiseLevel;
		

		
		
		color += (noise1 - noise2) * 0.02;
		
		if(passThrough > 0.) {
			float feedbackLuma = (color.r + color.g + color.b) / 3.;	
			float luma = (original.r + original.g + original.b) / 3.;
			
			if(luma > 1. - passThrough && luma > feedbackLuma)
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
