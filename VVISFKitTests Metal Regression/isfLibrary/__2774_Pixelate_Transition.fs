/*
{
  "CATEGORIES" : [
    "transition"
  ],
  "DESCRIPTION" : "GL Transition",
  "INPUTS" : [
    {
      "NAME" : "startImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "endImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "progress",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.28199762105941772,
      "LABEL" : "Progress",
      "MIN" : 0
    },
    {
      "NAME" : "squaresMin",
      "TYPE" : "point2D",
      "MAX" : [
        100,
        100
      ],
      "DEFAULT" : [
        2,
        2
      ],
      "LABEL" : "squaresMin",
      "MIN" : [
        0,
        0
      ]
    },
    {
      "NAME" : "steps",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 50,
      "LABEL" : "steps",
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "by Dan Moore"
}
*/

float d = min(progress, 1.0 - progress);
float dist = ceil(d * (steps)) / (steps);
vec2 squareSize = 2.0 * dist / squaresMin;

vec4 transition(vec2 uv) {
  vec2 p = dist>0.0 ? (floor(uv / squareSize) + 0.5) * squareSize : uv;
  return mix( IMG_NORM_PIXEL(startImage, p),  IMG_NORM_PIXEL(endImage, p), progress);
}

void main() {
	gl_FragColor = transition(isf_FragNormCoord.xy);
}