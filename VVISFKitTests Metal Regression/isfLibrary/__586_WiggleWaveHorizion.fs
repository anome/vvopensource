/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#26504.0"
}
*/


#ifdef GL_ES
precision mediump float;
#endif


float f(float arg) {
	return 0.5+sin(arg*10.0+TIME*1.5)*0.05+sin(arg*15.0+TIME)*0.05+sin(arg*40.0-TIME)*0.01;
}

void main( void ) {
	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy ); position.x *= RENDERSIZE.x/RENDERSIZE.y; 
	vec3 color = (position.y>f(position.x))?vec3(0.4, 0.5, 0.9)*0.5/distance(position, vec2(1.0, 0.4)):vec3(0.0, 0.2, 0.6)-pow((f(position.x)-position.y), (sin(TIME)+1.9)*0.6);
	gl_FragColor = vec4(color, 1.0 );
}