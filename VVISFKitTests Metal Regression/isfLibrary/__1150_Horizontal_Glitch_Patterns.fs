/*{
    "CATEGORIES": [
        "Color",
        "Pattern"
    ],
    "CREDIT": "",
    "INPUTS": [
        {
            "DEFAULT": 0.05,
            "MAX": 1,
            "MIN": 0,
            "NAME": "height",
            "TYPE": "float"
        },
        {
            "DEFAULT": 4,
            "MAX": 16,
            "MIN": 1,
            "NAME": "subSection",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 4,
            "MIN": 1,
            "NAME": "mainFrequency",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1,
            "MAX": 400,
            "MIN": 1,
            "NAME": "subMaxFrequency",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                0.5,
                0
            ],
            "MAX": [
                1,
                1
            ],
            "MIN": [
                0,
                0
            ],
            "NAME": "offset",
            "TYPE": "point2D"
        },
        {
            "DEFAULT": [
                0,
                0,
                0,
                1
            ],
            "NAME": "color1",
            "TYPE": "color"
        },
        {
            "DEFAULT": [
                1,
                1,
                1,
                1
            ],
            "NAME": "color2",
            "TYPE": "color"
        },
        {
            "DEFAULT": [
                1,
                0.9,
                0.9,
                1
            ],
            "NAME": "color3",
            "TYPE": "color"
        },
        {
            "DEFAULT": 0.83,
            "MAX": 1,
            "MIN": 0,
            "NAME": "rowSeed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.73,
            "MAX": 1,
            "MIN": 0,
            "NAME": "offsetSeed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.1,
            "MAX": 2,
            "MIN": 0,
            "NAME": "offsetRandAmount",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2"
}
*/

const float pi = 3.14159265359;

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float func(float x, float r, float rs)	{
	float	randRow = 10.0*rand(vec2(r,rs));
	float	randRow2 = subMaxFrequency*randRow;
	float	randOffset = (rand(vec2(x+isf_FragNormCoord.y,offsetSeed*r))-0.5)*offsetRandAmount*2.0;
	float	returnMe = (1.0+sin(randOffset+(offset.x+x)*pi*mainFrequency))/6.0 + (1.0+sin(randOffset+(offset.x+x)*pi*randRow))/6.0  + (1.0+sin(randOffset+(offset.x+x)*randRow2*pi))/6.0;
	return returnMe;
}

void main() {

	float		h = (height == 0.0) ? 1.0/RENDERSIZE.y : height;
	vec4		out_color = color2;
	float		rowIndex = 2.0+floor(mod(offset.y+isf_FragNormCoord.y,1.0)/h);
	float		val = func(isf_FragNormCoord.x, rowIndex, rowSeed);
	out_color = mix(color1,color2,val)/2.0;
	rowIndex = 2.0+floor(mod(offset.y+isf_FragNormCoord.y,1.0)/(h/floor(subSection)));
	val = func(isf_FragNormCoord.x, rowIndex,rowSeed);
	out_color += mix(out_color,color3,val)/2.0;
	gl_FragColor = out_color;
}