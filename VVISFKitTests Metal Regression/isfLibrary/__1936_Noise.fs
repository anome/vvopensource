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
			"NAME": "ditherAmount",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "Animated",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "Monochrome",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
      	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

/**
* trying out couple of dithering methods to get rid of quantization artefacts.
* http://www.loopit.dk/banding_in_games.pdf
*/

// Based on "Dithering Methods" by kuvkar: https://www.shadertoy.com/view/ld3XWl

// standard "rand" function 
float rand(vec2 co){
  return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

///////////////////////// dithering functions from ////////////////
// https://www.shadertoy.com/view/MslGR8# by hornet           /////
///////////////////////////////////////////////////////////////////

#define MOD3 vec3(443.8975,397.2973, 491.1871)
float hash12(vec2 p)
{
	vec3 p3  = fract(vec3(p.xyx) * MOD3);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
	vec4 col = texture2D(iChannel0, uv);
	vec2 seed = uv;
    
    vec4 ditherCol;
    if (Animated) seed += fract(iGlobalTime);
	
	ditherCol.r = (hash12( seed ) + hash12(seed + 0.59374) - 1.);
    seed += 0.1;
    ditherCol.g = (hash12( seed ) + hash12(seed + 0.59374) - 1.);
    seed += 0.04;
    ditherCol.b = (hash12( seed ) + hash12(seed + 0.59374) - 1.);

	if (Monochrome)
	{
	ditherCol.g = ditherCol.r;
	ditherCol.b = ditherCol.r;
	}

    col += ditherCol * ditherAmount;

  	fragColor = col;    
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}