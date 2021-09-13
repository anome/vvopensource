/*{
  "DESCRIPTION": "horizontal pixel sort",
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
      "DEFAULT": 0.15
    },
    {
      "NAME": "THRESHOLD",
      "TYPE": "float",
      "MIN": 0.010,
      "MAX": 0.5,
      "DEFAULT": 0.05
    }
  ]
}
*/

// isf shorts
#define time TIME
#define R RENDERSIZE
#define FI FRAMEINDEX

#define PI 3.141592653589793

// luma & gamma macros
#define GAMMA 2.0
#define HOLYGREY vec4(0.2126, 0.7152, 0.0722, 0.)
#define luma( rgba ) ( dot(rgba, HOLYGREY) )

// sRGB -> linear
#define degamma( rgba ) ( pow(max(rgba, 0.), vec4(GAMMA)) )
// linear -> sRGB
#define gamma( rgba ) ( pow(max(rgba, 0.), vec4(1./GAMMA)) )


void main(){
	vec4 col = vec4(0.);
         
        
        vec2 p = -1.0 + 2.0 * gl_FragCoord.xy / R.xy;
        vec2 uv;
        
        float a = atan(p.y,p.x);
        float r = sqrt(dot(p,p));
        
        //a += sin(0.5*r-0.5*time );

        uv.x = 0.1/r + 0.01 * time;
        uv.y = a/(3.1416);

        gl_FragColor = IMG_NORM_PIXEL(inputImage, uv);
   
}
