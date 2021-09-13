/*{
	"CREDIT": "by miguerodri87",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "scaleInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -100.0,
			"MAX": 1.0
		},
		{
			"NAME": "rotateInput",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": -6.28,
			"MAX": 6.28
		},
		{
			"NAME": "translateX",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": -1,
			"MAX": 1
		},
		{
			"NAME": "translateY",
			"TYPE": "float",
			"DEFAULT": 0,
			"MIN": -1,
			"MAX": 1
		}
	]
}*/




void main() {
	
	vec2 uv = isf_FragNormCoord.xy;
	
	uv = translate(uv, translateX, translateY);
	uv = rotateKeepProportions(uv, rotateInput, RENDERSIZE);
	uv = zoom(uv, scaleInput);
	
	vec4 pixel = IMG_NORM_PIXEL(inputImage, uv);
	
//	uv = Rotate(uv, rotateInput);
	
	gl_FragColor = pixel;
}