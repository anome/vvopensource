/*
{
  "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "kali",
    "fractal"
  ],
  "DESCRIPTION" : "",
  "INPUTS": [
    {
      "NAME":       "color1",
      "TYPE":       "float",
      "DEFAULT":    3.1,
      "MIN":        2,
      "MAX":        8
    },
    {
      "NAME":       "color2",
      "TYPE":       "float",
      "DEFAULT":    2.08,
      "MIN":        1,
      "MAX":        5
    },
    {
      "NAME":       "color3",
      "TYPE":       "float",
      "DEFAULT":    0.43,
      "MIN":        0,
      "MAX":        8
    },
    {
      "NAME" :      "center",
      "TYPE" :      "point2D",
      "DEFAULT" :   [ 0.0, 0.0 ],
      "MAX" :       [ 6.0, 6.0 ],
      "MIN" :       [ -6.0, -6.0 ]
    },
    {
      "NAME":       "scale",
      "TYPE":       "float",
      "DEFAULT":    4.25,
      "MIN":        0.1,
      "MAX":        5
    },
    {
      "NAME":       "reps",
      "TYPE":       "float",
      "DEFAULT":    4.0,
      "MIN":        2.0,
      "MAX":        24.0
    },
    {
      "NAME":       "freq",
      "TYPE":       "float",
      "DEFAULT":    0.925,
      "MIN":        0.6667,
      "MAX":        0.9999
    },
    {
      "NAME":       "freq2",
      "TYPE":       "float",
      "DEFAULT":    0.575,
      "MIN":        0.1,
      "MAX":        1.0
    },
    {
      "NAME":       "offset",
      "TYPE":       "float",
      "DEFAULT":    0.6,
      "MIN":        -1.0,
      "MAX":        1.0
    },
    {
      "NAME":       "depth",
      "TYPE":       "float",
      "DEFAULT":    13,
      "MIN":        2,
      "MAX":        32
    },
    {
      "NAME":       "rate",
      "TYPE":       "float",
      "DEFAULT":    0.25,
      "MIN":        -0.5,
      "MAX":        0.5
    }
  ],
   "ISFVSN" : 2.0
}
*/


////////////////////////////////////////////////////////////////////
// KaliFractaleidoscope  by mojovideotech
//
// based on :
// shadertoy.com/view/MlKfWm by iridule
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif

#define     twpi    6.283185307179586   // two pi, 2*pi
#define 	rot(a) 	mat2(cos(a), sin(a), -sin(a), cos(a))

vec3 hueToRGB(float hue) {
        return clamp(abs(mod(hue * 6.0 + vec3(0.0, color3, 2.0), color1) - color2) - 1.0, 0.0, 1.0);
}

void main() {
    vec2 R = RENDERSIZE.xy;
    vec2 uv = (5.1 - scale) * ((2.0 * gl_FragCoord.xy - R) / R.y) - center.xy;   
    for (int i = 0; i < 4; i++) {
        float n = floor(reps),
              a = atan(uv.x, uv.y),
              l = length(uv.xy);
        n = twpi / n;
        a = mod(a + n/2.0, n) - n/2.0;
        uv.xy = l * vec2(cos(a), sin(a)) - vec2(1, 0);
    }
    float s = freq2, b = 1.0;
    for (int i = 0; i < 32; i++) {
        if (b>depth) break;
    	uv.xy = abs(uv.xy) / dot(uv.xy, uv.xy) - s;
        uv.xy *= rot(TIME * rate);
        s *= freq;
        b += 1.0;
    }
    float col = (offset / length(uv.xy));
    gl_FragColor = vec4(vec3(hueToRGB(col)),1.0);
}
