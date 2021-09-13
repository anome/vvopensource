
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

#define rot(a) mat2(cos(a),sin(a),-sin(a),cos(a))

vec3 render(vec2 p) {
    p*=rot(iTime*.1)*(.0002+.7*pow(smoothstep(0.,.5,abs(.5-fract(iTime*.01))),3.));
    p.y-=.2266;
    p.x+=.2082;
    vec2 ot=vec2(100.);
    float m=100.;
    for (int i=0; i<150; i++) {
        vec2 cp=vec2(p.x,-p.y);
		p=p+cp/dot(p,p)-vec2(0.,.25);
        p*=.1;
        p*=rot(1.5);
        ot=min(ot,abs(p)+.15*fract(max(abs(p.x),abs(p.y))*.25+iTime*.1+float(i)*.15));
        m=min(m,abs(length(p.y)));
    }
    ot=exp(-200.*ot)*2.;
    m=exp(-200.*m);
    return vec3(ot.x,ot.y*.5+ot.x*.3,ot.y)+m*.2;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord-iResolution.xy*.5)/iResolution.y;
    vec2 d=vec2(0.,.5)/iResolution.xy;
    vec3 col = render(uv)+render(uv+d.xy)+render(uv-d.xy)+render(uv+d.yx)+render(uv-d.yx);
    fragColor = vec4(col*.2,1.0);
}
