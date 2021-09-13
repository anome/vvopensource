/*
{
  "CATEGORIES" : [
    "Automatically Converted",
    "GLSLSandbox"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#36696.0"
}
*/

void main( void ) {
	vec2 p = (gl_FragCoord.xy*2.0-RENDERSIZE)/min(RENDERSIZE.x,RENDERSIZE.y);
	float ratio = (RENDERSIZE.x/2.)/(RENDERSIZE.y);
	vec3 destColor = vec3(.6);
	float f = 0.0;
	for(float i = 0.0; i < 10.0; i++){
        	float s = sin(i * 0.628318) * 0.5 * sin(TIME);
        	float c = cos(i * 0.628318) * 0.5 * sin(TIME);
		f += 0.005 / abs(length(p + vec2(c, s)) - 0.5);
	}
	gl_FragColor = vec4(destColor*f, 1.0);
}