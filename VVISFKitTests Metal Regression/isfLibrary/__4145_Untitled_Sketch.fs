/*{
	"CREDIT": "by clementciuro",
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
}


uniform sampler2D tex0;
const float PI = 3.14159265358979323846264;

void main(void) {
	vec2 texCoord = gl_TexCoord[0].st;
	
	vec2 texOff = gl_Color.ba;
	
	//texCoord = (texCoord-texOff)/texSize;
	
	//polar 2 rect filter
	vec2 norm = texCoord * 2.0 - 1.0;
	float theta = PI + norm.x * PI;
	float r = (1.0 + norm.y) * 0.5 ;
	vec2 cart = vec2(-r * sin(theta), -r * cos(theta));
	cart = ( (cart/2.0) + 0.5 );
	
	//sample from polarized coords
	vec4 color = texture2D(tex0, cart);
	
	//if under 30% transparency, we'll use "texCoord.t" as our distance
	//otherwise use white (1.0) as a max value
	float distColor = color.a>0.3f ? texCoord.t : 1.0f;
	
	//vec4 c = texture2D(tex0, texCoord*texSize+texOff);
	//gl_FragColor = c; *texSize+texOff
	
	gl_FragColor = vec4(distColor, distColor, distColor, .4);
} 

