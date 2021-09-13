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
      "DEFAULT" : 0,
      "LABEL" : "Progress",
      "MIN" : 0
    },
    {
      "NAME" : "toStep",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0.3321184991274837,
        1,
        1
      ],
      "LABEL" : "toStep"
    },
    {
      "NAME" : "fromStep",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0.3321184991274837,
        1,
        1
      ],
      "LABEL" : "fromStep"
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "by Dan Moore"
}
*/


vec4 transition (vec2 uv) {
  vec4 a = IMG_NORM_PIXEL(startImage, uv);
  vec4 b = IMG_NORM_PIXEL(endImage, uv);
  return mix(a, b, smoothstep(fromStep, toStep, vec4(progress)));
}

void main() {
	gl_FragColor = transition(isf_FragNormCoord.xy);
}