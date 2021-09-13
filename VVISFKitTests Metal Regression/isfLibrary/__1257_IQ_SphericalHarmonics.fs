/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "3d",
    "sphericalharmonics",
    "harmonics",
    "Automatically Converted"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/lsfXWH by iq.  Four bands of Spherical Harmonics functions (or atomic orbitals if you want). For reference and fun.",

    "INPUTS": [
      {
      "MAX": [
        10,
        10
      ],
      "MIN": [
        -10,
        -10
      ],
      "DEFAULT":[0.5,0.4],
      "NAME": "matrix",
      "TYPE": "point2D"
    },
    	{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.67,
			"MIN": 0.01,
			"MAX": 2.5
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": -2.0,
			"MAX": 2.0
		},
		{
			"NAME": "colorCycle",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.1,
			"MAX": 3.0
		}
  ]
}
*/

// SphericalHarmonics by mojovideotech
// hack & mod of :
// https://www.shadertoy.com/view//lsfXWH  by IQ


// Created by inigo quilez - iq/2013
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.


// Four bands of Spherical Harmonics functions (or atomic orbitals if you want). For
// reference and fun.



// antialias level (try 1, 2, 3, ...)
#define AA 1

//#define SHOW_SPHERES

//---------------------------------------------------------------------------------

// Constants, see here: http://en.wikipedia.org/wiki/Table_of_spherical_harmonics
#define k01 0.2820947918 // sqrt(  1/PI)/2
#define kr1 	0.027425693123298 	// reciprocal of pipi  , 1/pipi
#define	kr2  	0.497149872694134 	// log(pi)      
#define k02 0.4886025119 // sqrt(  3/PI)/2
#define k03 1.0925484306 // sqrt( 15/PI)/2
#define k04 0.3153915652 // sqrt(  5/PI)/4
#define k05 0.5462742153 // sqrt( 15/PI)/4
#define k06 0.5900435860 // sqrt( 70/PI)/8
#define k07 2.8906114210 // sqrt(105/PI)/2
#define k08 0.4570214810 // sqrt( 42/PI)/8
#define k09 0.3731763300 // sqrt(  7/PI)/4
#define k10 1.4453057110 // sqrt(105/PI)/4

/* Y_l_m(s), where l is the band and m the range in [-l..l] 
float SH( in int l, in int m, in vec3 s ) 
{ 
	vec3 n = s.zxy;
	
    //----------------------------------------------------------
    if( l==0 )          return  k01;
    //----------------------------------------------------------
	if( l==1 && m==-1 ) return -k02*n.y;
    if( l==1 && m== 0 ) return  k02*n.z;
    if( l==1 && m== 1 ) return -k02*n.x;
    //----------------------------------------------------------
	if( l==2 && m==-2 ) return  k03*n.x*n.y;
    if( l==2 && m==-1 ) return -k03*n.y*n.z;
    if( l==2 && m== 0 ) return  k04*(3.0*n.z*n.z-1.0);
    if( l==2 && m== 1 ) return -k03*n.x*n.z;
    if( l==2 && m== 2 ) return  k05*(n.x*n.x-n.y*n.y);
    /*----------------------------------------------------------
    if( l==3 && m==-3 ) return -k06*n.y*(3.0*n.x*n.x-n.y*n.y);
    if( l==3 && m==-2 ) return  k07*n.z*n.y*n.x;
    if( l==3 && m==-1 ) return -k08*n.y*(5.0*n.z*n.z-1.0);
    if( l==3 && m== 0 ) return  k09*n.z*(5.0*n.z*n.z-3.0);
    if( l==3 && m== 1 ) return -k08*n.x*(5.0*n.z*n.z-1.0);
    if( l==3 && m== 2 ) return  k10*n.z*(n.x*n.x-n.y*n.y);
    if( l==3 && m== 3 ) return -k06*n.x*(n.x*n.x-3.0*n.y*n.y);
    

	return 0.0;
} */

