/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "fractal",
    "equirectangular"
  ],
  "INPUTS" : [
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 3.1,
      "MIN": 2,
      "MAX": 8
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 2.67,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 3.27,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "scale",
      "TYPE": "float",
      "DEFAULT": 0.85,
      "MIN": 0.1,
      "MAX": 3
    },
    {
      "NAME": "freq",
      "TYPE": "float",
      "DEFAULT": 0.999,
      "MIN": 0.6667,
      "MAX": 1.3333
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.0018,
      "MIN": 0.0,
      "MAX": 0.0025
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 93,
      "MIN": 2,
      "MAX": 120
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 29,
      "MIN": 2,
      "MAX": 60
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
    },
    {
      "NAME": "flap",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    },
    {
      "NAME": "flup",
      "TYPE": "bool",
      "DEFAULT": "FALSE"
    }
  ],
  "DESCRIPTION" : ""
}*/


////////////////////////////////////////////////////////////
// FractilianSpheroid  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi
#define 	phi   	1.618033988749895 	// golden ratio


vec3 HRGB(float hue) {
  return clamp(abs(mod(hue * 6.0 + vec3(0.0, color3, 2.0),color1) - color2) - 1.0 ,0.0 ,1.0);
}

void main( void ) {
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float tg =  uv.t * pi, pg =  uv.s * 2.0 * pi;
  	vec3 pp = vec3(4.0-scale * vec3(sin(tg) * cos(pg), sin(tg) * sin(pg), cos(tg)));
  	vec2 p;
  	if (flip) {
  		p = vec2(pp.x,pp.z);
  		if (flop) p.y = pp.y;
  	} 
  	else if (flop) p = vec2(pp.z,pp.y);
  	else p = vec2(pp.x,pp.y);
    float t = TIME*rate, k = cos(t), l = sin(t);        
    float b = floor(depth);
    float s = .1;
    for(int i=0; i<120; ++i) {
        p  = abs(p) - s;   
        p *= mat2(k,-l,l,k);
        s *= freq;        
        b -= 1.0;
        if (b <= 0.0) { break; }
    }
    float th =  p.t * phi, ph =  p.s * 2.0 * phi;
  	vec3 sp = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
  	vec2 q;
  	if (flap) {
  		q = vec2(sp.x,sp.z);
  		if (flup) q.y = sp.y;
  	} 
  	else if (flup) q = vec2(sp.z,sp.y);
  	else q = vec2(sp.x,sp.y);
    float x = .5 + .5*tan(twpi*cos(density*length(q)));
    gl_FragColor = vec4(vec3(HRGB(x)), 1.0);
}