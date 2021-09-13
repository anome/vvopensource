/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator",
		"patterns",
		"morie"
	],
	"INPUTS": [
		{
			"NAME" : 		"S",
			"TYPE" : 		"float",
			"DEFAULT" : 	345.0,
			"MIN" : 		1.0,
			"MAX" : 		1000.0
		},
				{
			"NAME" : 		"O",
			"TYPE" : 		"float",
			"DEFAULT" : 	432.0,
			"MIN" : 		10.0,
			"MAX" : 		10000.0
		},
		{
			"NAME" : 		"R",
			"TYPE" : 		"float",
			"DEFAULT" : 	0.5,
			"MIN" : 		0.1,
			"MAX" : 		5.0
		},
		{
			"NAME" :	 	"XY",
			"TYPE"	:		"point2D",
			"DEFAULT" :		[ 0.01, 0.1 ],
			"MAX" : 		[ 1.0, 1.0 ],
     		"MIN" : 		[ 0.0, 0.0 ]
		},
		{
			"NAME" :	 	"ZW",
			"TYPE"	:		"point2D",
			"DEFAULT" :		[ 0.5, 0.05 ],
			"MAX" : 		[ 0.99, 0.99 ],
     		"MIN" : 		[ 0.01, 0.01 ]
		},
		{
			"NAME" :	 	"M",
			"TYPE" : 		"float",
			"DEFAULT" : 	1.23,
			"MIN" : 		0.33,
			"MAX" : 		1.5
		}
	]
}*/

////////////////////////////////////////////////////////////////////
// MorieExplorer  by mojovideotech
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

void main() 
{
	float t = O+sin(TIME*R)*S;
    float d = distance(gl_FragCoord.xy/RENDERSIZE.y,RENDERSIZE.xy*XY.xy);
    t *= sin(gl_FragCoord.y*ZW.x)*ZW.y;
    t *= -cos(gl_FragCoord.x*ZW.x)*ZW.y;
    gl_FragColor = vec4(floor(mod(d - t, M)*3.0));
}