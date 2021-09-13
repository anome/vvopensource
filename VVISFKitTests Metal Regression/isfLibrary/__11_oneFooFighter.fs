
/*{
	"DESCRIPTION": "lightmoving3 (variation from lewis lepton tutorials)",
	"CREDIT": "uaralab",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
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
    vec2 coord = 10.0*(gl_FragCoord.xy*2.0-RENDERSIZE)/min(RENDERSIZE.x,RENDERSIZE.y);
    coord.x+=sin(TIME)+cos(TIME*1.0);
    coord.y+=cos(TIME)+sin(TIME*0.6);
    
    float color = 0.0;
    
    color += 0.1 * (abs(sin(TIME)) + 0.1)/ length(coord);
    
    gl_FragColor = vec4(vec3(color),1.0);
}
