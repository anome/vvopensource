/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "procedural noise using mod hash functions with mathematical constants",
  "CATEGORIES": [
    "procedural",
    "noise"
  ],
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 198.98,
      "MIN": 1,
      "MAX": 200
    },
    {
      "NAME": "range",
      "TYPE": "float",
      "DEFAULT": 41.99,
      "MIN": 0.1,
      "MAX": 50
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 0.03,
      "MIN": 0.01,
      "MAX": 2
    },
    {
      "NAME": "multiplier",
      "TYPE": "float",
      "DEFAULT": 74.3,
      "MIN": 2,
      "MAX": 200
    },
    {
      "NAME": "RhX",
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
      "NAME": "RhY",
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
      "DEFAULT": 10
    },
    {
      "NAME": "GhX",
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
      "DEFAULT": 2
    },
    {
      "NAME": "GhY",
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
      "NAME": "BhX",
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
      "DEFAULT": 5
    },
    {
      "NAME": "BhY",
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
      "DEFAULT": 7
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
      "NAME": "swiz",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "Rdetail",
      "TYPE": "float",
      "DEFAULT": 0.67,
      "MIN": 0.1,
      "MAX": 0.9
    },
    {
      "NAME": "Gdetail",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.1,
      "MAX": 0.9
    },
    {
      "NAME": "Bdetail",
      "TYPE": "float",
      "DEFAULT": 0.33,
      "MIN": 0.1,
      "MAX": 0.9
    }
  ]
}*/


////////////////////////////////////////////////////////////
// RGBModNoiseFactory  by mojovideotech
//
// RGB version of tool for the evaluation of procedural noise 
// using mod hash functions with mathematical constants
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

float pash(float x)	{	return mod(mod(x, PI )*x, 1.);	}
float lash(float x)	{	return mod(mod(x, EUL)*x, 1.);	}
float kash(float x)	{	return mod(mod(x, KCH)*x, 1.);	}
float hash(float x)	{	return mod(mod(x, PHI)*x, 1.);	}
float sash(float x)	{	return mod(mod(x, MU )*x, 1.);	}
float yash(float x)	{	return mod(mod(x, PYT)*x, 1.);	}
float gash(float x)	{	return mod(mod(x, GSH)*x, 1.);	}
float dash(float x)	{	return mod(mod(x, DEL)*x, 1.);	}
float zash(float x)	{	return mod(mod(x, APR)*x, 1.);	}
float cash(float x)	{	return mod(mod(x, CTN)*x, 1.);	}
float nash(float x)	{	return mod(mod(x, NAT)*x, 1.);	}
float mash(float x)	{	return mod(mod(x, EGM)*x, 1.);	}

