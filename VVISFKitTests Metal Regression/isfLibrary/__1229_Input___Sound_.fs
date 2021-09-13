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
			"NAME": "DrawWaveform",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "WaveSampleRow",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "FFTSampleRow",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "WaveWidth",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "MuteFFT",
			"TYPE": "bool",
			"DEFAULT": 0.0
		}
      	]
}*/

// Ported from "Input - Sound" by iq: https://www.shadertoy.com/view/Xds3Rr

vec3 iResolution = vec3(RENDERSIZE, 1.);

// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // create pixel coordinates
	vec2 uv = fragCoord.xy / iResolution.xy;

	// first texture row is frequency data
	float fft  = texture2D( iChannel0, vec2(uv.x,FFTSampleRow) ).x; 
	
    // second texture row is the sound wave
	float wave = texture2D( iChannel0, vec2(uv.x,WaveSampleRow) ).x;
	
	// convert frequency to colors
	vec3 col = vec3( fft, 4.0*fft*(1.0-fft), 1.0-fft ) * fft;
	
	if (MuteFFT) { col = vec3 (0.,0.,0.);}

    // add wave form on top	
	if (DrawWaveform) { col += 1.0 -  smoothstep( 0.0, (WaveWidth+.001), abs(wave - uv.y) ); }
	
	// output final color
	fragColor = vec4(col,1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}