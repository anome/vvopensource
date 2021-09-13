/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4dyfW1 by iridule.  mind over matter",
  "INPUTS" : [
    {
      "NAME" : "Rott",
      "TYPE" : "float",
      "MAX" : 600,
      "DEFAULT" : 64,
      "LABEL" : "Rott",
      "MIN" : -600
    },
    {
      "NAME" : "Speed",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "LABEL" : "Speed",
      "MIN" : -2
    },
    {
      "NAME" : "Shit",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 5,
      "LABEL" : "Color",
      "MIN" : -100
    }
  ],
  "ISFVSN" : "2"
}
*/


#define rotate(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define spiral(u, a, r, t, d) abs(sin(t + r * length(u) + a * (d * atan(u.y, u.x))))
#define sinp(a) .5 + sin(a) * .5


void main() {

	
    vec2 st = (2.0 * gl_FragCoord.xy - RENDERSIZE.xy) / RENDERSIZE.y;
 	st = rotate(-TIME / 10.) * st;
	
	vec3 col;
	float t = TIME*Speed;
    vec2 o = vec2(cos(TIME / 10.), sin(TIME / 2.));
	for (int i = 0; i < 3; i++) {
		t += 0.3 * spiral(vec2(o + st), 16., 16. + Rott * o.x - o.y, -TIME / 100., 1.)
            * spiral(vec2(o - st), 16., 16. + 64. * o.x - o.y, TIME / 100., -1.);
		col[i] = sin(Shit * t - length(st) * 10. * sinp(t));
	}
	
	gl_FragColor = vec4(col, 1.0);
    
}
