/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	{
            "NAME": "amplitude",
            "TYPE": "float",
           "DEFAULT": 2.0,
            "MIN": 0.5,
            "MAX": 8.0
        },
		{
            "NAME": "frequency",
            "TYPE": "float",
           "DEFAULT": 0.005,
            "MIN": 0.001,
            "MAX": 0.01
        },
        {
            "NAME": "phase",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 0.9
        }
	]
}*/

// RGBWaves by mojovideotech

void main ( void )
{
    float amp = amplitude;
    float freq = abs(cos(TIME*frequency));
    float shift = 1.0 - TIME * phase;
    float xx = 1.0 - abs((gl_FragCoord.y / RENDERSIZE.y - .6) * amp - sin((gl_FragCoord.x / RENDERSIZE.x - shift) * freq));
    float xz = 1.0 - abs((gl_FragCoord.y / RENDERSIZE.y - .6) * amp - sin((gl_FragCoord.x / RENDERSIZE.x - 1.0 - shift * -.5) * freq));
    float xr = 1.0 - abs((gl_FragCoord.y / RENDERSIZE.y - .6) * amp - sin((gl_FragCoord.x / RENDERSIZE.x - 2.5 - shift * 1.5) * freq));
	gl_FragColor.b = xz;
    gl_FragColor.r = xx;
    gl_FragColor.g = xr;
    gl_FragColor.a = 0.5;
}