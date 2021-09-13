/*{
	"CREDIT": "by mojovideotech",
    "CATEGORIES" : [
    "generator",
    "voronoi"
  ],
  "DESCRIPTION" : "from https://www.shadertoy.com/view/ld3yRn by tomkh.",
  "INPUTS" : [
	{
		"NAME" : 		"scale",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.1,
		"MAX" : 		2.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.0,
		"MIN" : 		0.0,
		"MAX" : 		3.0
	},
	{
		"NAME" : 		"smooth",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.75, 0.25 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ 0.0, 0.0 ]
	},
	{
		"NAME" : 		"offset",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.33,
		"MIN" : 		0.0,
		"MAX" : 		0.5
	},
	{
		"NAME" : 		"detail",
		"TYPE" : 		"float",
		"DEFAULT" : 	40.0,
		"MIN" : 		10.0,
		"MAX" : 		100.0
	},
	{
		"NAME" : 		"deform",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.25,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"ddepth",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.25,
		"MIN" : 		0.01,
		"MAX" : 		0.5
	},
	{
		"NAME" : 		"dfreq",
		"TYPE" : 		"float",
		"DEFAULT" : 	 6.0,
		"MIN" : 		-20.0,
		"MAX" : 		20.0
	},
	{
     	"NAME" :		"seed1",
     	"TYPE" : 		"float",
     	"DEFAULT" :		0.1999,
     	"MIN" : 		0.0333,
     	"MAX" :			0.3333
	},
    {
      	"NAME" :		"seed2",
      	"TYPE" :		"float",
      	"DEFAULT" :		0.0999,
      	"MIN" : 		0.0033,
      	"MAX" :			0.1333	
	},
    {
     	"NAME" :		"seed3",
      	"TYPE" :		"float",
     	"DEFAULT" :		0.0667,
     	"MIN" :			0.0099,
     	"MAX" :			0.0999
    }
  ]
}
*/

////////////////////////////////////////////////////////////
// SmoothVoronoiDeformation  by mojovideotech
//
// based on :
// shadertoy.com/ld3yRn  by Tomasz Dobrowolski' 2018
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	pi   	3.141592653589793 	// pi


vec2 hash2(vec2 p) {
  	vec3 p3 = fract(vec3(p.xyx) * vec3(seed1, seed2, seed3));
 	p3 *= dot(p3, p3.yzx+19.19);
	vec2 o = fract(vec2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
 	o = 0.5 + offset*sin(TIME*rate + o*twpi );
	return o;
}

float smin(float a, float b, float r) {
   float f = max(0.0,1.0-abs(b-a)/r);
   return min(a,b) - r*0.25*f*f;
}

float sabs(float x, float r) {
   float f = max(0.0,1.0-abs(x + x)/r);
   return abs(x) + r*0.25*(f*f - 1.0);
}

float closest( in vec2 n, in vec2 f, out vec2 mr, out vec2 mg ) {
    vec2 h = step(0.5,f) - 2.0;
    vec2 n2 = n + h;
    vec2 f2 = f - h;
    float md = 8.0;
    for( int j=0; j<=3; j++ )
    for( int i=0; i<=3; i++ )
    {
        vec2 g = vec2(float(i),float(j));
        vec2 o = hash2( n2 + g );
        vec2 r = g + o - f2;
        float d = dot(r,r);
        if( d<md ) {
            md = d;
            mr = r;
            mg = g;
        }
    }
    mg += h;
    return md;
}

vec3 voronoi_rounder( in vec2 p, in float s, in float e ) {
	vec2 x = mix(p,p+sin(p.yx*dfreq+cos(deform))*ddepth,deform);
    vec2 n = floor(x);
    vec2 f = fract(x);
    vec2 mr, mg;
    float md = closest(n,f,mr,mg);
    md = 8.0;
    for( int j=-2; j<=2; j++ )
    for( int i=-2; i<=2; i++ )
    {
        vec2 g = mg + vec2(float(i),float(j));
        vec2 o = hash2( n + g );
        vec2 r = g + o - f;
        if( dot(mr-r,mr-r)>0.005) {
            float d = dot( 0.5*(mr+r), normalize(r-mr) );
            md = smin(d, md, s*d);
        }
    }
    md *= 0.5 + s;
    md = sabs(md, e);
    return vec3( md, mr );
}

vec3 plot( vec2 p, float ss ) {
	vec2 sm = smooth.xy * RENDERSIZE.xy;
    float s = clamp(sm.x/RENDERSIZE.x,0.0,1.0)*0.95+0.05;
    float e = max(.01,sm.y/RENDERSIZE.y)*0.5;
    if (length(sm.xy) < 0.5) {
		s = 0.5;
        e = 0.005;
    }
    vec3 c = voronoi_rounder(p, s, e);
    vec2 eps = vec2(0.01,0.0);
    float fdx = (voronoi_rounder(p + eps.xy, s, e).x - c.x)/eps.x;
    float fdy = (voronoi_rounder(p + eps.yx, s, e).x - c.x)/eps.x;
    vec3 norm = normalize(vec3(fdx, fdy, 1.5));
    float fw1 = (abs(fdx) + abs(fdy))*ss;
    float fw2 = fw1*(detail/pi*2.7);
    float f0 = sin(0.7/detail);
    float f = sin(c.x*detail-0.7);
    float od = abs(f);
    vec3 ldir = normalize(vec3(-0.2,-0.3,0.6));
    float dd = dot(norm,ldir);
    float rd = pow(max(0.0,reflect(-ldir,norm).z),16.0);
    float ld = dd*dd*0.7+0.35;
    float c0 = c.x*0.7-step(0.0,f)*0.05+0.6;
    float c1 = c.x*0.7+0.33;
    vec3 col = mix(vec3(c0*ld+rd*0.23), vec3(c1*ld), smoothstep(fw2,0.0,od)*0.7);
    col = mix(col, vec3(0.1,0.15,0.1), smoothstep(f0+fw1,f0,c.x)*0.5);
  //  col = sqrt(col)*1.5-0.53;
    return col;
}

void main() 
{
    float sc = (step(512.0, RENDERSIZE.y)*2.0 + 3.0) * (2.1-scale);
    float ss = sc / RENDERSIZE.y;
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*0.5) * ss;
    
    gl_FragColor = vec4(plot(uv, ss), 1.0);
}