// unrolled version of the above
float SH_0_0( in vec3 s ) { vec3 n = s.zxy; return  k10*n.z*(n.x*n.x-n.y*n.y); }
float SH_1_0( in vec3 s ) { vec3 n = s.zxy; return -k02*n.y; }
float SH_1_1( in vec3 s ) { vec3 n = s.zxy; return  k02*n.z; }
float SH_1_2( in vec3 s ) { vec3 n = s.zxy; return -k02*n.x; }
float SH_2_0( in vec3 s ) { vec3 n = s.zxy; return  k03*n.x*n.y; }
//float SH_2_1( in vec3 s ) { vec3 n = s.zxy; return -k03*n.y*n.z; }
float SH_2_2( in vec3 s ) { vec3 n = s.zxy; return  k04*(3.0*n.z*n.z-1.0); }
/*float SH_2_3( in vec3 s ) { vec3 n = s.zxy; return -k03*n.x*n.z; }
float SH_2_4( in vec3 s ) { vec3 n = s.zxy; return  k03*(n.x*n.x-n.y*n.y); }
float SH_3_0( in vec3 s ) { vec3 n = s.zxy; return -k06*n.y*(3.0*n.x*n.x-n.y*n.y); }
float SH_3_1( in vec3 s ) { vec3 n = s.zxy; return  k07*n.z*n.y*n.x; }
float SH_3_2( in vec3 s ) { vec3 n = s.zxy; return -k08*n.y*(5.0*n.z*n.z-1.0); }
float SH_3_3( in vec3 s ) { vec3 n = s.zxy; return  k09*n.z*(5.0*n.z*n.z-3.0); }
float SH_3_4( in vec3 s ) { vec3 n = s.zxy; return -k08*n.x*(5.0*n.z*n.z-1.0); }
float SH_3_5( in vec3 s ) { vec3 n = s.zxy; return  k10*n.z*(n.x*n.x-n.y*n.y); } */
float SH_3_6( in vec3 s ) { vec3 n = s.zxy; return  k03*n.x*(n.z*n.y-1.0*n.y); } 

vec3 map( in vec3 p )
{
	float trv = clamp(mod(-0.5,1.0)*(TIME*rate*3.0),1.0,-1.0);
	
    vec3 p00 = p - vec3( 0.0,0.5-trv,0.0);
	vec3 p01 = p - vec3(0.0,trv,0.0);
	vec3 p02 = p - vec3( 0.0, 1.0-trv,0.0);	
	vec3 p03 = p - vec3(0.0, trv+1.0,0.0);
	vec3 p04 = p - vec3(0.0,trv-0.5,0.0);
//	vec3 p05 = p - vec3(-1.25,-0.5,0.0);
	vec3 p06 = p - vec3( 0.0,trv-0.2125,0.0);
/*	vec3 p07 = p - vec3( 1.25,-0.5,0.0);
	vec3 p08 = p - vec3( 2.50,-0.5,0.0);
	vec3 p09 = p - vec3(-3.75,-2.0,0.0);
	vec3 p10 = p - vec3(-2.50,-2.0,0.0);
	vec3 p11 = p - vec3(-1.25,-2.0,0.0);
	vec3 p12 = p - vec3( 0.00,-2.0,0.0);
	vec3 p13 = p - vec3( 1.25,-2.0,0.0);
	vec3 p14 = p - vec3( 2.50,-2.0,0.0); */
	vec3 p15 = p - vec3( 0.0,0.25+trv,0.0);
	
	float r, d; vec3 n, s, res;
	
//  #ifdef SHOW_SPHERES
//	#define SHAPE (vec3(d-0.35, -1.0+2.0*clamp(0.5 + 16.0*r,0.0,1.0),d))
//	#else
	#define SHAPE (vec3(d-abs(r), sign(r),d))
//	#endif
	d=length(p00); n=p00/d; r = SH_0_0( n ); s = SHAPE; res = s;
	d=length(p01); n=p01/d; r = SH_1_0( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p02); n=p02/d; r = SH_1_1( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p03); n=p03/d; r = SH_1_2( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p04); n=p04/d; r = SH_2_0( n ); s = SHAPE; if( s.x<res.x ) res=s;
//	d=length(p05); n=p05/d; r = SH_2_1( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p06); n=p06/d; r = SH_2_2( n ); s = SHAPE; if( s.x<res.x ) res=s;
/*	d=length(p07); n=p07/d; r = SH_2_3( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p08); n=p08/d; r = SH_2_4( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p09); n=p09/d; r = SH_3_0( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p10); n=p10/d; r = SH_3_1( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p11); n=p11/d; r = SH_3_2( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p12); n=p12/d; r = SH_3_3( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p13); n=p13/d; r = SH_3_4( n ); s = SHAPE; if( s.x<res.x ) res=s;
	d=length(p14); n=p14/d; r = SH_3_5( n ); s = SHAPE; if( s.x<res.x ) res=s; */
	d=length(p15); n=p15/d; r = SH_3_6( n ); s = SHAPE; if( s.x<res.x ) res=s; 
	
	return vec3( res.x, 0.25+0.25*res.y, res.z );
}

