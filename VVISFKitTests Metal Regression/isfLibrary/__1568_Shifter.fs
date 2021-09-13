/*
{
  "CREDIT": "by wilstonoreo",
  "DESCRIPTION": "",
  "CATEGORIES": ["LiCHTPiRATEN"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "shiftImage",
      "TYPE": "image"
    },
    {
      "NAME": "amplitude",
      "TYPE": "float",
      "DEFAULT" : 0.5,
      "MIN" : -1.0,
      "MAX" : 1.0
    },
    {
      "NAME": "shift_direction",
      "TYPE": "long",
      "VALUES" : [0,1],
      "LABELS" : ["Horizontal","Vertical"]
    }
  ]
}*/

void main() 
{
  vec2 v = vv_FragNormCoord.xy;
  if (shift_direction == 0) v = v.yx;
  
  vec4 c = IMG_NORM_PIXEL(shiftImage,vec2( v.y,0.5));
  float shift = 0.299 * c.r + 0.587*c.g + 0.114*c.b - 0.5;
  vec2 s = fract(vec2(shift*amplitude,0.0) +v.xy);
  
  if (shift_direction == 0) s = s.yx;
  
  gl_FragColor = IMG_NORM_PIXEL(inputImage,vec2(s));
}