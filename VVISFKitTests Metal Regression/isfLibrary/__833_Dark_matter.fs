/*{
	"CREDIT": "by zebox02",
	"DESCRIPTION": "",
	"CATEGORIES": [
		""
	],
	"INPUTS": [
	
	]
}*/

float rand(vec2 n) { 
    return fract(cos(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 n) {
    const vec2 d = vec2(0.0, 1.0);
    vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
    return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

float fbm(vec2 n) {
    float total = 0.0, amplitude = .9;
    for (int i = 0; i < 7; i++) {
        total += sin(noise(n) * amplitude);
        n += n;
        amplitude *= .9;
    }
    return total;
}

void main() {
	float t = TIME;
    const vec3 c1 = vec3(.4, 1., .4);
    const vec3 c2 = vec3(.8, .8, .7);
    const vec3 c4 = vec3(.5, .8, .7);
    const vec3 c3 = vec3(.7, .8, .7);
    const vec3 c5 = vec3(0.2);
    const vec3 c6 = vec3(0.9);
    vec2 p = gl_FragCoord.xy / 100.0;
    float q = fbm(p - t * 0.1);
    vec2 r = vec2(fbm(p + q + t - p.x - p.y), fbm(p + q - t * .9));
    vec3 c = mix(c1, c2, fbm(p + r * t)) + mix(c3, c4, r.x) - mix(c5, c6, r.y);
    gl_FragColor = vec4(c, 1.0);
}