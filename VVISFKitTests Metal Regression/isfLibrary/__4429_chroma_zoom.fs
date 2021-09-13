
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
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.45,
				0.7
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/4.0",
			"HEIGHT": "$HEIGHT/4.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

#define ZOOM_ITER 80.0
#define ZOOM_CENTER pointInput
#define ZOOM_AMP 0.15
#define ZOOM_OUTSIDE_AMP 0.3
#define ZOOM_OUTSIDE_OFFSET 0.3
#define CHROMA_MIX 0.3

#define PI 3.14159265
#define saturate(i) clamp(i,0.,1.)

#define R RENDERSIZE

void main()	{
    
    vec2 uv = gl_FragCoord.xy / R.xy;
    float len = length( uv - ZOOM_CENTER );
    
    vec3 tex = vec3( 0.0 );
    for ( float i = 0.0; i < ZOOM_ITER; i += 1.0 ) {
        float fi = ( i  + 0.5 ) / ZOOM_ITER ;
        vec3 blurA = saturate( vec3(
          1.0 - 4.0 * abs( 1.0 / 4.0 - fi ),
          1.0 - 4.0 * abs( 2.0 / 4.0 - fi ),
          1.0 - 4.0 * abs( 3.0 / 4.0 - fi )
        ) ) * 4.0;
        vec3 blurB = vec3(
            1.0 - 2.0 * abs( 1.0 / 2.0 - fi )
        ) * 2.0;
        vec3 blur = mix( blurA, blurB, CHROMA_MIX ) / ZOOM_ITER ;
        float scaleAmp = ( ZOOM_OUTSIDE_AMP * len + ZOOM_AMP ) * fi + ZOOM_OUTSIDE_OFFSET * len;
        vec2 uvt = ( 1.0 - scaleAmp ) * ( uv - ZOOM_CENTER ) + ZOOM_CENTER;
        tex += blur * IMG_NORM_PIXEL(inputImage, uvt).rgb;
    }
  
	gl_FragColor = vec4( tex, 1.0 );
}
