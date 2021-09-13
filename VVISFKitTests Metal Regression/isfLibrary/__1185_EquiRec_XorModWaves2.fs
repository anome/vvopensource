/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "Spherical / Equirectangular version of XorModWaves2",
  "CATEGORIES": [
  	"equirectangular",
    "generator",
    "waves"
  ],
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -3,
      "MAX": 3
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
      "MAX": [ 2, 1 ],
      "MIN": [ -1, -1 ]
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
// EquiRecXorModWaves2  by mojovideotech
//
// Spherical / Equirectangular version of 
// interactiveshaderformat.com/sketches/789
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi   	3.141592653589793 	// pi

float xor(float a, float b) { return mod((a - b), 1.5); }
 
void main( void ) {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float th =  uv.t * pi, ph = uv.s * 2.0 * pi;
  	vec3 sp = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
	float T = TIME*rate, x = sp.x, y = sp.y, a = shift.y;
	sp.z += T;
	float b = - shift.x + mod(1.0 - abs(offset.y*(T*0.01) - 1.0), 1.0);
	float radius = mod(1.0 - abs(offset.x*(T*0.05) - 1.0), 1.0);
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
