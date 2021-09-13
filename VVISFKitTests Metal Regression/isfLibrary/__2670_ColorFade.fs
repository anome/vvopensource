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
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : "by Dan Moore"
}
*/



vec4 transition (vec2 uv) {
  return mix(
    mix(vec4(color, 1.0), IMG_NORM_PIXEL(endImage, uv), smoothstep(1.0-colorPhase, 0.0, progress)),
    mix(vec4(color, 1.0), IMG_NORM_PIXEL(endImage, uv), smoothstep(    colorPhase, 1.0, progress)),
    progress);
}

void main() {
	gl_FragColor = transition(isf_FragNormCoord.xy);
}