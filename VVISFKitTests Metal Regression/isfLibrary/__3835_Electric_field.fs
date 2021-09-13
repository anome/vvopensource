/*{
	"CREDIT": "by gosub7777777",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	]
}*/

#define TAU 6.2831853071

vec2 uv;

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void updateField(inout vec2 f, vec3 p) {
    vec2 dispvec = uv-p.xy;
    vec2 dir = normalize(dispvec);
    float len = length(dispvec);
    float mag = p.z/(len*len);
    f += dir*mag;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	uv = fragCoord.xy / RENDERSIZE.xy;
    uv = uv * 2.0 - 1.0;
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    vec2 field = vec2(0.0);
    float time = TIME * 0.01;
    
    vec3 p1 = vec3(sin(time*4.78904365), cos(time*5.142093847), -0.2);
    vec3 p2 = vec3(sin(time*3.32252), cos(time*2.70329481), 0.2);
    vec3 p3 = vec3(sin(time*1.23947), cos(time*0.6598234), -0.25);
    vec3 p4 = vec3(sin(time*0.72783957), cos(time*1.2034242), 0.25);
    vec3 p5 = vec3(sin(time*3.234058), cos(time*3.7583429), 0.1);
    vec3 p6 = vec3(sin(time*2.82094583), cos(time*3.40534298), 0.1);
    updateField(field, p1);
    updateField(field, p2);
    updateField(field, p3);
    updateField(field, p4);
    updateField(field, p5);
    updateField(field, p6);
    
    float direction = atan(field.y, field.x)/TAU;
    float intensity = 1.0-1.0/(1.0+length(field));
    direction = sin(direction * 100.0) * 0.5 + 0.5;
    direction = 1. - pow(direction, 40.);
    vec3 col = hsv2rgb( vec3(0.002, 0.98, direction * 0.6) );
    col = pow(col, vec3(0.45));
    
	fragColor = vec4(col,1.0);
}

void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);
}