/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "procedural",
    "2d",
    "psychedelic",
    "trippy"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "center",
      "TYPE": "point2D",
      "DEFAULT": [
        0,
        0
      ],
      "MAX": [
        4,
        4
      ],
      "MIN": [
        -4,
        -4
      ]
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.0001,
      "MAX": 0.1
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 1,
      "MAX": 6
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 0.125,
      "MIN": 0.01,
      "MAX": 0.5
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -1,
      "MAX": 1
    }
  ]
}*/

///////////////////////////////////////////
// CandyRaver  by mojovideotech
//
// based on:
// shadertoy.com/\view/\lsBSzt  by ErnstHot
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	hfpi  	1.570796326794897 	// half pi, 1/pi

vec3 ct[9]; 

vec3 lookup(const float pos) {
    float p = fract(pos) * 7.999;
    int i = int(p);
    float f = fract(p);
    vec3 res, a, b;
    if (i < 4) {
        if (i < 2) {
            if (i == 0) {a = ct[0]; b = ct[1]; }
            else { a = ct[1]; b = ct[2]; }            
        }
        else {
            if (i == 2) { a = ct[2]; b = ct[3]; }
            else { a = ct[3]; b = ct[4]; }            
        }    
    }
    else
    {
	    if (i < 6) {
            if (i == 4) { a = ct[4]; b = ct[5]; }
            else { a = ct[5]; b = ct[6]; }            
        }
        else {
        	if (i == 6) { a = ct[6]; b = ct[7]; }
            else
            { a = ct[7]; b = ct[8]; }            
        }    
    }
	return mix(a, b, f);
}


void main()
{
// Alternative colors.
/*
	ct[0] = vec3(0.5, 0.0, 0.3);
	ct[1] = vec3(1.0, 0.4, 0.0);
	ct[2] = vec3(1.0, 0.0, 0.3);
	ct[3] = vec3(1.0, 0.6, 0.0);
	ct[4] = vec3(1.0, 0.0, 0.6);
	ct[5] = vec3(1.0, 0.8, 0.0);
	ct[6] = vec3(1.0, 0.4, 0.0);
	ct[7] = vec3(1.0, 0.6, 0.0);
	ct[8] = vec3(0.5, 0.0, 0.3);
*/  
	ct[0] = vec3(0.5, 0.0, 0.3);
	ct[1] = vec3(0.3, 0.1, 0.5);
	ct[2] = vec3(1.0, 0.3, 0.7);
	ct[3] = vec3(1.0, 0.6, 0.0);
	ct[4] = vec3(0.0, 0.9, 0.0);
	ct[5] = vec3(0.0, 0.6, 0.9);
	ct[6] = vec3(1.0, 0.3, 0.0);
	ct[7] = vec3(0.3, 0.1, 1.0);
	ct[8] = vec3(0.5, 0.0, 0.3);
    
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy + center;
	vec2 uvCos = gl_FragCoord.xy / RENDERSIZE.xy;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    uvCos.x -= cos(uvCos.x * twpi) * 0.5 + 0.5;
    uvCos.y -= cos(uvCos.y * twpi) * 0.5 + 0.5;
    float T = TIME * rate;
    float zp = -sin(hfpi + TIME * zoom) * 0.5 + 0.5;
    float s = 0.5 + 8.0 * zp * ((0.8 + uvCos.x * 0.9) + (0.9 + (1.0 - -cos(uvCos.y + T)) * 1.3) + (0.9 + (1.0 - -cos(uvCos.x + T)) * 1.3) );
    uv.xy *= s;
    uv.x -= s;
    uv.y -= s * 0.5;
    float x = 0.5 + 0.5 * -cos(uv.x * twpi + T * 2.0)  * (uvCos.y + 1.0);
    x += 0.25 + 0.25 * -cos((uv.x + T * 0.2) * twpi * 2.5);
    float y = 0.5 + 0.5 * -cos(uv.y * twpi);
    float blah = (0.5 + 0.5 * -cos(T * 0.134)) * (uv.x + T * 0.1);
    y += 0.25 + 1.0 * -cos(blah + uv.y * twpi * 0.1);
    
	gl_FragColor = vec4(lookup(x * y * (1.1 - pow(zp, depth)) * detail + TIME * 0.001), 1.0);
}