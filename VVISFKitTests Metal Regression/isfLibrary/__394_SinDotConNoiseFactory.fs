/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "procedural noise using sin dot hash functions with mathematical constants",
  "CATEGORIES": [
    "procedural",
    "noise"
  ],
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 67,
      "MIN": 1,
      "MAX": 100
    },
    {
      "NAME": "range",
      "TYPE": "float",
      "DEFAULT": 3.6,
      "MIN": 0.1,
      "MAX": 5
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 0.02,
      "MIN": 0.01,
      "MAX": 2
    },
    {
      "NAME": "hX",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11
      ],
      "LABELS": [
        "P",
        "E",
        "K",
        "H",
        "M",
        "T",
        "A",
        "D",
        "Z",
        "C",
        "L",
        "G"
      ],
      "DEFAULT": 8
    },
    {
      "NAME": "hY",
      "TYPE": "long",
      "VALUES": [
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11
      ],
      "LABELS": [
        "P",
        "E",
        "K",
        "H",
        "M",
        "T",
        "A",
        "D",
        "Z",
        "C",
        "L",
        "G"
      ],
      "DEFAULT": 3
    },
    {
      "NAME": "rot",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 0.67,
      "MIN": 0.1,
      "MAX": 0.9
    }
  ]
}*/

////////////////////////////////////////////////////////////
// SinDotConNoiseFactory  by mojovideotech
//
// a tool for the evaluation of procedural noise using
// sin dot hash functions with mathematical constants
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif

#define 	PI  	3.1415926535897932384626433832795 	// pi	   					( P )
#define 	EUL	    2.7182818284590452353602874713527 	// Euler					( E )
#define  	KCH  	2.6854520010653064453097148354818  	// Khinchin 				( K )
#define 	PHI 	1.6180339887498948482045868343656 	// Golden Ratio, phi		( H )
#define 	MU 		1.4513692348833810502839684858920	// Soldner, mu				( M )
#define  	PYT 	1.4142135623730950488016887242097 	// Pythagoras, sq root of 2	( T )
#define  	GSH  	1.2824271291006226368753425688698 	// Glaisher					( A )
#define  	DEL 	1.2599210498948731647672106072782  	// Delian, cube root of 2	( D )
#define 	APR 	1.2020569031595942853997381615114 	// Apery, zeta[3] 			( Z )
#define  	CTN  	0.9159655941772190150546035149324 	// Catalan 					( C )
#define  	NAT 	0.6931471805599453094172321214582 	// natural log of 2			( L )
#define  	EGM 	0.5772156649015328606065120900824	// Euler Gamma 				( G )

float pash(float x)	{	return fract(sin(dot(x, PI ))*x);	}
float lash(float x)	{	return fract(sin(dot(x, EUL ))*x);	}
float kash(float x)	{	return fract(sin(dot(x, KCH ))*x);	}
float hash(float x)	{	return fract(sin(dot(x, PHI ))*x);	}
float sash(float x)	{	return fract(sin(dot(x, MU ))*x);	}
float yash(float x)	{	return fract(sin(dot(x, PYT ))*x);	}
float gash(float x)	{	return fract(sin(dot(x, GSH ))*x);	}
float dash(float x)	{	return fract(sin(dot(x, DEL ))*x);	}
float zash(float x)	{	return fract(sin(dot(x, APR ))*x);	}
float cash(float x)	{	return fract(sin(dot(x, CTN ))*x);	}
float nash(float x)	{	return fract(sin(dot(x, NAT ))*x);	}
float mash(float x)	{	return fract(sin(dot(x, EGM ))*x);	}

void main() 
{
    float T = TIME * (1./pow((101.-rate),(5.1-range)));
    float cY; float col; float X; float Y;
    vec2 XY = gl_FragCoord.xy*scale;
    if (rot) {	X = XY.y; Y = XY.x;	}
    	else {	X = XY.x; Y = XY.y;	}
    if (hX == 0)		{	cY = pash(X);	}
	else if (hX == 1)	{	cY = lash(X);	}
	else if (hX == 2)	{	cY = kash(X);	}
	else if (hX == 3)	{	cY = hash(X);	}
	else if (hX == 4)	{	cY = sash(X);	}
    else if (hX == 5)	{	cY = yash(X);	}
    else if (hX == 6)	{	cY = gash(X);	}
	else if (hX == 7)	{	cY = dash(X);	}
	else if (hX == 8)	{	cY = zash(X);	}
	else if (hX == 9)	{	cY = cash(X);	}
    else if (hX == 10)	{	cY = nash(X);	}
    else if (hX == 11)	{	cY = mash(X);	}
    cY *=11.;
	if (hY == 0)		{	col = pash(Y+cY*T);	}
	else if (hY == 1)	{	col = lash(Y+cY*T);	}
	else if (hY == 2)	{	col = kash(Y+cY*T);	}
	else if (hY == 3)	{	col = hash(Y+cY*T);	}
	else if (hY == 4)	{	col = sash(Y+cY*T);	}
	else if (hY == 5)	{	col = yash(Y+cY*T);	}
	else if (hY == 6)	{	col = gash(Y+cY*T);	}
	else if (hY == 7)	{	col = dash(Y+cY*T);	}
	else if (hY == 8)	{	col = zash(Y+cY*T);	}
	else if (hY == 9)	{	col = cash(Y+cY*T);	}
	else if (hY == 10)	{	col = nash(Y+cY*T);	}
	else if (hY == 11)	{	col = mash(Y+cY*T);	}
	col *= pow( clamp(col,0.1,0.9), detail);
	if (invert)	{ gl_FragColor = vec4(vec3(1.0-col),1.0); }	
		else { gl_FragColor = vec4(vec3(col),1.0); }
}