/*{
	"CREDIT": "by INKA",
	"CATEGORIES": [
		"Color Effect",
		"INKA"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "color_chord",
			"TYPE": "long",
			"MIN": 0,
			"MAX": 5,
			"VALUES": [
				0,
				1,
				2,
				3,
				4
			],
			"LABELS": [
				"no filter",
				"esmerald",
				"marocco",
				"latte", 
				"peach"
			],
			"DEFAULT": 2
		}
	]
}*/


//	adapted from vidvox's thermal vision effect

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

vec4 color_gradient(vec4 colors[5]) {
	vec4 color = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	vec4 colorL = IMG_NORM_PIXEL(inputImage, left_coord);
	vec4 colorR = IMG_NORM_PIXEL(inputImage, right_coord);
	vec4 colorA = IMG_NORM_PIXEL(inputImage, above_coord);
	vec4 colorB = IMG_NORM_PIXEL(inputImage, below_coord);

	vec4 colorLA = IMG_NORM_PIXEL(inputImage, lefta_coord);
	vec4 colorRA = IMG_NORM_PIXEL(inputImage, righta_coord);
	vec4 colorLB = IMG_NORM_PIXEL(inputImage, leftb_coord);
	vec4 colorRB = IMG_NORM_PIXEL(inputImage, rightb_coord);

	vec4 avg = (color + colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB) / 9.0;
	
	float lum = dot(vec3(0.30, 0.59, 0.11), avg.rgb);

	lum = pow(lum,1.4);

	int ix = 0;
	float range = 1.0 / 5.0;
	
	vec4 startColor;
	vec4 endColor;

	if (lum > range * 3.0)	{
		startColor = colors[3];
		endColor = colors[4];
		ix = 3;
	}
	else if (lum > range * 2.0)	{
		startColor = colors[2];
		endColor = colors[3];
		ix = 2;
	}
	else if (lum > range)	{
		startColor = colors[1];
		endColor = colors[2];
		ix = 1;
	}
	else {
		startColor = colors[0];
		endColor = colors[1];
	}

	vec4 result = mix(startColor, endColor, (lum-float(ix)*range)/range);

	return result;
}

void main ()	{
	vec4 colors[5];
	vec4 color = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	if(color_chord == 1) {
		colors[0] = vec4(0., 0., .10, 1.);
		colors[1] = vec4(0., .36, .294,  1.);
		colors[2] = vec4(.631, .082, 0.,  1.);
		colors[3] = vec4(.843, .85, 0.,  1.);
		colors[4] = vec4(.827, .827, .827,  1.);
	} else if (color_chord == 2) {
		colors[0] = vec4(.082, .05, .180, 1.);
		colors[1] = vec4(.635, .396, 0., 1.);
		colors[2] = vec4(.254, 0., 0., 1.);
		colors[3] = vec4(.116, .286, .349, 1.);
		colors[4] = vec4(.937, .756, .384, 1.);
	} else if (color_chord == 3) {
		colors[0] = vec4(0.56, 0.16, 0.00, 1.);
		colors[1] = vec4(1.00, 0.94, 0.65, 1.);
		colors[2] = vec4(0.27, 0.54, 0.40, 1.);
		colors[3] = vec4(0.71, 0.29, 0.15, 1.);
		colors[4] = vec4(1.00, 0.69, 0.23, 1.);
	} else if (color_chord == 4) {
		colors[0] = vec4(1.00, 0.24, 0.50, 1.);
		colors[3] = vec4(0.25, 0.72, 0.69, 1.);
		colors[2] = vec4(0.50, 0.78, 0.69, 1.);
		colors[1] = vec4(1.00, 0.62, 0.62, 1.);
		colors[4] = vec4(0.85, 0.85, 0.65, 1.);
	}
	if(color_chord > 0) {
		color = color_gradient(colors);	
	}
	
	gl_FragColor = color;

}