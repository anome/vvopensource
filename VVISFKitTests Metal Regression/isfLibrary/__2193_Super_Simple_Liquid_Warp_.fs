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
			"NAME": "violence",
			"TYPE": "float",
			"DEFAULT": 0.03,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "levelOfExtreme",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "Frequency",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		}
      	]
}*/

// Original shader "super simple liquid warp" by jmickle: https://www.shadertoy.com/view/ldyXRR

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

// set up constants, effects ripple behaviour
//float violence = 0.103;
//float levelOfExtreme = 6.0;

// function warp x or y coordinate based on time and sin that shit
float warp( float a )
{
    float timeMod = iGlobalTime + a;
    return fract(a + cos( timeMod*Frequency * levelOfExtreme*10.0) * violence + sin( timeMod * levelOfExtreme*10.0/4.1317 ) * violence); //doubled the function. fract makes it tile!
}

// main thing
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // get 0..1 uv coords instead of pixel coords
    vec2 uv = (fragCoord.xy / iResolution.xy);
    
    // warp each uv coord seperately using our cool function
    uv.x = warp(uv.x);
    uv.y = warp(uv.y);
    
    // return the color from the texture with the warped uv
	fragColor = texture2D(iChannel0, uv);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}