
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/


float NoiseSeed;

float randomFloat(){
  NoiseSeed = sin(NoiseSeed) * 84522.13219145687;
  return fract(NoiseSeed);
}

vec3 ACESFilm(vec3 x)
{
    float a = 2.51;
    float b = 0.03;
    float c = 2.43;
    float d = 0.59;
    float e = 0.14;
    return (x*(a*x+b))/(x*(c*x+d)+e);
}



void main()	{
    NoiseSeed = float(RENDERSIZE)* .003186154 + gl_FragCoord.y * 17.2986546543 + gl_FragCoord.x;
    
    vec2 uv = isf_FragNormCoord.xy;
    
    vec2 d = (uv-vec2(.5)) * .0075;
    vec3 color = vec3(IMG_NORM_PIXEL(inputImage, uv - 0.0 * d).r,
                      IMG_NORM_PIXEL(inputImage, uv - 1.0 * d).g,
                      IMG_NORM_PIXEL(inputImage, uv - 2.0 * d).b);
    
    
    float noise = .9 + randomFloat()*.15;
  	gl_FragColor = vec4(ACESFilm(color*noise), 1.0);
}
