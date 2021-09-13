
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "intensity",
			"TYPE": "point2D",
			"DEFAULT": [0.3, 0.2],
			"MIN": [-1, -1],
			"MAX": [1, 1]
		}	
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/
// Author @patriciogv - 2015
// Title: Ikeda Data Stream

#ifdef GL_ES
precision mediump float;
#endif

// uniform vec2 u_resolution;
// uniform vec2 u_mouse;
// uniform float u_time;

#define u_resolution RENDERSIZE
#define u_time TIME
#define u_mouse intensity

float random (in float x) {
    return fract(sin(x)*1e4);
}

float random (in vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898,78.233)))* 43758.5453123);
}

float thinger(vec2 st, vec2 v, float t) {
    vec2 p = floor(st+v);
    return step(t, random(100.+p.x*.000001)+random(p.x)*1.2 );
}

vec3 pattern(vec2 st, vec2 v, float t, vec2 offset){
  vec3 c = vec3(0.);
  c.r = thinger(st+offset, v, 0.5+t);
  c.g = thinger(st, v, 0.5+t);
  c.b = thinger(st-offset, v, 0.5+t);
  return c;
}

void main() {
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st.x *= u_resolution.x/u_resolution.y;

    vec2 grid = vec2(100.0,20.);
    st *= grid;

    vec2 ipos = floor(st);  // integer
    vec2 fpos = fract(st);  // fraction

    vec2 vel = vec2(u_time*2.*max(grid.x,grid.y)); // time
    vel *= vec2(-1.,0.0) * random(1.0+ipos.y); // direction

    // Assign a random value base on the integer coord
    vec2 offset = vec2(0.1,0.);

    vec3 color = vec3(0.);
    color = pattern(st, vel, intensity.x, offset);
//    color.r = pattern(st+offset,vel,0.5+u_mouse.x/u_resolution.x);
//    color.g = pattern(st,vel,0.5+u_mouse.x/u_resolution.x);
//    color.b = pattern(st-offset,vel,0.5+u_mouse.x/u_resolution.x);

    // Margins
    color *= step(0.2,fpos.y);

    gl_FragColor = vec4(1.0-color,1.0);
}

// void main()	{
// 	vec4		inputPixelColor;
// 	//	both of these are the same
// 	inputPixelColor = IMG_THIS_PIXEL(inputImage);
// 	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
// 	//	both of these are also the same
// 	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
// 	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
// 	gl_FragColor = inputPixelColor;
// }
