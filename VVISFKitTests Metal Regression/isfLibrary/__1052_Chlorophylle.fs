/*{
	"CREDIT": "Silvia",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
	{
      "NAME" : "Contrast",
      "TYPE" : "float",
      "MAX" : 0.5,
      "DEFAULT" : 0.45,
      "MIN" : 0.34
    },
    {
      "NAME" : "Snuggle",
      "TYPE" : "float",
      "MAX" : 800.0,
      "DEFAULT" : 289.0,
      "MIN" : 100.0
    },
    {
      "NAME" : "Cells",
      "TYPE" : "float",
      "MAX" : 5.0,
      "DEFAULT" : 2.0,
      "MIN" : 0.0
    }				
	]
}*/
#ifdef GL_ES
precision mediump float;
#endif
float Tempo = sin (TIME/2.0); 
vec3 mod289(vec3 x) { return x - floor(x * (1.0 / (Snuggle - (Tempo-4.0)))) *289.00; }
vec2 mod289(vec2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
vec3 permute(vec3 x) { return mod289(((x*34.0)+1.0)*x); }

float random (in vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

float noise (in vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) +
            (c - a)* u.y * (1.0 - u.x) +
            (d - b) * u.x * u.y;
}

float snoise(vec2 v) {
	
    const vec4 C = vec4(0.211324865405187,  
                        0.366025403784439,  
                        -0.577350269189626,  
                        0.024390243902439); 
    vec2 i  = floor(v + dot(v, C.yy) );
    vec2 x0 = v -   i + dot(i, C.xx);
    vec2 i1;
    i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
    vec4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    i = mod289(i); // Avoid truncation effects in permutation
    vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
        + i.x + vec3(0.0, i1.x, 1.0 ));

    vec3 m = max(Contrast - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
    m = m*m ;
    m = m*m ;
    vec3 x = Cells * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;
    m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
    vec3 g;
    g.x  = a0.x  * x0.x  + h.x  * x0.y;
    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
    return 130.0 * dot(m, g);
}

#define OCTAVES 6
float fbm (in vec2 st) {
    // Initial values
    float value = 0.0;
    float amplitud = .5;
    float frequency = 0.;
    //
    // Loop of octaves
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitud * noise(st);
        st *= 2.;
        amplitud *= .5;
    }
    return value;
}

float turbulence(in vec2 st) {
    float value = 0.0;
    float amplitude = 1.0;
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitude * abs(snoise(st));
        st *= 2.;
        amplitude *= .5;
    }
    return value;
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    st.x *= RENDERSIZE.x/RENDERSIZE.y;
    st.x += fbm(st) * 0.5;
    st.y += fbm(st + vec2(1.0)) * 0.2;
    vec3 color = vec3(0.0,0.8,0.1);
    color = mix (color, vec3(0.02,0.0598,0.16), turbulence(st* 2.0));
    color = mix(color, vec3(0.009,0.060,0.2), turbulence(st* 9.0));
    gl_FragColor = vec4(color,1.0);
}

