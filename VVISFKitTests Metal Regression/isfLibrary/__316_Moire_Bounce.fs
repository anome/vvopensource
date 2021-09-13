/*{
	"CREDIT": "by echophons",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "shape",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 3.0,
			"DEFAULT": 0.05
		},
				{
			"NAME": "spacing",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 100.0,
			"DEFAULT": 10.0
		},
		{
			"NAME": "bounce",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 40.0,
			"DEFAULT": 5.0
		}
	]
}*/

vec3   iResolution = vec3(RENDERSIZE, 1.0);
float  iGlobalTime = TIME;

void main(){	
	vec2 uv = gl_FragCoord.xy / iResolution.xy;
	float distance = sqrt( pow(abs(uv[0]-0.5), 0.5) + pow(abs(uv[1]-0.5), abs(sin(iGlobalTime/bounce)*shape)) );
	float color = sin(distance*iGlobalTime*spacing) * cos(distance*iGlobalTime*0.5);
	gl_FragColor = vec4(color, color, color,1.0);
}