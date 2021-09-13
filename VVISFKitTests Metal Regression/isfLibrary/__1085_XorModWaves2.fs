/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "generator",
    "waves"
  ],
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.01,
      "MAX": 0.5
    },
    {
      "NAME": "shift",
      "TYPE": "point2D",
      "DEFAULT": [ 0.5, 0.1 ],
      "MAX": [ 1, 1 ],
      "MIN": [ 0, 0 ]
    },
    {
      "NAME": "offset",
      "TYPE": "point2D",
      "DEFAULT": [ 0.75, 0.75 ],
      "MAX": [ 1, 1 ],
      "MIN": [ 0.05, 0.5 ]
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

////////////////////////////////////////////////////////////
// XorModWaves2  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


float xor(float a, float b) { return mod((a - b), 1.5); }
 
void main( void ) {
	vec2 aspRat = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
	vec2 curPix = (gl_FragCoord.xy / RENDERSIZE.xy*aspRat.xy - aspRat.yx / shift.x) - shift.y;
	float T = TIME*rate, x = curPix.x, y = curPix.y, a = 0.5;
	float b = -0.5 + mod(1.0 - abs(offset.y*fract(T*0.05) - 1.0), 1.0);
	float radius = mod(1.0 - abs(offset.x*fract(T*0.01) - 1.0), 1.0);
	float invx = a + xor((exp(radius)*(a - x)), -sin((radius - x) + (a - x) - cos(sin(b - y))*pow((a - y), exp(radius))));
	float invy = b + xor((exp(radius)*(b - y)), sin((a - x)*(radius - x) - sin(cos(a - y))*pow((b - y), sqrt(radius))));
	float clr = smoothstep(mod(scale*1.5, invx), mod(invy, scale), scale);
	float clrr = xor(clr, smoothstep(mod(invy, scale*1.5), mod(-invx, scale), scale));
	vec3 col = vec3(clrr,xor(clrr,clr),cos(clrr));
   	if (flip) col = vec3(xor(clrr,clr),cos(clrr),clrr);
	vec3 cc = col;
	if (flop) cc -= cc.gbr;
	if (invert) gl_FragColor = vec4(cc, 1.0 );
	else gl_FragColor = vec4(vec3(1.0-cc), 1.0);
}
