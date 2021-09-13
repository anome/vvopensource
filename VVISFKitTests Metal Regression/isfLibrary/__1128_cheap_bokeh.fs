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
      "DEFAULT": 1.0,
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
      "MIN": 1,
      "MAX": 100
    }
  ]
}*/

#define R RENDERSIZE

#define rot mat2( 0.54, 0.84, -0.84, 0.54);

// radial bokeh

vec3 bokeh(sampler2D texture, vec2 uv, float radius, float amount){
	vec3 acc = vec3(0.); 
	vec3 div = vec3(0.);
	vec3 bokeh = vec3(0.);
	vec3 col = vec3(0.);
	
    vec2 pixel = 1.0 / R.xy;  
    vec2 angle = vec2(radius, radius);
    	
    float r = 1.0;
    
    amount += radius * MULTIPLIER;
    
    float n = ITERATIONS;
	for (float j = 0.0; j < 100.0; j += 1.0) {  
    	if ( j > ITERATIONS ) { break; }
    	
        r += 1. / r;
        
		angle *= rot;
        // (r-1.0) === sqrt(0, 1, 2, 3...)
        
        col = IMG_PIXEL(texture, uv * vec2(R.xy) + pixel*(r-1.) * angle).rgb;
		bokeh = pow(col, vec3(INTENSITY)) * amount;
		
		//add contrast
		acc += col * bokeh;
		div += bokeh;
	}
	return acc / div;
}

void main() {
   vec2 uv = gl_FragCoord.xy/ R.xy;
   gl_FragColor = vec4( bokeh(inputImage, uv , RADIUS, 10.), 1.0);
}