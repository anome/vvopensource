/*{
  "DESCRIPTION": "odd/even horizontal pixel sorting",
  "CREDIT": "Andrea Bovo <spleen666@gmail.com>", 
  "CATEGORIES": [
    "Distortion Effect",
    "pixelsort"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "coordX",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.15
    },
    {
      "NAME": "THRESHOLD",
      "TYPE": "float",
      "MIN": 1e-3,
      "MAX": 5e-2,
      "DEFAULT": 2e-2
    }
  ],
  "PASSES": [
    {
      "TARGET": "buffer",
      "persistent": true
    },
    {
        
    }
  ]
}
*/

// isf 
#define time TIME
#define R RENDERSIZE
#define FI FRAMEINDEX

// math
#define PI 3.141592653589793

// luma 
#define HOLYGREY vec4(0.2126, 0.7152, 0.0722, 0.)
#define luma( rgba ) ( dot(rgba, HOLYGREY) )

// gamma
// sRGB -> linear
#define degamma( rgba ) ( rgba * rgba )
// linear -> sRGB
#define gamma( rgba ) (sqrt(rgba) )

#define speed 4.0

void main(){
	vec2 uv =  gl_FragCoord.xy / R.xy;
	vec4 col = vec4(0.);
    //
	if (PASSINDEX == 0)	{
	     float x = coordX;
    	 if ( uv.x > x) {
    	     
    	    if (FI < int(speed * 7.) ) {
    		  gl_FragColor = gamma( IMG_NORM_PIXEL(inputImage, uv));
    		  return;
    	    }
         	
     	    //frame number parity, -1 is odd 1 is even
        	float fParity = mod( float(FI), 2.) * 2. - 1.;
        	
            // every 1/2 pixel on the horizontal axis, will be -1 or 1
            float vp = mod( floor(uv.x * R.x), 2.0) * 2. - 1.;
            
            vec2 dir = vec2(5.0, 0.);
            dir*= fParity * vp;
        	dir/= R.xy;
    		
    		vec4 buff = IMG_NORM_PIXEL(buffer, uv);
    		
    		// we sort
	        vec4 curr = degamma(IMG_NORM_PIXEL(inputImage, uv));
        	vec4 comp = degamma(IMG_NORM_PIXEL(buffer, uv + dir));
        	
        	// prevent the sort from happening on the borders
        	if (uv.x + dir.x < 0.0 || uv.x + dir.x > 1.0) {
        		gl_FragColor = curr;
        		discard;
        	}
        	
        	float gCurr = luma(curr);
        	float gComp = luma(comp);
        	
    	    // the direction of the displacement defines the order of the comparaison 
        	if (dir.x < 0.0) {
        		col = gCurr > THRESHOLD && gComp > gCurr ? comp : curr;
        	} else {
        	    col = gComp > THRESHOLD && gCurr >= gComp ? comp : curr;
        	}
        	
    		gl_FragColor = gamma( col );
    	} 
    	else if (uv.x < x  ){
    	    gl_FragColor = IMG_NORM_PIXEL(inputImage, uv) ;
    	}
	} else if (PASSINDEX == 1)	{
		gl_FragColor =IMG_NORM_PIXEL(buffer, uv);
	}
}
