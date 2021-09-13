/*{
	"CREDIT": "by You",
	"DESCRIPTION": "Filter",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "exponent",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 12.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "exponent",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 12.0,
			"DEFAULT": 0.0
		},
		{
			"NAME": "strength",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 12.0,
			"DEFAULT": 0.0
		}
	]
}
*/

void main() {
	vec4 center = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	vec4 color = vec4(0.0);
	float total = 0.0;
	for (float x = -4.0; x <= 4.0; x += 1.0) {
		for (float y = -4.0; y <= 4.0; y += 1.0) {
			vec4 sample = texture2D(tInput, vUv + vec2(x, y) / 1);
			float weight = 1.0 - abs(dot(sample.rgb - center.rgb, vec3(0.25)));
			weight = pow(weight, exponent);
			color += sample * weight;
			total += weight;
		}
	}
	gl_FragColor = color / total;
}

