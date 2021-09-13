/*{
	"CREDIT": "by kawaiiidesu",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},{
			"NAME": "size",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.25,
			"MAX": 3.0
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 1.5,
			"MIN": 0.25,
			"MAX": 3.0
		},
		{
			"NAME": "mixer",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "offset",
			"TYPE": "point2D",
        "DEFAULT": [
		1.0,
		1.75
	  ],
      "MAX" : [
        2.0,
        2.0
      ],
      "MIN" : [
        -2.0,
        -2.0
      ]
    }
	]
}*/
// TwoPointPerspectiveZoom by mojovideotech
// effect modifications by BlckBox

// TwoPointPerspectiveZoom by mojovideotech

void main() {
	vec2 R = RENDERSIZE.xy; 
	vec2 I = gl_FragCoord.xy;
    float a = abs(I=(I+I-R)/-R.y).y*offset.y;
    vec2 S = vec2(dot(R,I));
    float b = abs(S=(I+I-R)/-R.x).x*offset.x;
	vec2 RA = sin(6.0*vec2(I.x/a,2.0/a+(rate*TIME))*(3.25-size));
	//поменял R на RA
	gl_FragColor -= -sign((RA.x*R.y) - pow(R.x*S.y,mix(a*a,b*b,mixer))*b)*a;
//	gl_FragColor -= exp(R.x*S.y)*a*b;
}