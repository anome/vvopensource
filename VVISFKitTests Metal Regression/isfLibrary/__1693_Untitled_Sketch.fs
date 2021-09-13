/*{
	"CREDIT": "by thedantheman",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		}
	]
}*/

varying vec2 left_coord;
varying vec2 right_coord;
varying vec2 above_coord;
varying vec2 below_coord;

varying vec2 lefta_coord;
varying vec2 righta_coord;
varying vec2 leftb_coord;
varying vec2 rightb_coord;



void main() {
   float sCoord		= vv_FragNormCoord.x;
   float tCoord		= vv_FragNormCoord.y;
   
   float xOffset = 1.0/RENDERSIZE.x;
   float yOffset = 1.0/RENDERSIZE.y;
   
   float center		= IMG_THIS_PIXEL(inputImage).r;
   float topLeft	= IMG_NORM_PIXEL(inputImage, clamp(vec2(sCoord-xOffset, tCoord+yOffset), 0., 1.)).r;
   float left		= IMG_NORM_PIXEL(inputImage, vec2(sCoord-xOffset, tCoord) ).r;
   float bottomLeft	= IMG_NORM_PIXEL(inputImage, vec2(sCoord-xOffset, tCoord - yOffset	) ).r;
   float top		= IMG_NORM_PIXEL(inputImage, vec2(sCoord, tCoord+yOffset) ).r;
   float bottom		= IMG_NORM_PIXEL(inputImage, vec2(sCoord, tCoord-yOffset) ).r;
   float topRight	= IMG_NORM_PIXEL(inputImage, vec2(sCoord+xOffset, tCoord + yOffset	) ).r;
   float right		= IMG_NORM_PIXEL(inputImage, vec2(sCoord+xOffset, tCoord) ).r;
   float bottomRight= IMG_NORM_PIXEL(inputImage, vec2(sCoord+xOffset, tCoord - yOffset	)  ).r;
   
   float dX = topRight + 2.0 * right + bottomRight - topLeft - 2.0 * left - bottomLeft;
   float dY = bottomLeft + 2.0 * bottom + bottomRight - topLeft - 2.0 * top - topRight;
   
   vec3 N = normalize( vec3( dX, dY, 0.01) );
   
   N *= 0.5;
   N += 0.5;
   
   gl_FragColor = vec4(N, 1.0);
}