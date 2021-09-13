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
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "xValue",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "yValue",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
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

#define fov 20.
#define RES 10.
#define LAYERS 8.

vec2 hash( vec2 p ) {                       // rand in [-1,1]
    p = vec2( dot(p,vec2(127.1,311.7)),
              dot(p,vec2(269.5,183.3)) );
    return -1. + 2.*fract(sin(p+20.)*53758.5453123);
}

float noise( vec2 p ) {
    vec2 i = floor((p)), f = fract((p));
    vec2 u = f*f*(3.-2.*f);
    return mix( mix( dot( hash( i + vec2(0.,0.) ), f - vec2(0.,0.) ), 
                     dot( hash( i + vec2(1.,0.) ), f - vec2(1.,0.) ), u.x),
                mix( dot( hash( i + vec2(0.,1.) ), f - vec2(0.,1.) ), 
                     dot( hash( i + vec2(1.,1.) ), f - vec2(1.,1.) ), u.x), u.y);
}

vec4 dust(vec2 p, vec4 image) {
    p = p / RENDERSIZE.y - .5;  
    float d, s;
    float t = TIME * 0.1;
    
    
    for (float i = 1.; i < LAYERS; i++) {
        float size = LAYERS - (i / LAYERS);
    	vec2 u = p * tan (radians (i * 4. + fov) / 2.0);
        u.x -= (xValue / 100. * size) + t/100. - i/4.;
        u.y += (yValue / 100. * size) + sin( i*2. - t/4. + u.x ) * .1;
        s = .012 * noise(ceil(u*RES + i*1.) + i + t * vec2(.2, .01));
        u = mod(u, 1./RES);
        d = length(u - .5/RES);
        image += smoothstep(0., d, 0.0005);

    }
    return image;
    
}
void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	vec4 result = dust(gl_FragCoord.xy, inputPixelColor);
	gl_FragColor = result;
}
