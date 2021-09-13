
/*{
	"DESCRIPTION": "",
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
			"NAME": "noiseAmplitude",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "noiseFrequency",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
        {
            "NAME": "noiseColorA",
            "TYPE": "color",
            "DEFAULT": [0.004,0.85,1.0,1]
        },
        {
            "NAME": "noiseColorB",
            "TYPE": "color",
            "DEFAULT": [0.93,0,1.0,1]
        },
        {
			"NAME": "noiseOffsetX",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "noiseOffsetY",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "noiseOffsetZ",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "mixNoise",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "brightness",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "contrast",
			"TYPE": "float",
			"DEFAULT": 1.3,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "gamma",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 5.0
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

float rand(vec3 n) { return fract(sin(dot(n, vec3(12.9898, 4.1414, 543.5223))) * 43758.5453); }


vec2 hash( vec2 p ) // replace this by something better
{
	p = vec2( dot(p,vec2(127.1,311.7)), dot(p,vec2(269.5,183.3)) );
	return -1.0 + 2.0*fract(sin(p)*43758.5453123);
}

float noise( vec2 p )
{
    const float K1 = 0.366025404; // (sqrt(3)-1)/2;
    const float K2 = 0.211324865; // (3-sqrt(3))/6;

	vec2  i = floor( p + (p.x+p.y)*K1 );
    vec2  a = p - i + (i.x+i.y)*K2;
    float m = step(a.y,a.x); 
    vec2  o = vec2(m,1.0-m);
    vec2  b = a - o + K2;
	vec2  c = a - 1.0 + 2.0*K2;
    vec3  h = max( 0.5-vec3(dot(a,a), dot(b,b), dot(c,c) ), 0.0 );
	vec3  n = h*h*h*h*vec3( dot(a,hash(i+0.0)), dot(b,hash(i+o)), dot(c,hash(i+1.0)));
    return dot( n, vec3(70.0) );
}


// float noise(vec3 p)
// {
// 	vec3 ip = floor(p);
// 	vec3 u = fract(p);
// 	u = u*u*(3.0-2.0*u);
// 	vec2 o = vec2(0.0, 1.0);
// 	float res = mix(
//     	mix(
//     	    mix(rand(ip),rand(ip+o.yxx),u.x),
//     	    mix(rand(ip+o.xyx),rand(ip+o.yyx),u.x),
//     	    u.y
//         ),
//     	mix(
//     	    mix(rand(ip+o.xxy),rand(ip+o.yxy),u.x),
//     	    mix(rand(ip+o.xyy),rand(ip+o.yyy),u.x),
//     	    u.y
//         ),
//         u.z
//     );
        
// 	return res*res;
// }

float Overlay(float base, float blend)
{
    return base<0.5?(2.0*base*blend):(1.0-2.0*(1.0-base)*(1.0-blend));
}

void main()	{
	//	both of these are the same
	vec2 xy = vec2(gl_FragCoord.xy);
	vec2 res = IMG_SIZE(inputImage);
    vec2 uv = isf_FragNormCoord.xy;

//	vec4 inputPixelColor = IMG_THIS_PIXEL(inputImage);
	vec4 inputPixelColor = IMG_NORM_PIXEL(inputImage, uv);
	inputPixelColor = IMG_PIXEL(inputImage, xy);

    float n = noise(uv*noiseFrequency + vec2(noiseOffsetX,noiseOffsetY))*noiseAmplitude;
    vec4 noiseColor = mix(noiseColorA, noiseColorB, clamp(n,0.0,1.0));
    //vec4 outCol = mix(inputPixelColor, inputPixelColor*noiseColor, mixNoise);

    vec3 c = vec3(Overlay(inputPixelColor.r, noiseColor.r),Overlay(inputPixelColor.g, noiseColor.g),Overlay(inputPixelColor.b, noiseColor.b));
    
    vec4 outCol = mix(inputPixelColor, vec4(c,1.0), mixNoise);
    outCol.rgb = (outCol.rgb - 0.5)*contrast + 0.5 + brightness;
    outCol.rgb = pow(outCol.rgb, vec3(1./gamma));
    
    // outCol = mix(outCol, noiseColor, min(length(outCol.rgb)*.9, 1.0));
    // outCol = inputPixelColor*noiseColor;
	gl_FragColor = outCol;
}
