
/*{
	"DESCRIPTION": "Tile Ascention",
	"CREDIT": "by Ivan Verdugo",
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

#define pi 3.14159235659

void main()	{
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
    uv.x *= RENDERSIZE.x/RENDERSIZE.y;
   //float forma = sin(uv.y*10*pi+time)*0.5+0.5;
   //forma += sin(uv.x*10*pi-time)*0.8+0.1;
   
   float forma = sin(uv.x*10.0*pi-TIME+sin(uv.y*20.0))*0.8+0.1;
     forma += sin(uv.y*10.0*pi-TIME)*0.8+0.1;

   
   
   
   float r =  forma;
   float g =  forma;
   float b =  forma;
   
	
	gl_FragColor = vec4(r,g,b,1.0);
}
