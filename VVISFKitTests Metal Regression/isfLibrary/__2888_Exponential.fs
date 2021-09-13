/*{
	"CREDIT": "by Silvia Fabiani",
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
    },
    {
			"NAME": "crescendo",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MAX": 1.0,
			"MIN": 0.0
		}

      ]
}
*/

//////////////////////////////////////////////
// Based on: RegDivTiles  by mojovideotech


void main() {
   vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    float per;
    if (horizontal) { per = uv.x; }
    else { per = uv.y; }
    uv.xy *= crescendo * exp2(ceil(-log2(0.9-per)))-1.; 
	gl_FragColor = IMG_NORM_PIXEL(inputImage,mod(fract(uv),1.0));
	
}
