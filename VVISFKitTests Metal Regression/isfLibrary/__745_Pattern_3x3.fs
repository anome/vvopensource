/*{
    "CATEGORIES": [
        "Drawing"
    ],
    "CREDIT": "by VIDVOX",
    "INPUTS": [
        {
            "DEFAULT": 11,
            "MAX": 511,
            "MIN": 0,
            "NAME": "patternMode",
            "TYPE": "float"
        },
        {
            "DEFAULT": 5,
            "MAX": 10,
            "MIN": 1,
            "NAME": "zoomLevel",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/

float pattern(vec2 c)	{
	float	returnMe = 0.0;
	vec2	modCoord = mod(floor(c),3.0);
	
	//	when i=0 and j=0, pIndex should be 1
	//	when i=0 and j=1, pIndex should be 2
	//	when i=0 and j=1, pIndex should be 4
	//	when i=1 and j=0, pIndex should be 8
	float	pIndex = pow(2.0,modCoord.y*3.0+modCoord.x);
	//	for this given patternMode, is this pIndex on or off?
	float	pVal = 0.0;
	float	remainder = patternMode;
	float	divideMe = 256.0;
	
	if (pIndex <= patternMode)	{
		for (int i = 0;i < 3;i++)	{
			for (int j = 0;j < 3;j++)	{
				if (patternMode > divideMe)	{
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
	vec2 coord = gl_FragCoord.xy;
	gl_FragColor = vec4(pattern(coord/zoomLevel));
}
