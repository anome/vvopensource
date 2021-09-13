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
			"NAME": "sample",
			"TYPE": "float",
			"DEFAULT": 66.5,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 10.5,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "amplitude",
			"TYPE": "float",
			"DEFAULT": 70.5,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "thick",
			"TYPE": "float",
			"DEFAULT": 4.1,
			"MIN": 0.1,
			"MAX": 10.0
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



float Hash( vec2 p)
{
     vec3 p2 = vec3(p.xy,1.0);
    return fract(sin(dot(p2,vec3(7.1,31.7, 2.4)))*312327.5453123);
}

float noise(in vec2 p)
{
    vec2 i = floor(p);
     vec2 f = fract(p);
	
     f *= f*(3.0-2.0*f);
	
    return mix(mix(Hash(i + vec2(0.,0.)), Hash(i + vec2(1.,0.)),f.x),
               mix(Hash(i + vec2(0.,1.)), Hash(i + vec2(1.,1.)),f.x),
               f.y);
}

float fbm(vec2 p)
{
     float v = 0.0;
     v += noise(p*1.0) * .1;
     v += noise(p*2.)  * .1;
     v += noise(p*3.)  * .1;
     v += noise(p*4.)  * .1;
     return v;
}

void main( void ) 
{

	vec2 uv = vv_FragNormCoord.xy * 2.0 - 1.0;
	uv.x *= RENDERSIZE.x/RENDERSIZE.y; 
	uv.y -= 0.50;

	float timeVal = TIME/speed;

	vec3 finalColor = vec3( 0.0 );
	float indexAsFloat;
	float amp;
	float period;
	float thickness;
	float t;
	for( int i=0; i < 10; ++i )
	{
		 indexAsFloat = float(i);
		 amp = amplitude + (indexAsFloat*sample);
		 period = 2.0 + (indexAsFloat*2.0);
		 thickness = indexAsFloat/thick;
		 t = abs( 0.4 / (sin(uv.y + fbm( uv + timeVal * period )) * amp) * thickness );
	
		finalColor +=  t * vec3( 1.3, 0.5, .5 );
	}
	
	gl_FragColor = vec4( finalColor, 1.0 );

}