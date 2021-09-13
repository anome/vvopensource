
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "greenImg",
			"TYPE": "image"
		},
		{
			"NAME": "yellowImg",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/



// grain
// https://www.shadertoy.com/view/4t2fRz
#define SHOW_NOISE 0
#define SRGB 0
// 0: Addition, 1: Screen, 2: Overlay, 3: Soft Light, 4: Lighten-Only
#define BLEND_MODE 2
#define SPEED 1.0
#define INTENSITY 0.075
// What gray level noise should tend to.
#define MEAN 0.0
// Controls the contrast/variance of noise.
#define VARIANCE 0.8

vec3 channel_mix(vec3 a, vec3 b, vec3 w) {
    return vec3(mix(a.r, b.r, w.r), mix(a.g, b.g, w.g), mix(a.b, b.b, w.b));
}

float gaussian(float z, float u, float o) {
    return (1.0 / (o * sqrt(2.0 * 3.1415))) * exp(-(((z - u) * (z - u)) / (2.0 * (o * o))));
}

vec3 madd(vec3 a, vec3 b, float w) {
    return a + a * b * w;
}

vec3 screen(vec3 a, vec3 b, float w) {
    return mix(a, vec3(1.0) - (vec3(1.0) - a) * (vec3(1.0) - b), w);
}

vec3 overlay(vec3 a, vec3 b, float w) {
    return mix(a, channel_mix(
        2.0 * a * b,
        vec3(1.0) - 2.0 * (vec3(1.0) - a) * (vec3(1.0) - b),
        step(vec3(0.5), a)
    ), w);
}

vec3 soft_light(vec3 a, vec3 b, float w) {
    return mix(a, pow(a, pow(vec3(2.0), 2.0 * (vec3(0.5) - b))), w);
}

vec4 grain1() {
    vec2 uv =isf_FragNormCoord.xy ;// - vec2(0.5) ;
    vec4 color =IMG_THIS_PIXEL(greenImg);
    #if SRGB
    color = pow(color, vec4(2.2));
    #endif
    
    float t = TIME * float(SPEED);
    float seed = dot(uv, vec2(12.9898, 78.233));
    float noise = fract(sin(seed) * 43758.5453 + t);
    noise = gaussian(noise, float(MEAN), float(VARIANCE) * float(VARIANCE));
    
    #if SHOW_NOISE
    color = vec4(noise);
    #else    
    // Ignore these mouse stuff if you're porting this
    // and just use an arbitrary intensity value.
    float w = 0.75;
    if (0.5 > 0.0) {
        w = 0.5 * uv.y;
        w *= step(uv.x, 0.5);
    }
	
    vec3 grain = vec3(noise) * (1.0 - color.rgb);
    
    #if BLEND_MODE == 0
    color.rgb += grain * w;
    #elif BLEND_MODE == 1
    color.rgb = screen(color.rgb, grain, w);
    #elif BLEND_MODE == 2
    color.rgb = overlay(color.rgb, grain, w);
    #elif BLEND_MODE == 3
    color.rgb = soft_light(color.rgb, grain, w);
    #elif BLEND_MODE == 4
    color.rgb = max(color.rgb, grain * w);
    #endif
        
    #if SRGB
    color = pow(color, vec4(1.0 / 2.2));
    #endif
    #endif
    return color;
}

vec4 grain_uv(vec4 c) {
    vec2 uv =isf_FragNormCoord.xy ;// - vec2(0.5) ;
    vec4 color =c;
    #if SRGB
    color = pow(color, vec4(2.2));
    #endif
    
    float t = TIME * float(SPEED);
    float seed = dot(uv, vec2(12.9898, 78.233));
    float noise = fract(sin(seed) * 43758.5453 + t);
    noise = gaussian(noise, float(MEAN), float(VARIANCE) * float(VARIANCE));
    
    #if SHOW_NOISE
    color = vec4(noise);
    #else    
    // Ignore these mouse stuff if you're porting this
    // and just use an arbitrary intensity value.
    float w = 0.75;
    
    vec3 grain = vec3(noise) * (1.0 - color.rgb);
    
    #if BLEND_MODE == 0
    color.rgb += grain * w;
    #elif BLEND_MODE == 1
    color.rgb = screen(color.rgb, grain, w);
    #elif BLEND_MODE == 2
    color.rgb = overlay(color.rgb, grain, w);
    #elif BLEND_MODE == 3
    color.rgb = soft_light(color.rgb, grain, w);
    #elif BLEND_MODE == 4
    color.rgb = max(color.rgb, grain * w);
    #endif
        
    #if SRGB
    color = pow(color, vec4(1.0 / 2.2));
    #endif
    #endif
    return color;
}

#define ITER_DIST 4
#define saturate(x) (clamp((x), 0.0, 1.0))


float random (in vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
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

float fbm (in vec2 st, float amp) {
    // Initial values
    float value = 0.0;
    float amplitude = amp;
    //
    // Loop of octaves
    for (int i = 0; i < 4; i++) {
        value += amplitude * noise(st);
        st *= 4.3;
        amplitude *= .5;
    }
    return value;
}

// RGB ramp stolen from Ferris
vec4 aberrationColor(float f)
{
	f = f * 3.0 - 1.5;
	return vec4(saturate(vec3(-f, 1.0 - abs(f), f)),1.0);
}

vec2 distort1(vec2 uv, float i) {
    float dx = fbm(vec2(uv.y, TIME/20.0), i)/2.0;
    return vec2(uv.x + dx, uv.y);
}
vec2 distort2(vec2 uv, float i) {
    float dx = fbm(vec2(uv.y, TIME/10.0), i)/2.0;
    return vec2(uv.x + dx, uv.y);
}

vec4 distortGreen(vec2 uv) {
    const float step_size = 1.0 / (float(ITER_DIST) - 1.0);
    float t = step_size; //* hash2(uv + sin(iTime)); // We pseudo randomize the step to have some dithering pattern.

    vec4 sum_color = vec4(0.0);
    vec4 sum_weight = vec4(0.0);
    for (int i = 0; i < ITER_DIST; ++i) {
	    vec4 weight = aberrationColor(t);
        sum_weight += weight;
        vec2 distortedUV = distort1(uv,  0.1+(float(i)/float(ITER_DIST))/32.0);
        vec4 img = IMG_NORM_PIXEL(greenImg, distortedUV);
        sum_color += weight *img;
        t += step_size;
    }

    sum_color /= sum_weight;
    return sum_color;
}

vec4 distortYellow(vec2 uv) {
    const float step_size = 1.0 / (float(ITER_DIST) - 1.0);
    float t = step_size; //* hash2(uv + sin(iTime)); // We pseudo randomize the step to have some dithering pattern.

    vec4 sum_color = vec4(0.0);
    vec4 sum_weight = vec4(0.0);
    for (int i = 0; i < ITER_DIST; ++i) {
	    vec4 weight = aberrationColor(t);
        sum_weight += weight;
        vec2 distortedUV = distort2(uv,  0.1+(float(i)/float(ITER_DIST))/32.0);
        vec4 img = IMG_NORM_PIXEL(yellowImg, distortedUV);
        sum_color += weight *img;
        t += step_size;
    }

    sum_color /= sum_weight;
    return sum_color;
}




vec4 layerText(vec4 front, vec4 back){
    

    return front * front.a + back * (1.0 - front.a);
}







void main()	{
    /*
	vec4		inputPixelColor;
	//	both of these are the same
	inputPixelColor = IMG_THIS_PIXEL(inputImage);
	inputPixelColor = IMG_PIXEL(inputImage, gl_FragCoord.xy);
	
	//	both of these are also the same
	inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	inputPixelColor = IMG_THIS_NORM_PIXEL(inputImage);
	
	inputPixelColor = grain1();
	gl_FragColor = inputPixelColor;
	*/
	vec4 greenColor = IMG_THIS_PIXEL(greenImg);
//	vec4 yellowColor = IMG_THIS_PIXEL(yellowImg);
	vec4 distortG = distortGreen(isf_FragNormCoord.xy);
	vec4 distortY = distortYellow(isf_FragNormCoord.xy);
//	gl_FragColor = (layerText(distortG,layerText(distortY,vec4(1.0))));

	gl_FragColor = grain_uv(greenColor);
}
