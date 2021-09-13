
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
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

#define PI 3.1415

float distort(float x, float param) {
    return tan((x - 0.5)*PI) / 10. + param;
    //return tan((pow(x, param) - 0.5)*PI) / 10. + 0.5;
    //return smoothstep(0., 1., x);
}

vec2 polarToDecart(vec2 polar) {
    float alpha = polar.x;
    float R = polar.y;
    float x = sin(alpha) * R;
    float y = cos(alpha) * R;
    return vec2(x, y);
}

vec2 decartToPolar(vec2 decart) {
    float alpha = atan(decart.x, decart.y);
    float R = length(decart);
    return vec2(alpha, R);
}

void main()	{
	vec4		inputPixelColor;
	//inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	vec2 p = isf_FragNormCoord.xy;
	
    float debug = step(distort(p.x, pointInput.x) - 0.005, p.y);
    debug -= step(distort(p.x, pointInput.x) + 0.005, p.y);

    p -= pointInput;
	p = decartToPolar(p);
	//p.x = distort(p.x, pointInput.x);
	p.y += (0.5 + 0.5 * sin(p.x * 5.))  * p.y;
	//p.x += TIME;
	p = polarToDecart(p);
    p += pointInput;
    inputPixelColor = IMG_NORM_PIXEL(inputImage, p);

    //inputPixelColor += debug;

	gl_FragColor = inputPixelColor;
}
