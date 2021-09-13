/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    ""
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 4.85,
      "MIN": 2,
      "MAX": 8
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 3.35,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 4.02,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "radius",
      "TYPE": "float",
      "DEFAULT": 0.02,
      "MIN": 0.01,
      "MAX": 1
    },
    {
      "NAME": "shape",
      "TYPE": "float",
      "DEFAULT": 3.96,
      "MIN": 0.0001,
      "MAX": 5
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 1.31,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "scope",
      "TYPE": "float",
      "DEFAULT": 0.74,
      "MIN": 0.01,
      "MAX": 1
    },
    {
      "NAME": "xpand",
      "TYPE": "float",
      "DEFAULT": 0.67,
      "MIN": -0.67,
      "MAX": 0.67
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.94,
      "MIN": 0.6667,
      "MAX": 0.9999
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.02,
      "MIN": -0.025,
      "MAX": 0.025
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 93,
      "MIN": 2,
      "MAX": 120
    },
    {
      "NAME": "multiplier",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": 2,
      "MAX": 24
    }
  ]
}*/

////////////////////////////////////////////////////////////
// FractilianVortex3  by mojovideotech
//
// variation of :
// www.interactiveshaderformat.com/\sketches/\621
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	phi   	1.618033988749895 	// golden ratio
#define 	hfpi  	1.570796326794897 	// half pi, 1/pi


vec3 HRGB(float hue) {
  return clamp(abs(mod(hue * 6.0 + vec3(0.0, color3, 2.0),color1) - color2) - 1.0 ,0.0 ,1.0);
}

void main( void ) {
    vec2 uv = scale * (2.0 * gl_FragCoord.xy-RENDERSIZE)/RENDERSIZE.y;
    float N = floor(multiplier);
    float aa = atan(uv.x, uv.y) * shape;
    float bb = twpi / N;
    uv /= (smoothstep(radius - (radius * 0.1), 
    		radius - (radius * 0.9999 - xpand), 
    		cos(floor(0.5 + aa / bb) * bb - aa) * length(uv.xy))) / (scope);
    float t = TIME*rate, k = cos(t), l = sin(t), b = 1.0, s = phi;
    s -= dot(k,l);
    for(int i=0; i<120; ++i) {
    	b += 1.0;
    	if (b>depth) break;
        uv = abs(uv) - s; 
        uv *= mat2(k,-l,l,k);
        s  *= freq;          
    }
 	float x =  cos(hfpi * (333.0 * length(uv)));
  	gl_FragColor = vec4(vec3(HRGB(x)), 1.0);
}    