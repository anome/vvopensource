/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "fractal",
    "equirectangular"
  ],
  "DESCRIPTION": "Spherical / Equirectangular version of FractilianLace3",
  "INPUTS": [
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 5.55,
      "MIN": 2,
      "MAX": 8
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 1.81,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 1.23,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 1,
      "MAX": 3
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.925,
      "MIN": 0.6667,
      "MAX": 0.9999
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": -0.025,
      "MAX": 0.025
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 72,
      "MIN": 2,
      "MAX": 120
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
  ]
}*/


////////////////////////////////////////////////////////////
// EquiRecFractilianLace3  by mojovideotech
//
// Spherical / Equirectangular version of :
// interactiveshaderformat.com/sketches/621
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	pisq  	9.869604401089359	// pi squared, pi^2
#define 	pi   	3.141592653589793 	// pi
#define 	rcpi  	0.318309886183791	// reciprocal of pi, 1/pi 

vec3 hueToRGB(float hue) {
	return clamp(abs(mod(hue * 6.0 + vec3(0.0,color3, 2.0), color1) - color2) - 1.0, 0.0, 1.0);
}

void main() {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float th =  uv.t * pi, ph =  uv.s * 2.0 * pi;
  	vec3 sp = vec3(4.0-scale * vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th)));
  	vec2 p;
  	if (flip) {
  		p = vec2(sp.x,sp.z);
  		if (flop) p.y = sp.y;
  	} 
  	else if (flop) p = vec2(sp.z,sp.y);
  	else p = vec2(sp.x,sp.y);
    float t = TIME*rate, k = cos(t), l = sin(t), s = pisq, b = 1.0;
    s -= dot(k,l);
    for(int i=0; i<120; ++i) {
        b += 1.0;
    	if (b>depth) break;
        p  = abs(p) - s; 
        p *= mat2(k,-l,l,k);
        s  *= freq;  
    }
    float x = cos(rcpi*(359.*length(p)));
    
	gl_FragColor = vec4(hueToRGB(x),1.0);
}

