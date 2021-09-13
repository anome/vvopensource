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
			"LABEL": "Intensity",
			"NAME": "Intensity",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -10.0,
			"MAX": 1.0
		},
		{
			"LABEL": "FactorA",
			"NAME": "FactorA",
			"TYPE": "float",
			"DEFAULT": -0.02,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "FactorB",
			"NAME": "FactorB",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"LABEL": "colorScalar",
			"NAME": "colorScalar",
			"TYPE": "float",
			"DEFAULT": 0.99,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

//Ported from "Lens/Film Chromatic Aberration" by Weaseltron: https://www.shadertoy.com/view/llK3RR

vec3 iResolution = vec3(RENDERSIZE, 1.);
vec2 iMouse = vec2(Intensity*RENDERSIZE.x, Intensity*RENDERSIZE.x);

// Given a vec2 in [-1,+1], generate a texture coord in [0,+1]
vec2 barrelDistortion( vec2 p, vec2 amt )
{
    p = 2.0 * p - 1.0;

    /*
    const float maxBarrelPower = 5.0;
	//note: http://glsl.heroku.com/e#3290.7 , copied from Little Grasshopper
    float theta  = atan(p.y, p.x);
    vec2 radius = vec2( length(p) );
    radius = pow(radius, 1.0 + maxBarrelPower * amt);
    p.x = radius.x * cos(theta);
    p.y = radius.y * sin(theta);

	/*/
    // much faster version
    //const float maxBarrelPower = 5.0;
    //float radius = length(p);
    float maxBarrelPower = sqrt(5.0);
    float radius = dot(p,p); //faster but doesn't match above accurately
    p *= pow(vec2(radius), maxBarrelPower * amt);
	/* */

    return p * 0.5 + 0.5;
}

//note: from https://www.shadertoy.com/view/MlSXR3
vec2 brownConradyDistortion(vec2 uv, float scalar)
{
// AH!!!    uv = uv * 2.0 - 1.0;
    uv = (uv - 0.5 ) * 2.0;
    
    if( true )
    {
        // positive values of K1 give barrel distortion, negative give pincushion
        float barrelDistortion1 = FactorA * scalar; // K1 in text books
        float barrelDistortion2 = FactorB * scalar; // K2 in text books

        float r2 = dot(uv,uv);
        uv *= 1.0 + barrelDistortion1 * r2 + barrelDistortion2 * r2 * r2;
        //uv *= 1.0 + barrelDistortion1 * r2;
    }
    
    // tangential distortion (due to off center lens elements)
    // is not modeled in this function, but if it was, the terms would go here
//    return uv * 0.5 + 0.5;
   return (uv / 2.0) + 0.5;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    
    float maxDistort = 4.0 * (1.0-iMouse.x/iResolution.x);

    float scalar = 1.0 * maxDistort;
//    vec4 colourScalar = vec4(2.0, 1.5, 1.0, 1.0);
    vec4 colourScalar = vec4(700.0, 560.0, 490.0, 1.0);	// Based on the true wavelengths of red, green, blue light.
    colourScalar /= max(max(colourScalar.x, colourScalar.y), colourScalar.z);
    colourScalar *= 2.0;
    
    colourScalar *= scalar;
    
    vec4 sourceCol = texture2D(iChannel0, uv);

    const float numTaps = 16.;
    
    fragColor = vec4( 0.0 );
    for( float tap = 0.0; tap < numTaps; tap += 1.0 )
   //if (tap > Iterations) {break;}
    {
        fragColor.r += texture2D(iChannel0, brownConradyDistortion(uv, colourScalar.r)).r;
        fragColor.g += texture2D(iChannel0, brownConradyDistortion(uv, colourScalar.g)).g;
        fragColor.b += texture2D(iChannel0, brownConradyDistortion(uv, colourScalar.b)).b;
        
       colourScalar *= (.99+colorScalar/100.);
    }
    
    fragColor /= numTaps;
  
    fragColor.a = 1.0;
}



void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}