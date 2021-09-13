
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
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
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
const float pi = 3.14159265359;

void kaleidoscope(inout vec2 pos, float N, vec2 center, float angle) {
    float ang = atan(pos.y-center.y, pos.x-center.x) + angle;
    float i = floor(N * ang / pi);
    float f = fract(N * ang / pi);
    
    float saw = (pi/N) * 1. * abs(f - 0.5);
    float c = cos(saw);
    float s = sin(saw);
    float r = length(pos-center);
    
    pos = vec2(r*c,r*s) + center;
}

void main()	{
	vec4 inputPixelColor;
	vec2 pos = isf_FragNormCoord.xy;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
	
	float angle3 = TIME * 2.0;
	float d = dot(vec2(cos(angle3),sin(angle3)), pos - vec2(0.5));
	float v = exp(-pow(mod(d+TIME,1.),2.) / 0.001);
	
	float N = 6.;
	float angle = TIME / 10.;
	vec2 center = vec2(0.5, 0.5);
	kaleidoscope(pos, N, center, angle);
	
	float angle2 = TIME * 1.;
	vec2 p = 0.25 * vec2(cos(angle2),sin(angle2)) + center;
	pos = 1.0 * (pos-vec2(0.5))+p;
	pos = mod(abs(pos),1.);
	
	vec4 color = IMG_NORM_PIXEL(inputImage, pos);
	//color += vec4(v,0.,0.,0.);
	gl_FragColor = color;
}
