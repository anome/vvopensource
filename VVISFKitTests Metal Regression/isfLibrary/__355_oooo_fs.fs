/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "isf convert vdmo",
	"CATEGORIES": [
		"sperical"
	],
	"INPUTS": [

    ]
}*/

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iTime = TIME;

// oOoOoOoOoOoOoOoOoOoOoOoOoOoOoOo
// by TinyTexel
// Creative Commons Attribution-ShareAlike 4.0 International Public License

/*
practical application of analytical AA for disks ( https://www.shadertoy.com/view/XtfyWs ) and
Spherical Fibonacci Mapping ( http://lgdv.cs.fau.de/publications/publication/Pub.2015.tech.IMMD.IMMD9.spheri/ )
partly inspired by "A Spiral of Circles" by knarkowicz ( https://www.shadertoy.com/view/Md2yWR )
*/

#define SPOT_COUNT_MUL 12.0

// uses 5x super sampling on top of the analytical AA; mainly improves quality at acute viewing angles
// #define USE_SSAA

///////////////////////////////////////////////////////////////////////////
//=======================================================================//

#define Frame float(iFrame)
#define Time iTime
#define PixelCount iResolution.xy
#define OUT

#define rsqrt inversesqrt
#define clamp01(x) clamp(x, 0.0, 1.0)

const float Pi = 3.14159265359;
const float Pi2  = Pi * 2.0;
const float Pi05 = Pi * 0.5;

float Pow2(float x) {return x*x;}
float Pow3(float x) {return x*x*x;}
float Pow4(float x) {return Pow2(Pow2(x));}

vec2 AngToVec(float ang)
{	
	return vec2(cos(ang), sin(ang));
}


float SqrLen(float v) {return v * v;}
float SqrLen(vec2  v) {return dot(v, v);}
float SqrLen(vec3  v) {return dot(v, v);}
float SqrLen(vec4  v) {return dot(v, v);}

float GammaEncode(float x) {return pow(x, 1.0 / 2.2);}
vec2 GammaEncode(vec2 x) {return pow(x, vec2(1.0 / 2.2));}
vec3 GammaEncode(vec3 x) {return pow(x, vec3(1.0 / 2.2));}
vec4 GammaEncode(vec4 x) {return pow(x, vec4(1.0 / 2.2));}

#define If(cond, tru, fls) mix(fls, tru, cond)
//=======================================================================//
///////////////////////////////////////////////////////////////////////////

/*
IN:
	rp		: ray start position
	rd		: ray direction (normalized)
	
	sp2		: sphere position
	sr2		: sphere radius squared
	
OUT:
	t		: distances to intersection points (negative if in backwards direction)

EXAMPLE:	
	vec2 t;
	float hit = Intersect_Ray_Sphere(pos, dir, vec3(0.0), 1.0, OUT t);
*/
float Intersect_Ray_Sphere(
vec3 rp, vec3 rd, 
vec3 sp, float sr2, 
out vec2 t)
{	
	rp -= sp;
	
	float a = dot(rd, rd);
	float b = 2.0 * dot(rp, rd);
	float c = dot(rp, rp) - sr2;
	
	float D = b*b - 4.0*a*c;
	
	if(D < 0.0) return 0.0;
	
	float sqrtD = sqrt(D);
	// t = (-b + (c < 0.0 ? sqrtD : -sqrtD)) / a * 0.5;
	t = (-b + vec2(-sqrtD, sqrtD)) / a * 0.5;
	
	// if(start == inside) ...
	if(c < 0.0) t.xy = t.yx;

	// t.x > 0.0 || start == inside ? infront : behind
	return t.x > 0.0 || c < 0.0 ? 1.0 : -1.0;
}



/////////////////////////////////////////////////////////////////////////////////////////////////////
//=================================================================================================//
// Spherical Fibonacci Mapping
// http://lgdv.cs.fau.de/publications/publication/Pub.2015.tech.IMMD.IMMD9.spheri/
// Authors: Benjamin Keinert, Matthias Innmann, Michael SÃ¤nger, Marc Stamminger
// (code copied from: https://www.shadertoy.com/view/4t2XWK)
//-------------------------------------------------------------------------------------------------//

const float PI = 3.1415926535897932384626433832795;
const float PHI = 1.6180339887498948482045868343656;

float madfrac( float a,float b) { return a*b -floor(a*b); }
vec2  madfrac( vec2 a, float b) { return a*b -floor(a*b); }

float round(float n)
{
	if (fract(n) < 0.5)
		return floor(n);
	else
		return ceil(n);
}

