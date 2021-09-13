/*{
"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "pov",
    "lattice"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/4tlSWl by Shane.  The lattice structure in this example is really simple to construct, and represents just one of infinitely many combinations.",
  "INPUTS" : [
	{
			"NAME": "iChannel0",
			"TYPE": "image"
		},
			{
			"NAME": "iChannel1",
			"TYPE": "image"
		}
  ]
}
*/

// SteelLatticePOV by mojovideotech
// based on:
// www.shadertoy.com/view/4tlSWl


#define MAP 1
#define sEPS 0.0125
#define FAR 9. 
#define phi 1.618034


float sminP( float a, float b, float smoothing )
{
    float h = clamp( 0.5+0.5*(b-a)/smoothing, 0.0, 1.0 );
    return mix( b, a, h ) - smoothing*h*(1.0-h);
}

mat2 rot(float th) { float cs = cos(th), si = sin(th); return mat2(cs, -si, si, cs); }


vec3 tex3D( sampler2D tex, in vec3 p, in vec3 n )
{
   n = max(mod(n,p)/phi, n * 0.001); 
   // n = max(abs(n), 0.001);
    n /= (n.x + n.y + n.z );  
	return (texture2D(tex, p.yz)*n.x + texture2D(tex, p.zx)*n.y + texture2D(tex, p.xy)*n.z).xyz;
}

float bumpSurf3D( in vec3 p, in vec3 n )
{
    p = abs(mod(p, 0.0225)-0.0125);
    float x = min(p.x,max(p.y,p.z))/0.0325;
	x *= sin(x*phi+sin(x*phi)*phi)*0.5 + 0.5; 
    p = sin(p*286.57+sin(p.yzx*514.229));
    float surfaceNoise = (p.x+p.y+p.z);
    return clamp(x *3.33 + surfaceNoise*0.005, 0., 1.);
}

vec3 doBumpMap(in vec3 p, in vec3 nor, float bumpfactor)
{
    const float eps = 0.01;
    float ref = bumpSurf3D(p, nor);                 
    vec3 grad = vec3( bumpSurf3D(vec3(p.x-eps, p.y, p.z), nor)-ref,
                      bumpSurf3D(vec3(p.x, p.y-eps, p.z), nor)-ref,
                      bumpSurf3D(vec3(p.x, p.y, p.z-eps), nor)-ref )/eps;                     
    grad -= nor*dot(nor, grad);          
    return normalize( nor + bumpfactor*grad );
}


float map(vec3 p)
{
	float x1; float x2;
    p = mod(p, 2.)-1.;
    if (MAP == 1 ) x1 = sminP(length(p.xy),sminP(length(p.yz),length(p.xz), 0.25), 0.25)-0.5;
    else x1 = sqrt(min(dot(p.xy, p.xy),min(dot(p.yz, p.yz),dot(p.xz, p.xz))))-0.5;
    p = abs(mod(p, 0.5)-0.25);
    if (MAP == 2 )
    {
    	x2 = min(p.x,min(p.y,p.z)); 
    	return sqrt(x1*x1+x2*x2)-.05;
    }
    else x2 = min(max(p.x, p.y),min(max(p.y, p.z),max(p.x, p.z)))-0.125;
	return max(x1, x2)-.05; 
}

float raymarch(vec3 ro, vec3 rd) {
	float d, t = 0.0;
    for (int i = 0; i < 128; i++)
    {
        d = map(ro + rd *t);
        if (d<sEPS || t>FAR) break;  
        t += d*0.75;
    }
    if (d<sEPS) t += d;
    return t;
}
 
float calculateAO(vec3 p, vec3 n) 
{
    const float AO_SAMPLES = 5.0;
    float r = 0.0, w = 1.0, d;
    for (float i=1.0; i<AO_SAMPLES+1.1; i++)
    {
        d = i/AO_SAMPLES;
        r += w*(d - map(p + n*d));
        w *= 0.5;
    }
    return 1.0-clamp(r,0.0,1.0);
}

vec3 getNormal(in vec3 p)
{
	const float eps = 0.001;
	return normalize(vec3(
		map(vec3(p.x+eps,p.y,p.z))-map(vec3(p.x-eps,p.y,p.z)),
		map(vec3(p.x,p.y+eps,p.z))-map(vec3(p.x,p.y-eps,p.z)),
		map(vec3(p.x,p.y,p.z+eps))-map(vec3(p.x,p.y,p.z-eps))
	));
}

void main()
{
	vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*0.5) / RENDERSIZE.y;
    vec3 rd = normalize(vec3(uv, 0.5));
   // vec3 rd = normalize(vec3(uv, sqrt(1.-dot(uv, uv))*0.5)); // Mild fish lens, if you'd prefer.
    rd.xy *= rot(TIME*0.25);
    rd.xz *= rot(TIME*0.125); // Extra variance.
    vec3 ro = vec3(0.0, 0.0, TIME*0.125);
   // vec3 ro = vec3(0.7 + TIME*0.125, 0.0, TIME*0.125); // Another lattice traversal cliche.
    vec3 lp = vec3(0.0, 0.125, -0.125);
    lp.xy *= rot(TIME*0.25);
    lp.xz *= rot(TIME*0.125);
    lp += ro + vec3(0.0, 1.0, 0.0);
    vec3 sceneCol = vec3(0.);
    float dist = raymarch(ro, rd);
    if (dist < FAR)
    {
        vec3 sp = ro + rd*dist;
        vec3 sn = getNormal(sp);
	    sn = doBumpMap(sp, sn, 3.125/(phi*dist));
	    vec3 ld = lp-sp;
	    vec3 objCol = tex3D( iChannel0, sp, sn );
	   // objCol *= bumpSurf3D(sp, sn)*0.5+0.5;
	    float lDist = max(length(ld), 0.01); 
	    ld /= lDist;
	    float atten = min( 1.0 /( lDist*0.5 + lDist*lDist*0.1 ), 1.0 ); 
	    float ambient = .25;
	    float diffuse = max( 0.0, dot(sn, ld) ); 
	    float specular = max( 0.0, dot( reflect(-ld, sn), -rd) ); 
	    specular = pow(specular, 6.0); 
        float ao = calculateAO(sp, sn)*0.5+0.5;
	    sceneCol = objCol*(vec3(0.6,0.7,0.9)*diffuse + ambient)  + vec3(0.7,0.8,0.9)*specular*0.75;
		sceneCol *= atten*ao;
	}
	gl_FragColor = vec4(clamp(sceneCol, 0., 1.), 1.0);
}