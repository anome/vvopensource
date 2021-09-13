
/*{
	"DESCRIPTION": "watercolors",
	"CREDIT": "uará lab",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 23.4,
			"MIN": 0.0,
			"MAX": 1000.0
		},
		{
			"NAME": "moveXY",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		},
		{
			"NAME": "timeMultX",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
				{
			"NAME": "timeMultY",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "rDisplacement",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "gDisplacement",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
				{
			"NAME": "bDisplacement",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 2.0
		}
	]
	
}*/

void main()	{
vec2 move = moveXY.xy * vec2(zoom*2.0,zoom*2.0);
vec2 coord = zoom*gl_FragCoord.xy/RENDERSIZE.xy+move.xy;


    float len;
     
    for(int i = 0; i <20; i++){
        len = length(vec2(coord.x, coord.y));
        coord.x = coord.x - cos(coord.y+sin(len))+ cos(TIME/timeMultX);
        coord.y = coord.y + sin(coord.x+cos(len))+ sin(TIME/timeMultY);

    }

	
 gl_FragColor = vec4(sin(len*rDisplacement),sin(len*gDisplacement),sin(len*bDisplacement),1.0);
}
