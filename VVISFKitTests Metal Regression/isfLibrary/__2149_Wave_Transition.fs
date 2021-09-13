/*{
	"CREDIT": "by Dan Moore",
	"DESCRIPTION": "GL Transition",
	"CATEGORIES": [
		"transition"
	],
	"INPUTS": [
		{
			"NAME": "startImage",
			"TYPE": "image"
		},
		{
			"NAME": "endImage",
			"TYPE": "image"
		},
	    {
	      "NAME": "size",
	      "LABEL": "Size",
	      "TYPE": "float",
	      "MIN": 0,
	      "MAX": 0.5,
	      "DEFAULT": 0.1
	    },
	   	{
	      "NAME": "progress",
	      "LABEL": "Progress",
	      "TYPE": "float",
	      "MIN": 0,
	      "MAX": 1,
	      "DEFAULT": 0
	    }
	]
}*/


float rand (vec2 co) {
  return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec4 transition (vec2 uv) {
  float r = rand(vec2(0, uv.y));
  float m = smoothstep(0.0, -size, uv.x*(1.0-size) + size*r - (progress * (1.0 + size)));
  return mix(
    IMG_NORM_PIXEL(startImage, uv),
    IMG_NORM_PIXEL(endImage, uv),
    m
  );
}

void main() {
	gl_FragColor = transition(isf_FragNormCoord.xy);
}