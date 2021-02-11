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
			"LABEL": "AMOUNT",
			"NAME": "AMOUNT",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
		},
      	{
			"LABEL": "INTENSITY",
			"NAME": "INTENSITY",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 1.0,
			"MAX": 10.0
		},
		{
			"LABEL": "ITERATIONS",
			"NAME": "ITERATIONS",
			"TYPE": "float",
			"DEFAULT": 150.0,
			"MIN": 1.0,
			"MAX": 250.0
		}
		
		
	]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

// Bokeh disc.
// by David Hoskins.
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// https://www.shadertoy.com/view/4d2Xzw

#define USE_MIPMAP

// The Golden Angle is (3.-sqrt(5.0))*PI radians, which doesn't precompiled for some reason.
// The compiler is a dunce I tells-ya!!

#define GOLDEN_ANGLE 2.39996323

// #define ITERATIONS 500

mat2 rot = mat2(cos(GOLDEN_ANGLE), sin(GOLDEN_ANGLE), -sin(GOLDEN_ANGLE), cos(GOLDEN_ANGLE));

//-------------------------------------------------------------------------------------------

vec3 Bokeh(vec2 uv, float radius, float amount)
{
	vec3 acc = vec3(0.0);
	vec3 div = vec3(0.0);
    vec2 pixel = 1.0 / iResolution.xy;
    float r = 1.0;
    vec2 vangle = vec2(0.0,radius); // Start angle
    amount += radius*500.0;
    
	for (int j = 0; j < 250; j++)
    {  
    	
    	if ( j > int(ITERATIONS) ) { break; }
    	
        r += 1. / r;
	    vangle = rot * vangle;
        // (r-1.0) here is the equivalent to sqrt(0, 1, 2, 3...)
        
       vec3 col = IMG_NORM_PIXEL(inputImage, uv + pixel * (r-1.) * vangle).xyz;
        
        // col = col * col * 1.5; // ...contrast it for better highlights - leave this out elsewhere.
		
		vec3 bokeh = pow(col, vec3(INTENSITY)) * amount+.4;
		acc += col * bokeh;
		div += bokeh;
	}
	return acc / div;
}

//-------------------------------------------------------------------------------------------
void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    float time = iGlobalTime*.2 + .5;
	float r = AMOUNT*10.;
       
	float a = 40.0;
  
  uv *= vec2(1.0, 1.0);
    
   fragColor = vec4(Bokeh(uv, r, a), 1.0);
    
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}