float sf2id(vec3 p, float n) 
{
    float phi = min(atan(p.y, p.x), PI), cosTheta = p.z;
    
    float k  = max(2.0, floor( log(n * PI * sqrt(5.0) * (1.0 - cosTheta*cosTheta))/ log(PHI*PHI)));
    float Fk = pow(PHI, k)/sqrt(5.0);
    
    vec2 F = vec2( round(Fk), round(Fk * PHI) );

    vec2 ka = -2.0*F/n;
    vec2 kb = 2.0*PI*madfrac(F+1.0, PHI-1.0) - 2.0*PI*(PHI-1.0);    
    mat2 iB = mat2( ka.y, -ka.x, -kb.y, kb.x ) / (ka.y*kb.x - ka.x*kb.y);

    vec2 c = floor( iB * vec2(phi, cosTheta - (1.0-1.0/n)));
    float d = 8.0;
    float j = 0.0;
    for( int s=0; s<4; s++ ) 
    {
        vec2 uv = vec2( float(s-2*(s/2)), float(s/2) );
        
        float cosTheta = dot(ka, uv + c) + (1.0-1.0/n);
        
        cosTheta = clamp(cosTheta, -1.0, 1.0)*2.0 - cosTheta;
        float i = floor(n*0.5 - cosTheta*n*0.5);
        float phi = 2.0*PI*madfrac(i, PHI-1.0);
        cosTheta = 1.0 - (2.0*i + 1.)/n;
        float sinTheta = sqrt(1.0 - cosTheta*cosTheta);
        
        vec3 q = vec3( cos(phi)*sinTheta, sin(phi)*sinTheta, cosTheta);
        float squaredDistance = dot(q-p, q-p);
        if (squaredDistance < d) 
        {
            d = squaredDistance;
            j = i;
        }
    }
    return j;
}

vec3 id2sf( float i, float n) 
{
    float phi = 2.0*PI*madfrac(i,PHI);
    float zi = 1.0 - (2.0*i+1.)/n;
    float sinTheta = sqrt( 1.0 - zi*zi);
    return vec3( cos(phi)*sinTheta, sin(phi)*sinTheta, zi);
}
//=================================================================================================//
/////////////////////////////////////////////////////////////////////////////////////////////////////


