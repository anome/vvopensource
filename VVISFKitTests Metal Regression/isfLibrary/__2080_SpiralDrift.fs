/*
{
  "CATEGORIES": [
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    
  ]
}
*/

// SpiralDrift by mojovideotech
// based on :
// http://glslsandbox.com/e#8531.0
#ifdef GL_ES
precision mediump float;
#endif


void main( void ) {

	vec2 posScale = vec2((log2(TIME/RENDERSIZE.y),RENDERSIZE.x)/sqrt(exp2(TIME/RENDERSIZE.x))/RENDERSIZE.xy);
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy );

	float sum = 0.;
	float qsum = 0.;
	
	for (float i = 0.; i < 33.; i++) {
		float x2 = i*i*.3165+(sin(log2(TIME)/6.)*i*0.1)+.5;
		float y2 = i*.161235+(tan(cos(TIME)/9.)*i*0.1)+.5;
		vec2 p = (fract(position-vec2(x2,y2))-vec2(sin(TIME*0.09)))/posScale;
		float a = atan(p.y,p.x);
		float r = length(p)*500.;
		float e = exp(-r*.0925);
		sum += sin(r+a+TIME)*e;
		qsum += e;
	}
	
	float color = sum/qsum;
	
	gl_FragColor.gbra = vec4(cos(color+(TIME*0.333))*.2 + .1, tan(color+(TIME*.67))*.5 + .2, sin(color+(TIME*0.5))* .6 + .3, 1.0 );
}