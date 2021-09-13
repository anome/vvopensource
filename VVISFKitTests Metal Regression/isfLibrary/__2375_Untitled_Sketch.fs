/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
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
	]
}*/

void main() {
	gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	
	
	vec2 st = isf_FragNormCoord;
    float pct = 0.0;
    float asp =  RENDERSIZE.x/RENDERSIZE.y;


    vec2 toCenter = st-vec2(0.5, 0.5);
    toCenter.x*=asp;
    pct = length(toCenter);
    float d  = smoothstep(pct*0.99,pct,floatInput);


    vec3 color = vec3(d*pct);

	gl_FragColor = vec4( color, 1.0 );
	
}