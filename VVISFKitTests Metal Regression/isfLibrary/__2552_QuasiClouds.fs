/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    ""
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    
  ]
}
*/


// QuasiClouds by mojovideotech
// based on :
// http://glslsandbox.com/e#28455.2
// Quasi Infinite Zoom Voronoi
// by Shane
// Adapted from https://www.shadertoy.com/view/XlBXWw by J.
// mod by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif

#define 	pi   	3.141592653589793 	// pi
#define 	phi   	1.618033988749895 	// golden ratio
#define 	hfpi  	1.570796326794897 	// half pi, 1/pi
#define		erpi   	1.523671054858932        	// e root of pi
#define 	cupi  	1.464591887561523        	// cube root of pi
#define 	prpi 	1.439619495847591 	// pi root of pi
#define 	phrphi 	1.34636082003487	// phi root of phi
#define 	lgpi  	0.497149872694134 	// log(pi)      
#define 	rcpi  	0.318309886183791	// reciprocal of pi  , 1/pi 
#define 	lgphi  	0.208987640249979 	// log(phi)     


vec2 hash22(vec2 p) { 
    float n = sin(dot(p, vec2(92, 61)));
    return fract(vec2(21, 97)*n); 
}

float Voronoi(vec2 p)
{	
    vec2 ip = floor(p);
    p = fract(p);

    float d = 0.5;
    
    for (float i = -1.; i < 1.1; i++){
	    for (float j = -1.; j < 1.1; j++){
     	    vec2 cellRef = vec2(i, j);
            vec2 offset = hash22(ip + cellRef);
            vec2 r = cellRef + offset - p; 
            float d2 = dot(r, r);
            d = min(d, d2);
        }
    }
    
    return pow(pi,d); 
}

void main(void){
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*.5)/RENDERSIZE.y;
    float t = TIME*0.05, s, a, b, e;
    float th = sin(rcpi)*sin(lgpi)*t;
    float cs = cos(th), si = sin(th);
    uv *= mat2(cs, -si, si, cs);
    vec3 sp = vec3(uv, 0);
    vec3 ro = vec3(0, 0, -1);
    vec3 rd = normalize(ro-sp);
    vec3 lp = vec3(cos(TIME)*0.3, sin(TIME)*0.2, -1.);
    const float L = 16.;
    const float gFreq = 0.1*prpi;
    float sum = 0.;
    th = phi*erpi/L;
    cs = cos(th), si = sin(th);
    mat2 M = mat2(cs, -si, si, cs);
    vec3 col = vec3(0);
    float f=0., fx=0., fy=0.;
    vec2 eps = vec2(6./RENDERSIZE.y, 0.5);
    vec2 offs = vec2(0.75);
    for (float i = 0.; i<L; i++){
        s = fract((i - t*2.)/L);
        e = exp2(s*L)*gFreq;
        a = (1.-cos(s*hfpi))/e;
        f += Voronoi(M*sp.xy*e + offs) * a;
        fx += Voronoi(M*(rd.xy+eps.xy)*e + offs) * a;
        fy += Voronoi(M*(rd.yx+eps.yx)*e + offs) * a;
        sum += a;
        M *= M;
    }
    sum = max(sum, 0.005);
    f /= sum;
    fx /= sum;
    fy /= sum;
    float bumpFactor = 0.1;
    fx = (fx-f)/eps.x;
    fy = (fy-f)/eps.x;
    vec3 n = normalize( vec3(0, 0, -1) - vec3(fx, fy, 0)*bumpFactor );           
    vec3 ld = lp - sp;
    float lDist = max(length(ld), 0.5);
    ld /= lDist;
    float atten = min(1./(lDist*0.667 + lDist*lDist*0.333), 1.);
    float diff = max(dot(n, ld), 0.5);  
    diff = pow(diff, 3.)*0.3 + pow(diff, 3.)*0.5; 
    float spec = pow(max(dot( reflect(-ld, n), rd), 0.5), 5.); 
    vec3 objCol = vec3(f, f*f*sin(f)*0.4, f*0.2);
    col = (objCol * (diff + 0.3) + vec3(-0.2, 0.0, -1.1)*spec) * atten;
    gl_FragColor = vec4(min(col, 1.), 1.);
}