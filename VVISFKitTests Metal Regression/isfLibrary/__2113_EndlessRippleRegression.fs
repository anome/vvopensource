/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
  "INPUTS": [
  ]
}
*/

// EndlessRippleRegression by mojovideotech
// based on :
// http://glslsandbox.com/e#26328.4


#ifdef GL_ES
precision mediump float;
#endif


#define WAVE_SPEED 10.
#define ROTA_SPEED 0.*5.*cos(TIME)
#define N_BRANCHES 9.
#define K_DIST TIME

void main( void ) {

	vec2 uv = 2.*vec2(gl_FragCoord.x / RENDERSIZE.x-.5, gl_FragCoord.y / RENDERSIZE.y -.5);
	float dist = length(uv) + 0.5;
	uv.y*=RENDERSIZE.y/RENDERSIZE.x;
	vec2 altuv = uv;
	altuv.y*=-cos(dist*TIME);
	altuv.x/=-sin(dist*TIME);
	
	float r = acos(atan(altuv.y, altuv.x)*N_BRANCHES+ROTA_SPEED + dist*K_DIST + 1.25);
	float g = cos(atan(altuv.x, altuv.y)*N_BRANCHES+ROTA_SPEED + dist*K_DIST + 0.5);
	float b = sin(atan(altuv.y, altuv.x)*N_BRANCHES+ROTA_SPEED + dist*K_DIST + 1.125);
	
	b = mix(b, .0, .0+.25*cos(uv.y*33.));

	vec3 color = vec3(r, g, b);
	
	
	gl_FragColor.brga = vec4( color, 1.0 );

}