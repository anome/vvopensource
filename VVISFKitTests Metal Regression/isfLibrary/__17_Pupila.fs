
/*{
	"DESCRIPTION": "",
	"CREDIT": "forked from http://glslsandbox.com/e#64575.0",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
    	{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "irisR",
			"TYPE": "float",
			"DEFAULT": 0.42,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisG",
			"TYPE": "float",
			"DEFAULT": 0.52,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisB",
			"TYPE": "float",
			"DEFAULT": 0.78,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisBorderR",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisBorderG",
			"TYPE": "float",
			"DEFAULT": 0.53,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "irisBorderB",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "bright1",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 0.05
		},
		{
			"NAME": "bright2",
			"TYPE": "float",
			"DEFAULT": 0.1,
			"MIN": 0.1,
			"MAX": 1.0
		},
		{
			"NAME": "bright3",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
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

/*
 * Original shader from: https://www.shadertoy.com/view/wtX3Dl
 */



// shadertoy emulation
#define iTime TIME
#define iResolution RENDERSIZE

// --------[ Original ShaderToy begins here ]---------- //
#define PI 3.1415926
#define PUPIL_RADIUS 0.1
#define IRIS_RADIUS 0.45
#define ALIASING 0.01

#define POW2(x) x*x

float hash(vec2 uv)
{
 	return fract(sin(dot(uv, vec2(20.974, 80.2171))) * 300.963);   
}

float noise(vec2 uv)
{
    vec2 f = smoothstep(0.0, 1.0, fract(uv));
 	vec2 uv00 = floor(uv);
    vec2 uv01 = uv00 + vec2(0,1);
    vec2 uv10 = uv00 + vec2(1,0);
    vec2 uv11 = uv00 + 1.0;
    
    float v00 = hash(uv00);
    float v01 = hash(uv01);
    float v10 = hash(uv10);
    float v11 = hash(uv11);
    
    float v0 = mix(v00, v01, f.y);
    float v1 = mix(v10, v11, f.y);
    
    return mix(v0, v1, f.x);
}

float radialNoise(vec2 uv, float dist, float noiseScale)
{
    //x - 360 angle mapped from 0 to 1
 	vec2 polarUV = vec2((atan(uv.y, uv.x) + PI) / (2.0 * PI), dist);
    
    //x - angle mapped drom 0 to noiseScale
    polarUV.x *= noiseScale;
    
    if(polarUV.x + 0.5 > noiseScale) polarUV.x -= noiseScale;
    
    //for debug
    	//polarUV.x = floor(polarUV.x);
    polarUV.x += hash(vec2(polarUV.y, 2.0)) * 834.1;
    
    return noise(polarUV);
}

float irisNoise(vec2 uv, float len, float deltaDist, float densityMultiplier, float distanceStart)
{
 	float dist = len + (radialNoise(uv, deltaDist, 30.0) - 0.5) * 0.1 + distanceStart;
    float lerpVal = fract(dist / deltaDist);
    
    float firstIndex = floor(dist / deltaDist);
    float secondIndex = firstIndex + 1.0;
    
    float firstDist = firstIndex * deltaDist;
    float secondDist = secondIndex * deltaDist;
    
    float firstDensity = firstIndex * densityMultiplier;
    float secondDensity = secondIndex * densityMultiplier;
    
    float firstNoise = radialNoise(uv, firstDist, firstDensity);
    float secondNoise = radialNoise(uv, secondDist, secondDensity);
    
    return mix(firstNoise, secondNoise, lerpVal);
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (2.0 * fragCoord.xy - iResolution.xy) / iResolution.x*2.0*zoom;

    float dist = length(uv+(pointInput*2.0-1.0));
    
    float pupilModifier = (1.0 - sqrt(noise(vec2(iTime * 0.5, 2.1))) * 0.75);
    float vDist = (-IRIS_RADIUS + dist) * pupilModifier;
    
    
    float pupilRadius = PUPIL_RADIUS;
    
    float pupil = 1.0 - smoothstep(pupilRadius, pupilRadius + ALIASING * pupilModifier, vDist + pupilRadius * 2.0);
    float pupil2 = 1.0 - smoothstep(pupilRadius * 1.05, pupilRadius * 1.05 + ALIASING * 4.0 * pupilModifier, vDist + pupilRadius * 2.0);
    float iris = irisNoise(uv, vDist,  0.015, 60.0, 0.15);
    float irisCenter = irisNoise(uv, vDist,  0.015, 120.0, 0.15);
    float irisDetails = irisNoise(uv, vDist,  0.016, 150.0, 0.14);
    float irisBorder = 1.0 - smoothstep(0.0, IRIS_RADIUS, abs(dist - IRIS_RADIUS));
    float irisWhiteBorder = 1.0 - smoothstep(0.0, 0.2 * (1.0 - pow(pupilModifier, 1.4)), abs(dist - IRIS_RADIUS + 0.001));
    float whiteEye = smoothstep(IRIS_RADIUS, IRIS_RADIUS + ALIASING * 3.0, dist);
    
    vec3 eye = smoothstep(-0.5, 1.0, iris) * vec3(irisR, irisG, irisB) * 0.9;
    eye = mix(eye, eye * irisDetails * irisDetails + irisWhiteBorder * 0.7 * vec3(irisBorderR,irisBorderG,irisBorderB), 0.3);
    eye = mix(eye, eye * irisCenter * 0.3, pupil2);
    eye = mix(eye, vec3(0.0), pupil);
    eye = mix(eye, vec3(1.0), whiteEye);
    eye = mix(eye, vec3(irisBorder), 0.4);
    //eye = mix(eye, vec3(irisWhiteBorder) * irisDetails, 0.5);
    
    eye = smoothstep(bright2, bright3, eye);
    
    float reflection = smoothstep(bright1, 0.088, 0.05 - distance(uv, vec2(0.16, 0.22))) * 1.2;
    
    fragColor = vec4(eye + reflection, 1.0);
}
// --------[ Original ShaderToy ends here ]---------- //

void main(void)
{
    mainImage(gl_FragColor, gl_FragCoord.xy);
}