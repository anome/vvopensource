/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "Playmodes",
	"CATEGORIES": [
		"Joshua Batty"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "xdiv",
			"TYPE": "float",
			"DEFAULT": 10,
			"MIN": 1,
			"MAX": 20
		},
		{
			"NAME": "ydiv",
			"TYPE": "float",
			"DEFAULT": 10,
			"MIN": 1,
			"MAX": 20
		}
	],
	"PASSES": [
    {
      "TARGET": "RenderBufferA",
      "PERSISTENT": true,
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "Pass 0"
    },
    {
      "TARGET": "RenderBufferB",
      "PERSISTENT": true,
      "WIDTH": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "DESCRIPTION": "Pass 1"
    }
	]
}*/

// Ported from https://www.shadertoy.com/view/4syGDR#

#define backbuffer(uv) (IMG_NORM_PIXEL(RenderBufferA,uv).rgb)
#define webcam(uv) (IMG_NORM_PIXEL(inputImage,uv).rgb)

    
void main() {
	
		if (PASSINDEX==0) {
			vec2 uv = isf_FragNormCoord.xy;

    		vec2 uv2 = vec2(uv.x-(1./xdiv),uv.y); //Take the picture from one step left
    		if (uv.x < (1./xdiv)) { //At left
        		uv2 = vec2(uv.x+((xdiv-1.)/xdiv),uv.y-(1./ydiv)); //Take the right to left and one step up
    		}

    		vec3 outc = (uv.x < (1./xdiv) && uv.y < (1./ydiv)) ? webcam(fract(uv*vec2(xdiv,ydiv))) : backbuffer(uv2);
    		gl_FragColor = vec4(outc,1.0);
		}
  	 	 else if (PASSINDEX==1) {
			///---------------- FINAL OUTPUT IMAGE
			vec2 uv = isf_FragNormCoord.xy;
		 	gl_FragColor = IMG_NORM_PIXEL(RenderBufferA, uv);
	  	 }

}