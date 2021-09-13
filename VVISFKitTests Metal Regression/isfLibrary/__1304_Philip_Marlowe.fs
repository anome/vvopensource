/*{
  "DESCRIPTION": "Small essay about b/w photography post/treatment via shaders - Blondes, Bottles, and Brass Knuckles: The Legacy of Philip Marlowe",
  "CREDIT": "Andrea Bovo <spleen666@gmail.com>", 
  "CATEGORIES": [
    "Distortion Effect",
    "Warp",
    "Black White",
    "black",
    "white",
    "noir",
    "grain", 
    "portrait",
    "photography"
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
      "DEFAULT": 0.5
    },
    {
      "NAME": "feedbackLoop",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.9
    },
    {
      "NAME": "passThrough",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1,
      "DEFAULT": 0.9
    },
    {
      "NAME": "grainAmount",
      "TYPE": "float",
      "MIN": 6,
      "MAX": 16,
      "DEFAULT": 8
    },
    {
      "NAME": "zoomAmount",
      "TYPE": "float",
      "MIN": 0.0,
      "MAX": 2.0,
      "DEFAULT": 1.0
    },
    {
      "NAME": "rotationAmount",
      "TYPE": "float",
      "MIN": -5,
      "MAX": 5,
      "DEFAULT": 1.34
    },
    {
      "NAME": "invert",
      "TYPE": "bool"
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

// 
#define time TIME
#define R RENDERSIZE

#define PI 3.141


/**Tnear luminance is calculated as a weighted sum of the three linear-intensity values. 
 * The sRGB color space is defined in terms of the CIE 1931 linear luminance Ylinear, 
 * which is given by */
 
 /** These three particular coefficients represent the intensity (luminance) perception 
 * of typical trichromat humans to light of the precise Rec. 709 additive 
 primary colors (chromaticities) that are used in the definition of sRGB. */
 // luma & gamma macros
#define GAMMA 2.0

// r * 0.2162 + g * 07152 + b * 0.0722
#define luma( rgba ) ( dot(rgba, vec4(0.2126, 0.7152, 0.0722, 0.)) )

//#define gammao( rgba, gm ) ( gm > 1.0 ? rgba * rgba : sqrt(rgba) ) 

// sRGB -> linear
#define degamma( rgba ) ( pow(max(rgba, 0.), vec4(GAMMA)) )
// linear -> sRGB
#define gamma( rgba ) ( pow(max(rgba, 0.), vec4(1./GAMMA)) )

// easy peasy grain function
// time  
// uv - [0..1] coords
// strength
vec4 grain(float time, vec2 uv, float strength){
    float x = (uv.x + 4.) * (uv.y + 4.) * (time *10.);
    vec4 grain = vec4( mod((mod(x, 13.) + 1.) * (mod(x, 123.) + 1.), 0.01) - 0.005 );
    return grain * strength;
}

// rotate matrix - deg (clockwise)
mat2 rotate( float deg ) {
	float theta = radians(deg);
	float s = sin(theta);
	float c = cos(theta);
    return mat2(c, -s, s, c);
}

void main(){
	vec2 uv =  gl_FragCoord.xy / R.xy;
	vec4 col = vec4(0.);
    //
	if (PASSINDEX == 0)	{
	    float x = coordX;
    	if ( uv.x > (x + 0.001)) {
    	    // get original color from image
    		vec4 originalColor =  IMG_NORM_PIXEL(inputImage, uv);
    		// SRGB -> linear ( decompress gamma)
    		originalColor = degamma( originalColor );
    		// get luminance(luma)
    		float originalLuma = luma(originalColor);
    		// to greyscale
    		originalColor = vec4(vec3(originalLuma), 1.0);
     		// warp uv coords
     		vec2 uvWarp = (uv - 0.5) * zoomAmount;
    	    uvWarp *= rotate(rotationAmount);
    		uvWarp += 0.5;
    	    // get color from buffer, use warped uv
     		vec4 bufferColor = degamma(IMG_NORM_PIXEL(buffer, uvWarp)); 
     		
    		float _feedback = feedbackLoop;
     		float bufferLuma = luma(bufferColor);
     		
    		if(passThrough > 0.0) {
    			// apply passtrough compare luminance of original image/buffer 
    			if(originalLuma > 1. - passThrough && originalLuma > bufferLuma){
    			    _feedback = 0.;
    			} 
    			//_feedback = smoothstep(1. - passThrough, warpedLuma, originalLuma);
    		}
    		// _feedback = 0: original image
    		// _feedback = 1: warped image
    		col = mix(originalColor + grain(time, uv, grainAmount), bufferColor, _feedback);
    		// linear -> SRGB ( compress gamma)
    	    gl_FragColor = gamma(col );
    	} 
    	else if (uv.x == (x - 0.001)){
    	    gl_FragColor = vec4(0.0);  
    	} 
    	else if (uv.x < x  ){
    	    col = gamma( grain(time, uv, grainAmount) + IMG_NORM_PIXEL(inputImage, uv) ) ;
    	    gl_FragColor = col;
    	}
	} 
	// write to buffer (already gamma corrected)
	else if (PASSINDEX == 1)	{
		gl_FragColor = IMG_THIS_PIXEL(buffer);
	}
}