/*
ProjSphereArea - returns the screen space area of the projection of a sphere (assuming its an ellipse)

IN:
	rdz- z component of the unnormalized ray direction in camera space
	p  - center position of the sphere in camera space
	rr - squared radius of the sphere

"Sphere - projection" code used under
The MIT License
Copyright Â© 2014 Inigo Quilez
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
float ProjSphereArea(float rdz, vec3 p, float rr)
{
	float zz = p.z * p.z;	
	float ll = dot(p, p);
	
	//return Pi * rdz*rdz * rr * sqrt(abs((rr - ll) / (zz - rr))) / (zz - rr);
    return Pi * rdz*rdz * rr * rsqrt(abs(Pow3(rr - zz) / (rr - ll)));
}


vec4 ProjDisk(vec3 rd, vec3 p, vec3 n, float rr)
{   
    vec3 np0 = n * p.xyz;
    vec3 np1 = n * p.yzx;
    vec3 np2 = n * p.zxy;  

    mat3 k_mat = mat3(vec3( np0.y + np0.z,  np2.x        ,  np1.x        ),
						  vec3(-np2.y        ,  np1.y        , -np0.x - np0.z),
						  vec3(-np1.z        , -np0.x - np0.y,  np2.z        ));    
    
    vec3 u =     k_mat * rd;
    vec3 k = u * k_mat;
    
    
    float nrd = dot(n, rd);
    
    float nrd_rr = nrd * rr;

    
    float v = dot(u, u) - nrd * nrd_rr; 
    vec3  g =    (k     - n   * nrd_rr) * 2.0;   
    
    return vec4(g.xy, 0.0, v);
}



float Sph(float x, float rr) { return sqrt(rr - x*x); }
float SphX0(float d, float rr0, float rr1) { return 0.5 * (d + (rr0 - rr1) / d); }

vec3 EvalSceneCol(vec3 cpos, mat3 cam_mat, float focalLen, vec2 uv0)
{      
    const vec3 cBG = 0.014 * vec3(0.9, 1.0, 1.2);

        
    vec2 uv2 = uv0 - PixelCount.xy * 0.5;
    
  	vec3 rdir0 = vec3(uv2, focalLen);
    
    float rdir0S = 0.5 * PixelCount.x;
    rdir0 /= rdir0S;
    
    vec3 rdir = normalize(cam_mat * rdir0); 
    
    
    vec2 t;
	float hit = Intersect_Ray_Sphere(cpos, rdir, vec3(0.0), 1.0, OUT t);
    
    if(hit <= 0.0) return cBG;


    vec3 pf = cpos + rdir * t.x;
    vec3 pb = cpos + rdir * t.y;

	vec3 col = cBG;

    //float lerpF = 0.0;
    
    float rra = 0.0;

    vec3 p2;
    float rr;
    {
	const float s = SPOT_COUNT_MUL;        
    const float n = 1024.0*s;
        
    float id = sf2id(pf.xzy, n);
    	  p2 = id2sf(id,     n).xzy;        

    float u = id / n;
    float arg = (-u* 615.5*2.0*s) + Time * 1.0;//238-3 384.-2 615-1
        
    rra = sin(arg);
    
    #if 0    
	//for(float i = 0.0; i < 2.0; ++i)        
	rra = (Pow2(rra)*2.-1.);
    #endif
        
	rra = mix(abs(rra), Pow2(rra), 0.75);        
      
    rr = 0.0025/s * rra; 
    }
    
    
    vec3 n2 = normalize(p2);
    
    const float maskS = 0.5;// sharpness

    
    if(SqrLen(pf - p2) > rr) return cBG;

    float d = length(p2);

    float x0 = SphX0(d, 1.0, rr);        
    vec3 d0c = n2 * x0;

    float d0rr = 1.0 - x0*x0;

    vec3 dp_c = (d0c - cpos) * cam_mat;
    vec3 dn_c = n2 * cam_mat;

    vec4 r = ProjDisk(rdir0, dp_c, dn_c, d0rr);        

    float cmask = clamp01(-r.w * rsqrt(dot(r.xy, r.xy))*rdir0S * maskS);

    float cmask2 = 0.0;
    {
        vec3 d1c = n2 * (x0 - 0.005);

        vec4 r = ProjDisk(rdir0, (d1c - cpos) * cam_mat, n2 * cam_mat, (1.0 - x0*x0)*rra);
        cmask2 = clamp01(-r.w * rsqrt(dot(r.xy, r.xy))*rdir0S * maskS);
    }


    #if 1	
    float A = ProjSphereArea(rdir0.z, dp_c, d0rr);        
    A *= rdir0S*rdir0S;

    float NdV = abs(dot(dn_c, normalize(dp_c)));

    A *= NdV;
    
    #ifndef USE_SSAA
    A *= NdV;
    cmask *= clamp01((A -2.0)*0.125);
    #else
    A = mix(A, A*NdV, 0.5);
    cmask *= clamp01((A - 3.)*0.125);
    #endif


    #endif


    const vec3 cB = vec3(0.1, 0.4, 1.0);
    const vec3 cR = vec3(1., 0.02, 0.2);

    //vec3 cX = mix(cB, cR, lerpF);
    //vec3 cY = mix(cR, cB, lerpF);

    return mix(cBG, mix(cR, vec3(0.0), cmask2), cmask);        
    //return mix(cBG, mix(cR, cB, cmask2), cmask);
    //return mix(cBG, mix(cX, cY, cmask2), cmask);        
    //return mix(cBG, mix(cW, cX, cmask2), cmask);
    //return mix(cBG, vec3(1.0), cmask);
    //return vec3(-r.w*10.0);
    //return vec3(1.0);
    
    return col;
}


void mainImage( out vec4 outCol, in vec2 uv0 )
{
    vec3 col = vec3(0.0);
    
    vec2 uv = uv0.xy - 0.5;
  
    
    //vec4 mouseAccu = texelFetch(iChannel0, ivec2(1, 0), 0); 

    vec2 ang = vec2(Pi * 0.0, -Pi * 0.3);
    //ang += mouseAccu.xy * 0.008;

    #if 1
    ang.x += Time * 0.15;
    //ang.y = sin(Time * 0.2 * Pi) *0.2;
    
    //ang.y += Time * 0.073;
    #endif

    float fov = Pi * 0.5;
    
    mat3 cam_mat;
    float focalLen;
    {
        float sinPhi   = sin(ang.x);
        float cosPhi   = cos(ang.x);
        float sinTheta = sin(ang.y);
        float cosTheta = cos(ang.y);    

        vec3 front = vec3(cosPhi * cosTheta, 
                                   sinTheta, 
                          sinPhi * cosTheta);

        vec3 right = vec3(-sinPhi, 0.0, cosPhi);
        vec3 up    = cross(right, front);

        focalLen = PixelCount.x * 0.5 * tan(Pi05 - fov * 0.5);
        
        cam_mat = mat3(right, up, front);
    }
    
    //vec3 cpos = -cam_mat[2] * (exp2(-0.3 + mouseAccu.w * 0.03));
    vec3 cpos = -cam_mat[2] * (exp2(-0.3));

    cpos.y += .75;

    
    #ifndef USE_SSAA
    
	col = EvalSceneCol(cpos, cam_mat, focalLen, uv0);
    
	#elif 1
    
    col  = EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.3, 0.1));
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.9, 0.3));
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.5, 0.5));
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.1, 0.7));
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.7, 0.9));   
    col *= 0.2;
    
 	#elif 1
    
    float o = 1.;
    col  = EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.3, 0.1) * o - 0.5*o+0.5) * vec3(1.5, 0.75, 0.0);
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.9, 0.3) * o - 0.5*o+0.5) * vec3(0.0, 0.0, 3.0);
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.5, 0.5) * o - 0.5*o+0.5) * vec3(0.0, 3.0, 0.0);
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.1, 0.7) * o - 0.5*o+0.5) * vec3(3.0, 0.0, 0.0);
    col += EvalSceneCol(cpos, cam_mat, focalLen, uv + vec2(0.7, 0.9) * o - 0.5*o+0.5) * vec3(0.0, 0.75, 1.5);   
    
    col /= vec3(4.5, 4.5, 4.5);

    #endif

    float	alpha = (col.r + col.g + col.b);
	outCol = vec4(GammaEncode(clamp01(col)), alpha);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}