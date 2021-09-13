
/*{
	"DESCRIPTION": "penrose black flower",
	"CREDIT": "isf version of https://www.shadertoy.com/view/3d23Rc <spleen666@gmail.com>",
	"ISFVSN": "2",
	"CATEGORIES": [
		"Escher",
		"Triangle"
	],
	"INPUTS": [
	
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
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
			"WIDTH": "$WIDTH/1.0",
			"HEIGHT": "$HEIGHT/1.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/


#define luma( rgb ) ( dot( rgb, vec3(0.2126, 0.7152, 0.0722) ) )
#define R RENDERSIZE
#define t TIME

void main() {
    
    // get normalized texture coordinates
    vec2 uv = ( gl_FragCoord.xy - .5 * R ) / R.y;
    
    float a = -.25 + max( 1.0 * abs(uv.x) + uv.y * 1., -uv.y);
    // i like this variation: float a = 0.15 + min( 1.0 * abs(p.x) + p.y * 0.75, -p.y);
   
    vec2 p = vec2( a, atan(uv.x, uv.y) );
    
    float red = 25. * log(t) * sin(t * .25 ) / t;
    
    vec4 s = .15 * cos( vec4( red, 1., 10., 0.) + t - p.y );
    
    vec4 e = s.yzxy;
       
    vec4 f = min(p.x - s, e - p.x); 
    
    vec4 col = dot( clamp( f * 80., 0., 1.), 20. * (s - e) ) * (e -.1) ;
    
    gl_FragColor = vec4( vec3( 1. - luma(col.rgb), 0., 0.), 1.);
}
