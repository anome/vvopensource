/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "gears",
    "Automatically Converted"
  ],
  "DESCRIPTION": "https://www.shadertoy.com/view/XslXW2 by iq.  Something annoyingly mechanical",
  "IMPORTED": [],
  "INPUTS": [
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.1,
      "MAX": 1.25
    },
    {
      "NAME": "blur",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.5,
      "MAX": 2
    }
  ]
}*/

////////////////////////////////////////////////////////////
// IQ_AnnoyinglyMechanical  by mojovideotech
//
// source : shadertoy.com/\view/XslXW2
// Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
////////////////////////////////////////////////////////////


float fstep(float x)
{
	// originally "df=fwidth(x)" but returned error, changed to an input // - mojovideotech
    float df = 0.01*blur;
    return smoothstep( -df, df,  x);
}

float hash( vec2 p )
{
    return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453123);
}

void main()
{
    vec2  p = (-RENDERSIZE.xy + 2.0*gl_FragCoord.xy)/RENDERSIZE.y;
    vec2  q = gl_FragCoord.xy/RENDERSIZE.xy;
    
    float di = hash( gl_FragCoord.xy );
    
    const int numSamples = 6;
    
    float img = 0.0;
    float acc = 0.0;
    for( int i=0; i<numSamples; i++ )
    {
        float nt = (float(i)+di)/float(numSamples);
            
        float time = (TIME - nt*(1.0/24.0))*rate;
        
        float si = mod( floor(time*0.25), 2.0 );

        float ftime = pow(clamp(4.0*fract(time),0.0,1.0),4.0);
        float itime = floor(time);
        float atime = (itime + ftime)*1.0 + time;
        
        vec2 ce = vec2(0.0,0.0);
        ce.x = -0.30 + 1.25*(ftime - fract(time));
        ce.y =  0.15 - 0.15*smoothstep(0.0,0.3,abs(fract(time+0.65)-0.5))*(1.0-si);
        
        float d = length( p - ce );
        float a = atan( p.y - ce.y, p.x - ce.x ) + atime;
        float r = 0.7 + 0.1*smoothstep(-0.3,0.3,cos(10.0*a));
        float h = r - d;
        
        float f = fstep( h );
        f *= fstep( abs(d-0.4) - 0.1 + 0.05*smoothstep(-0.3,0.3,cos(5.0*a)) );
        f *= fstep( abs(d-0.1) - 0.02 );

        vec2 c = vec2(d,a);
        float pe = 6.2831/10.0;
        c.y = mod( a+pe*0.5, pe ) - pe*0.5;
        c = c.x*vec2( cos(c.y), sin(c.y) );
        f *= fstep( abs(length(c-vec2(0.6,0.0))-0.05)-0.01 );

        a -= si*0.5*clamp(4.0*fract(time*4.0),0.0,1.0);
        c = vec2(d,a);
        c.y = mod( a+pe*0.5, pe ) - pe*0.5;
        c = c.x*vec2( cos(c.y), sin(c.y) );
        f *= fstep( length(c-vec2(0.2,0.0))-0.02 );

        f = max( f, 1.0*fstep( -0.8-p.y+ce.y + 0.1*smoothstep(-0.3,0.3,sin(1.5+14.0*(p.x-ce.x) +10.0*atime)) ) );
        
        float w =1.0 - 0.2*nt;
        img += w * f;
        acc += w;
    }
    img /= acc;
    
    vec3 bg = vec3(1.0- 0.6*q.y) - 0.15*hash( gl_FragCoord.yy) + 0.05*di;
    bg = smoothstep( vec3(0.2125,0.2,0.15), vec3(0.8,0.75,0.9), bg );
    
    vec3 fg = vec3(0.075,0.1,0.0);
    vec3 col = mix( bg, fg, img );
    
      col *= pow(16.0*q.x*q.y*(1.0-q.x)*(1.0-q.y), 0.2);
      col /= smoothstep( 0.0, 3.0, TIME*rate);
   // col += ( img / exp(di));

	gl_FragColor = vec4( col, 1.0 );
}