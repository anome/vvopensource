/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#26907.0",
  "INPUTS": []
}*/

// RotoSponge by mojovideotech
// http://glslsandbox.com/e#26907.0

#ifdef GL_ES
precision mediump float;
#endif

vec2 rot(vec2 p, float a) {
	return vec2(
		p.x * cos(a) - p.y * sin(a),
		p.x * sin(a) + p.y * cos(a));
}

float map(vec3 p) {
	p.xz = rot(p.xz, TIME * 0.025);
	return length(mod(p.xy, 2.0) - 1.0) - 0.7 + (cos(p.z * 20.3)+ cos(p.x* 10.3)+ cos(p.y * 10.3) ) * 0.05;
}

void main( void ) {
	vec2 uv = ( gl_FragCoord.xy / RENDERSIZE.xy ) * 2.0 - 1.0;
	uv.x *= RENDERSIZE.x / RENDERSIZE.y;
	vec3 pos = vec3(0.0, 0.0, -0.5 + (TIME*0.125));
	vec3 dir = normalize(vec3(uv, 1.0));
	dir.xy = rot(dir.xy, TIME * 0.3);
	//dir.zy = rot(dir.zy, TIME * 0.03);
	float t = 0.0;
	for(int i = 0; i < 60; i++) {
		t += map(pos + dir * t);
	}
	vec3 ip = pos + dir * t;
	gl_FragColor = vec4(t * 0.1) + map(ip + 0.1) + pow(vec4(dir, 1.0) * 0.1, vec4(1.5));

}