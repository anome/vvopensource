/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "XXX"
  ],
  "INPUTS": [
    {
      "NAME": "size",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.25,
      "MAX": 3
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.25,
      "MAX": 3
    },
    {
      "NAME": "mixer",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "offset",
      "TYPE": "point2D",
      "DEFAULT": [
        1,
        1.75
      ],
      "MAX": [
        2,
        2
      ],
      "MIN": [
        -2,
        -2
      ]
    }
  ]
}*/

////////////////////////////////////
// TwoPointPerspectiveZoom  by mojovideotech
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
////////////////////////////////////

void main() {
	vec2 R = RENDERSIZE.xy; vec2 I = gl_FragCoord.xy;
    float a = abs(I=(I+I-R)/-R.y).y*offset.y;
    vec2 S = vec2(dot(R,I));
    float b = abs(S=(I+I-R)/-R.x).x*offset.x;
	R = sin(6.0*vec2(I.x/a,2.0/a+(rate*TIME))*(3.25-size));
	gl_FragColor -= -sign((R.x*R.y) - pow(R.x*S.y,mix(a*a,b*b,mixer))*b)*a;
//	gl_FragColor -= exp(R.x*S.y)*a*b;
}