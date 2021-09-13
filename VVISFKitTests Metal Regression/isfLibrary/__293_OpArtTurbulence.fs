/*{
    "CREDIT": "mojovideotech, Enhanced by zerbzman",
  "CATEGORIES": [
    "Generator"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#27863.0",
  "INPUTS": [
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": -0.1,
      "MAX": 0.1
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 10
    },
    {
      "NAME": "frontColor",
      "LABEL": "Front Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        0,
        0,
        1
      ]
    },
    {
      "NAME": "backColor",
      "LABEL": "Back Color",
      "TYPE": "color",
      "DEFAULT": [
        1,
        1,
        1,
        1
      ]
    },
    {
      "NAME": "invert",
      "LABEL": "Invert",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}*/

// OpArtTurbulence by mojovideotech
// http://glslsandbox.com/e#27863.0

#ifdef GL_ES
precision mediump float;
#endif

const float pi = 3.1415926535;

vec2 size = RENDERSIZE;
float scale = 3.0;

const int num = 33;

float wave(vec2 pos, float angle, float wavelength, float phase, int i) {
  return sin(dot(pos, vec2(cos(angle), sin(angle))) * 2.0 * pi / wavelength + phase + float(i));
}

float wave2(vec2 pos, int i, float ls) {
  vec2 me = vec2(size.x * 0.4 * cos(0.0737 * TIME * speed * (float(i + 1) + 0.34) + float(i)), size.y * 0.4 * sin(0.0876 * TIME * speed * (float(i + 1) + 0.56) + float(i)));
  vec2 diff = pos - me;
  float angle2 = cos(atan(diff.y, diff.x) + float(i) * pi / float(num) + 0.3 * TIME * speed);
  float dist = length(diff);
  //return sin(dot(pos,vec2(cos(angle),sin(angle)))*2.0*pi/(wavelength)+phase+float(i));
  return (3.0 + float(i * 2)) * angle2 + 20.0 * sin(6.0 * dist / ls) * sin(0.5 * TIME * speed + (float(i + 3) * 2.34)) - TIME * speed;
}

void main() {
  vec2 pos = gl_FragCoord.xy / scale - size / 2.0;
  
  pos *= zoom;

  float amp = 0.0;
  float ls = length(size);
  for (int i = 0; i < num; i++) {
    amp += wave2(pos, i, ls);
  }

  float y = gl_FragCoord.y / size.y;

  float c = sin(pi * 12.0 * (amp + float(num)) / float(num) / 2.0);
  c = clamp(c, 0.0, 1.0);
  c = pow(c, 0.2);
  vec4 color = frontColor + (backColor - frontColor) * clamp((c + y) * 0.6, 0.0, 1.0);
  gl_FragColor = color;
}