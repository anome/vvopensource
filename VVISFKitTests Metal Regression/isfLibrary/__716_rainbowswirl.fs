
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "swirlyness",
			"TYPE": "float",
			"DEFAULT": 40.0,
			"MIN": 0.0,
			"MAX": 1000.0
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
	vec2 coord = (gl_FragCoord.xy/RENDERSIZE.xy);
    vec3 color = vec3(0.0);
    
    float angle = atan(-coord.y+0.25, coord.x-0.5)*0.1;
	float len = length(coord - vec2(0.5,0.25));
	color.r+=sin(len*40.0+angle*swirlyness+40.0+TIME);
	color.g+=cos(len*30.0+angle*swirlyness+60.0-TIME);
	color.b+=tan(len*50.0+angle*swirlyness+50.0-TIME*2.0);
	gl_FragColor = vec4(color,1.0);
}
