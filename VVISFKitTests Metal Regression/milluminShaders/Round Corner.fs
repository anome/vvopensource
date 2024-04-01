/*{
	"CREDIT": "by Anomes",
	"CATEGORIES": [
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "radius",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1000.0,
			"DEFAULT": 30.0
		},
		{
			"NAME": "border",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1000.0,
			"DEFAULT": 10.0
		},
		{
			"NAME": "borderColor",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		}
	]
}*/



void main() 
{
	vec4 color = IMG_NORM_PIXEL(inputImage, vv_FragNormCoord);
	vec2 position = abs(vv_FragNormCoord-0.5)*RENDERSIZE;
	float m = length(  max(position-(0.5*RENDERSIZE-radius), 0.)  );
	float n = length(  max(position-(0.5*RENDERSIZE-max(radius,border)), 0.)  );
	if( 0. < border && 0. < n && radius < n+border )
	{
		color = borderColor;
	}
	if( radius < m )
	{
		color.a = smoothstep(1., 0., m-radius);
	}
	gl_FragColor = color;
}
