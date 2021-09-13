/*{
	"DESCRIPTION": "Playing with log",
	"CREDIT": "SilviaFabiani",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "SPEED",
			"TYPE": "float",
			"DEFAULT": 0.06,
			"MAX": 1.0,
			"MIN": 0.0
		},
		{
			"NAME": "SYMMETRY",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MAX": 1.0,
			"MIN": 0.16
		},
		{
			"NAME": "SPIRALICITY",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MAX": 10.0,
			"MIN": -10.0
		},
		{
			"NAME": "smallerTILES",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MAX": 3.5,
			"MIN": 0.7
		},
		{
      "NAME": "EXPLODE",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "CIRCLES",
      "TYPE": "bool",
      "DEFAULT": 1
    }
      	]
}*/

// Based on "SPIRALING VIDEO" by FabriceNeyret2: https://www.shadertoy.com/view/MddSRB#


vec3 iResolution = vec3(RENDERSIZE, 1.);

void mainImage( out vec4 O, vec2 U )
{
     if (CIRCLES)
     {vec2 R = iResolution.xy; U = (U+U-R)/R.y; 
    U = vec2(atan(U.y,U.x)*SYMMETRY*3./2.1416,log(length(U)));}
    else
    {vec2 R = iResolution.xy; 
    U = (U+U-R)/R.xy; 
    U = vec2(atan(U.y,U.x)*SYMMETRY/2./1.1416,log(length(U )));
    U.y += U.x*SPIRALICITY;} 
    // conformal polar
    // multiply U for smaller tiles
   if (EXPLODE) U.y += U.x/SPIRALICITY;     O = IMG_NORM_PIXEL(inputImage, fract(U/smallerTILES+TIME*SPEED)/2.);
}


void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}
