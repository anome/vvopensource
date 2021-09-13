/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
  		"generator"
  ],
  "INPUTS": [
    {
      "NAME": "size",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 1,
      "MAX": 5
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 1.6,
      "MIN": -1,
      "MAX": 3
    },
    {
      "NAME": "red",
      "TYPE": "float",
      "DEFAULT": 2.32,
      "MIN": 0,
      "MAX": 3
    },
    {
      "NAME": "green",
      "TYPE": "float",
      "DEFAULT": 2.07,
      "MIN": 0,
      "MAX": 3
    },
    {
      "NAME": "blue",
      "TYPE": "float",
      "DEFAULT": 2.14,
      "MIN": 0,
      "MAX": 3
    }
  ]
}*/


float rand(vec2 co) {
  return fract(tan(dot(co.xy, vec2(2000))) * log2(TIME));
}

void main (void) {
	vec2 v = gl_FragCoord.xy / (size*70.00);
	vec3 brightness = vec3 ( fract(rand(floor(v)) + TIME / 2.1 * speed), 
							fract(rand(floor(v)) + TIME / 2.2 * speed), 
							fract(rand(floor(v)) + TIME / 3.3 * speed));
	brightness *= 0.5 - distance(fract(v), vec2(0.15, 0.15));
	gl_FragColor = vec4(brightness.r*red, brightness.g*green, brightness.b*blue, 1.0);
}