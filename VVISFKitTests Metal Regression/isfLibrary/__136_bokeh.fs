/*{
  "DESCRIPTION": "cheap bokeh shader",
  "CREDIT": "based on https://www.shadertoy.com/view/4d2Xzw",
  "CATEGORIES": [
    "bokeh"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "LABEL": "MULTIPLIER",
      "NAME": "MULTIPLIER",
      "TYPE": "float",
      "DEFAULT": 100,
      "MIN": 0,
      "MAX": 500
    },
    {
      "LABEL": "RADIUS",
      "NAME": "RADIUS",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0,
      "MAX": 5
    },
    {
      "LABEL": "INTENSITY",
      "NAME": "INTENSITY",
      "TYPE": "float",
      "DEFAULT": 4,
      "MIN": 1,
      "MAX": 10
    },
    {
      "LABEL": "ITERATIONS",
      "NAME": "ITERATIONS",
      "TYPE": "float",
      "DEFAULT": 60,
      "MIN": 30,
      "MAX": 100
    }
  ]
}*/

#define R RENDERSIZE
#define SAMPLES 100.0
#define MAT_ROT mat2( 0.54, 0.84, -0.84, 0.54);

// radial bokeh

vec4 bokeh(sampler2D texture, vec2 uv, float radius, float amount){
	
	vec4 acc = vec4(0.), 
		div = vec4(0.),
	 	bokeh = vec4(0.),
		col = vec4(0.);
	
    vec2 pixel = 1.0 / R.xy,  
    	vangle = vec2(radius);
    	
    float r = 1.0;
    
    amount += radius * MULTIPLIER;

	for (float j = 0.0; j < SAMPLES; j+=1.0){  
    	
    	if ( j > ITERATIONS ) { 
    		break; 
    	}
    	
        r += 1. / r;
        
		vangle *= MAT_ROT;
        // (r-1.0) == to sqrt(0, 1, 2, 3...)
        
        col = IMG_PIXEL(texture, uv * vec2(R.xy) + pixel*(r-1.) * vangle);
		bokeh = pow(col, vec4(INTENSITY)) * amount;
		
		//add contrast
		acc += col * bokeh;
		div += bokeh;
	}
	return acc / div;
}

void main() {
   vec2 uv = gl_FragCoord.xy/ R.xy;
   vec4 col = bokeh(inputImage, uv, RADIUS, 10.0);
   gl_FragColor = col;
}