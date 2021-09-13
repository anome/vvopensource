
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

float sat( float t ) {
	return clamp( t, 0.0, 1.0 );
}

vec2 sat( vec2 t ) {
	return clamp( t, 0.0, 1.0 );
}

//remaps inteval [a;b] to [0;1]
float remap  ( float t, float a, float b ) {
	return sat( (t - a) / (b - a) );
}

//note: /\ t=[0;0.5;1], y=[0;1;0]
float linterp( float t ) {
	return sat( 1.0 - abs( 2.0*t - 1.0 ) );
}

vec3 spectrum_offset( float t ) {
	vec3 ret;
	float lo = step(t,0.5);
	float hi = 1.0-lo;
	float w = linterp( remap( t, 1.0/6.0, 5.0/6.0 ) );
	float neg_w = 1.0-w;
	ret = vec3(lo,1.0,hi) * vec3(neg_w, w, neg_w);
	return pow( ret, vec3(1.0/2.2) );
}

//note: [0;1]
float rand( vec2 n ) {
  return fract(sin(dot(n.xy, vec2(12.9898, 78.233)))* 43758.5453);
}

//note: [-1;1]
float srand( vec2 n ) {
	return rand(n) * 2.0 - 1.0;
}

float mytrunc( float x, float num_levels )
{
	return floor(x*num_levels) / num_levels;
}
vec2 mytrunc( vec2 x, float num_levels )
{
	return floor(x*num_levels) / num_levels;
}

/*

void main()	{
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	vec2 uv = isf_FragNormCoord.xy;
    uv.y = uv.y;
	
	float theTime = mod(TIME*100.0, 32.0)/110.0; // + modelmat[0].x + modelmat[0].z;

	float GLITCH = floatInput * 0.07;
	
	float gnm = sat( GLITCH );

	float pxrnd = rand( uv + theTime );

	float ofs = 0.1 * GLITCH;
	ofs += 0.5 * pxrnd * ofs;

    const int NUM_SAMPLES = 20;
    const float RCP_NUM_SAMPLES_F = 1.0 / float(NUM_SAMPLES);
    
	vec4 sum = vec4(0.0);
	vec3 wsum = vec3(0.0);
	for( int i=0; i<NUM_SAMPLES; ++i )
	{
                float dir = mod(float(i), 2.0) == 1.0 ? 0.9 : -1.4;

		float t = float(i) * RCP_NUM_SAMPLES_F;
		uv.x = sat( uv.x + ofs * t);
		vec4 samplecol = IMG_NORM_PIXEL(inputImage, uv);
		vec3 s = spectrum_offset( t );
		samplecol.rgb = samplecol.rgb * s;
		sum += samplecol;
		wsum += s;
	}
	sum.rgb /= wsum;
	sum.a *= RCP_NUM_SAMPLES_F;


	gl_FragColor = sum; // * outcol0.a;
}
*/