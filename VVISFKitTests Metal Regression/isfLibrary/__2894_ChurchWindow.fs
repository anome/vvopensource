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
      "NAME" : "Count",
      "TYPE" : "float",
      "MAX" : 50,
      "DEFAULT" : 0.5,
      "MIN" : 0.2
    },
    {
      "NAME" : "Split",
      "TYPE" : "float",
      "MAX" : 0.09,
      "DEFAULT" : 0.05,
      "MIN" : 0.01
    },
    {
      "NAME" : "Mask",
      "TYPE" : "float",
      "MAX" : 25,
      "DEFAULT" : 15,
      "MIN" : 1
    },
    {
      "NAME" : "TY",
      "TYPE" : "float",
      "MAX" : 9.5,
      "DEFAULT" : 6.5,
      "MIN" : 1.3
    }
  ],
  "ISFVSN" : "2"
}
*/


#define PI 3.1415926535897932384626433832795

void main() {



    int rayCount = 12;
    vec3 color1 = vec3(0.0,0.0,0.0);
    vec3 color2 = vec3(1.0,1.0,1.0);
    
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
    float distRound = (1. - ceil(dist*Count*0.5+.25)*Split);//Count influence le nombre de petales et Split la dilatation
    float time2 = (fract(dist*0.5*.01+.01) > TIME ? -TIME : TIME) * distRound*0.4; // cw/ccw alternatively
    float ngfract = fract(angle * floor(10. / (distRound * distRound)) + time2 * .6);
    ngfract = abs(ngfract*2. - 1.);
    ngfract *= fract(dist*2.) > .8 ? -1. : TY;
    mask -= ceil(dist*Mask + .5 + ngfract*.5)*0.10000000149011612;
    
    mask *= .8 + .2 * fract(dist*12.*0.2+.65);
    
    // output
    gl_FragColor = vec4(mix(vec3(1.0,1.0,1.0)*0.01,mix(color2, color1, mask),distRound),1.0);
}
