/*
{
  "CATEGORIES" : [
    "Stylize"
  ],
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
      {
      "NAME" : "delta",
      "TYPE" : "float",
       "DEFAULT": 0.01,
      "MIN": 0.0,
      "MAX": 1.0
    }
  ],
 
  "ISFVSN" : "2"
}
*/


//	Adapted from https://www.shadertoy.com/view/4lV3RG


void main() {
	vec2 blend_uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 uv = vec2(blend_uv.x, blend_uv.y);
	vec4 inPix = IMG_NORM_PIXEL(inputImage, uv);
	vec3 intensity = 1.0 - inPix.rgb;
	
	float vidSample = dot(vec3(1.0), inPix.rgb);
	// float delta = 0.105;
	float vidSampleDx = dot(vec3(1.0),IMG_NORM_PIXEL(inputImage, uv + vec2(delta, 0.0)).rgb);
	float vidSampleDy = dot(vec3(1.0),IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, delta)).rgb);
	    
	vec2 flow = delta * vec2 (vidSampleDy - vidSample, vidSample);
	intensity = 0.05 * intensity + 0.95 * (1.0 - IMG_NORM_PIXEL(inputImage,uv + vec2(-delta, delta) * flow).rgb);
	
	gl_FragColor = vec4(1.0 - intensity,1.0);
}
