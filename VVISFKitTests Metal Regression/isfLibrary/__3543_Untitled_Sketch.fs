/*{
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    {
      "NAME": "scale",
      "TYPE": "float",
      "MAX": 2.0,
      "MIN": 0.0001,
      "DEFAULT": 0.002
    },
    {
      "NAME": "modVal",
      "TYPE": "float",
      "MAX": 1,
      "MIN": 0.1,
      "DEFAULT": 0.4
    },
    {
      "NAME": "edgeOne",
      "TYPE": "float",
      "MAX": 1.0,
      "MIN": 0.001,
      "DEFAULT": 0.04
    },
    {
      "NAME": "edgeTwo",
      "TYPE": "float",
      "MAX": 1.0,
      "MIN": 0.001,
      "DEFAULT": 0.04
    },
    {
      "NAME": "finalMulti",
      "TYPE": "float",
      "MAX": 1,
      "MIN": 0,
      "DEFAULT": 0.7
    }
  ]
}*/


#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265

void main()
{
	vec4 c;
	float l = 0.0;
	vec2 uv;
	vec2 p=gl_FragCoord.xy/RENDERSIZE;
	p-=0.5;
	p *= scale;
	p.x*=RENDERSIZE.x/RENDERSIZE.y;
	uv=p;
	l=length(p);
	uv/=p/l;
	c=vec4(length(smoothstep(mod(uv, modVal), vec2(edgeOne), vec2(edgeTwo))));

	gl_FragColor=vec4(c/l*finalMulti);
}