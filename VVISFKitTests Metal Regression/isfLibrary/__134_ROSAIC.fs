/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/MstfRj by hu9o.  .",
  "INPUTS" : [
    {
      "NAME" : "Red",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.69999998807907104,
      "LABEL" : "Red",
      "MIN" : -1
    },
    {
      "NAME" : "Mask",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 10,
      "MIN" : 9
    },
    {
      "NAME" : "TY",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "MIN" : 0
    },
    {
      "NAME" : "op",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.10000000149011612,
      "MIN" : 0
    },
    {
      "NAME" : "Ti",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 1,
      "MIN" : -5
    },
    {
      "NAME" : "Ko",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        0.22514341771602631,
        0.029171200469136238,
        1
      ]
    }
  ],
  "ISFVSN" : "2"
}
*/


#define PI 3.1415926535897932384626433832795

void main() {



    int rayCount = 12;
    vec3 color1 = vec3(1.,.9,0.);
    vec3 color2 = vec3(0.,0.4,.3);
    
    // center
    vec2 c = (gl_FragCoord.xy - vec2(RENDERSIZE) * .5) / RENDERSIZE.y;
    
    // cartesian to polar
	float angle = atan(c.y, c.x);
    float dist = length(c);
    
    // normalize angle
    angle /= (2.*PI);
    
    // fraction angle
    float mask = 1.;
    
    // radial gradient
    float distRound = (1. - ceil(dist*10.*.5+.25)*0.1);
    float time2 = (fract(dist*10.*.5+.25) > TIME ? -TIME : TIME) * distRound*Ti; // cw/ccw alternatively
    float ngfract = fract(angle * floor(10. / (distRound * distRound)) + time2 * .6);
    ngfract = abs(ngfract*2. - 1.);
    ngfract *= fract(dist*10.) > .5 ? -1. : TY;
    mask -= ceil(dist*Mask + .5 + ngfract*.5)*op;
    
    //mask *= .8 + .2 * fract(dist*10.*.5+.25);
    
    // output
    gl_FragColor = vec4(mix(vec3(Red, .0, .9)*.3,mix(color2, color1, mask),distRound),1.0);
}
