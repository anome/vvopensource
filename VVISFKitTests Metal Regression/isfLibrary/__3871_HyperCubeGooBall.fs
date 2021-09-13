/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [],
  "INPUTS": [
    {
      "NAME": "p0",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0.1
    },
    {
      "NAME": "p1",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0.1
    },
    {
      "NAME": "p2",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 0.5,
      "DEFAULT": 0.1
    }
  ]
}*/


// HyperCubeGooBall a work in progress by mojovideotech

// based on :
// https://www.shadertoy.com/view/XtSGDK#
// Created by inigo quilez - iq/2015
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0

// set AA to 1 if you have a slow machine

#define AA 1


float udRoundBox( vec3 p, vec3 b, float r )
{
  return length(max(abs(p)-b,0.0))-r;
}

vec3 deform( in vec3 p, in float time, out float sca )
{
    float s = 0.1 * sqrt(dot(p*p,p*p));
    // float s = 1.0;

    p = p/s;

    p.xyz += 4.0*sin(0.5*vec3(1.0,1.1,1.3)*time+vec3(0.0,2.0,4.0));
    
    sca = s;
    
	return p;
}

float shape( vec3 p, float t)
{
    vec3 q =  mod( p+1.0, 2.0 ) - 1.1;

    float d1 = udRoundBox(q,vec3(0.01,0.01,2.50),0.1);
    float d2 = udRoundBox(q,vec3(0.01,2.50,0.01),0.1);
    float d3 = udRoundBox(q,vec3(2.50, 0.01, 0.01),0.1);
    float d4 = udRoundBox(mod( p + t, 2.0 ) - 1.1,vec3(p1),0.1);
 
    return min( min(d1,d2), min(d3,d4) );
}

float map( vec3 p, float t )
{
    float s = 1.1;
    p = deform( p, t, s );
    return shape( p, t ) * s;
}

vec3 calcNormal( in vec3 pos, in float eps, in float t )
{
    vec2 e = vec2(1.0,-1.0)*0.5773*eps;
    return normalize( e.xyy*map( pos + e.xyy, t ) + 
					  e.yyx*map( pos + e.yyx, t ) + 
					  e.yxy*map( pos + e.yxy, t ) + 
					  e.xxx*map( pos + e.xxx, t ) );
}

// vec3 calcNormal2( in vec3 pos, in float eps )
// {
//     vec2 e = vec2(1.0,-1.0)*0.5773*eps;
//     return normalize( e.xyy*shape( pos + e.xyy ) + 
// 					  e.yyx*shape( pos + e.yyx ) + 
// 					  e.yxy*shape( pos + e.yxy ) + 
// 					  e.xxx*shape( pos + e.xxx ) );
// }

// float calcAO( in vec3 pos, in vec3 nor )
// {
// 	float occ = 0.5;
//     for( int i=0; i<7; i++ )
//     {
//         float h = 0.01 + 0.5*float(i)/7.0;
//         occ += (h-shape( pos + h*nor ));
//     }
//     return clamp( 1.0 - 4.0*occ/6.0, 0.0, 1.0 );    
// }



vec4 texcube( sampler2D sam, in vec3 p, in vec3 n, in float k)
{
    vec3 m = pow( abs( n ), vec3(k) );
	vec4 x = IMG_PIXEL( sam, p.yz );
	vec4 y = IMG_PIXEL( sam, p.zx );
	vec4 z = IMG_PIXEL( sam, p.xy );
	return (x*m.x + y*m.y + z*m.z) / (m.x + m.y + m.z);
}

vec3 shade( in vec3 ro, in vec3 rd, in float t, float time)
{

    return vec3(1. - t);        
}

float intersect( in vec3 ro, in vec3 rd, const float maxdist, float time)
{
    float res = -1.5;
    vec3 resP = vec3(0.0);
    float t = 0.01;
    float iter = 0.0;
    for( int i=0; i<43; i++ )
    {
        vec3 p = ro + t*rd;
        float h = map( p, time);
        res = t;

        if( h<(0.01*t) || t>maxdist ) break;
        
        t += h*0.9;
        iter++;
    }
	return iter / 43.;
}

vec3 render( in vec3 ro, in vec3 rd, float time )
{
    vec3 col = vec3(0.0);
    
    const float maxdist = 43.0;
    float t = intersect( ro, rd, maxdist, time );
    if( t < maxdist )
    {
        col = shade( ro, rd, t, time );
    }

    return pow( col, vec3(0.5) );
}

mat3 setCamera( in vec3 ro, in vec3 rt, in float cr )
{
	vec3 cw = normalize(rt-ro);
	vec3 cp = vec3(cr,cr,0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, -cw );
}

void main ( void )
{	

    vec2 p = (-RENDERSIZE.xy+2.0*(gl_FragCoord.xy))/RENDERSIZE.y;
    
    float time = TIME*0.1;
    time = 41.73 + time;
    float an = 6.0 + 0.1*time;

    vec3 ro = vec3(0.5,0.0,0.5) + 2.0*vec3(cos(an),1.0,sin(an));
    vec3 ta = vec3(-0.1,0.1,-0.1);
    mat3 ca = setCamera( ro, ta, 0.1 );
    vec3 rd = normalize( ca * vec3(p,-1.5) );

    vec3 col = render( ro, rd, time );
	gl_FragColor = vec4( col, 1.0 );
}
