/*{
	"DESCRIPTION": "trails",
	"CREDIT": "SHELTRON visuals",
	"CATEGORIES": [
		"Distortion Effect"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "read_row",
			"TYPE": "float",
			"MIN": 0.0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		}
	],
	"PERSISTENT_BUFFERS": [
		"buffer"
	],
	"PASSES": [
		{
			"TARGET":"buffer"
		},
		{
			
		}
	]
	
}*/


void main()
{
	vec2 uv =  gl_FragCoord.xy /  RENDERSIZE;
	float tgt_y = mod(TIME*60., RENDERSIZE.y) / RENDERSIZE.y;

	if (PASSINDEX == 0)	{

		
		if ( abs(uv.y - tgt_y) < 1. / RENDERSIZE.y)
			gl_FragColor = texture2D(inputImage, vec2(uv.x, read_row));
		else
			gl_FragColor = IMG_THIS_PIXEL(buffer);
	}
	else if (PASSINDEX == 1)	{	

		// change y coord so that last frame is always at 0
	
		gl_FragColor = texture2D(buffer, vec2(uv.x, fract(uv.y + tgt_y)));
	}
}
