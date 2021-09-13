/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Generator"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
            {
            "NAME": "rate",
            "TYPE": "float",
           "DEFAULT": 0.0125,
            "MIN": 0.0001,
            "MAX": 0.025
        },
         {
            "NAME": "offset",
            "TYPE": "float",
           "DEFAULT": 0.35,
            "MIN": -1.0,
            "MAX": 1.0
        }, 
        {
            "NAME": "loops",
            "TYPE": "float",
           "DEFAULT": 11,
            "MIN": 1,
            "MAX": 22
        }, 
        {
            "NAME": "stepsize",
            "TYPE": "float",
           "DEFAULT": 0.065,
            "MIN": 0.0005,
            "MAX": 1.0
        },
        {
            "NAME": "thickness",
            "TYPE": "float",
           "DEFAULT": 108,
            "MIN": 10,
            "MAX": 500
        },
                      {
            "NAME": "iterator",
            "TYPE": "float",
           "DEFAULT": 123,
            "MIN": 3,
            "MAX": 333
        },
        {
            "NAME": "BGhue",
            "TYPE": "float",
           "DEFAULT": 0.63,
            "MIN": 0.05,
            "MAX": 1.00
        },
        	{
			"NAME": "CColor",
			"TYPE": "color",
			"DEFAULT": [
				0.5,
				0.4,
				0.1,
				1.0
			]
		}
  ]
}
*/


#ifdef GL_ES
precision mediump float;
#endif

// ElipticalEnvelopment by mojovideotech


void main( void )
{

    float x = gl_FragCoord.x/RENDERSIZE.x;
    float y = gl_FragCoord.y/RENDERSIZE.y;
    float ndcx = x * 2.0 - 1.0;
    float ndcy = y * 2.0 - 1.0;	
    vec2 ndc = vec2(ndcx, ndcy);
    
	float N = floor(iterator);
	float tt = TIME * rate;
	
	float best = 0.0;
		for(int i=0;i<377;++i)
	{   
		N = N-1.0;
	    if (N<=float(i))
	       break;
		float a1 = 2.0*3.1415927*float(N)/float(loops);
		vec2 pos = offset*vec2(cos(a1), sin(a1));
		float tm = max(tt + float(N)*stepsize,-1.0);
		tm = mod(tm, mod(tm, loops*stepsize));
		float d = (511.-thickness)*abs(length(pos - ndc) - tm);
		if(d<1.0)
			best = max(best, 1.0-d);
	}

	vec4 color1 = vec4(0.000, 0.000, 0.000, 1.0);
	vec4 color2 = vec4(CColor);
	     color1 = vec4(1.0-BGhue,0.0,BGhue,0.5);
	gl_FragColor = mix(color1, color2, min(best*2.0, 1.0));
}