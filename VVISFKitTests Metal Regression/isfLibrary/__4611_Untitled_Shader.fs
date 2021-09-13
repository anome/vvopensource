
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
			"NAME": "amountPixelsVisible",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "amountPixelsBlack",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
            "NAME": "stretchImage",
            "TYPE": "bool",
            "DEFAULT": 0
        },
		{
            "NAME": "addNoiseColors",
            "TYPE": "bool",
            "DEFAULT": 1
        },
        {
			"NAME": "noiseAmplitude",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "noiseFrequency",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
        {
            "NAME": "noiseColorA",
            "TYPE": "color",
            "DEFAULT": [0.7,0.4,0.6,1]
        },
        {
            "NAME": "noiseColorB",
            "TYPE": "color",
            "DEFAULT": [0.9,0.7,0.8,1]
        },
        {
			"NAME": "noiseOffsetX",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "noiseOffsetY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "noiseOffsetZ",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
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

float rand(vec3 n) { return fract(sin(dot(n, vec3(12.9898, 4.1414,548.6432))) * 43758.5453); }

float noise(vec3 p)
{
	vec3 ip = floor(p);
	vec3 u = fract(p);
	u = u*u*(3.0-2.0*u);
	float res = mix(
	    mix(rand(ip),rand(ip+vec2(1.0,0.0)),u.x),
	    mix(rand(ip+vec2(0.0,1.0)),rand(ip+vec2(1.0,1.0)),u.x),
	    u.y
    );
	return res*res;
}

void main()	{
	//	both of these are the same
	vec2 xy = vec2(gl_FragCoord.xy);
	vec2 res = IMG_SIZE(inputImage);
    vec2 uv = isf_FragNormCoord.xy;

	float pixels = amountPixelsVisible + amountPixelsBlack;
	xy.x -= res.x/2.;
	float curLines = xy.x / pixels;
	float off = fract(xy.x / pixels)*pixels;
    if (!stretchImage) xy.x += curLines;
	xy.x += res.x/2.;
	vec4 inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, xy);
	inputPixelColor *= step(amountPixelsBlack,off);

    if (addNoiseColors) {
        float n = noise(uv*noiseFrequency)*noiseAmplitude;
        vec4 noiseColor = mix(noiseColorA, noiseColorB, clamp(n,0.0,1.0));
        inputPixelColor *= noiseColor;
    }
    
	
	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	gl_FragColor = inputPixelColor;
}
