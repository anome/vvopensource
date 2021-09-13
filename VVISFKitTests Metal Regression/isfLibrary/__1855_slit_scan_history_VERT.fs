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
	float tgt_x = mod(TIME*50., RENDERSIZE.x) / RENDERSIZE.x;

	if (PASSINDEX == 0)	{

		
		if ( abs(uv.x - tgt_x) < 1. / RENDERSIZE.x)
			gl_FragColor = texture2D(inputImage, vec2(read_row,uv.y));
		else
			gl_FragColor = IMG_THIS_PIXEL(buffer);
	}
	else if (PASSINDEX == 1)	{	

		// change y coord so that last frame is always at 0
	
		gl_FragColor = texture2D(buffer, vec2(fract(uv.x + tgt_x),uv.y));
	}
}
