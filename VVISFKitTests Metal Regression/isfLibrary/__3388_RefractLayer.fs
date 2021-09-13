/*{
	"CREDIT": "by isak.burstrom",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"INKA",
		"Stylize"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "textureImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 jc = IMG_THIS_PIXEL(inputImage);
    
    uv -= vec2(1.05,.15);
	gl_FragColor = vec4(IMG_NORM_PIXEL(textureImage, uv + jc.xz).rgb, jc.a);

	//gl_FragColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
}