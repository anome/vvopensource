/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
		{
			"NAME": "DEPTH",
			"TYPE": "float",
			"DEFAULT": 1.35,
			"MAX": 5.0,
			"MIN": 0.0
		},
		{
			"NAME": "GAIN",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MAX": 1.0,
			"MIN": 0.0
		},
		{
			"NAME": "LAYERS",
			"TYPE": "float",
			"DEFAULT": 10,
			"MAX": 100.0,
			"MIN": 1.0
		},
		{
			"NAME": "TILE",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
		
      	]
}*/

// Based off https://www.shadertoy.com/view/ldjXWw# Uploaded by sehugg
// Takes input texture and layers it to create a volumetric zoom.

vec3 iResolution = vec3(RENDERSIZE, 1.0);
float iGlobalTime = TIME;

#ifdef GL_ES
#define LOWP lowp
precision mediump float;
#else
#define LOWP 
#endif

#define PI 3.14159
#define NLAYERS 10
//#define MAXZ 1.0
#define SPEED .1

void mainImage( out vec4 fragColor, in vec2 fragCoord )

{
	float n = 1.0 / float(NLAYERS);
	vec4 frag = vec4(0,0,0,0);
	vec2 texcoord = (fragCoord.xy / iResolution.xy - 0.5)*DEPTH;
	for (int i=0; i<int(100); i++)
	{
		if (i > int(LAYERS)) {break;}
		float p = fract(fract(iGlobalTime * SPEED) + float(i) * n);
		float c = 1.0 - abs(p-0.5)*2.0; // sin(p*PI);
 		
 		vec2 uv = 0.5 + texcoord * (1.0-p);
 		
 		if (TILE) { uv = fract(0.5 + texcoord * (1.0-p)); }
 		
 		frag += texture2D(iChannel0, uv) * c * GAIN;
	}
	fragColor = (frag * frag);
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}