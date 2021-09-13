/*
{
  "CATEGORIES" : [
    "Glitch",
    "Blur"
  ],
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
			"NAME": "hasard",
			"TYPE": "float",
			"MIN": 40.0,
			"MAX": 520.0,
			"DEFAULT": 125.0
		},
		{
			"NAME": "stripes",
			"TYPE": "float",
			"MIN": 1.0,
			"MAX": 32.0,
			"DEFAULT": 8.0
		},
		{
			"NAME": "mixer",
			"TYPE": "float"
		}
  ],
  "PASSES" : [
    {
      "TARGET" : "bufferA",
      "PERSISTENT" : true
    }
  ],
  "ISFVSN" : "2",
  "DESCRIPTION" : "glitch mixed"
}
*/

const lowp float delta_x = 0.05;
const lowp float delta_y = 0.05;


void main() {

	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;

	
	vec2 flow;

	vec4 pixel = IMG_NORM_PIXEL(inputImage, uv);
	
	vec3 intensity = vec3(0.99) - pixel.rgb;

	float position = floor(uv.y * stripes),
		number  = floor(uv.x * hasard),
		bits = 1.0;

	float vidSample = dot( vec3(1.0) , pixel.rgb  * bits * .95);
	float vidSampleDx = dot( vec3(1.0), IMG_NORM_PIXEL(inputImage, uv + vec2(delta_x, 0.0)).rgb ),
		vidSampleDy = dot( vec3(1.0), IMG_NORM_PIXEL(inputImage, uv + vec2(0.0, delta_y)).rgb );

	flow = delta_x * bits * vec2(vidSampleDx - vidSample, vidSample - vidSampleDy);

	intensity *= 0.055;

	if ( PASSINDEX == 0){
		gl_FragColor = mix(pixel,  IMG_NORM_PIXEL(bufferA, uv + vec2( delta_x, delta_y) *  flow), mixer);
	}
	else{
		gl_FragColor = vec4(1.0 - intensity, 1.0);
	}


}
