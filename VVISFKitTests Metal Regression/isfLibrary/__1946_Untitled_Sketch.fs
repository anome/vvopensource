/*{
 "DESCRIPTION": "RGB Average"
,"CREDIT": "by dantheman"
,"CATEGORIES": ["stylized Effect"]
,"INPUTS": [{"NAME": "image0",
"TYPE": "image"},
{"NAME": "image1",
"TYPE": "image"},
{"NAME": "image2",
"TYPE": "image"},
{"NAME": "image3",
"TYPE": "image"},
{"NAME": "image4",
"TYPE": "image"},
{"NAME": "image5",
"TYPE": "image"},
{"NAME": "image6",
"TYPE": "image"},
{"NAME": "image7",
"TYPE": "image"},
{"NAME": "image8",
"TYPE": "image"},
{"NAME": "image9",
"TYPE": "image"}]}*/

void main(void) {
vec2 uv = vv_FragNormCoord.xy;
vec4	final = vec4(0.0);
vec4	srcPixel0 = IMG_NORM_PIXEL(image0,uv);
final+=srcPixel0;
vec4	srcPixel1 = IMG_NORM_PIXEL(image1,uv);
final+=srcPixel1;
vec4	srcPixel2 = IMG_NORM_PIXEL(image2,uv);
final+=srcPixel2;
vec4	srcPixel3 = IMG_NORM_PIXEL(image3,uv);
final+=srcPixel3;
vec4	srcPixel4 = IMG_NORM_PIXEL(image4,uv);
final+=srcPixel4;
vec4	srcPixel5 = IMG_NORM_PIXEL(image5,uv);
final+=srcPixel5;
vec4	srcPixel6 = IMG_NORM_PIXEL(image6,uv);
final+=srcPixel6;
vec4	srcPixel7 = IMG_NORM_PIXEL(image7,uv);
final+=srcPixel7;
vec4	srcPixel8 = IMG_NORM_PIXEL(image8,uv);
final+=srcPixel8;
vec4	srcPixel9 = IMG_NORM_PIXEL(image9,uv);
final+=srcPixel9;
gl_FragColor = vec4(vec3(final)/10.0, 1.0);

}
