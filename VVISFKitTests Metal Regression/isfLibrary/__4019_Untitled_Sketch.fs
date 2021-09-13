/*{
	"CREDIT": "by cyshahacys",
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


#ifdef GL_ES
precision mediump float;
#endif
 
#define 	pi   	3.141592653589793 	// pi

void main() {
	float T = TIME * rate1;
	float TT = TIME * rate2;
	vec2 p=(2.*isf_FragNormCoord);
	for(int i=1;i<11;i++) {
    	vec2 newp=p;
		float ii = float(i);  
    	newp.x+=depthX/ii*sin(ii*pi*p.y+T*nudge+cos((TT/(5.0*ii))*ii));
    	newp.y+=depthY/ii*cos(ii*pi*p.x+TT+nudge+sin((T/(5.0*ii))*ii));
    	p=newp+log(DATE.w)/loopcycle;
  }
  vec3 col=vec3(cos(p.x+p.y+3.0*color1)*0.5+0.5,sin(p.x+p.y+6.0*cycle1)*0.5+0.5,(sin(p.x+p.y+9.0*color2)+cos(p.x+p.y+12.0*cycle2))*0.25+.5);
  gl_FragColor=vec4(col*col, 1.0);
}