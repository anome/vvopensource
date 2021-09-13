
/*{
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "mode",
			"TYPE": "long",
			"VALUES": [
				0,
				1
			],
			"LABELS": [
				"Color only",
				"Alpha only"
			],
			"DEFAULT": 0
		}
	]
	
}*/

void main()
{
	vec4 color = IMG_THIS_PIXEL(inputImage);
	if( mode == 0 )
	{
		color.rgb = mix(vec3(0.), color.rgb, color.a);
		color.a = 1.;
		gl_FragColor = color;
	}
	else if( mode == 1 )
	{
		color.rgb = vec3(color.a);
		color.a = 1.;
		gl_FragColor = color;
	}
}
