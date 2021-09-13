/*{
    "CATEGORIES": [
        "generator"
    ],
    "CREDIT": "by Mattias DUPUIS",
    "DESCRIPTION": "Shade of gray with new Target",
    "INPUTS": [
        {
            "DEFAULT": 0.01,
            "LABEL": "step",
            "MAX": 0.3,
            "MIN": 0.01,
            "NAME": "step",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "LABEL": "min",
            "MAX": 1,
            "MIN": 0,
            "NAME": "min",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "LABEL": "max",
            "MAX": 1,
            "MIN": 0,
            "NAME": "max",
            "TYPE": "float"
        }
    ],
    "PASSES": [
        {
            "TARGET": "bufferVariableNameA"
        },
        {
            "TARGET": "infos",
            "PERSISTENT": true
        }
    ]
}
*/


void main()
{
		vec4 stalePixel = IMG_THIS_PIXEL(bufferVariableNameA);
		vec4 target = IMG_THIS_PIXEL(infos);
		
		float inter = max-min;
        float t = 1.0/inter;
		
		
		if(stalePixel.rgb == target.rgb)
		{
			float rand = fract(sin(TIME)*1.0);
			rand = rand/t;
			rand = rand + min;
			target.r = rand;
			target.g = rand;
			target.b = rand;
		}

		if(target.r > stalePixel.r)
		{
			if(stalePixel.r + step >= target.r)
			{
				stalePixel.r = target.r;
			}
			else
			{
				stalePixel.r = stalePixel.r + step;
			}
		}
		else
		{
			if(stalePixel.r - step <= target.r)
			{
				stalePixel.r = target.r;
			}
			else
			{
				stalePixel.r = stalePixel.r - step;
			}
		}
		stalePixel.g = stalePixel.r;
		stalePixel.b = stalePixel.r;
		gl_FragColor = stalePixel;
	}
	
