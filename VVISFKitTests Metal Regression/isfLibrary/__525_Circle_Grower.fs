
/*{
	"DESCRIPTION": "Growing Circles",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"vjs-generator"
	],
	"INPUTS": [
		{
			"NAME": "color",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
		    "NAME": "speed",
		    "TYPE": "float",
		    "MIN": -1,
		    "MAX": 1,
		    "DEFAULT": 1
		},
	   {
		    "NAME": "size",
		    "TYPE": "float",
		    "MIN": 0,
		    "MAX": 1,
		    "DEFAULT": 0.3
		}
	]
	
}*/

void main()	{
	float numCircles = size * 10.0;
	float cSize = 1.0 / numCircles;
	vec2 pos = mod(isf_FragNormCoord.xy * 2.0 - 1.0, vec2(cSize)) * numCircles - vec2(cSize * numCircles / 2.0);
	float dist = sqrt(dot(pos, pos));
	dist = dist * numCircles + TIME * speed;
	
	gl_FragColor = sin(dist * 2.0) > 0.0 ? color : vec4(0.0);
}
