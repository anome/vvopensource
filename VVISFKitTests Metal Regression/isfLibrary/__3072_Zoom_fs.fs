/*{
  "CREDIT": "by Dan Moore",
  "CATEGORIES": [
    "Distortion Effect",
    "Geometry Adjustment"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "level",
      "TYPE": "float",
      "MIN": 0.01,
      "MAX": 10,
      "DEFAULT": 0.5
    },
    {
      "NAME": "center",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ]
    }
  ]
}*/

void main() {
	//normalized frag coord
	vec2 loc = gl_FragCoord.xy;
	// loc.x *= RENDERSIZE.y/RENDERSIZE.x;
	loc.x /= RENDERSIZE.x;
	loc.y /= RENDERSIZE.y;
	vec2 modifiedCenter = center;
	loc.x = (loc.x - modifiedCenter.x)*(1.0/level) + modifiedCenter.x;
	loc.y = (loc.y - modifiedCenter.y)*(1.0/level) + modifiedCenter.y;
	if ((loc.x < 0.0)||(loc.y < 0.0)||(loc.x > 1.0)||(loc.y > 1.0))	{
		gl_FragColor = vec4(0.0);
	}
	else	{
		gl_FragColor = IMG_NORM_PIXEL(inputImage,loc);
	}
}
