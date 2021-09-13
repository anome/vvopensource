
/*{
	"DESCRIPTION": "ported and modified from https://www.shadertoy.com/view/XdXBDH",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "mod1",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "mod2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "mod3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "mod4",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
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
const float PERIOD = 15.;

const float RADIUS = 0.5;

const float LINE_WIDTH = 0.5;

const int NUM_STEPS = 50;

const int TILE_MIN = -5;
const int TILE_MAX = 5;

const float PI = 4. * atan(1.);

// Smooth HSV to RGB conversion 
// https://www.shadertoy.com/view/MsS3Wc
vec3 hsv2rgb_smooth(float hue, float saturation, float value) {
    vec3 rgb = clamp(abs(mod(hue*6.0+vec3(0.0,4.0,2.0),6.0)-3.0)-1.0, 0.0, 1.0);

	rgb = rgb*rgb*(3.0-2.0*rgb);  // Cubic smoothing	

	return value * mix(vec3(1.0), rgb, saturation);
}

vec2 curve(float t, float a, float b, float d) {
	return vec2(
        sin(a * t + d) * cos(t + d),
        cos(b * t) * sin(t + d)
    );
}


void main()	{
    const int numTiles = TILE_MAX - TILE_MIN + 1;
    
    float minRes = min(RENDERSIZE.x, RENDERSIZE.y);
    float scale = float(numTiles) / minRes;

    int tilex = int(-4. + (mod3 * 8.));
    int tiley = int(-4. + (mod4 * 8.));
	vec2 uv = 1.0 - (gl_FragCoord.xy / RENDERSIZE.xy) * 2.;

    if (tiley > 0) {
            uv.y = -uv.y;
        }
	float d =  (TIME / PERIOD) * 2. * PI;
    float hueOffset = 4. * mod2;

    // If the tile coordinates have different parity,
    // only half of the period is needed.
    //bool halfPeriod = mod(float(abs(tileCoord.x * tileCoord.y)), 2.) == 1.;
	bool halfPeriod = abs(mod(float(tilex * tiley), 2.)) == 1.;
	float tPeriod = halfPeriod ? PI : 2. * PI;
    vec2 p1 = curve(0., float(tilex), float(tiley), d) * RADIUS;
	float minDist = 1.0;
    float minDistI;
    int numSteps = halfPeriod ? NUM_STEPS : 2 * NUM_STEPS;
    for (int i = 1; i <= 2* NUM_STEPS; i++) {
        if(i > numSteps) {
            break;
        }
    
        float t = float(i) / float(numSteps) * tPeriod;
        vec2 p2 = curve(t, float(tilex), float(tiley), d) * RADIUS;

		// Distance to line
        vec2 pa = uv - p2;
        vec2 ba = p1 - p2;

        float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);

        vec2 q = pa - ba * h;

        float dist = dot(q, q);
        if (dist < minDist) {
            minDist = dist;
            minDistI = (float(i) - h);
        }
        p1 = p2;
    }

    float hue = fract(hueOffset + minDistI / float(numSteps));

	float v = smoothstep(LINE_WIDTH * scale, 0.0, sqrt(minDist));	
	gl_FragColor = vec4(hsv2rgb_smooth(hue, 1., v), 1.0);
	
}
