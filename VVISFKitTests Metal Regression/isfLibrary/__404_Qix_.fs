/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
  "INPUTS": [
    	        {
      "MAX": [
        0.5,
        1.5
      ],
      "MIN": [
        0.01,
        0.5
      ],
      "DEFAULT":[0.25,1.0],
      "NAME": "density",
      "TYPE": "point2D"
	},
		{
      "MAX": 0.5,
      "MIN": 0.001,
      "DEFAULT":0.25,
      "NAME": "warper",
      "TYPE": "float"
    },
	{
      "MAX": 0.100,
      "MIN": 0.001,
      "DEFAULT":0.075,
      "NAME": "StepSize",
      "TYPE": "float"
    },
    	{
      "MAX": 0.9999,
      "MIN": 0.9000,
      "DEFAULT": 0.9900,
      "NAME": "LineCount",
      "TYPE": "float"
    },
    	{
      "MAX": 5.000,
      "MIN": 0.005,
      "DEFAULT":0.75,
      "NAME": "rate",
      "TYPE": "float"
    },
        	{
      "MAX": 2.000,
      "MIN": 0.125,
      "DEFAULT":1.5,
      "NAME": "span",
      "TYPE": "float"
    }
  ]
}
*/

// Qix! by mojovideotech
// based on :
// http://glslsandbox.com/e#26569.0
//
// forked from :
//
// From shadertoy by Mr Andre
// Gigatron for glslsandbox ; 


#ifdef GL_ES
precision highp float;
#endif



float iGlobalTime=TIME*rate;
//Function to draw a line, taken from the watch shader
float line(vec2 p, vec2 a, vec2 b, float thickness )
{
	vec2 pa = p - a;
	vec2 ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, span);
	return 1.0 - smoothstep(thickness * density.x, thickness * density.y, length(pa - ba * h));
}	

void main() 
{
	vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy) * 2.0 - 1.0;

	// convert the input coordinates by a cosinus
	// warpMultiplier is the frequency
	float warpMultiplier = (0.5 + 0.2345 * sin(iGlobalTime * 0.99)+1.111);
	vec2 warped = atan(uv * 6.28318530718 * warpMultiplier);

	// blend between the warpeffect and no effect
	// don't go all the way to the warp effect
	float warpornot = smoothstep(warper, 1.667, -sin((iGlobalTime * 0.067)+3.333));
	uv = mix(uv, warped, warpornot);

	// Variate the thickness of the lines
	float thickness = pow(3.333 - 6.667 / cos(iGlobalTime * 0.67), 0.125) / (RENDERSIZE.x*0.125);
	thickness *= 1.111 - (warpMultiplier * warpornot);
	float gt = floor(iGlobalTime * 33.0) * StepSize;

	// Add 10 lines to the pixel
	vec4 color = vec4(0.0, 0.0, 0.0, 1.0);
	for (int i = 0; i < 111; i++)
	{
		gt += StepSize;

		//Calculate the next two points
		vec2 point1 = vec2(sin(gt * 0.39), cos(gt * 0.63));
		vec2 point2 = vec2(cos(gt * 0.69), sin(gt * 0.29));

		// Fade older lines
		color.rgb = LineCount * color.rgb;

		// Add new line
		color.rgb += line(	uv,
							point1, point2,
							thickness		)
					//With color
					* ( 0.3 +
						0.3 * vec3(	sin(gt * 3.13),
									sin(gt * 1.69),
									sin(gt * 2.67)));
}

	// Clamp oversaturation
	gl_FragColor = clamp(color, 0.0, 1.0);
}
//void main( void ){vec4 color = vec4(0.0,0.0,0.0,1.0);mainImage( color, gl_FragCoord.xy );color.w = 1.0;gl_FragColor = color;}