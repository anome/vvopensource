// License: MIT 2021 by NERDDISCO (Tim Pietrusky)

/*{
	"DESCRIPTION": "",
	"CREDIT": "NERDDISCO",
	"ISFVSN": "2",
	"CATEGORIES": [
		""
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "rotate",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "keep_aspect_ratio",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "cutoff",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "middle_x",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "middle_y",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/

// https://gist.github.com/ayamflow/c06bc0c8a64f985dd431bd0ac5b557cd
vec2 rotateUV(vec2 uv, float rotation, vec2 mid) {
    return vec2(
      cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
      cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
    );
}

void main()	{
	vec2 uv = vec2(0.0);
	vec4 color = vec4(0.0);
	
	if (keep_aspect_ratio) {
        // Inspired by https://www.shadertoy.com/view/3dXBR7
	    uv = rotateUV(gl_FragCoord.xy, rotate, vec2(middle_x, middle_y) * RENDERSIZE);
	    color = IMG_NORM_PIXEL(inputImage, uv / RENDERSIZE.xy);
	} else {
	    uv = rotateUV(gl_FragCoord.xy / RENDERSIZE.xy, rotate, vec2(middle_x, middle_y));
	    color = IMG_NORM_PIXEL(inputImage, uv);
	}
	
	// Don't show the rest of the inputImage
	if (cutoff && ((uv.x < 0.0) || (uv.y < 0.0) || (uv.x > RENDERSIZE.x) || (uv.y > RENDERSIZE.y))) {
		color = vec4(0.0);
	}

	gl_FragColor = vec4(color);
}