/*{
	"CREDIT": "",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "color1",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "color2",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				0.0,
				1.0
			]
		}
	]
}*/


void main()
{	
	float width = 0.2;
	float offset = mod(TIME/4.0, 1.);
	float x = isf_FragNormCoord.x;
	if(  mod( (x-offset)/width , 1.0 )  <  0.5  )
	{
		gl_FragColor = color1;
	}
	else
	{
		gl_FragColor = color2;
	}
}
