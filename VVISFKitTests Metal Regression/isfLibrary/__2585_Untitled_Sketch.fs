/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		}
	]
}*/

const float kNumCols = 3.0;

void main() {
	vec2 pos = isf_FragNormCoord.xy;
	pos = mod(pos * kNumCols, 1.0);
	
	float val = pos;
	gl_FragColor = vec4(pos, pos, pos, 1.0);
}