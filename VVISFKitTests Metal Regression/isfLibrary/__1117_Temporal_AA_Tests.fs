/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "noiseSampler",
			"TYPE": "image"
		},
		{
			"NAME": "SCALE",
			"TYPE": "float",
			"MIN": -10,
			"MAX": 10
		},
		{
			"NAME": "dthresh",
			"TYPE": "float",
			"MIN": 0,
			"MAX":5
		},
		{
			"NAME": "minRad2",
			"TYPE": "float",
			"MIN": 0,
			"MAX":2
		},
		{
			"NAME": "dhue",
			"TYPE": "float",
			"MIN": 0,
			"MAX":3.14
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": 0,
			"MAX":2000
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"MIN": 0,
			"MAX":100
		},
		{
			"NAME": "e0",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 10
		},
		{
			"NAME": "x",
			"TYPE": "float",
			"MIN": -1,
			"MAX": 1
		},
		{
			"NAME": "y",
			"TYPE": "float",
			"MIN": -1,
			"MAX": 1
		},
		{
			"NAME": "z",
			"TYPE": "float",
			"MIN": -1,
			"MAX": 1
		}
	],
		
	"PERSISTENT_BUFFERS": [
		"swap",
		"reconstruction",
		"thisFrameTrace"
		
	],
	"PASSES": [

		{
			"WIDTH:": "$WIDTH/2",
			"HEIGHT": "$HEIGHT/2",
			"TARGET":"thisFrameTrace"
		},
		{
			"WIDTH:": "$WIDTH",
			"HEIGHT": "$HEIGHT",
			"TARGET":"swap"
		},
		{
			"WIDTH:": "$WIDTH",
			"HEIGHT": "$HEIGHT",
			"TARGET":"reconstruction"
		},
		{
		}
		
	]
}*/


#define PI 3.1415

#define MAX_ITER  100

vec4 scale = vec4(SCALE, SCALE, SCALE, abs(SCALE)) / minRad2;



float DE(vec3 pos)
{
    //	return (length(pos)-4.0);
    
    vec4 p = vec4(pos,1);
    vec4 p0 = p;  // p.w is the distance estimate
    
    for (int i = 0; i < 10; i++)
    {
    	// box fold
        p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;
        
        // sphere folding: if (r2 < minRad2) p /= minRad2; else if (r2 < 1.0) p /= r2;
        float r2 = dot(p.xyz, p.xyz);
        p *= clamp(max(minRad2/r2, minRad2), 0.0, 1.0);
        
        // scale, translate
        p = p*scale + p0;
    }
    
    return ((length(p.xyz) - abs(SCALE - 1.0)) / p.w);
}

vec3 gradient(vec3 p, float d) {
    
    vec2 e = vec2(0., d );
    return normalize(
                     vec3(
                          DE(p+e.yxx) - DE(p-e.yxx),
                          DE(p+e.xyx) - DE(p-e.xyx),
                          DE(p+e.xxy) - DE(p-e.xxy)
                          )
                     );
}


float AO(vec3 p, vec3 n, float delta){
    const int steps = 3;
    
    float a = 0.0;
    float weight = 0.75;
    float m;
    for(int i=1; i<=steps; i++) {
        float d = (float(i) / float(steps)) * delta;
        a += weight*(d - DE(p + n*d));
        weight *= 0.5;
    }
    return clamp(1.0 - a, 0.0, 1.0);
}


void raymarch()
{

    //raymarcher!
    vec3 camera = vec3(x, y, z) * e0;
    vec3 point;
    bool hit = false;
    
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy - 0.5 + 0.5 / RENDERSIZE.xy;
    
    vec4 n = IMG_NORM_PIXEL(noiseSampler, fract(TIME + TIME * uv * sin(TIME * 100.)));
    vec2 jitter = 2.0 * (n.xy - 0.5) /RENDERSIZE.xy;
    
    vec3 ray = normalize( vec3(uv + jitter, 1.0) );


    float t = 0.0; 
    float iter = 0.0; 
    
    
    // ray stepping
    for(int i = 0; i < MAX_ITER; i++) {
        point = camera + ray * t;
        float dist = DE(point);
        
        if (dist <exp ( t * dthresh)/ pow(10.0, 3.4) ) //exp ( t * dthresh) )
            break;

        iter ++;
        t += dist;
    }
    
    vec3 normal = gradient(point, exp ( t * dthresh)/ pow(15.0, 3.4) );
    
    float ao = AO(point, normal, length(point)/6.);
    
    
    float shade = abs(dot(normal, ray));

    
    gl_FragColor = vec4( iter/float(MAX_ITER) , shade, t , ao );
	
}

void reconstruct()
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy + 0.5 / RENDERSIZE.xy;
	
	    vec4 n = IMG_NORM_PIXEL(noiseSampler, fract(TIME + TIME * uv * sin(TIME * 100.))) - 0.5;

	vec4 thisFrame  = IMG_NORM_PIXEL(thisFrameTrace, uv + 1.0 * n.xy / RENDERSIZE.xy);
	vec4 model  = IMG_NORM_PIXEL(swap, uv);
	
	if ( uv.x > 0.5)
	{
		gl_FragColor = thisFrame;

	}
	else
	{
		float d = thisFrame.x - model.x ;
		float alpha = 1.0 / exp(d * d) ;
		gl_FragColor =  model * alpha + thisFrame * (1.0 - alpha);

	}
}

void copy()
{gl_FragColor = IMG_NORM_PIXEL(reconstruction, gl_FragCoord.xy / RENDERSIZE.xy);}


void draw() {
	
vec4 r = IMG_NORM_PIXEL(reconstruction, gl_FragCoord.xy / RENDERSIZE.xy  + 0.5 / RENDERSIZE.xy);

gl_FragColor = 1. - r.xxxx;


}

void main() {
	if (PASSINDEX == 0)	{
		raymarch();	
	} else if (PASSINDEX == 1) {
		copy();

	} else if (PASSINDEX == 2) {
		reconstruct();
	} else {
		draw();
	}
	
}
