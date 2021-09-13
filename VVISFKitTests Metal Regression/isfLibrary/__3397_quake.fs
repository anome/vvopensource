/*{
  "CREDIT": "by Sergey & Nik",
  "CATEGORIES": [
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "magnitude",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 1
    },
    {
      "NAME": "intensity",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 1
    }
  ]
}*/

float rand(vec2 co) {
  return fract(sin(dot(co.xy, vec2(12.9898,78.233))) * 43758.5453);
}

void main(void) {
  float offset = 0.1 * magnitude;
  vec2 uv = ;
  float yOffset = sin(TIME * 1.0 * cos(TIME * intensity) * rotation);
  float xOffset = cos(TIME * 1.0 * cos(TIME * intensity) * rotation);

  uv.y += yOffset * offset * rand(vec2(PASSINDEX, TIME));
  uv.x += xOffset * offset * rand(vec2(PASSINDEX, TIME));

  gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
}