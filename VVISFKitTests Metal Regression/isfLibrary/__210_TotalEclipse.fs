/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted",
    "GLSLSandbox"
  ],
  "INPUTS" : [

  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#42084.1"
}
*/

////////////////////////////////////////////////////////////
// TotalEclipse  by mojovideotech
//
// based on:
// glslsandbox/e#42084.1  by Brandon Fogerty
//
// License: 
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



float circle( vec2 uv, vec2 pos, float scale )
{
	return length(pos - uv) * scale;
}

vec3 sun(vec2 uv, vec2 pos)
{
	float t = 1.0 - smoothstep( 0.2, 1.0, circle(uv, pos, 0.9));
	vec3 fc = t * vec3( 4.0, 2.0, 1.0);
	
	return fc;
}

vec3 moon(vec2 uv, vec2 pos)
{
	float t = 1.0 - smoothstep( 0.3, 1.0, circle(uv, pos, 1.2 ));
	vec3 fc = t * vec3( 100.0, 100.0, 200.0);
	
	return fc;
}

void main( void ) {

	vec2 uv = ( gl_FragCoord.xy / RENDERSIZE.xy ) * 2.0 - 1.0;
	uv.x *= RENDERSIZE.x / RENDERSIZE.y;

	float t = sin(TIME * 0.1 + sin(TIME * 0.1)) * 0.25 + 0.5;
	vec2 moonPos = vec2(mix(-3.0, 3.0, t), 0.0);
	
	
	vec3 sunColor = sun(uv, vec2(0.0));
	vec3 moonColor = moon(uv, moonPos);
	
	vec3 fc = (sunColor - moonColor);
	

	gl_FragColor = vec4( fc, 1.0 );

}