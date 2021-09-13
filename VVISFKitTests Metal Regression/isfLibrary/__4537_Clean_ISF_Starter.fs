// License: MIT 2021 by NERDDISCO (Tim Pietrusky)

/*{
	"DESCRIPTION": "",
	"CREDIT": "NERDDISCO",
	"ISFVSN": "2",
	"CATEGORIES": [
		""
	],
	"INPUTS": [
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/

#define PI 3.1415926535897932384626433832795

void main()	{
    // ISF Variables: https://docs.isf.video/ref_variables.html
    // ISF Functions: https://docs.isf.video/ref_functions.html
    
    // ISF compared to other websites like Shadertoy
    // RENDERSIZE = resolution = iResolution
    // TIME = time = iTime
    // isf_FragNormCoord.xy = gl_FragCoord.xy / RENDERSIZE.xy = uv
    
    // Shadertoy Default-Template
	gl_FragColor = vec4(0.5 + 0.5 * cos(TIME + isf_FragNormCoord.xyx + vec3(0, 2, 4)), 1.0);
}