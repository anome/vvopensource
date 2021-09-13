/*{
  "CATEGORIES": [
    "gradient",
    "heatmap",
    "Color",
    "Film",
    "predator",
    "Examples",
    "Color Effect",
    "multipass",
    "Blur"
  ],
  "CREDIT": "splenooname - Andrea Bovo <spleen666@gmail.com>",
  "DESCRIPTION": "simulating predator ihermal vision.",
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    }
  ],
  "ISFVSN": "2",
  "PASSES": [
    {
      "TARGET": "buffer",
      "DESCRIPTION": "blur image source",
      "WIDTH": "floor($WIDTH/1.0)",
      "HEIGHT": "floor($HEIGHT/1.0)",
      "PERSISTENT": true
    },
     {
      "TARGET": "buffer1",
      "DESCRIPTION": "blur image source",
      "WIDTH": "floor($WIDTH/1.0)",
      "HEIGHT": "floor($HEIGHT/1.0)",
      "PERSISTENT": true
    }
  ]
}*/

// isf shortcuts
#define R RENDERSIZE
#define DELTA TIMEDELTA
#define T TIME

// luma (brightness) 
#define LUMA( rgba ) dot( vec4(0.299, 0.587, 0.114, 0.), rgba )

// blur
#define BLUR_RADIUS 4.0
#define PI 3.141592

// vec2 uv - uv coordinates
// return rgba color, blurred
vec4 blur( vec2 uv ){
    vec2 pixelOffset = 1.0 / R.xy;
    float start = 2.0 / BLUR_RADIUS;
	vec2 scale = 0.5 * BLUR_RADIUS * 2.0 * pixelOffset.xy;
    float w = 1.0 / (BLUR_RADIUS + 1.0);
    vec4 color = vec4(0);
    for (float i = 0.0; i < BLUR_RADIUS; i++) {
        float rad = (PI * 2.0 * (1.0 / BLUR_RADIUS)) * (i + start);
        color += IMG_NORM_PIXEL(inputImage, uv + vec2( sin(rad), cos(rad) ) * scale) * w;
    }
    return color;
}

// float p - any float
// return random 0..1
float hash11(float p){
    p = fract(p * .1031);
    p *= p + 33.33;
    p *= p + p;
    return fract(p);
}

// vec2 uv - uv coordinates
// return 0..1
float hash21(vec2 uv){
    return fract(sin(dot(uv.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

/**
 *  fake heatmap vision. 
 *  vec4 color - the r,g,b,a color
 *  vec2 uv - uv coordinates
 **/
vec4 heat(vec4 color, vec2 uv) {
    //get brightness from color sample
    float brightness = LUMA( color );
    //The human eye is most sensitive to green, so using green 
	float value = smoothstep( 0.0, color.g, distance(color.rb, uv) );
    /*
    *Follow refs, useful for experimenting and mixing with colours:
    *
    * https://www.quora.com/What-color-does-mixing-green-and-blue-make
    * https://thebookofshaders.com/glossary/?search=smoothstep
    
    * facts:
    * - each float color contribution varies from 0 to 1 
    * - smoothstep creates a "soft" interpolation between 0..1, with 2 edges values. 
    */
    float r = smoothstep(0.5, 0.25, value);
    float g = value < 0.25 ? smoothstep(0.0, 0.25, value) : smoothstep(1., 0.5, value);
    float b = smoothstep(0.4, 0.6, value);
	float f =  0.5 + 0.5 * smoothstep(0.0, 0.9, value);
    return f * vec4(r, g, b, 1.);
}


void main() {
	// normalize uv coords [0,1] vertically
	vec2 uv = gl_FragCoord.xy / R.xy;
    // compute noise contribute from vertical uv and time
    float noise = hash21( vec2(uv.y, T*0.1) ) * .08;
    // 1: blurring, no sRGB -> flat here. Empirical choice result based.
    vec4 blurCol = blur(uv);
    // 2: heatamp from blurred image
    vec4 heatCol = heat( blurCol, uv ) * (1. - noise);
    // add a (little) jittering with DELTA time
    heatCol += DELTA * 0.15;
    
    if (PASSINDEX == 0) {
        
    } else if (PASSINDEX == 1) {
        
    }
    // get color from buffer
    vec4 bufferCol = IMG_NORM_PIXEL(buffer, uv);
    // linear interpolation from heatCol to bufferCol: create motion blur
	gl_FragColor = mix( heatCol,  bufferCol, 0.75 + smoothstep(0.1, 0.2, noise) ) ;
}