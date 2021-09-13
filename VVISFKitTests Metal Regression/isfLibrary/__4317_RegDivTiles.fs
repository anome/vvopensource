/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
  	"tile effect",
    "escher",
    "tiles",
    "scaling"
  ],
  "DESCRIPTION" : "from https://www.shadertoy.com/view/4dGXDV by roywig.  After Escher's Regular Division of The Plane VI\nhttp://www.wikiart.org/en/m-c-escher/regular-division-of-the-plane-vi",
  "INPUTS" : [
    {
      "TYPE" : "image",
      "NAME" : "inputImage"
    },
    {
      "NAME": "horizontal",
      "TYPE": "bool",
      "DEFAULT": false
    }
  ]
}
*/

//////////////////////////////////////////////
// RegDivTiles  by mojovideotech
//
// mod of :
// www.shadertoy.com/\view/\4dGXDV  by roywig
//
// based on :
// Escher's Regular Division of The Plane
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
/////////////////////////////////////////////

void main() {
   vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float a;
    if (horizontal) { a = uv.x; }
    else { a = uv.y; }
    uv.xy *= exp2(ceil(-log2(1.-a)));
	gl_FragColor = IMG_NORM_PIXEL(inputImage,mod(fract(uv),1.0));
}

