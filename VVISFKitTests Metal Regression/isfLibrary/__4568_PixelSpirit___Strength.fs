// License: MIT 2021 by NERDDISCO (Tim Pietrusky)

/*{
	"DESCRIPTION": "",
	"CREDIT": "NERDDISCO",
	"ISFVSN": "2",
	"CATEGORIES": [
		""
	],
	"INPUTS": [
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "movement",
			"TYPE": "float",
			"DEFAULT": 1.45,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "amount",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 50.0
		},
		{
			"NAME": "glitch",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": 0,
			"MAX": 1.0
		},
		{
			"NAME": "x1",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "x2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "n",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -1.0,
			"MAX": 1.0
		}
	]
}*/

#define PI 3.1415926535897932384626433832795

// https://gist.github.com/ayamflow/c06bc0c8a64f985dd431bd0ac5b557cd
vec2 rotateUV(vec2 uv, float rotation) {
    float mid = 0.5;
    return vec2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}

float strength(vec2 st) {
    float strength = cos(st.y * PI * amount) * cos(movement * TIME);
    float x1 = (x1 + strength) * scale;
    float x2 = (x2 + strength) * scale;
    float wave = smoothstep(x1, x2, abs(abs(st.x) - n));
    
    return wave;
}

void main()	{
    // RENDERSIZE = resolution
    // TIME = time
    
    //vec3 color = vec3(1.0, 0.0, 0.0);
    vec4 color = IMG_THIS_PIXEL(inputImage);
    vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
    
    st = rotateUV(st, rotation);

    color += strength(st);
    

    
    vec4 colorA = vec4(0.149,0.141,0.912, 1.0);
    //vec4 colorB = vec4(1.000,0.833,0.224, 1.0);
    
    color /= step(cos(glitch - TIME * 1.5), color);
    
    //color = mix(colorA, color, st.y);
    

    
	gl_FragColor = vec4(color);
}