void main() {
    float T = TIME * (1./pow((201.-rate),(50.1-range)));
    float cY, colR, colG, colB, X, Y; vec3 col;
    vec2 XY = gl_FragCoord.xy*scale;
    if (rot) {	X = XY.y; Y = XY.x;	}
    	else {	X = XY.x; Y = XY.y;	}
    if (RhX == 0)		{	cY = pash(X);	}
	else if (RhX == 1)	{	cY = lash(X);	}
	else if (RhX == 2)	{	cY = kash(X);	}
	else if (RhX == 3)	{	cY = hash(X);	}
	else if (RhX == 4)	{	cY = sash(X);	}
    else if (RhX == 5)	{	cY = yash(X);	}
    else if (RhX == 6)	{	cY = gash(X);	}
	else if (RhX == 7)	{	cY = dash(X);	}
	else if (RhX == 8)	{	cY = zash(X);	}
	else if (RhX == 9)	{	cY = cash(X);	}
    else if (RhX == 10)	{	cY = nash(X);	}
    else if (RhX == 11)	{	cY = mash(X);	}
     if (GhX == 0)		{	cY = pash(X);	}
	else if (GhX == 1)	{	cY = lash(X);	}
	else if (GhX == 2)	{	cY = kash(X);	}
	else if (GhX == 3)	{	cY = hash(X);	}
	else if (GhX == 4)	{	cY = sash(X);	}
    else if (GhX == 5)	{	cY = yash(X);	}
    else if (GhX == 6)	{	cY = gash(X);	}
	else if (GhX == 7)	{	cY = dash(X);	}
	else if (GhX == 8)	{	cY = zash(X);	}
	else if (GhX == 9)	{	cY = cash(X);	}
    else if (GhX == 10)	{	cY = nash(X);	}
    else if (GhX == 11)	{	cY = mash(X);	}
     if (BhX == 0)		{	cY = pash(X);	}
	else if (BhX == 1)	{	cY = lash(X);	}
	else if (BhX == 2)	{	cY = kash(X);	}
	else if (BhX == 3)	{	cY = hash(X);	}
	else if (BhX == 4)	{	cY = sash(X);	}
    else if (BhX == 5)	{	cY = yash(X);	}
    else if (BhX == 6)	{	cY = gash(X);	}
	else if (BhX == 7)	{	cY = dash(X);	}
	else if (BhX == 8)	{	cY = zash(X);	}
	else if (BhX == 9)	{	cY = cash(X);	}
    else if (BhX == 10)	{	cY = nash(X);	}
    else if (BhX == 11)	{	cY = mash(X);	}
    cY *=multiplier;
	if (RhY == 0)		{	colR = pash(Y+cY*T);	}
	else if (RhY == 1)	{	colR = lash(Y+cY*T);	}
	else if (RhY == 2)	{	colR = kash(Y+cY*T);	}
	else if (RhY == 3)	{	colR = hash(Y+cY*T);	}
	else if (RhY == 4)	{	colR = sash(Y+cY*T);	}
	else if (RhY == 5)	{	colR = yash(Y+cY*T);	}
	else if (RhY == 6)	{	colR = gash(Y+cY*T);	}
	else if (RhY == 7)	{	colR = dash(Y+cY*T);	}
	else if (RhY == 8)	{	colR = zash(Y+cY*T);	}
	else if (RhY == 9)	{	colR = cash(Y+cY*T);	}
	else if (RhY == 10)	{	colR = nash(Y+cY*T);	}
	else if (RhY == 11)	{	colR = mash(Y+cY*T);	}
	if (GhY == 0)		{	colG = pash(Y+cY*T);	}
	else if (GhY == 1)	{	colG = lash(Y+cY*T);	}
	else if (GhY == 2)	{	colG = kash(Y+cY*T);	}
	else if (GhY == 3)	{	colG = hash(Y+cY*T);	}
	else if (GhY == 4)	{	colG = sash(Y+cY*T);	}
	else if (GhY == 5)	{	colG = yash(Y+cY*T);	}
	else if (GhY == 6)	{	colG = gash(Y+cY*T);	}
	else if (GhY == 7)	{	colG = dash(Y+cY*T);	}
	else if (GhY == 8)	{	colG = zash(Y+cY*T);	}
	else if (GhY == 9)	{	colG = cash(Y+cY*T);	}
	else if (GhY == 10)	{	colG = nash(Y+cY*T);	}
	else if (GhY == 11)	{	colG = mash(Y+cY*T);	}
	if (BhY == 0)		{	colB = pash(Y+cY*T);	}
	else if (BhY == 1)	{	colB = lash(Y+cY*T);	}
	else if (BhY == 2)	{	colB = kash(Y+cY*T);	}
	else if (BhY == 3)	{	colB = hash(Y+cY*T);	}
	else if (BhY == 4)	{	colB = sash(Y+cY*T);	}
	else if (BhY == 5)	{	colB = yash(Y+cY*T);	}
	else if (BhY == 6)	{	colB = gash(Y+cY*T);	}
	else if (BhY == 7)	{	colB = dash(Y+cY*T);	}
	else if (BhY == 8)	{	colB = zash(Y+cY*T);	}
	else if (BhY == 9)	{	colB = cash(Y+cY*T);	}
	else if (BhY == 10)	{	colB = nash(Y+cY*T);	}
	else if (BhY == 11)	{	colB = mash(Y+cY*T);	}
	colR *= pow( clamp(colR,0.1,0.9), Rdetail);
	colG *= pow( clamp(colG,0.1,0.9), Gdetail);
	colB *= pow( clamp(colB,0.1,0.9), Bdetail);
	if (swiz) { col = vec3(colG,colB,colR); }
		else { col = vec3(colR,colG,colB); }
	if (invert)	{ gl_FragColor = vec4(vec3(1.0-vec3(col)),1.0); }	
		else { gl_FragColor = vec4(vec3(col),1.0); }
}