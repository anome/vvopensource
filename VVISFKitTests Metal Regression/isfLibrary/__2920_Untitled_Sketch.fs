/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"filter"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

void main() {
	
	float xOffset = 0.1;
	float yOffset = 0.1;
	float sCoord= isf_FragNormCoord.x;
    float tCoord= isf_FragNormCoord.y;
  	float center= IMG_NORM_PIXEL(inputImage, vec2( sCoord, tCoord ) ).r;
    float topLeft	= IMG_NORM_PIXEL(inputImage, vec2(sCoord - xOffset	, tCoord - yOffset	) ).r;
    float left= IMG_NORM_PIXEL(inputImage, vec2(sCoord - xOffset	, tCoord	) ).r;
    float bottomLeft	= IMG_NORM_PIXEL(inputImage, vec2(sCoord - xOffset	, tCoord + yOffset	) ).r;
    float top= IMG_NORM_PIXEL(inputImage, vec2(sCoord	, tCoord - yOffset	) ).r;
    float bottom= IMG_NORM_PIXEL(inputImage, vec2(sCoord	, tCoord + yOffset	) ).r;
    float topRight	= IMG_NORM_PIXEL(inputImage, vec2(sCoord + xOffset	, tCoord - yOffset	) ).r;
    float right= IMG_NORM_PIXEL(inputImage, vec2(sCoord + xOffset	, tCoord	) ).r;
    float bottomRight= IMG_NORM_PIXEL(inputImage, vec2(sCoord + xOffset	, tCoord + yOffset	) ).r;
                                       
    float dX = topRight + 2.0 * right + bottomRight - topLeft - 2.0 * left - bottomLeft;
    float dY = bottomLeft + 2.0 * bottom + bottomRight - topLeft - 2.0 * top - topRight;
                                       
    vec3 N = normalize( vec3( dX, dY, 0.01) );
                                       
    N *= 0.5;
    N += 0.5;
                                       
    gl_FragColor.rgb = N;
    gl_FragColor.a = 1.0;
}