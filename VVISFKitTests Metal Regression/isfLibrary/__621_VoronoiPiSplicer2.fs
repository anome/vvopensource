/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		
	]
}*/

// VoronoiPiSplicer2 by mojovideotech
// utilizing the powers of Pi to mercilessly brutalize IQ's masterpiece :
// https://www.shadertoy.com/view/ldl3W8
// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

#define 	ptpi 	1385.4557313670110891409199368797 	// powten(pi)
#define 	pipi  	36.462159607207911770990826022692 	// pi pied, pi^pi
#define 	picu  	31.006276680299820175476315067101 	// pi cubed, pi^3
#define 	pepi 	23.140692632779269005729086367949 	// powe(pi);
#define 	chpi 	11.59195327552152062775175205256  	// cosh(pi)
#define 	shpi 	11.548739357257748377977334315388 	// sinh(pi)
#define 	pisq  	9.8696044010893586188344909998762 	// pi squared, pi^2
#define 	twpi  	6.283185307179586476925286766559  	// two pi, 2*pi
#define 	pi   	3.1415926535897932384626433832795 	// pi
#define 	e     	2.7182818284590452353602874713526 	// eulers number
#define 	sqpi 	1.7724538509055160272981674833411 	// square root of pi
#define 	phi   	1.6180339887498948482045868343656 	// golden ratio
#define 	cupi  	1.4645918875615232630201425272638 	// cube root of pi
#define 	prpi 	1.4396194958475906883364908049738 	// pi root of pi
#define 	lnpi  	1.1447298858494001741434273513531 	// logn(pi);
#define 	lgpi  	0.4971498726941338543512682882909 	// log(pi)      

#define ANIMATE

vec2 hash2( vec2 p )
{

    // procedural white noise	
	return fract(sin(vec2(dot(p,vec2(pepi,pipi*phi)),dot(p,vec2(ptpi,picu*e))))/cos(pisq*pi)*prpi);
}

vec3 voronoi( in vec2 x )
{
    vec2 n = floor(x);
    vec2 f = fract(x);

    //----------------------------------
    // first pass: regular voronoi
    //----------------------------------
	vec2 mg, mr;

    float md = chpi;
    for( int j=-1; j<=1; j++ )
    for( int i=-1; i<=1; i++ )
    {
        vec2 g = vec2(float(i),float(j));
		vec2 o = hash2( n + g );
		#ifdef ANIMATE
        o = 0.5 + lgpi*sin( TIME+ twpi*o );
        #endif	
        vec2 r = g + o - f;
        float d = dot(r,r);

        if( d<md )
        {
            md = d;
            mr = r;
            mg = g;
        }
    }

    //----------------------------------
    // second pass: distance to borders
    //----------------------------------
    md = shpi;
    for( int j=-2; j<=2; j++ )
    for( int i=-2; i<=2; i++ )
    {
        vec2 g = mg + vec2(float(i),float(j));
		vec2 o = hash2( n + g );
		#ifdef ANIMATE
        o = 0.5 + lgpi*sin( TIME + 6.2831*o );
        #endif	
        vec2 r = g + o - f;

        if( dot(mr-r,mr-r)>0.00001 )
        md = min( md, dot( 0.5*(mr+r), normalize(r-mr) ) );
    }

    return vec3( md, mr );
}

void main ( void )
{
    vec2 p = gl_FragCoord.xy/RENDERSIZE.yx;

    vec3 c = voronoi( (lnpi*10.0)*p );

	// isolines
    vec3 col = c.z*(0.9 + 0.5*sin(96.0*c.x))*vec3(0.8);
    // borders	
    col = mix( vec3(0.0,0.1,1.0), col, smoothstep( 0.001, 0.067, c.x ) );
    // feature points
	float dd = length(cos(c.xy)*-sin(c.xz));
	float ddd = length(sin(c.yz)*cos(c.yx));
	col = mix( vec3(0.4,0.5,0.0), col, smoothstep( 0.99, 0.25, dd) );
	col += vec3(0.4,0.0,0.2)*(1.1-smoothstep( 0.567, 0.0667, ddd));

	gl_FragColor = vec4(col,0.825);
}
