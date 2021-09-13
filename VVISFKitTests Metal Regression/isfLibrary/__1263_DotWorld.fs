/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "colorshift",
      "TYPE": "point2D",
      "MAX": [ 0.9, 0.9 ],
      "MIN": [ 0.01, 0.0 ]
    },
    {
      "MAX": 300,
      "MIN": 2,
      "DEFAULT": 30,
      "NAME": "size",
      "TYPE": "float"
    },
    {
      "MAX": 2,
      "MIN": -8,
      "DEFAULT": -3,
      "NAME": "brightness",
      "TYPE": "float"
    },
    {
      "MAX": 2,
      "MIN": -2,
      "DEFAULT": 0.5,
      "NAME": "rate",
      "TYPE": "float"
    },
    {
      "MAX": 0.8,
      "MIN": 0.05,
      "DEFAULT": 0.35,
      "NAME": "tint",
      "TYPE": "float"
    }
  ]
}*/

////////////////////////////////////////////////////////////
// DotWorld  by mojovideotech
//
// based on:
// glslsandbox.com/\e#26933.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

void main( void ) {
	float N = size;
	float invN = 1.0/N;
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy ) + mod(TIME,invN) / 3.0;
	vec2 cell = vec2(ivec2(invN * gl_FragCoord.xy));
	vec2 center = N * vec2(cell) + vec2(0.5 * N, 0.5 * N);
	float d = distance(gl_FragCoord.xy, center);
	float c = 1.0 - smoothstep(0.4 * N, 0.45 * N, d * 0.9);
	vec4 bg = vec4(0.9 - tint, 0.1, 0.1 + tint, 1.0);
	float a0 = colorshift.x + 0.5 * sin(0.9 * cell.x + TIME) * sin(cell.y + 5.0 * cos(0.4 * TIME));
	float a1 = colorshift.y + 0.5 * sin(0.1 * cell.y + 10.0 * sin(cell.x) * TIME * rate);
	float y = 0.5 * (a0 + a1);
	vec4 top_bw = vec4(y - tint, y, y + tint, y);
	vec4 top_c = vec4(a0, a1, 0.0 ,1.0);
	float d2 = distance(RENDERSIZE.xy * inversesqrt(-TIME), center);
	float s = smoothstep(-0.5 * N, 3.0 * N, d2) - smoothstep(3.0 * N, 6.0 * N, d2);
	s = step(8.0 * N, d2)-step(9.0 * N, d2) + 1. - step(0.5 * N, d2);
	vec4 top = mix(0.5 * top_bw, top_c, s * s);
	gl_FragColor = mix(bg, top, c);
	gl_FragColor *= (top_c, bg, s + c) / mod(y, TIME) + brightness;
}