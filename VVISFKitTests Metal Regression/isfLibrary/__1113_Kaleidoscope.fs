
/*{
  "CREDIT": "by VIDVOX",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "sides",
      "TYPE": "float",
      "MIN": 1,
      "MAX": 32,
      "DEFAULT": 6
    },
    {
      "NAME": "angle",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "slidex",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "slidey",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/


const float tau = 6.28318530718;




void main() {
  // normalize to the center
  	vec2 center = vec2(0.5);
	vec2 loc = gl_FragCoord.xy / RENDERSIZE.xy;
	float r = distance(vec2(0.5), loc);
	float a = atan ((loc.y-center.y),(loc.x-center.x));
	
	// kaleidoscope
	a = mod(a, tau/sides);
	a = abs(a - tau/sides/2.);
	
	loc.x = r * cos(a + tau * angle);
	loc.y = r * sin(a + tau * angle);
	
	loc = (center + loc);
	
	loc.x = mod(loc.x + slidex, 1.0);
	loc.y = mod(loc.y + slidey, 1.0);

	// sample the image
	gl_FragColor = IMG_NORM_PIXEL(inputImage, loc);;
}