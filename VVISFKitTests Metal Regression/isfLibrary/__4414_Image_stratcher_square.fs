
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
			"NAME": "distortK",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 2.0,
			"DEFAULT": 0.5
		},
		{
			"NAME": "debugOn",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "rayNumber",
			"TYPE": "float",
			"MIN": 1.0,
			"STEP": 1.0,
			"MAX": 10.0,
			"DEFAULT": 8.0
		},
		{
			"NAME": "dc",
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

#define distortK (1. * sin(TIME / 7.) + 1.5)
#define dc (vec2(.2 * sin(TIME / 3.), .2 * cos(TIME / 5.)) + 0.5)

#define PI 3.1415

float gain(float x, float k) 
{
    float a = 0.5*pow(2.0*((x<0.5)?x:1.0-x), k);
    return (x<0.5)?a:1.0-a;
}

float distort(float angle, float r) {
    //return gain(r, (sin(angle * rayNumber) * 0.5 + 0.5) * distortK);
    float amp = sin(angle * rayNumber) * 0.5 + 0.5;
    amp = mix(0., amp, r * 2.);
    return mix(r, pow(r, distortK), amp);
}

vec2 polarToDecart(vec2 polar) {
    float angle = polar.x;
    float R = polar.y;
    float x = sin(angle) * R;
    float y = cos(angle) * R;
    return vec2(x, y);
}

vec2 decartToPolar(vec2 decart) {
    float angle = atan(decart.x, decart.y);
    float R = length(decart);
    return vec2(angle, R);
}

float square(float angle, float size) {
    return min(size/abs(cos(angle)),size/abs(sin(angle)));
}

void main()	{
	vec4 inputPixelColor = vec4(0);
	//inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	vec2 p;
	p = isf_FragNormCoord.xy;


    p -= dc;
	p = decartToPolar(p);

    float debug = step(square(p.x, .5) - 0.005, p.y);
    debug -= step(square(p.x, 0.5) + 0.005, p.y);
    float distToSquare = square(p.x, .5);
    
    p.y = distort(p.x, p.y / distToSquare) * distToSquare;
    
	p = polarToDecart(p);

	if(debugOn)inputPixelColor = vec4(vec3(fract(length(p * 50.))), 1.);
    p += dc;
    inputPixelColor += IMG_NORM_PIXEL(inputImage, p);

	gl_FragColor = inputPixelColor;
}
