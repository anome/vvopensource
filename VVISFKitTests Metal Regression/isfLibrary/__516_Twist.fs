// SaturdayShader Week 31 : Twist
// by Joseph Fiola (http://www.joefiola.com)
// 2016-03-19

//Based on "Twist" Shadertoy by fb39ca4 - https://www.shadertoy.com/view/XsXXDH
//Inspired by Matthew DiVito's gif - http://cargocollective.com/matthewdivito/Animated-Gifs-02

//Added speed and color controls - zerbzman
//20200218

/*{
  "CREDIT": "Joseph Fiola, Matthew DiVito, Shadertoy user fb39ca4. Forked by zerbzman",
  "DESCRIPTION": "Added speed and color controls.",
  "CATEGORIES": [
    "Generators"
  ],
  "INPUTS": [
    {
      "NAME": "speed",
      "LABEL": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": -0.1,
      "MAX": 0.1
    },
    {
      "NAME": "iterations",
      "LABEL": "Iterations",
      "TYPE": "float",
      "DEFAULT": 30,
      "MIN": 0,
      "MAX": 60
    },
    {
      "NAME": "zoom",
      "LABEL": "Zoom",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": 0.125,
      "MAX": 3
    },
    {
      "NAME": "twist",
      "LABEL": "Twist",
      "TYPE": "float",
      "DEFAULT": 0.51,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "offset",
      "LABEL": "Offset",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "rotation",
      "LABEL": "Rotation",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "linesOffset",
      "LABEL": "Lines Offset",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "fade",
      "LABEL": "Fade",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "stripes",
      "LABEL": "Stripes",
      "TYPE": "bool",
      "DEFAULT": true
    },
    {
      "NAME" : "frontColor",
      "LABEL": "Front Color",
      "TYPE" : "color",
      "DEFAULT" : [
        1.0,
        0.0,
        0.478,
        1.0
      ]
    },
    {
      "NAME" : "backColor",
      "LABEL": "Back Color",
      "TYPE" : "color",
      "DEFAULT" : [
        0.6,
        0.0,
        0.0,
        1.0
      ]
    },
    {
      "NAME": "invert",
      "LABEL": "Invert",
      "TYPE": "bool",
      "DEFAULT": false
    },
    {
      "NAME": "pos",
      "LABEL": "Position",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        0,
        0
      ],
      "MAX": [
        1,
        1
      ]
    }
  ]
}*/

const float PI = 3.14159265;

vec2 rotate(vec2 v, float a) {
  float sinA = sin(a);
  float cosA = cos(a);
  return vec2(v.x * cosA - v.y * sinA, v.y * cosA + v.x * sinA);
}

float square(vec2 uv, float d) {
  return max(abs(uv.x), abs(uv.y)) - d;
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  uv -= vec2(pos);

  uv.x *= RENDERSIZE.x / RENDERSIZE.y;
  uv *= zoom;

  uv = rotate(uv, rotation * PI);

  float blurAmount = -5. / RENDERSIZE.y * (zoom * .5);

  // 	float speed = 0.01;
  float time = TIME * speed;
  time = mod(time, 1.);
  time += twist;

  gl_FragColor = vec4(0., 0., 0., 1.);
  float grey = 0.;
  for (int i = 0; i < 60; i++) {

    float n = float(i);
    float size = 1. - n / iterations;
    float rotateAmount = (n * .5 + .25) * PI * 2.;
    grey = mix(grey, 1., smoothstep(0., blurAmount, square(rotate(uv, -rotateAmount * time), size)));

    float blackOffset = mix(linesOffset / 4., linesOffset / 2., n / (iterations * offset)) / (iterations * offset);
    grey = mix(grey, 0., smoothstep(0., blurAmount, square(rotate(uv, -(rotateAmount + PI / 2.) * time), size - blackOffset)));

    if (stripes) {
      grey = (-grey + 1.) * fade;
    } else {
      grey = grey * fade;
    }
  }

  if (invert) grey = -grey + 1.;

  vec4 color = mix(backColor, frontColor, grey);

  gl_FragColor = color;
}