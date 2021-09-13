/*{
  "CREDIT": "by mojovideotech",
  "ISFVSN": "2",
  "CATEGORIES": [
    "noise"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 999.0,
      "MIN": 11.0,
      "MAX": 1111.0
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 9899.0,
      "MIN": 111.0,
      "MAX": 11111.0
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": -0.2,
      "MIN": -2.0,
      "MAX": 2.0
    },
    {
      "NAME": "freq1",
      "TYPE": "float",
      "DEFAULT": 14.0,
      "MIN": -100.0,
      "MAX": 100.0
    },
    {
      "NAME": "freq2",
      "TYPE": "float",
      "DEFAULT": -11.0,
      "MIN": -100.0,
      "MAX": 100.0
    },
    {
      "NAME": "freq3",
      "TYPE": "float",
      "DEFAULT": -70.0,
      "MIN": -100.0,
      "MAX": 100.0
    },
    {
      "NAME": "freq4",
      "TYPE": "float",
      "DEFAULT": -10.0,
      "MIN": -100.0,
      "MAX": 100.0
    },
    {
      "NAME" :  "gamma",
      "TYPE" :  "float",
      "DEFAULT" : 2.0,
      "MIN" :   0.1,
      "MAX" :   4.0
    },
    {
      "NAME" :  "level",
      "TYPE" :  "float",
      "DEFAULT" : 1.0,
      "MIN" :   0.0,
      "MAX" :   1.0
    }
  ]
}*/


////////////////////////////////////////////////////////////
// RGBGlitchStrobe  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

float T = (sin(TIME)+cos(TIME+rate*10.0))*rate;

float mash(float x) { return pow(mod(x, seed1*cos(seed2*T))*x, freq4); }

float hash(float x) { return mod(mod(x, seed2*sin(seed2*T))*x, freq1); }

float bash(float x) { return mod(mod(x, seed2*sin(seed1*T))*x, freq2); }
    
float cash(float x) { return mod(mod(x, seed1*cos(seed1*T))*x, freq3); }

void main( void )
{
	vec4 i = IMG_PIXEL(inputImage,gl_FragCoord.xy);
	float c1 = fract(hash(mash(i.x)*hash(i.y)));
	float c2 = fract(bash(mash(i.x)*cash(i.y))); 
	float c3 = fract(cash(mash(i.x)*bash(i.y))); 
	vec4 col = vec4(c1,c2,c3,1.0)-i;
    col.rgb = sqrt(pow(gamma * col.rgb, vec3(1.0))); 
    col =  mix(i, 1.0-col, level);
    
	gl_FragColor = vec4(col);
}	
	