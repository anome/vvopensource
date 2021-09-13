/*{
	"CREDIT": "by isakburstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "noiseImage",
			"TYPE": "image"
		}, {
			"NAME": "noiseType",
			"TYPE": "float"
		}
	]
}*/
float quadOffset(float coord, float offset) {
	return coord * .5 + mod(floor(offset + noiseType * 2.), 2.) * .5;
}
void main() {
	vec2 uv = isf_FragNormCoord.xy;
	uv = vec2(quadOffset(uv.x, 0.), quadOffset(uv.y, 0.5));
	vec4 noise = IMG_NORM_PIXEL(noiseImage, uv);
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy) + noise;
}