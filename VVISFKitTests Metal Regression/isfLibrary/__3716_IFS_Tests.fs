/*{
  "CREDIT": "by You",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": [
    {
      "NAME": "colormap",
      "TYPE": "image"
    },
    {
      "NAME": "noiseSampler",
      "TYPE": "image"
    },
    {
      "NAME": "SCALE",
      "TYPE": "float",
      "MIN": 0.5,
      "MAX": 1.5,
      "DEFAULT": 1.2
    },
    {
      "NAME": "dthresh",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 5,
      "DEFAULT": 1
    },
    {
      "NAME": "minRad2",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 2,
      "DEFAULT": 0.1
    },
    {
      "NAME": "dhue",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3.14
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 15,
      "DEFAULT": 5
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 3.14,
      "DEFAULT": 1.2
    },
    {
      "NAME": "iterCount",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 10,
      "DEFAULT": 5
    },
    {
      "NAME": "x",
      "TYPE": "float",
      "MIN": -3,
      "MAX": 3,
      "DEFAULT": 1.2
    },
    {
      "NAME": "y",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 5,
      "DEFAULT": 3
    },
    {
      "NAME": "z",
      "TYPE": "float",
      "MIN": -1,
      "MAX": 5
    }
  ],
  "PASSES": [
    {
      "WIDTH:": "$WIDTH/2",
      "HEIGHT": "$HEIGHT/2",
      "TARGET": "thisFrameTrace",
      "persistent": true
    },
    {
      "WIDTH:": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "TARGET": "swap",
      "persistent": true
    },
    {
      "WIDTH:": "$WIDTH",
      "HEIGHT": "$HEIGHT",
      "TARGET": "reconstruction",
      "persistent": true
    },
    {}
  ]
}*/


#define PI 3.1415

#define MAX_ITER  50

vec4 scale = vec4(SCALE, SCALE, SCALE, abs(SCALE)) / minRad2;


// float DE(vec3 pos)
// {

//     vec4 p = vec4(pos,1);
//     vec4 p0 = p;  // p.w is the distance estimate
    
//     for (int i = 0; i < 10; i++)
//     {
//     	// box fold
//         p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;
        
//         // sphere folding: if (r2 < minRad2) p /= minRad2; else if (r2 < 1.0) p /= r2;
//         float r2 = dot(p.xyz, p.xyz);
//         p *= clamp(max(minRad2/r2, minRad2), 0.0, 1.0);
        
//         // scale, translate
//         p = p*scale + p0;
//     }
    
//     return ((length(p.xyz) - abs(SCALE - 1.0)) / p.w);
// }
vec2 rot2D (vec2 q, float a)
{
  return q * cos (a) + q.yx * sin (a) * vec2 (-1., 1.);
}


float DE(vec3 p){
 
    
    vec3 offs = vec3(x, y, z); // Offset point.

    
    float s = SCALE;
    
    float d = 1e5; // Distance.
    
    
    p  = abs(fract(p*.5)*2. - 1.); // Standard spacial repetition.
     
    
    float amp = 1./s; // Analogous to layer amplitude.

    for(int i=0; i<8; i++){
        if( float(i) > iterCount)
          break;
          
          
        // Rotating.
        p.xy = rot2D(p.xy, rotation);
        p.yz = rot2D(p.yz, dhue);
        
        p = abs(p);
		
        p.xy += step(p.x, p.y)*(p.yx - p.xy);
        p.xz += step(p.x, p.z)*(p.zx - p.xz);
        p.yz += step(p.y, p.z)*(p.zy - p.yz);
 
        // Stretching about an offset.
		p = p*s + offs*(1. - s);
        
		// Branchless equivalent to:
        // if( p.z < offs.z*(1. - s)*.5)  p.z -= offs.z*(1. - s);
        p.z -= step(p.z, offs.z*(1. - s)*.5)*offs.z*(1. - s);
        
 
        p=abs(p);
        
        d = min(d,max(max(p.x, p.y), p.z) * amp);
        
        amp /= s; // Decrease the amplitude by the scaling factor.
    }
 
 	return d - minRad2 / 2.0 * SCALE; // .35 is analous to the object size.
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



float softshadow( in vec3 ro, in vec3 rd)
{
    float res = 1.0;
    float t = exp ( -4.0 *  dthresh ) + 0.001;
    
    for( int i = 0; i < 5; i ++)
    {
        float h = DE(ro + rd*t);
        
        if (h < exp ( -4.0 *  dthresh ))
            return 0.0;
        
        res = min( res, 10.0 * h/t );
        t += h;
    }
    return res;
}

void raymarch()
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    vec4 n = IMG_NORM_PIXEL(noiseSampler, fract(TIME + TIME * uv * sin(TIME * 100.)));
    vec2 jitter =  (n.xy - 0.5);
    
    vec3 rd = (vec3(2.*gl_FragCoord.xy - RENDERSIZE.xy + jitter, RENDERSIZE.x));
    
    // Barrel distortion;
    rd = normalize(vec3(rd.xy, sqrt(rd.z*rd.z - dot(rd.xy, rd.xy)*0.2)));
	
	// vec2 coord = / vec2(1.0, RENDERSIZE.x/RENDERSIZE.y);
    // rd = normalize(vec3((uv - 0.5) , 1.0));
    
    // Rotating the ray with Fabrice's cost cuttting matrix. I'm still pretty happy with this also. :)
    // vec2 m = sin(vec2(1.57079632, 0) + TIME/50.);
    // rd.xy = rd.xy*mat2(m.xy, -m.y, m.x);
    // rd.xz = rd.xz*mat2(m.xy, -m.y, m.x);
    
    vec3 ro = vec3(0.0, 0.0, TIME * speed);
    // vec3 ro = vec3(0.0, 0.0, -2.0);

    vec3 point;
    bool hit = false;
    


    float t = 0.0; 
    float iter = 0.0; 
    
    
    // ray stepping
    for(int i = 0; i < MAX_ITER; i++) {
        point = ro + rd * t;
        float dist = DE(point);
        
        if (dist < exp ( -4.0 *  dthresh ))
            break;

        iter ++;
        t += dist;
    }
    
    // vec3 normal = gradient(point, 0.01);
    
    // float ao = softshadow(point, vec3(0.0, 1.0, 0.0));
    
    
    // float shade = abs(dot(normal, rd));

    
    gl_FragColor = vec4( iter/float(MAX_ITER), 0., 0., 0. );
	
}

void reconstruct()
{
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	
	vec4 thisFrame  = IMG_NORM_PIXEL(thisFrameTrace, uv );
	vec4 model  = IMG_NORM_PIXEL(swap, uv);
	
	// float d = thisFrame.x - model.x ;
	// float alpha = 1.0 / exp(d * d) ;
	// alpha = 0.9;
	float alpha = 0.5;
	gl_FragColor = model * alpha + thisFrame * (1.0 - alpha);

}

void copy() {gl_FragColor = IMG_NORM_PIXEL(reconstruction, gl_FragCoord.xy / RENDERSIZE.xy);}


void draw() {
	vec4 r = IMG_NORM_PIXEL(reconstruction, gl_FragCoord.xy / RENDERSIZE.xy  + 0.5 / RENDERSIZE.xy);
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec4 col = IMG_NORM_PIXEL(colormap, vec2((color + 0.3) * (1.0 / 15.0 ), r.x));


	gl_FragColor = col * (1.0 - r.x);
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
