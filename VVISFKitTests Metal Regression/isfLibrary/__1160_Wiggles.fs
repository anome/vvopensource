
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "wiggle1",
			"TYPE": "float",
			"DEFAULT": 34.0,
			"MIN": 0.0,
			"MAX": 180.0
		},
		{
			"NAME": "wiggle2",
			"TYPE": "float",
			"DEFAULT": 15.0,
			"MIN": 0.0,
			"MAX": 180.0
		}
	
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/



void main()	{
    vec2 coord = (gl_FragCoord.xy/RENDERSIZE);
    float color = 0.0;
    // for(int n = 1; n < 8; n++){
    //     float i = float(n);
       
    //     color+=cos(coord.x*6.0+cos(TIME+coord.y*90.0+sin(coord.x*30.0+TIME*2.0)))*0.5;
    // }
    color+=sin(coord.x*6.0+sin(TIME+coord.y*wiggle1+cos(coord.x*30.0+TIME*2.0)))*0.5;
    color+=cos(coord.x*6.0+cos(TIME+coord.y*wiggle2+sin(coord.x*30.0+TIME*2.0)))*0.5;
	
	gl_FragColor = vec4(vec3(color+coord.x,color+coord.x,color+coord.x),1.0);
}
