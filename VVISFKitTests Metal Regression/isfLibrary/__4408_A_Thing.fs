/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/MsBGDR by w23.  Playing with ray-triangle intersection.",
    "IMPORTED": {
    },
    "INPUTS": [
    ]
}

*/


#define t (TIME * 2.)

#define DETAIL 8

float hash(float v) { return fract(sin(v)*45841.117); }
float noise(float v) {
	float F = floor(v), f = fract(v);
	f = f * f * (3. - 2. *f);
	return mix(hash(F), hash(F+1.), f);
}
float noise(vec2 v) {
	vec2 F = floor(v), f = fract(v);
	float V = F.x + F.y * 117.;
	f = f * f * (3. - 2. *f);
	return mix(mix(hash(V), hash(V+1.), f.x),
			   mix(hash(V+117.), hash(V+118.), f.x), f.y);
}
float fnoise(vec2 v) {
	return .5 * noise(v) + .25*noise(v*1.98) + .125 * noise(v * 4.12);
}

float isect_tri(in vec3 O, in vec3 D, in vec3 v0, in vec3 v1, in vec3 v2, out vec3 n, out vec3 p, out vec3 c) {
	// MÃ¶llerâ€“Trumbore
	vec3 e1 = v1 - v0;
	vec3 e2 = v2 - v0;
	vec3 P, Q, T;
	float det, inv_det, u, v, tt;
	P = cross(D, e2);
	det = dot(e1, P);
	if (abs(det) < 1e-4) return -1.;
	inv_det = 1. / det;
	T = O - v0;
	u = dot(T, P) * inv_det;
	if (u < 0. || u > 1.) return -1.;
	Q = cross(T, e1);
	v = dot(D, Q) * inv_det;
	if (v < 0. || (v+u) > 1.) return -1.;
	tt = dot(e2, Q) * inv_det;
	if (tt <= 0.) return -1.;
	n = normalize(cross(e1, e2));
	p = O + D * tt;
	c = vec3(u, v, 1. - u - v);
	return tt;
}

vec3 lookat(vec3 pos, vec3 at, vec3 rdir) {
	vec3 f = normalize(at - pos);
	vec3 r = cross(f, vec3(0., 1., 0.));
	vec3 u = cross(r, f);
	return mat3(r, u, -f) * rdir;
}

struct material_t {
	vec3 diffuse;
	vec3 specular;
	float specular_power;
};

vec3 light_dir(vec3 at, vec3 normal, vec3 l_eye, material_t m, vec3 l_color, vec3 l_dir) {
	vec3 color = m.diffuse * l_color * max(0.,dot(normal,l_dir));
	
	if (m.specular_power > 0.) {
		vec3 h = normalize(l_dir + l_eye);
		color += l_color * m.specular * pow(max(0.,dot(normal,h)), m.specular_power) * (m.specular_power + 8.) / 25.;
	}
	return color;
}

vec3 v_i(in vec2 v) {
	float phi = v.x * 6.2831;
	float theta = v.y * 3.1416;
	float r = sin(theta);
	float R = 1.5 + .6*sin(phi*2.+t+sin(theta*3.+t));//noise(sin(10.*fract(v)+t*1.4));
	return R * vec3(r*cos(phi), cos(theta), r*sin(phi));
}

void main() {

	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy * 2. - 1.;
	uv.x *= RENDERSIZE.x / RENDERSIZE.y;
	
	float ta = 3.5 * cos(t*.14);
	vec3 at = .3*(vec3(noise(t+.3), noise(ta), noise(t+7.6)) - .5);
	vec3 O = 5. * vec3(cos(ta), cos(t*.3), sin(ta));
	vec3 D = lookat(O, at, normalize(vec3(uv, -2.)));
	
	vec3 color = vec3(0.,.2,.1);
	vec3 N, P, C;
	float minlen = 10000.;
	
	material_t m;
	m.diffuse = vec3(.8);
	m.specular = 10.*vec3(1., .2, 1.);
	m.specular_power = 30.;
	
	for (int i = 0; i < DETAIL*DETAIL; ++i) {
		vec2 vi = vec2(mod(float(i),float(DETAIL)), float(i/DETAIL)) / float(DETAIL);
		vec3 v0 = v_i(vi);
		vec3 v1 = v_i(vi + vec2(1./float(DETAIL), 0.));
		vec3 v2 = v_i(vi + vec2(.0, 1./float(DETAIL)));
		vec3 v3 = v_i(vi + vec2(1./float(DETAIL), 1./float(DETAIL)));
		vec3 n, p, c;
		float len = isect_tri(O, D, v0, v3, v1, n, p, c);
		if (len >= 0. && len < minlen && dot(n,D) >= 0.) {
			N = n;
			P = p;
			C = c;
			minlen = len;
		}
		len = isect_tri(O, D, v0, v2, v3, n, p, c);
		if (len >= 0. && len < minlen && dot(n,D) >= 0.) {
			N = n;
			P = p;
			C = c;
			minlen = len;
		}
	}
	
	if (minlen < 10000.) {
		C = vec3(smoothstep(.3,.8,pow(min(C.x, min(C.y, C.z)), .1)));
		vec3 eyedir = normalize(at-O);
		color = C * m.diffuse * .03;
		color += C * light_dir(P, N, eyedir, m, vec3(1.,.2,.1), normalize(vec3(1.,0.,1.)));
		color += C * light_dir(P, N, eyedir, m, vec3(.0,.2,.9), normalize(vec3(0.,1.,-1.)));
	} else {
		vec2 np = uv*20. + 2.7*at.xy;
		np.x += .2 * cos(fnoise(np) * 6.2831);
		np.y += .2 * sin(fnoise(np) * 6.2831);
		color *= .8 + .2 * fnoise(np);
	}
		
	gl_FragColor = vec4(pow(color, vec3(1./2.2)),1.0);
}
