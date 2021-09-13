/*{
	"CREDIT": "by Joris de Jong",
	"DESCRIPTION": "Vignette Spot",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "Position",
			"TYPE": "point2D",
			"DEFAULT": [0.5,0.5]
		},
		{ 
			"NAME": "Radius", 
			"TYPE": "float", 
			"DEFAULT": 0.5
		},
		{ 
			"NAME": "Softness", 
			"TYPE": "float", 
			"DEFAULT": 0.2
		},
		{ 
			"NAME": "Ratio", 
			"TYPE": "float", 
			"DEFAULT": 0.5
		}
		
	]
}*/


float circle(in vec2 center, in float lRadius, in float blur){
	float aspect = RENDERSIZE.x / RENDERSIZE.y;
	vec2 pixel = isf_FragNormCoord.xy;
    vec2 dist = pixel - center;
   	dist.y /= aspect * Ratio * 2.0;

	return 1.-smoothstep( lRadius - ( lRadius * blur ),
                         lRadius + ( lRadius * blur ),
                         dot( dist, dist ) * 4.0 );
}

void main() {
	float mask = circle( Position, Radius, Softness );
	vec4 color = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	color.a = mask;
	gl_FragColor = color;
}
