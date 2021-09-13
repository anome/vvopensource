/*
{
  "CATEGORIES" : [
    "INKA",
    "XXX", 
    "Geometry"
  ],
  "INPUTS" : [{
		"NAME": "inputImage",
		"TYPE": "image"
	},{
		"NAME": "mod1",
		"TYPE": "float",
		"DEFAULT": 0.5
	},{
		"NAME": "mod2",
		"TYPE": "float",
		"DEFAULT": 0.5
	},{
		"NAME": "mod3",
		"TYPE": "float",
		"DEFAULT": 0.5
	},{
		"NAME": "mod4",
		"TYPE": "float",
		"DEFAULT": 0.5
	},{
		"NAME": "color",
		"TYPE": "color",
		"DEFAULT": [
		1.0,
		1.0,
		1.0,
		1.0
		]
	}
  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#36696.0"
}
*/

void main( void ) {
	vec2 p = (gl_FragCoord.xy*2.0-RENDERSIZE)/min(RENDERSIZE.x,RENDERSIZE.y);
	float ratio = (RENDERSIZE.x/2.)/(RENDERSIZE.y);
	float phase, f, s, c, z, v;
	f = 0.;
	
	for(float i = 0.0; i < 20.0; i++){
			
		phase = TIME + i * 0.5 * 0.628318;
		s = sin(phase) * 0.9 * sin(TIME) * mod2;
		c = cos(phase) * 1.5 * mod1;
		z = (.05 * sin(phase) * (1. - mod2));
		v = smoothstep(.0025 * mod4 , 0.0, .1 * abs(length(p * 4. * mod4 + vec2(c, s)) - mod3 - z));
		f = max(f, ( 1. + z * 13. ) / 1.5 * v);
	}
	
	gl_FragColor = vec4(color.rgb * f, 1.0); 
}