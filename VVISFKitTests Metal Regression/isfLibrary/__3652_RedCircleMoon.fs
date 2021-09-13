/*{
	"CREDIT": "by spleen666",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generative",
		"red",
		"black",
		"circle",
		"moon",
		"sin",
		"cos"
	],
	"INPUTS": [

		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "floatInputY",
			"TYPE": "float",
			"DEFAULT": 2.5,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "floatRadius",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.25,
			"MAX": 1.0
		},
		{
			"NAME": "floatCircleSpread",
			"TYPE": "float",
			"DEFAULT": 50.0,
			"MIN": 0.0,
			"MAX": 100.0
		}
	
	]
}*/


precision highp float;

void main( void ) {
	
	vec2 p = 2.0*( gl_FragCoord.xy / RENDERSIZE.xy ) - 1.0;
	p.x *= RENDERSIZE.x/RENDERSIZE.y; 
	
	vec3 col = colorInput.rgb *  cos( p.y*floatInputY + TIME + clamp( 1.0/( floatCircleSpread*abs( length(p.xy)-floatRadius ) ), 0.0, 1.0) );
	
	gl_FragColor = vec4(col, 1.0); 
}
