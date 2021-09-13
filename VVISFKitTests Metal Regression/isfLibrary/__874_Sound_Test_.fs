/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	{
            "NAME": "RADIUS",
            "TYPE": "float",
            "DEFAULT": 0.21,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "DIVISIONS",
            "TYPE": "float",
            "DEFAULT": 0.07,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "PULSE",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "THICKNESS",
            "TYPE": "float",
            "DEFAULT": 0.01,
            "MIN": 0.0,
            "MAX": 1.0
          },
          {
            "NAME": "SPREAD",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0
          }
	]
}*/

// Ported/adapted from "Sound Test by notargs" by notargs: https://www.shadertoy.com/view/4l3GD2

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;

float rand(vec2 co)
{
	return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 uv = (fragCoord.xy - iResolution.xy / 2.0) / iResolution.y;
    float color = THICKNESS / abs(length(uv) - RADIUS - rand(vec2(iGlobalTime, floor(atan(uv.y, uv.x) * DIVISIONS * 100.0))) * (SPREAD*10. - fract(iGlobalTime / (PULSE+.001))) * RADIUS);
	fragColor = vec4(vec3(color),1.0);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}