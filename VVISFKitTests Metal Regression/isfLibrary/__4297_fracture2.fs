
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

float getAlpha(vec2 u0, vec2 u1, vec2 u2) {
    vec2 u21 = u2-u1;
    float num = u0.x*u21.y-u0.y*u21.x;
    float denum = u1.x*u21.y - u1.y*u21.x;
    return num / denum;
}

void iteration(inout vec2 pos, inout vec4 colmod, vec2 center, float theta) {
    float ang = 2.*pi/3.;
    
    vec2 u1 = vec2(cos(theta + 0.*ang), sin(theta + 0.*ang));
    vec2 u2 = vec2(cos(theta + 1.*ang), sin(theta + 1.*ang));
    vec2 u3 = vec2(cos(theta + 2.*ang), sin(theta + 2.*ang));
    
    vec2 c1 = (u1+u2) / length(u1+u2);
    vec2 c2 = (u2+u3) / length(u1+u2);
    vec2 c3 = (u3+u1) / length(u1+u2);
    
    float pos_ang = atan(pos.y-center.y, pos.x - center.x) + pi; // 0 2*pi
    
    float s1 = 0.5;
    float s2 = 0.5;
    float s3 = 0.5;
    if (pos_ang < theta) { // sector 3
        pos = center + s3*(pos-center+c3);
    } else if (pos_ang < (theta + ang)) { // sector 1
        pos = center + s1*(pos-center+c1);
    } else if (pos_ang < (theta + 2.*ang)) { // sector 2
        pos = center + s2*(pos-center+c2);
    } else { // sector 3 again!
        pos = center + s3*(pos-center+c3);
    }
    
}

void main()	{
	vec4 inputPixelColor;
	vec2 pos = isf_FragNormCoord.xy;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
	
	vec2 center = vec2(0.5, 0.5);
	vec4 colmod = vec4(1., 1., 1., 1.);
	
	float theta = 1.;
	iteration(pos, colmod, center, theta);
	
	gl_FragColor = IMG_NORM_PIXEL(inputImage, pos);
}
