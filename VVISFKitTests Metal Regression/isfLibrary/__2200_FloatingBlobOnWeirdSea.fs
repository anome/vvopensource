/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "Automatically Converted"
  ],
  "INPUTS" : [
  ],
  "DESCRIPTION" : "Automatically converted from http://glslsandbox.com/e#32769.0"
}
*/

// FloatingBlobOnWeirdSea by mojovideotech
// copied from glslsandbox.com/e#32769.0

#ifdef GL_ES
precision mediump float;
#endif

#extension GL_OES_standard_derivatives : enable

 float TT = TIME * 0.125;

vec2 scene(vec3 p) {
	float d = sin(p.x + TT)*sin(p.y + TT)*sin(p.z);
	vec2 d1 = vec2(length(p) - 1.5 + d, 1.0);
	
	return d1;
}

vec2 map(vec3 p) {
	vec2 d1 = vec2(p.y + 1.0 + 0.5*(cos(p.x+sin(TT/10.)*TT) + sin(p.z)), 0.0);
	vec2 d2 = scene(p);
	
	return d1.x < d2.x ? d1 : d2;
}

vec2 intersect(vec3 ro, vec3 rd) {
	float td = 0.0;
	float mid = 0.0;
	
	for(int i = 0; i < 64; i++) {
		vec2 s = map(ro + rd*td);
		if(s.x == 0.0) break;
		td += s.x;
		mid = s.y;
	}
	
	if(td > 20.0) mid = -1.0;
	return vec2(td, mid);
}

vec3 normal(vec3 p) {
	vec2 h = vec2(0.001, 0.0);
	vec3 n = vec3(
		map(p + h.xyy).x - map(p - h.xyy).x,
		map(p + h.yxy).x - map(p - h.yxy).x,
		map(p + h.yyx).x - map(p - h.yyx).x
	);
	
	return normalize(n);
}

float shadow(vec3 p, vec3 lig) {
	float res = 1.0;
	float td = 0.2;
	
	for(int i = 0; i < 16; i++) {
		float h = map(p + lig*td).x;
		td += h;
		res = min(res, 8.0*h/td);
		if(h == 0.0 || td > 10.0) break;
	}
	
	return clamp(res, 0.0, 1.0);
}

vec3 lighting(vec3 p, vec3 l, vec3 rd) {
	vec3 lig = normalize(l);
	vec3 n = normal(p);
	vec3 ref = reflect(rd, n);
	
	float amb = clamp(0.5+0.5*n.y, 0.0, 0.2);
	float dif = clamp(dot(n, lig*2.), 0.0, 1.0);
	float spe = pow(clamp(dot(ref, lig), 0.0, 1.0), 100.0);
	
	dif *= shadow(p, lig);
	
	vec3 lin = vec3(0);
	
	lin += 0.6*amb*vec3(1);
	lin += 0.825*dif*vec3(1.0, 0.97, 0.85);
	lin += 1.20*spe*vec3(0.55, 0.87, 0.55)*dif;
	
	return lin;
}

mat3 camera(vec3 e, vec3 la) {
	vec3 f = normalize(la - e);
	vec3 r = normalize(cross(vec3(0, 1, 0), f));
	vec3 u = normalize(cross(f, r));
	
	return mat3(r, u, f);
}

void main( void ) {
	vec2 uv = -1.0+2.0*(gl_FragCoord.xy/RENDERSIZE);
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	
	float a = TT*0.3;
	vec3 ro = 7.0*vec3(sin(a), 0.4, cos(a));
	vec3 rd = camera(ro, vec3(sin(TT)))*normalize(vec3(uv, 2.0));
	
	vec2 i = intersect(ro, rd);
	vec3 col = mix(vec3(0, .99, .97), vec3(0, .67, .97), uv.y)*0.7;
	
	if(i.y > -1.0) {
		vec3 p = ro + rd*i.x;
		if(i.y == 0.0) {
			col = mix(vec3(abs(1. * sin(TT * 8. * 1.0/RENDERSIZE.x)),abs(1.27*sin(TT)), abs(1.0*cos(TT*2.4))), vec3(0, 0.0, 0.8), fract(scene(p).x*3.0));
		}
		if(i.y == 1.0) col = vec3(.75, 0, .82);
		
		col *= lighting(p, 5.0*vec3(cos(TT), 1.0, sin(TT)), rd);
	}
	
	gl_FragColor = vec4(col, 1.0);
}
