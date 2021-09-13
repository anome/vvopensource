/*{
    "CATEGORIES": [
        "Pattern"
    ],
    "ISFVSN": "2",
    "CREDIT": "by VIDVOX",
    "INPUTS": [
        {
            "DEFAULT": 0.115,
            "MAX": 1,
            "MIN": 0,
            "NAME": "patternMode",
            "TYPE": "float"
        },
        {
            "DEFAULT": 100,
            "MAX": 500,
            "MIN": 1,
            "NAME": "repeatCount",
            "TYPE": "float"
        },
		{
			"DEFAULT": 4,
			"LABELS": [
				"2",
				"3",
				"4"
			],
			"NAME": "gridSize",
			"TYPE": "long",
			"VALUES": [
				2,
				3,
				4
			]
		}
    ],
	"PASSES": [ 
		{
			"TARGET": "lookupRender",
			"WIDTH": "floor($gridSize)",
			"HEIGHT": "floor($gridSize)",
			"DESCRIPTION": "Pass 0"
		},
		{
			"TARGET": "outputRender",
			"DESCRIPTION": "Pass 1"
		}
	]
}
*/

float pattern(vec2 c)	{
	float	returnMe = 0.0;
	float	gs = floor(float(gridSize));
	vec2	modCoord = mod(floor(c),gs);

	float	pIndex = pow(2.0,modCoord.y*gs+modCoord.x);

	float	lpm = patternMode * (pow(2.0,gs*gs) - 1.0);
	float	remainder = (lpm);
	float	divideMe = pow(2.0,(gs*gs)-1.0);
	
	if (pIndex <= lpm)	{
		for (int i = 0;i < 5;i++)	{
			if (i == gridSize)
				break;
			for (int j = 0;j < 5;j++)	{
				if (j == gridSize)
					break;
				if (lpm > divideMe)	{
					float	tmp = floor(remainder/divideMe);
					remainder = mod(remainder, divideMe);
					
					if (divideMe == pIndex)	{
						if (tmp != 0.0)	{
							returnMe = 1.0;
							break;
						}
					}
				}
				divideMe = divideMe / 2.0;
			}
			if (returnMe == 1.0)
				break;
		}
	}

	return returnMe;
}

void main() {
	vec4	returnMe = vec4(0.0);
	vec2	coord = gl_FragCoord.xy;
	float	gs = float(gridSize);
	if (PASSINDEX==0)	{
		returnMe = vec4(pattern(coord));
	}
	else	{
		vec2	modCoord = mod(isf_FragNormCoord*repeatCount,1.0);
		returnMe = IMG_NORM_PIXEL(lookupRender,modCoord);
		//returnMe = IMG_THIS_NORM_PIXEL(lookupRender);
	}
	gl_FragColor = returnMe;
}
