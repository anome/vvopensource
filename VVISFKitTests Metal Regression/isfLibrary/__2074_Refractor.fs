/*{
	"DESCRIPTION": "Takes two inputs and fakes refraction",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
      		{
			"NAME": "iChannel1",
			"TYPE": "image"
		},
		{
			"NAME": "XAMOUNT",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": -1.0,
			"MAX": 1.0
		},
			{
			"NAME": "YAMOUNT",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": -1.0,
			"MAX": 1.0
		}
      	]
}*/

// Takes two inputs and fakes refraction by XAMOUNT and YAMOUNT
// Based on: https://magicmusicvisuals.com/forums/viewtopic.php?f=3&t=529&p=2675&hilit=refract#p2675

vec3 iResolution = vec3(RENDERSIZE, 1.0);

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    
    vec4 r = IMG_NORM_PIXEL(inputImage,uv);
    
    uv = uv + vec2(r.x * XAMOUNT*2.,r.y * YAMOUNT*2.);
    fragColor = IMG_NORM_PIXEL(inputImage,uv);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}