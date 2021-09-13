/*{
	"DESCRIPTION": "Tunnel visualizer generated with mix of polar sine and cosine funtions. Program fades through different polar patterns",
	"CREDIT": "Adapted from example code in the Book of Shaders chapter 7",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "polarLeaves",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
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

	
	vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    vec3 color = vec3(0.0);

    vec2 pos = vec2(0.5)-st;

    float r = length(pos)*2.5;
    float a = atan(pos.y,pos.x);

    
    float  f = abs(cos(a*12.*polarLeaves)*sin(a*3.*polarLeaves))*cos(TIME)*.8+.1;


    color = vec3( 1.-smoothstep(f,f+0.02*100.0,r) );

   gl_FragColor = vec4(color, 0.9);
}
