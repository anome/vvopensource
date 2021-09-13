/*{
  "DESCRIPTION": "Cellular Automata vs Procedural Noise",
  "CREDIT": "Mojo Video Tech & VIDVOX",
  "CATEGORIES": [
    "Generator",
    "Noise",
    "Glitch",
    "Cellular Automata",
    "Procedural"
  ],
  "INPUTS": [
    {
      "NAME": "reSeed",
      "TYPE": "event"
    },
    {
      "NAME": "seed0",
      "TYPE": "float",
      "DEFAULT": 1.09,
      "MIN": 0.001,
      "MAX": 1.999
    },
    {
      "NAME": "growthRate",
      "TYPE": "float",
      "DEFAULT": 0.061,
      "MIN": 0,
      "MAX": 0.1
    },
    {
      "NAME": "deathRate",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0,
      "MAX": 0.1
    },
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 478,
      "MIN": 1,
      "MAX": 1000
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": 1,
      "MAX": 100
    },
    {
      "NAME": "freq1",
      "TYPE": "float",
      "DEFAULT": -29.8,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "freq2",
      "TYPE": "float",
      "DEFAULT": -36.33,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "freq3",
      "TYPE": "float",
      "DEFAULT": 54.29,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "mixer",
      "TYPE": "float",
      "DEFAULT": 0.58,
      "MIN": 0.25,
      "MAX": 0.75
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "flip",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "flop",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ],
  "PASSES": [
    {
      "TARGET": "lastData",
      "persistent": true
    }
  ]
}*/


///////////////////////////////////////////
// -------------------------------------
// GameOfGlitchSphericalish    
// -------------------------------------
// pseudo-spherical version of 
// GameOfGlitch  by mojovidotech
// Cellular Automata vs Procedural Noise
// -------------------------------------
// a mashup of 
// Life  by DavidLublin
// Based on Conway Game of Life 
// interactiveshaderformat.com/\sketches/\744
// - & - 
// GlitchNoise2  by mojovidotech
// interactiveshaderformat.com/\sketches/\717
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;
varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

#define Q floor(TIME*seed2)*(seed0/log(seed1))

float mash(float x) { return mod(mod(x, Q)*x, seed0); }

float hash(float x) { return mod(mod(x, Q)*x, growthRate*freq1); }

float bash(float x) { return mod(mod(x, Q)*x, deathRate*freq2); }

float rash(vec2 co) { 
	return fract(sin(dot(co.xy ,vec2(pow(floor(seed1),sin(freq1)),pow(floor(seed2),cos(freq2))))) * exp(freq3));
}

float gray(vec4 n) { return (n.r + n.g + n.b)/3.0; }

void main() {
	float yy = radians(180.*(gl_FragCoord.y/RENDERSIZE.y-0.5));
	float xz = radians(360.*(gl_FragCoord.x/RENDERSIZE.x-0.5));
	vec3 uv = vec3(sin(xz)*cos(yy), cos(yy), sin(yy)*cos(xz));
	
	float c1 = fract(hash(mash(uv.x)*mash(uv.x)));
	float c2 = fract(bash(mash(uv.y)*mash(uv.y))); 
	float c3 = fract(mash(mash(uv.z)*mash(uv.z)));  
	vec4 col = vec4(c1,c2,c3,seed0);
	vec4 ipc = vec4(col);
	vec2 loc = uv.xy;
	if ((TIME < 0.5)||(reSeed)) {
		float	alive = rash(vec2(mash(TIME)+1.0,2.1*mash(TIME)+0.1)*loc);
		if (alive > 1.0 - seed0) { ipc = vec4(1.0); }
	}
	else {
		vec4	color = IMG_PIXEL(lastData, loc);
		vec4	colorL = IMG_PIXEL(lastData, left_coord);
		vec4	colorR = IMG_PIXEL(lastData, right_coord);
		vec4	colorA = IMG_PIXEL(lastData, above_coord);
		vec4	colorB = IMG_PIXEL(lastData, below_coord);
		vec4	colorLA = IMG_PIXEL(lastData, lefta_coord);
		vec4	colorRA = IMG_PIXEL(lastData, righta_coord);
		vec4	colorLB = IMG_PIXEL(lastData, leftb_coord);
		vec4	colorRB = IMG_PIXEL(lastData, rightb_coord);
		float	neighborSum = gray(colorL + colorR + colorA + colorB + colorLA + colorRA + colorLB + colorRB);
		float	state = gray(color);
		if (state > 0.0) {
			if (neighborSum < 3.0) { ipc = vec4(0.5,0.0,0.0,0.5); }
			else if (neighborSum < 5.0)	{
			     ipc = vec4(0.9,0.9,1.0,1.0);
				 float alive = rash(vec2(bash(TIME)+1.0,2.1*hash(TIME)+0.1)*loc);
				 if (alive > 1.0 - deathRate) { ipc = vec4(0.0,0.5,0.0,0.5); }
			}
			else { ipc = vec4(0.0,0.0,0.5,0.5); }
		}
		else {
			if ((neighborSum > 3.0)&&(neighborSum < 5.0)) { ipc = vec4(1.0,0.0,1.0,1.0); }
			else if (neighborSum < 3.0) {
				 float alive = rash(vec2(hash(TIME)+1.0,2.1*bash(TIME)+0.1)*loc);
				 if (alive > 1.0 - growthRate) { ipc = vec4(1.0,0.0,1.0,1.0); }
			}
		}
	}
	vec4 mx = mix(ipc,col,mixer);
	vec4 fx = mx;
	if (flip) fx = mx.gbra;
	vec4 gfc = fx;
	if (flop) gfc = fx.brga;
	if (invert) gl_FragColor = gfc;
	else { gl_FragColor = vec4 (gfc.rgb * -1.0 + 1.0, 1.0); }
}
