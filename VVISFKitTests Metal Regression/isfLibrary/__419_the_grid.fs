/*{
	"CREDIT": "by spleen666",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"grid",
		"fractal"
	],
	"INPUTS": [
	    {
			"NAME": "timeUI",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1e6
		},
		{
			"NAME": "k1",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "k2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "k3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "k4",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "k7",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": 5.0,
			"MAX": 20.0
		},
		{
			"NAME": "k8",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": 0.1,
			"MAX": 20.0
		},
		{
			"NAME": "k11",
			"TYPE": "float",
			"DEFAULT": 0.004,
			"MIN": 0.0,
			"MAX": 0.008
		},
		{
			"NAME": "kx",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "ky",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/


#define time TIME
#define R RENDERSIZE
#define iterations 10.0


void main() {

  vec2 uv = ( gl_FragCoord.xy - .5 * R.xy ) / R.y;
  
  float t = time * .1;
 
 //
  uv.x += sin( uv.y * (1. + k1 ) + t * .1) / k8;
  
  //
  float kk = abs( cos(t * .01));
  uv *= k2 + kk * kk * ( 0.5 + k3 );
  
  //
  uv += vec2( sin(t * .01), cos(t * .01) );

  // rotation
  
  uv = vec2(uv.x * cos(t ) + uv.y * sin(t ),
                uv.y * cos(t) - uv.x * sin(t));

  float expsmooth = 0.;
  float average = 0.;
  float l = length(uv);
  float prevl;

  for (float i = 0.0; i <= iterations; i += 1.0) {

    uv = abs(uv * 2.*cos(t)) / (uv.x * uv.y) - vec2(kx, ky);

    prevl = l;
    l = length(uv);
    expsmooth += exp( -0.25 / abs(l - prevl));
    average += abs( l - prevl );
  }
  
  average /= iterations * k7;

  gl_FragColor = vec4( vec3(0.0,  expsmooth * k11 * average, 0.), 1.0);
}