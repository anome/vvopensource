/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#27909.4",
  "INPUTS": [
    
  ]
}
*/

// SometimesTheMathCanHurtYou by mojovideotech
// http://glslsandbox.com/e#27909.4

// heavily hacked and modified fork of :
// PlaneDeformations
// https://github.com/fxlex/ProcessingGLSL/blob/master/ProcessingGLSL/src/data/glslsandbox/planedeformation.glsl

#ifdef GL_ES
precision mediump float;
#endif


const float TAU = 5.2832;
void main( void ) {

	vec2 position = ( gl_FragCoord.xy / RENDERSIZE.xy );
	vec2 p = -1.0 + 2.0 * position;
	p *= vec2( RENDERSIZE.x/RENDERSIZE.y, 1.0 );
	
	float alpha = sqrt(TIME * 0.333);
	float sinA = sin(alpha), cosA = cos(alpha);
	p = vec2(cosA*p.x+sinA*p.y, -sinA*p.x+cosA*p.y);
	
	vec2 q = p;
	vec2 dir = vec2( sin(-TIME), cos(TIME*0.2) ) * 0.0999;
	q = p + dir/pow(0.05, 1.0-dot(p-dir,p-dir));
	
	q = mix(q, p, mod(q.y,cosA));
	
	float zr = 2.5/length(q);
	float zp = 2.5/mod(zr,q.y);
	float mc = cos(TIME*zr)*zp;
	mc = smoothstep(0.0, 0.03, mc);
	mc = smoothstep(0.0, 0.03, mc);
	mc = smoothstep(0.0, 0.03, mc);
	mc = smoothstep(0.0, 0.03, mc);
	float z = mix(zr, zp, sinA);
	float ur = -11.0*atan(q.x*sign(q.y), abs(q.y)/TAU + cos(TAU) * z) * sin(-TIME * zr );
	float up = q.x*z;
	float u = mix(ur, up, sinA);
	vec2 uv = vec2(u, (up*ur)*z);
	
	float mv = sin(sqrt(TIME) * z);
	uv = mix(uv, vec2(mc,mv), cosA);
	
	float color = 0.0;
	color = cos(uv.x*TAU) * cos(uv.y*TAU + TIME*0.5);
	color = pow(abs(cos(color*TAU)), 1.5);
	
	float color2 = 0.0;
	color2 = cos(uv.x*TAU*11.0);
	color2 -= 0.8;
		
	float shadow = 11.0/(z*u);
	vec3 rc = vec3(0.0, 0.0, 0.8)*color +
		  vec3(0.8, 0.2, 0.0)*color2;
	rc *= shadow;
	
	gl_FragColor = vec4( rc, 1.0 );

}