/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "noise"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 37,
      "MIN": -1000,
      "MAX": 1000
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 13,
      "MIN": 1,
      "MAX": 100
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": -31,
      "MIN": -60,
      "MAX": 60
    },
    {
      "NAME": "freq1",
      "TYPE": "float",
      "DEFAULT": -7,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "freq2",
      "TYPE": "float",
      "DEFAULT": -11,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "freq3",
      "TYPE": "float",
      "DEFAULT": 37,
      "MIN": -100,
      "MAX": 100
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": -0.4,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ]
}*/


////////////////////////////////////////////////////////////
// GlitchNoise2  by mojovideotech
//
// based on:
// glslsandbox.com/\e#29179.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define PHI floor(TIME*rate)+100./(seed1*sqrt(seed2))

float mash(float x) { return mod(mod(x, PHI)*x, scale);	}

float hash(float x) { return mod(mod(x, PHI)*x, freq1);	}

float bash(float x) { return mod(mod(x, PHI)*x,freq2);	}

float cash(float x) { return mod(mod(x, PHI)*x, freq3);	}

void main( void ) {
	float c1 = fract(hash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y)));
	float c2 = fract(bash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y))); 
	float c3 = fract(cash(mash(gl_FragCoord.x)*mash(gl_FragCoord.y))); 
	vec3 col = vec3(c1,c2,c3);
	if (invert) { col = col * -1.0 + 1.0; }
	gl_FragColor = vec4(vec3(col),1.0);
}	
	
