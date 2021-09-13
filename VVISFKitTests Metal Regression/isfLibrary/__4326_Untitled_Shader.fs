
/*{
	"DESCRIPTION": "transition",
	"CREDIT": "mcr",
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
	
}*/

void main()
{
    vec2 pt = isf_FragNormCoord;
    vec4 srcPixel = IMG_NORM_PIXEL(inputImage,pt);
    float val = (pt.x > fract(TIME)) ? 1.0 : 0.0;
    gl_FragColor = vec4(srcPixel.r,srcPixel.g,srcPixel.b,val) ;

}
