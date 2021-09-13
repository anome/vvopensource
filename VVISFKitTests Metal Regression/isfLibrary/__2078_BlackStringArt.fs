/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	]
}*/


// Remixed by MojoVideoTech 2015-05-23
//
// Based on : http://glslsandbox.com/e#25160.0
// Created by Vinicius Graciano Santos - vgs/2014
// https://www.shadertoy.com/view/lsBSDz

#define TAU 6.28318530718

float segment(vec2 p, vec2 a, vec2 b) {
    vec2 ab = b - a;
    vec2 ap = p - a;
    float k = clamp(dot(ap, ab)/dot(ab, ab), 0.001, 1.0);
    return smoothstep(0.001, 2.0/RENDERSIZE.y, length(ap - k*ab) - 0.001);
}

float shape(vec2 p, float angle) {
    float d = 10.0;
    vec2 a = vec2(1.0, 0.8), b;
    vec2 rot = vec2(cos(angle), sin(angle));
    
    for (int i = 0; i < 13; ++i) {
        b = a;
        for (int j = 0; j < 33; ++j) {
        	b = vec2(b.x*rot.x - b.y*rot.y, b.x*rot.y + b.y*rot.x);
        	d = min(d, segment(p,  a, b));
        }
        a = vec2(a.x*rot.x - a.y*rot.y, a.x*rot.y + a.y*rot.x);
    }
    return d;
}

void main(void) {
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec2 cc = (-RENDERSIZE.xy + 1.5*gl_FragCoord.xy) / RENDERSIZE.x;
        
    float col = shape(pow(abs(cc),vec2(1.+sin(TIME*0.03),1.+cos(TIME*0.1))), cos(0.2*sqrt(TIME))*TAU);
    col *= 1.5 + 2.1*pow(uv.x*uv.y*(-0.2-uv.x)*(0.9-uv.y), 0.9);
    
    
	gl_FragColor = vec4(vec3(reflect(col, 0.0 )),1.0);
}