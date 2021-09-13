/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/ldyBR3 by yx.  Maze distance function from the first round of NOVA 18 Shader Showdown",
  "INPUTS" : [
    {
      "NAME" : "VK",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 32,
      "MIN" : -100
    },
    {
      "NAME" : "KV",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 0.5,
      "MIN" : -2
    },
    {
      "NAME" : "RY",
      "TYPE" : "float",
      "MAX" : 4.3423000099999998,
      "DEFAULT" : 4.3423,
      "MIN" : 4.3422999999999998
    }
  ],
  "ISFVSN" : "2"
}
*/


float N(vec2 uv)
{
    return fract(sin(dot(uv,vec2(12.894300,4.342300)))*43242.32432);
}

float M(vec2 uv)
{
    vec2 a = floor(uv);
    vec2 b = fract(uv);
    float d = normalize(N(a)-.5);
    return (.5-abs(fract(b.y+d*b.x)-0.5))/sqrt(1.);
}

void main() {



    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.yy;
    // Output to screen
    gl_FragColor = vec4(M(uv*VK-vec2(0,5.*TIME)));
}
