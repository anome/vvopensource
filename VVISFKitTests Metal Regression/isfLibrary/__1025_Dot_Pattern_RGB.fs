/*{
    "CATEGORIES": [
        "Pattern"
    ],
    "ISFVSN": "2",
    "CREDIT": "by VIDVOX",
    "INPUTS": [
        {
            "DEFAULT": 0.125,
            "MAX": 1,
            "MIN": 0,
            "NAME": "pixelScale",
            "TYPE": "float"
        },
		{
			"DEFAULT": 0,
			"LABELS": [
				"Over",
				"Add",
				"Combo"
			],
			"NAME": "compMode",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			]
		},
        {
            "DEFAULT": 0.115,
            "MAX": 1,
            "MIN": 0,
            "NAME": "patternModeR",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.470,
            "MAX": 1,
            "MIN": 0,
            "NAME": "patternModeG",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.280,
            "MAX": 1,
            "MIN": 0,
            "NAME": "patternModeB",
            "TYPE": "float"
        },
		{
			"DEFAULT": 4,
			"LABELS": [
				"2",
				"3",
				"4"
			],
			"NAME": "gridSizeR",
			"TYPE": "long",
			"VALUES": [
				2,
				3,
				4
			]
		},
		{
			"DEFAULT": 4,
			"LABELS": [
				"2",
				"3",
				"4"
			],
			"NAME": "gridSizeG",
			"TYPE": "long",
			"VALUES": [
				2,
				3,
				4
			]
		},
		{
			"DEFAULT": 4,
			"LABELS": [
				"2",
				"3",
				"4"
			],
			"NAME": "gridSizeB",
			"TYPE": "long",
			"VALUES": [
				2,
				3,
				4
			]
		}
    ]
}
*/

float pattern(vec2 c, float patternMode, float gs)	{
	float	returnMe = 0.0;
	vec2	modCoord = mod(floor(c),gs);

	float	pIndex = pow(2.0,modCoord.y*gs+modCoord.x);

	float	lpm = patternMode * (pow(2.0,gs*gs) - 1.0);
	float	remainder = (lpm);
	float	divideMe = pow(2.0,(gs*gs)-1.0);
	
	if (pIndex <= lpm)	{
		for (int i = 0;i < 5;i++)	{
			if (i == int(gs))
				break;
			for (int j = 0;j < 5;j++)	{
				if (j == int(gs))
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
	float	zl = (pixelScale == 0.0) ? 1.0 / max(RENDERSIZE.x,RENDERSIZE.y) : pixelScale;
	vec2	modCoord = (coord*zl);
	
	if (compMode == 0)	{
		returnMe.r = pattern(modCoord,patternModeR,float(gridSizeR));
		returnMe.g = (returnMe.r == 1.0) ? 0.0 : pattern(modCoord,patternModeG,float(gridSizeG));
		returnMe.b = ((returnMe.r == 1.0)||(returnMe.g == 1.0)) ? 0.0 : pattern(modCoord,patternModeB,float(gridSizeB));
	}
	else if (compMode == 1)	{
		returnMe.r = pattern(modCoord,patternModeR,float(gridSizeR));
		returnMe.g = pattern(modCoord,patternModeG,float(gridSizeG));
		returnMe.b = pattern(modCoord,patternModeB,float(gridSizeB));
	}
	else if (compMode == 2)	{
		returnMe.r = pattern(modCoord,patternModeR,float(gridSizeR));
		returnMe.g = pattern(modCoord,patternModeG,float(gridSizeG));
		returnMe.b = pattern(modCoord,patternModeB,float(gridSizeB));
		if ((returnMe.r == 0.0)||(returnMe.g == 0.0)||(returnMe.b == 0.0))	{
			returnMe.r = pattern(modCoord,patternModeR,float(gridSizeR));
			returnMe.g = (returnMe.r == 1.0) ? 0.0 : returnMe.g;
			returnMe.b = ((returnMe.r == 1.0)||(returnMe.g == 1.0)) ? 0.0 : returnMe.b;
		}
	}
	//	set the alpha based on those vals
	returnMe.a = max(returnMe.r,returnMe.g);
	returnMe.a = max(returnMe.a,returnMe.b);
	gl_FragColor = returnMe;
}