/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/XdlcWf by macbooktall.  discontinuous floored displacements of an sdf. low coefficients can create a nice effect, but naturally creates artifacts.\nbase raymarching code from iq.\npModPolar and fOctahedron from Mercury's hg_sdf library.",
  "INPUTS" : [
    {
      "NAME" : "een",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 1,
      "MIN" : -10
    },
    {
      "NAME" : "twee",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : -1,
      "MIN" : -10
    },
    {
      "NAME" : "drie",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : -1,
      "MIN" : 0
    },
    {
      "NAME" : "vier",
      "TYPE" : "float",
      "MAX" : 20,
      "DEFAULT" : 3,
      "MIN" : 0
    },
    {
      "NAME" : "vijf",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "zes",
      "TYPE" : "float",
      "MAX" : 25,
      "DEFAULT" : 5,
      "MIN" : 1,
      "LABEL" : "animatie"
    },
    {
      "NAME" : "zeven",
      "TYPE" : "float",
      "MAX" : 5,
      "DEFAULT" : 0.75,
      "MIN" : 0
    },
    {
      "NAME" : "acht",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : -3,
      "MIN" : -10
    },
    {
      "NAME" : "negen",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0.59999999999999998,
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2"
}
*/


//  hg_sdf by MERCURY http://mercury.sexy
// 	Released as Creative Commons Attribution-NonCommercial (CC BY-NC)

#define PI 3.14159265
#define TAU (2*PI)
#define PHI (1.618033988749895)

#define GDFVector3 normalize(vec3(een, twee, 1 ))
#define GDFVector4 normalize(vec3(-1, 1, 1))
#define GDFVector5 normalize(vec3(1, -1, 1))
#define GDFVector6 normalize(vec3(1, 1, drie))

#define fGDFBegin float d = 0.;

// Version with variable exponent.
// This is slow and does not produce correct distances, but allows for bulging of objects.
#define fGDFExp(v) d += pow(abs(dot(p, v)), e);

// Version with without exponent, creates objects with sharp edges and flat faces
#define fGDF(v) d = max(d, abs(dot(p, v)));

#define fGDFExpEnd return pow(d, 1./e) - r;
#define fGDFEnd return d - r;

float fOctahedron(vec3 p, float r) {
fGDFBegin
    fGDF(GDFVector3) fGDF(GDFVector4) fGDF(GDFVector5) fGDF(GDFVector6)
    fGDFEnd
}


// Repeat around the origin by a fixed angle.
// For easier use, num of repetitions is use to specify the angle.
float pModPolar(inout vec2 p, float repetitions) {
    float angle = 2.*PI/repetitions;
    float a = atan(p.y, p.x) + angle/2.;
    float r = length(p);
    float c = floor(a/angle);
    a = mod(a,angle) - angle/2.;
    p = vec2(cos(a), sin(a))*r;
    // For an odd number of repetitions, fix cell index of the cell in -x direction
    // (cell index would be e.g. -5 and 5 in the two halves of the cell):
    if (abs(c) >= (repetitions/2.)) c = abs(c);
    return c;
}

////////////////////////////////////////////////////////////////
// The end of HG_SDF library
////////////////////////////////////////////////////////////////

float opS( float d1, float d2 )
{
    return max(-d1,d2);
}

float opU( float d1, float d2 )
{
    return min(d2,d1);
}

float roomWidth = vijf;

vec2 map( in vec3 pos ){
  	float rep = vier;
	vec3 p = pos;

    vec2 idx = floor((abs(pos.xz)) / 0.2)*0.5;

    float clock = TIME*zes;
    float phase = (idx.y+idx.x);

    float anim = sin(phase + clock);

    float i = pModPolar(pos.xz, rep);
    pos.x -= zeven + anim*0.1;

    float dist = fOctahedron(pos,roomWidth*negen);

    dist = opU(dist, fOctahedron(p, roomWidth*1.25 + anim*0.05));
                           
    vec2 res = vec2(dist, 0.);

    return res;
}

vec2 castRay( in vec3 ro, in vec3 rd )
{
    float tmin = 0.0;
    float tmax = 60.0;
    
    float t = tmin;
    float m = -1.0;
    for( int i=0; i<60; i++ )
    {
   		vec2 res = map( ro+rd*t );
        if(  t>tmax ) break;
        t += res.x;
    	m = res.y;
    }

    if( t>tmax ) m=-1.0;
    return vec2( t, m );
}


float softshadow( in vec3 ro, in vec3 rd, in float mint, in float tmax )
{
	float res = 1.0;
    float t = mint;
    
    for( int i=0; i<20; i++ )
    {
		float h = map( ro + rd*t ).x;
        res = min( res, 8.0*h/t );
        t += clamp( h, 0., 0.1 );
        if(t>tmax ) break;
    }
    return clamp( res, 0.0, 1.0 );

}

vec3 calcNormal( in vec3 pos )
{
	vec3 eps = vec3( 0.01, 0.0, 0.0 );
	vec3 nor = vec3(
   map(pos+eps.xyy).x - map(pos-eps.xyy).x,
   map(pos+eps.yxy).x - map(pos-eps.yxy).x,
   map(pos+eps.yyx).x - map(pos-eps.yyx).x );
	return normalize(nor);
}

float calcAO( in vec3 pos, in vec3 nor )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + .15*float(i)/4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = map( aopos ).x;
        occ += -(dd-hr)*sca;
        sca *= .95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}

vec3 palette( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
    return a + b*cos( 6.28318*(c*t+d) );
}

vec3 render( in vec3 ro, in vec3 rd )
{
    vec3 col = vec3(1.);
    vec2 res = castRay(ro,rd);
    const vec3 a = vec3(1., 1., 1.);
    const vec3 b = vec3(1., .0, .3);
    const vec3 c = vec3(0.2, 0., 0.5);
    const vec3 d = vec3(.8,.2,.2);
        
    if (res.x > 6.) return palette(.3,a,b,c,d);
    
    vec3 pos = ro + res.x*rd;
    vec3 nor = calcNormal( pos );
    vec3 ref = reflect( rd, nor );

   	float occ = calcAO( pos, nor );
    float dom = smoothstep( -0.6, 20.6, ref.y );

    dom *= softshadow( pos, ref, .9, .15 );

    col = palette(3.+res.x*.1,a,b,c,d)*occ*length(ref)*(1.+dom);
    col = mix( col, vec3(.0), max(1.0-exp( -0.05*res.x ),clamp(res.x*0.1,0.,1.)) );

    return vec3( clamp(col,0.0,1.0) );
}

mat3 setCamera( in vec3 ro, in vec3 ta, float cr )
{
	vec3 cw = normalize(ta-ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void main() {



	vec2 q = gl_FragCoord.xy/RENDERSIZE.xy;
    vec2 p = -1.0+2.0*q;
	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	vec3 ro = vec3(-acht, roomWidth*3., 1.5 + cos(TIME*2.)*0.3 );
  	vec3 ta = vec3(0.,.0,.0);
    mat3 ca = setCamera( ro, ta, 0. );
	vec3 rd = ca * normalize(vec3(p.xy,2.));
    vec3 col = render( ro, rd );
	col = pow(col, vec3(1.5));
    gl_FragColor = vec4( col, 1.0 );
}
