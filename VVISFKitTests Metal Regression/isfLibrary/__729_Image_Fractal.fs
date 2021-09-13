/*{
	"CREDIT": "by sheltron3030",
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
			"NAME" : "offs",
			"TYPE" : "point2D",
			"MIN" : [-1, 1],
			"MAX" : [1, 1],
			"DEFAULT": [0.5, 0.0]
		},
		{
			"NAME": "zoom",
			"TYPE" : "float",
			"MAX" : 3,
			"DEFAULT" : 1
		},
		{
			"NAME": "a",
			"TYPE" : "float",
			"DEFAULT": 0

		},
		{
			"NAME": "b",
			"TYPE" : "float",
			"DEFAULT": 0.8

		},
		{
			"NAME": "c",
			"TYPE" : "float",
			"DEFAULT": 1

		}
	]
}*/

// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// Instead of using a pont, circle, line or any mathematical shape for traping the orbit
// of fc(z), one can use any arbitrary shape. For example, a NyanCat :)
//
// I invented this technique more than 10 years ago (can have a look to those experiments 
// here http://www.iquilezles.org/www/articles/ftrapsbitmap/ftrapsbitmap.htm).

vec4 getNyanCatColor( vec2 p )
{
	
	// p /= 2.;
	// p += 0.25;
	// p = clamp(p, 0.1, 0.9);
	if ( p.x < 0.01 || p.x >0.99 || p.y < 0.01 || p.y >0.99)
		return vec4(0., 0., 0., 0.1);
		
	p.xy = 1. - p.xy;
	return texture2D( inputImage, p );
}
vec2 iResolution = RENDERSIZE.xy;
float iGlobalTime = TIME;

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 p = -1.0+2.0*fragCoord.xy / iResolution.xy;
	p.x *= iResolution.x/iResolution.y;

    // zoom	
	p = p*zoom ;
	
    vec4 col = vec4(0.0);
	vec3 s =  vec3(a, b, c); //, vec3( 0.5,-0.2,0.5), 0.5+0.5*sin(0.5*iGlobalTime) );

    // iterate Jc	
	vec2 c = offs;
	float f = 0.0;
	vec2 z = p;
	for( int i=0; i<100; i++ )
	{
		if( (dot(z,z)>4.0) || (col.w>0.1) ) continue; // break;

        // fc(z) = z² + c		
		z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
		
		col = getNyanCatColor( s.xy + s.z*z );
		f += 1.0;
	}
	
	vec3 bg = 0.5*vec3(0.5,0.5,0.5) * sqrt(f/100.0);
	
	col.xyz = mix( bg, col.xyz, col.w );
	
	fragColor = vec4( col.xyz,1.0);
}

void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);
}