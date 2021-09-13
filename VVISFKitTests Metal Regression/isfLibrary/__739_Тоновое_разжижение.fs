/*{
  "CREDIT": "by ",
  "CATEGORIES": [
    "Stylize"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.6
    },
    {
      "NAME": "xx",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 1,
      "DEFAULT": 0
    },
    {
      "NAME": "s",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0
    }
  ]
}*/
// kernel Convolution Filters алгоритмы

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


void main()	{
	vec4 inputPixelColor;
	
	vec4 fin = vec4(0.);
	float finHSV = 0.0;
	
	for(int dX = -1; dX <= 1; ++dX) {
		for(int dY = -1; dY <= 1; ++dY) {		
			vec2 offset = gl_FragCoord.xy + intensity * 10. * vec2(float(dX), float(dY));
			
			vec4 _ = IMG_PIXEL(inputImage, offset);
			float l = rgb2hsv(_.rgb).x;
    	    if(l > finHSV) {
    	        fin = _;
    	        finHSV = l;
    	    }
		}
	}
    
    inputPixelColor = fin;
	
	gl_FragColor = inputPixelColor;
}
