/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
      	{
			"NAME": "iChannel1",
			"TYPE": "image"
		},
		{	
			"NAME": "Steps",
			"TYPE": "float",
			"DEFAULT": 32.0,
			"MIN": 0.0,
			"MAX": 2560.0
		},{	
			"NAME": "space",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{	
			"LABEL": "Amplitude",
			"NAME": "ampl",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{	
			"LABEL": "Gain",
			"NAME": "gain",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 5.0
		},
		{	
			"LABEL": "Clip Floor",
			"NAME": "offset",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{	
			"LABEL": "Squelch",
			"NAME": "squelch",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{	
			"LABEL": "Scrub",
			"NAME": "scrub",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		}
      	]
}*/

// Original shader "Simple Sound Visualizer" by stduhpf: https://www.shadertoy.com/view/XdcXRs

// Steps is the number of bars in the spectrum.
// Spacing is the gap between bars. Recommend turning to 0.0 if using Squelch.
// Amplitude is the sample volume. 
// Gain is amplification factor.
// Squelch starts sampling from left edge, reducing size of sample window at expense of spatial resolution.
// Scrub shifts the sample window. (1.0 slides the window the width of the spectrum.)

vec3 iResolution = vec3(RENDERSIZE, 1.);

#define seuil .0
#define def .2

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    
    float sound = texture2D(iChannel0,vec2(floor((Steps)*uv.x*(1.0-squelch)+scrub*((Steps)/RENDERSIZE.x)*RENDERSIZE.x)/Steps+(1.0/Steps)-.001,0.)).r-offset;
    sound *= ampl*(gain+1.)*2.;
    sound -=seuil;
    sound = max(def, sound);
    if (uv.x*Steps-floor (uv.x*(Steps))<space)sound = 0.;

    vec4 color = texture2D(iChannel1,uv);
       // uv.y +=texture2D(iChannel0,vec2(floor(Steps*uv.x)/Steps,1)).r/20.; // make the spectrum analysis dance with the waveform Ã¢â¢Â«Ã¢â¢Â¥
    if (abs((0.,5.*uv.y-2.5)) < sound*sound*sound) color=mix(color, vec4(1),1.0);
	fragColor =color;
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}