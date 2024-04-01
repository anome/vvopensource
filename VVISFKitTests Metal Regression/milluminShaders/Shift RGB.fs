/*
{
  "CREDIT": "by Anomes",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "xOffset",
      "TYPE": "float",
      "MIN": -1.0,
      "MAX": 1.0,
      "DEFAULT": 0.1
    },
    {
      "NAME": "yOffset",
      "TYPE": "float",
      "MIN": -1.0,
      "MAX": 1.0,
      "DEFAULT": 0.1
    }
  ]
}
*/


void main(void)
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 posR = uv-vec2(xOffset,yOffset);
	vec2 posG = uv;
	vec2 posB = uv+vec2(xOffset,yOffset);
	vec4 colorR = vec4(0.);
	if( 0. <= posR.x  && posR.x <= 1. && 0. <= posR.y && posR.y <= 1. )
	{
		colorR = IMG_NORM_PIXEL(inputImage, posR);
	}
	vec4 colorG = IMG_NORM_PIXEL(inputImage, posG);
	vec4 colorB = vec4(0.);
	if( 0. <= posB.x  && posB.x <= 1. && 0. <= posB.y && posB.y <= 1. )
	{
		colorB = IMG_NORM_PIXEL(inputImage, posB);
	}
	gl_FragColor = vec4(colorR.r, colorG.g, colorB.b, colorG.a);
}


