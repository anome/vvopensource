/*{
	"CREDIT": "by gosub7777777",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	    {
      		"NAME": "restartNow",
      		"TYPE": "event"
    	},
		{
			"NAME": "colorA",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				0.0,
				0.0,
				1.0
			]
		},
		{
			"NAME": "colorB",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "center",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			],
			"MIN": [-1.0,-1.0],
			"MAX": [1.0,1.0]
		},
		{
			"NAME": "fallOff",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0,
			"MAX": 4.0
		},
		{
			"NAME": "colorFreq",
			"TYPE": "float",
			"DEFAULT": 40.0,
			"MIN": 0,
			"MAX": 40
		},
		{
			"NAME": "flowerRotationSepeed",
			"TYPE": "float",
			"DEFAULT": 4.0,
			"MIN": 0,
			"MAX": 4
		},
		{
			"NAME": "maxIterations",
			"TYPE": "float",
			"DEFAULT": 100.0,
			"MIN": 1.0,
			"MAX": 200.0
		}
	],
	"PASSES": [
		{
			"TARGET": "bufferA",
			"PERSISTENT": true,
			"FLOAT": true,
			"WIDTH": "16.0",
			"HEIGHT": "16.0"
		},
		{

		}
	]
}*/

#define ITERATION 200.0

vec2 rotate(vec2 p, vec2 pivot, float a){
	float s = sin(a);
	float c = cos(a);
	p -= pivot;
	p = vec2(p.x*c-p.y*s, p.x*s+p.y*c);
	p += pivot;
	return p;
}

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

float r = 2.0;
float r2 = r*r;
float flowerRotation = TIME * flowerRotationSepeed;

vec3 fractal(vec2 uv, vec2 center, float zoom, float rotation, float maxIterations){
	vec3 col = vec3(0);
	
	vec2 c = center + uv * zoom;
	c = rotate(c, center, rotation);
	
	vec2 z;
	vec2 zPrevious;
	float iter;
	vec2 inner = vec2(0.0,0.0);
	
	for(float i = 0.0; i < ITERATION; i+= 1.0){
		// zPrevious = rotate(z,vec2(0,0),flowerRotation);
		z = vec2(z.x*z.x-z.y*z.y, 2.0*z.x*z.y) + c;
		inner += z;
		iter = i;
		if(dot(z,z) > r2) break;
		if(i >= maxIterations) break;
	}
	
	// if(iter > maxIterations) return vec3(0.0);
	float dist = length(z);
	//float fracIter = (dist - r) / (r2 - r);
	float fracIter = log2( log(dist) / log(r));
	// float fracIter = log2(log2(dot(z,z)));
	
	
	//iter -= fracIter;
	float m = sqrt(iter / ITERATION);
	// vec3 col = pal( m * colorFreq + TIME*0.1  , vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
	col = mix(colorA.rgb, colorB.rgb, sin(m * colorFreq * 5.0 + TIME*0.1) * 0.5 + 0.5 );
	col = vec3(m);
	// fracIter = smoothstep(fallOff,0.0,fracIter);	
	// col *= vec3(fracIter);
	// float angle = atan(z.x,z.y);
	// float waves = 1.0-sin(angle * 2.0 + flowerRotation * 4.0)*0.2; 
	// col *= waves;
	col += vec3(inner/maxIterations,0.0);
	// col *= 5.0;
	
	
	return col;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    
    if (PASSINDEX == 0)	{
    	
		if(FRAMEINDEX == 0 || (restartNow)){
			fragColor = vec4(1.0,1.0,0.0,1.0);
		}else{
    		vec4 paramB = IMG_PIXEL(bufferA, vec2(0,0));
			// fragColor = vec4(bufferB.rgb,1.0);
			paramB.xy += center * TIMEDELTA * 100.0;
			paramB.w += zoom * TIMEDELTA * 100.0;
			paramB.z += rotation * TIMEDELTA * 100.0;
			
			// fragColor = paramB;
			// fragColor = paramB;
			// fragColor = vec4(0.0,1.0,1.0,1.0);
		}
	}
	
    if (PASSINDEX == 1 )	{
		
		vec3 col = vec3(0.0);
        
	    vec2 uv = (-RENDERSIZE.xy + 2.0*fragCoord.xy)/RENDERSIZE.y;
	    float time = TIME;
	    
	    //Specular
		//uv.x = abs(uv.x);
		
	    vec4 paramB = IMG_PIXEL(bufferA, vec2(0,0));
	    
	    // col = fractal(uv, center, zoom, rotation, maxIterations);
	    col = fractal(uv, paramB.xy, paramB.w, paramB.z, maxIterations);
    	// fragColor = vec4( col, 1.0 );
    	
    	fragColor = vec4(paramB.rgb, 1.0);
    	// fragColor = vec4(1.0);
	}
}

void main() {
	mainImage(gl_FragColor, gl_FragCoord.xy);
}