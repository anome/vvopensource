/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
				{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.01,
			"MAX": 0.5
		},
		{
			"NAME": "shift",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			  "MAX" : [
        		1,
        		1
      		],
      		"MIN" : [
        		0,
        		0
      		]
		},
		{
			"NAME": "offset",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			],
			  "MAX" : [
        		1.0,
        		1.0
      		],
      		"MIN" : [
        		0.05,
        		0.5
      		]
		},
		{
			"NAME": "flip",
			"TYPE": "bool",
	        "DEFAULT": 0
		},
		{
			"NAME": "flop",
			"TYPE": "bool",
	        "DEFAULT": 0
		},
		{
			"NAME": "invert",
			"TYPE": "bool",
			"DEFAULT": 0
		}
	]
}*/

#ifdef GL_ES
precision highp float;
#endif

// XorModWaves2 by mojovideotech

float xor( float a, float b ) 
{
	return mod( ( a - b ), 1.5 );
}
 
void main( void ) 
{
	vec2 aspRat = vec2( RENDERSIZE.x / RENDERSIZE.y, 1.0 );
	vec2 curPix = (gl_FragCoord.xy / RENDERSIZE.xy * aspRat.xy - aspRat.yx / shift.x) - shift.y ;
	float T = TIME * rate;
	float sqrSize = scale,
	      dblSqrSize = sqrSize * 1.5,
	      radius = mod( 1.0 - abs( offset.x * fract( T * 0.05 ) - offset.y ), 1.0 ),
	      a = 0.5,
	      b = -0.5 + mod( 1.0 - abs( offset.y * fract( T * 0.01 ) - offset.x ), 1.0 ),	
	      x = curPix.x,
	      y = curPix.y;
	
	// Inversion transform
	 //newX=a + (r^2*(-a + x))/((a - x)^2 + (b - y)^2)
	 //newY=b + (r^2*(-b + y))/((a - x)^2 + (b - y)^2) 
			  
	vec2 invPix = vec2( 1.0, 0.5 );
 
	invPix.x = a + xor (( radius * radius * ( a - x ) ),
	                       - sin ( ( radius - x ) + ( a - x ) - cos( sin(b - y)) * pow( ( a - y ), exp(radius))));
	                         
	invPix.y = b + xor(( radius * radius * ( b - y ) ),
	                        sin ( ( a - x ) * ( radius - x ) - sin ( cos(a - y) ) * pow( ( b - y ), sqrt(radius))));
	                         
	float clr = smoothstep( mod(  dblSqrSize, invPix.x),mod( invPix.y, sqrSize ),  sqrSize );
	float clrr = xor( clr, smoothstep( mod( invPix.y, dblSqrSize ),mod( -invPix.x, sqrSize ), sqrSize ) );
	vec3 col = vec3(clrr,xor(clrr,clr),cos(clrr));
   	if (flip) col = vec3(xor(clrr,clr),cos(clrr),clrr);
	vec3 cc = col;
	if (flop) cc -= cc.gbr;
	if (invert) gl_FragColor = vec4(cc, 1.0 );
	else 
	{
		gl_FragColor = vec4(vec3(1.0-cc), 1.0 );
	}
}
