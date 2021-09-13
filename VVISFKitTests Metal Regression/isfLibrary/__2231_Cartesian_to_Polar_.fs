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
            "NAME": "Radius",
            "TYPE": "float",
            "DEFAULT": 0.75,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "Sweep",
            "TYPE": "float",
            "DEFAULT": 1.0,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
			"NAME": "Repeat",
			"TYPE": "bool",
			"DEFAULT": false
		}
      	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);

// Ported from Cartesian to Polar Coordinates by KeyMaster: https://gist.github.com/KeyMaster-/70c13961a6ed65b6677d

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 relativePos = fragCoord.xy - (iResolution.xy / 2.0);
    vec2 polar;
    polar.y = sqrt(relativePos.x * relativePos.x + relativePos.y * relativePos.y);
    polar.y /= iResolution.x / ((1.0218-Radius) *100.);
    polar.y = 1.0 - polar.y;
    if (Repeat) polar.y = fract (polar.y);

    polar.x = -atan(relativePos.y, relativePos.x);
    polar.x -= 1.57079632679;
    if(polar.x < 0.0){
		polar.x += 6.28318530718;
    }
    polar.x /= 6.28318530718*Sweep;
    polar.x = 1.0 - polar.x;
    
    vec4 c = IMG_NORM_PIXEL(iChannel0, polar);
	fragColor = vec4(c);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}