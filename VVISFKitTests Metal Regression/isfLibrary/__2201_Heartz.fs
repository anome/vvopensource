/*{
	"CREDIT": "by mojovideotech",
    "CATEGORIES" : [
    "Automatically Converted",
    "GLSLSandbox"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#36754.0"
}
*/

// Heartz  by mojovideotech
// glslsandbox.com/e#36754.0   by Catzpaw 2016

float heart(float x,float y){return (1e-10>pow(x*x+y*y-1.,3.)-x*x*y*y*y)?1.:0.;}

vec2 rot(vec2 p,float a){return p*mat2(cos(a),-sin(a),sin(a),cos(a));}

void main(void){
	vec2 uv=(gl_FragCoord.xy*2.-RENDERSIZE.xy)/min(RENDERSIZE.x,RENDERSIZE.y)*10.;
	uv=rot(uv,-TIME*.2);
	uv=mod(uv,3.)-1.5;
	uv=rot(uv,TIME*.2);
	float s=clamp(sin(TIME*6.)*1.2,1.,2.),c=heart(uv.x*s,uv.y*s);
	gl_FragColor = vec4(vec3(1,.1,.5)*c,1);
}