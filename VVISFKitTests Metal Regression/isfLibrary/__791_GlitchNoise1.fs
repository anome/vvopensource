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
      "DEFAULT": 5,
      "MIN": 1,
      "MAX": 100000
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 1.00001,
      "MIN": 1.00000000001,
      "MAX": 1.1
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 6,
      "MIN": 0.06,
      "MAX": 60
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ]
}*/


////////////////////////////////////////////////////////////
// GlitchNoise1  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define PHI floor(TIME*rate)/(seed2/sqrt(floor(seed1)))

float hash(float x) {
	return pow(mod(mod(x, PHI)*x, freq),mod(x,log2(PHI)));	
}
float bash(float x) {
	x = cos(x+TIME/PHI);
	return pow(mod(mod(x, PHI)*x, freq),mod(x,log(PHI)));	
}
float cash(float x) {
	x = step(cos(x),sin(x));
	return pow(mod(mod(x, PHI)*x, freq),mod(x,log2(PHI)*x));	
}
float stash(float x) {
	x = smoothstep(sin(x),sin(x+0.1),(x*TIME));
	return pow(mod(mod(x, PHI)*TIME, freq),mod(PHI,fract(exp(x))));	
}
mat2 rot(float angle) {
    return mat2(tan(angle),sin(angle),
               sin(angle),-cos(angle));
}

void main( void ) {
	float c1 = fract(hash(bash(gl_FragCoord.x)*stash(gl_FragCoord.y)));
	float c2 = fract(bash(hash(gl_FragCoord.x)/cash(gl_FragCoord.y))); 
	float c3 = fract(stash(hash(gl_FragCoord.x)/cash(gl_FragCoord.y))); 
	vec3 col = vec3(c1,c2,c3);
	col.xy *= rot(TIME/c3);
	col.xz *= rot(TIME/col.y);
	col.yz *= rot(TIME/col.x);
	if (invert) { col = col * -1.0 + 0.5; }
	
	gl_FragColor = vec4(vec3(col),1.0);
}