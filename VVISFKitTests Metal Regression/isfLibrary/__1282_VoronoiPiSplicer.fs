/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	]
}*/

/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		
	]
}*/

// VoronoiPiSplicer by mojovideotech
// Pi constants variations from : 
// http://glslsandbox.com/e#25631.3
// jammed with reckless abandon into this masterpiece :
// https://www.shadertoy.com/view/ldl3W8
// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// http://www.iquilezles.org/www/articles/voronoilines/voronoilines.htm

#define ANIMATE
#define ptpi 1385.4557313670110891409199368797 //powten(pi)
#define pipi  36.462159607207911770990826022692 //pi pied, pi^pi
#define picu  31.006276680299820175476315067101 //pi cubed, pi^3
#define pepi  23.140692632779269005729086367949 //powe(pi);
#define chpi  11.59195327552152062775175205256  //cosh(pi)
#define shpi  11.548739357257748377977334315388 //sinh(pi)
#define pisq  9.8696044010893586188344909998762 //pi squared, pi^2
#define twpi  6.283185307179586476925286766559  //two pi, 2*pi
#define pi    3.1415926535897932384626433832795 //pi
#define e     2.7182818284590452353602874713526 //eulers number
#define sqpi  1.7724538509055160272981674833411 //square root of pi
#define phi   1.6180339887498948482045868343656 //golden ratio
#define hfpi  1.5707963267948966192313216916398 //half pi, 1/pi
#define cupi  1.4645918875615232630201425272638 //cube root of pi
#define prpi  1.4396194958475906883364908049738 //pi root of pi
#define lnpi  1.1447298858494001741434273513531 //logn(pi);
#define trpi  1.0471975511965977461542144610932 //one third of pi, pi/3
#define thpi  0.99627207622074994426469058001254//tanh(pi)
#define lgpi  0.4971498726941338543512682882909 //log(pi)      
#define rcpi  0.31830988618379067153776752674503// reciprocal of pi  , 1/pi 
#define rcpipi  0.0274256931232981061195562708591 // reciprocal of pipi  , 1/pipi

vec2 hash2( vec2 p )
{
	// texture based white noise
	//return texture2D( iChannel0, (p+0.5)/256.0, -100.0 ).xy;
	
    // procedural white noise	
	return fract(sin(vec2(dot(p,vec2(chpi,pepi)),dot(p,vec2(picu,pipi))))*cos(ptpi-p));
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
        o = lgpi + lgpi*sin( TIME+ twpi*o );
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
    md = chpi;
    for( int j=-2; j<=2; j++ )
    for( int i=-2; i<=2; i++ )
    {
        vec2 g = mg + vec2(float(i),float(j));
		vec2 o = hash2( n + g );
		#ifdef ANIMATE
        o = lgpi + lgpi*sin( TIME + twpi*o );
        #endif	
        vec2 r = g + o - f;

        if( dot(mr-r,mr-r)>0.00001 )
        md = min( md, dot( lgpi*(mr+r), normalize(r-mr) ) );
    }

    return vec3( md, mr );
}

void main ( void )
{
    vec2 p = gl_FragCoord.xy/RENDERSIZE.yy;

    vec3 c = voronoi( pisq*p );

	// isolines
    vec3 col = c.x*(lgpi + lgpi*sin((twpi*10.0)*c.x))*vec3(1.0);
    // borders	
    col = mix( vec3(0.1,0.0,1.0), col, smoothstep( rcpipi*0.1,rcpi*0.1, c.x ) );
 
	gl_FragColor = vec4(col,1.0);
}
