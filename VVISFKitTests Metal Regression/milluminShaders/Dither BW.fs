/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
            "NAME": "SEED",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0
        },
		{
			"NAME": "Static",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
      	]
}*/

// Based on Noise Dither by mbouchard: https://www.shadertoy.com/view/Xl3Xzl

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;



float nrand( vec2 n )
{
	return fract(sin(dot(n.xy, vec2(12.9898, 78.233)))* 43758.5453 * (SEED+.001));
}

float n1rand( vec2 n )
{
	float t = fract( iGlobalTime );
	if (Static) { t=1.0; }
    //t = 1.;
	float nrnd0 = nrand( n + t );
	return nrnd0;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{

    
	vec2 uv = fragCoord.xy / iResolution.xy;

    float N 	= n1rand(uv * 2.3);
 
    vec4 ditheredTex = IMG_NORM_PIXEL(inputImage, uv);
    float desaturateTex = dot(vec3(0.3, 0.59, 0.11),vec3(ditheredTex));
    desaturateTex = pow(1. - desaturateTex,1.);
   	desaturateTex = smoothstep(0.2,0.5, desaturateTex);
    
    vec3 final = vec3(step(desaturateTex,N)); 
	fragColor = vec4(final,1);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}