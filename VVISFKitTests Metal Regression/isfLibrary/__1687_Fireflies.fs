// Floating Dots Week 20 : Fractal Dots
// by INKA

// Based on "DUST" by @patu https://www.shadertoy.com/view/4d33DM



/*{
    "CREDIT": "INKA",
	"DESCRIPTION": "",
	"CATEGORIES": [
			"Generator, XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
	{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 3.0,
			"MIN": 0.0,
			"MAX": 20.0
		},
	{
			"NAME": "resolution",
			"TYPE": "float",
			"DEFAULT": 17.0,
			"MIN": 0.0,
			"MAX": 100.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.09,
			"MIN": 0.01,
			"MAX": 1.0
		},
		{
			"NAME": "light",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.01,
			"MAX": 4.0
		},
		{
			"NAME": "vertical_movement",
			"TYPE": "bool",
			"DEFAULT": false
		}
	]
}*/


#define LAYERS 8.

vec2 hash( vec2 p ) {                       // rand in [-1,1]
    p = vec2( dot(p,vec2(127.1,311.7)),
              dot(p,vec2(269.5,183.3)) );
    return -1. + 2.*fract(sin(p+20.)*53758.5453123);
}

float noise( vec2 p ) {
    vec2 i = floor((p)), f = fract((p));
    vec2 u = f*f*(3.-2.*f);
    return mix( mix( dot( hash( i + vec2(0.,0.) ), f - vec2(0.,0.) ), 
                     dot( hash( i + vec2(1.,0.) ), f - vec2(1.,0.) ), u.x),
                mix( dot( hash( i + vec2(0.,1.) ), f - vec2(0.,1.) ), 
                     dot( hash( i + vec2(1.,1.) ), f - vec2(1.,1.) ), u.x), u.y);
}

void main()
{
	float d, s;
	float t = TIME * speed;
    vec4 c, circle;
   
    
    for (float i = 1.; i < LAYERS; i++) {
    	vec2 uv = gl_FragCoord.xy/RENDERSIZE -.5;
    	uv *= tan(radians(i * 4. + zoom));
		uv.x *= RENDERSIZE.x/RENDERSIZE.y;

        //vec2 uv = n * tan(radians(i * 4. + zoom));
        
        float movementx = sin(i - t / 4. + uv.x) * .1;
        float movementy = cos(i - t / 4. + uv.y) * .1;
        
        uv += vec2(movementx, movementy / 2. - (t / 10. + i / 12.) * (vertical_movement ? 0.5 : 0.));
        
        
        s = .005 * noise(ceil(uv * resolution + i * 20.) + i + t * vec2(.2, .01));
        d = length(mod(uv, 1. / resolution) - .5 / resolution);

        if (d < s) c += 1. / i - d * i * 6.; 
        
        circle = vec4(c[0], c[1], 0. + i * 0.03 * sin(TIME / (5. + i)) , c[3]) * light;
     
    }
    
	gl_FragColor = circle;
    
}