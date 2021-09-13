
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"fbm",
		"fractal",
		"motion",
		"waves"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
    	{
			"NAME": "K",
			"TYPE": "float",
			"DEFAULT": 15.5,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "K2",
			"TYPE": "float",
			"DEFAULT": 4.5,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "floatInput",
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

#define R RENDERSIZE
#define time TIME
#define coord 0.2

// audio
#define wav(t) (texture2D(inputImage, vec2(t, coord)).y)
#define amp(t) (texture2D(inputImage, vec2(t, coord)).x)

void main() {

  vec2 uv = (gl_FragCoord.xy - R.xy*.5 ) / R.y ;

  float t = time * 0.6;
  float a = amp(uv.x);
  float c = sin( a +  K2 +  t ) * .15;

  for( float f = -.5; f < .5; f += .08){

    float d1 = (.015 + sin( (t+f*2.)* wav(uv.y) ) *.001 );
    
    vec2 k1 = vec2(uv.x + mod(uv.y*uv.x,.001), sin(uv.x*2.+uv.y*5.+t)*.5);
    vec2 k2 = vec2( sin( uv.x*5.+f*5.+t )*.1, f);
    
    float d2 = distance( k1, k2);

    c +=  min( d1/d2, .25 );

    float acc = (.015 + sin((t+f*2.)*3.)*.005 ) / fract( K  * d2);

    c += acc * uv.y *.5;
  }

   c += .05 * mod( t + K / length( uv - cos(t + uv.x + uv.y ) * .1 ), 0.25);
   c = smoothstep(1., 0.35, c) +uv.y;

  gl_FragColor = vec4(c, c, c, 1.0);
}