vec3 intersect( in vec3 ro, in vec3 rd )
{
	vec3 res = vec3(1e2,-1.0, 1.0);

	float maxd = 4.0;
    float h = 1.0;
    float t = 0.0;
    vec2  m = vec2(1.0);
    for( int i=0; i<150; i++ )
    {
        if( h<0.001||t>maxd ) break;
	    vec3 res = map( ro+rd*t );
        h = res.x;
		m = res.yz;
        t += h*0.3;
    }
	if( t<maxd && t<res.x ) res=vec3(t,m);
	

	return res;
}

vec3 calcNormal( in vec3 pos )
{
    vec3 eps = vec3(0.001,0.0,0.0);

	return normalize( vec3(
           map(pos+eps.xyy).x - map(pos-eps.xyy).x,
           map(pos+eps.yxy).x - map(pos-eps.yxy).x,
           map(pos+eps.yyx).x - map(pos-eps.yyx).x ) );
}

void main()
{
    vec3 tot = vec3(0.0);
    
    for( int m=0; m<AA; m++ )
    for( int n=0; n<AA; n++ )
    {        
        vec2 p = (-RENDERSIZE.xy + 2.0*(gl_FragCoord.xy+vec2(float(m),float(n))/float(AA))) / RENDERSIZE.y;

		float T = TIME * rate;

        // camera
        float an = rotation*T - 8.0*RENDERSIZE.x;
        vec3  ro = vec3(3.0*sin(an),0.0,3.0*cos(an));
        vec3  ta = vec3(0.0,0.25,0.0);

        // camera matrix
        vec3 ww = normalize( ta - ro );
        vec3 uu = normalize( cross(ww,vec3(0.0,1.0,1.0) ) );
        vec3 vv = normalize( cross(uu,ww));
        // create view ray
        vec3 rd = normalize( p.x*uu + p.y*vv + 2.0*ww );

        // background 
        vec3 col = vec3(0.3) * clamp(1.0-length(p)*0.5,0.0,1.0);

        // raymarch/*{
        vec3 tmat = intersect(ro,rd);
        if( tmat.y>-0.25 )
        {
            // geometry
            vec3 pos = ro + tmat.x*rd;
            vec3 nor = calcNormal(pos);
            vec3 ref = reflect( rd, nor );

            // material		
            vec3 mate = mix(vec3(0.9,0.1,0.1),vec3(mix( vec3(0.2,0.8,0.1), vec3(0.1,0.2,0.8), clamp(sin(T * tmat.z),0.3,0.9))), clamp(cos(T * colorCycle + tmat.y),0.3,0.9));
            float occ = clamp( 3.0*tmat.z, 0.0, 1.0 );
           float sss = pow( clamp( 1.0 + dot(nor,rd), 0.0, 1.0 ), 1.0 );

            // lights
            vec3 lin  = 2.5*occ*vec3(matrix.x,1.0,matrix.y)*(0.6+0.4*nor.x);
                 lin += 1.0*sss*vec3(1.0,0.95,0.70)*occ;		

            // surface-light interacion
            col = mate.xyz * lin;
        }

        // gamma
        col = pow( clamp(col,0.0,1.0), vec3(0.645) );
        tot += col;
    }
  // tot /= float(AA*AA);
    gl_FragColor = vec4( tot, 1.0 );
}