/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
	
	{
			"NAME": "SCALE",
			"TYPE": "float",
			"DEFAULT": 128.0,
			"MIN": 0.0,
			"MAX": 1920.0
	},
	{
			"NAME": "DENSITY",
			"TYPE": "float",
			"DEFAULT": 0.95,
			"MIN": 0.0,
			"MAX": 1.0
	},
	{
			"NAME": "LAYERS",
			"TYPE": "float",
			"DEFAULT": 6.0,
			"MIN": 0.0,
			"MAX": 32.0
	},
	{
			"NAME": "LENGTH",
			"TYPE": "float",
			"DEFAULT": 8.0,
			"MIN": 0.0,
			"MAX": 100.0
	},
	{
			"NAME": "LENGTH_SCALE",
			"TYPE": "float",
			"DEFAULT": 0.9,
			"MIN": 0.0,
			"MAX": 2.0
	},
	{
			"NAME": "FADE",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
	},
	{
			"NAME": "DROP_COLOR",
			"TYPE": "color",
			"DEFAULT": [
				0.54,
				0.8,
				0.94,
				1.0
			]
	},
	{
			"NAME": "BG_COLOR",
			"TYPE": "color",
			"DEFAULT": [
				0.23,
				0.38,
				0.6,
				1.0
			]
	},
	{
			"NAME": "SPEED",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.0,
			"MAX": 1.0
	}
	]
}*/

//Based on "Rain Generator" by TheBinaryCodeX: https://www.shadertoy.com/view/lt33zM

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;


// const int LAYERS = 6;             // number of layers of drops

// const float SCALE =128.0;        // overall scale of the drops
// const float LENGTH = 8.0;        // length of the drops
// const float LENGTH_SCALE = 0.9;   // how much the drop length changes every layer
// const float FADE = 0.6;           // how much the drops fade every layer

// const float SPEED = 8.0;          // how fast the drops fall

// const vec3 DROP_COLOR = vec3(0.54, 0.8, 0.94);
// const vec3 BG_COLOR = vec3(0.23, 0.38, 0.6);

highp float rand(vec2 co)
{
	highp float a = 12.9898;
	highp float b = 78.233;
	highp float c = 43758.5453;
	highp float dt = dot(co.xy, vec2(a, b));
	highp float sn = mod(dt, 3.14);

	return fract(sin(sn) * c);
}

float rainFactor(vec2 uv, float scale, float dripLength, vec2 offset, float cutoff)
{
    vec2 pos = uv * vec2(scale, scale / dripLength) + offset;
    vec2 dripOffset = vec2(0, floor(rand(floor(pos * vec2(1, 0))) * (dripLength - 0.0001)) / dripLength);
    float f = rand(floor(pos + dripOffset));
    
    return step(cutoff, f);
}

vec4 over(vec4 a, vec4 b)
{
    return vec4(mix(b, a, a.a).rgb, max(a.a, b.a));
}

void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
	vec2 uv = fragCoord.xy / iResolution.xy;
    float aspect = iResolution.x / iResolution.y;
    uv.x *= aspect;
    
    vec4 finalColor = vec4(0);
    
    float dropLength = LENGTH;
    float alpha = 1.0;
    
    for (int i = 0; i < 32; i++)
    {
    	if (i > int(LAYERS)) {break;}
    	
        float f = rainFactor(uv, SCALE, dropLength, vec2(SCALE * float(i), iGlobalTime * SPEED * 155.), DENSITY);
        
        vec4 color = vec4(DROP_COLOR.xyz, f * alpha);
        
        finalColor = over(finalColor, color);
        
        dropLength *= LENGTH_SCALE;
        alpha *= FADE;
    }
    
    finalColor = over(finalColor, vec4(BG_COLOR.xyz, 1.0));
    
	fragColor = finalColor;
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}