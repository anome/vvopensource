/*{
  "CREDIT": "SilviaFabiani",
  "CATEGORIES": [
    "2d",
    "twist",
    "offset",
    "Automatically Converted"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "iChannel0",
      "TYPE": "image"
    },
    {
      "NAME": "UVSpace",
      "TYPE": "bool",
      "DEFAULT": 1
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN":0.05,
      "MAX":1.0
    },
    {
      "NAME": "phase",
      "TYPE": "float",
      "DEFAULT": 9.0,
      "MIN": 8.0,
      "MAX": 48
    },
    {
      "NAME": "thickness",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0.22,
      "MAX": 1.67
    },
    {
      "NAME": "inner",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.22,
      "MAX": 1.0
    },
    {
      "NAME": "color",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0.05,
      "MAX": 1
    },
    {
      "NAME": "center",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": -1.5,
      "MAX": 5.0
    },
    {
      "NAME": "fluffy",
      "TYPE": "float",
      "DEFAULT": 6.28,
      "MIN": 2.5,
      "MAX": 8.9
    }
  ]
}*/

// Remix of: TotallyTwistedColumns by mojovideotech
// source : www.shadertoy.com/view/Xl2GRc
// created by IQ : www.iquilezles.org/
// interactive mods by DoctorMojo : www.mojovideotech.com/

///////////////////////////////////

// Created by inigo quilez - iq/2015
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

// Based on Flyguy's "Ring Teister" https://www.shadertoy.com/view/Xt23z3. I didn't write 
// this effect since around 1999. Only now it's antialiased, motion blurred, texture 
// filtered and high resolution. 


vec4 segment( float x0, float x1, vec2 uv, float id, float time, float f )
{
    float u = (uv.x - x0)/(x1 - x0);
    float v =-1.0*(id+0.5)*time+2.0*uv.y/3.141593 + f*2.0;
    float w = (x1 - x0);
    
    vec3 col = IMG_NORM_PIXEL(iChannel0,mod(vec2(u,v),1.0)).xyz;
    col += color*sin( 2.*f + 2.0*id + vec3(0.0,1.0,2.0) );

 col *= mix( 1.0, smoothstep(-0.95,-0.94, sin(8.0*6.2831*v + 3.0*u + 2.0*f)), smoothstep(0.4,0.5, log(f*13.0)) );
 
     col *= smoothstep( inner, 0.05, 0.5-abs(u-0.5) );
    
    // lighting
    col *= vec3(0.0,0.1,0.3) + w*vec3(0.7,0.6,0.5);
    col *= mix(1.0-u,1.0,w*w*w*0.9);
    
    float edge = 1.0-smoothstep( 0.5,0.5+0.02/w, abs(u-0.5) );
    return vec4(col,  edge * step(x0,x1) );
    
}

const int numSamples = 4;

void main()
{
	vec2 uv = (-RENDERSIZE.xy+center*gl_FragCoord.xy) / max(RENDERSIZE.x,RENDERSIZE.y)/1.5;

    uv *= 21.0-3.2;
    
    vec2 st = vec2( length(uv), atan(uv.y, uv.x) );
    st = (UVSpace==false) ? uv : st;  // cartersian coordinates

    float id = floor((st.x)/2.0);
    
    vec3 tot = vec3(0.0);
    for( int j=0; j<numSamples; j++ )
    {
        float h = float(j)/float(numSamples);
        float time = (TIME + h*(1.0/30.0))*rate;
        
        vec3 col = vec3(0.2)*(1.0-0.08*st.x);

        vec2 uvr = vec2( mod( st.x, 2.0 ) - 1.0, st.y );

        float a = uvr.y + (id+0.25*1.82) * 1.82*time + 0.01*sin(3.0*uvr.y)*sin(3.0*time);
        float r = thickness;
        
        float x0 = r*sin(a);
        for(int i=0; i<5; i++ )
        {
            float f = float(i+2)/phase;
            float x1 = (r*sin(a + fluffy*f ))/1.3;

            vec4 seg = segment(x0, x1, uvr, id, time, f );
            col = mix( col, seg.rgb, seg.a ); 
            
            x0 = x1;
        }
        col *= (1.6-0.1*st.x);
        tot += col;
    }
    
    tot = tot / float(numSamples);
    
 	gl_FragColor = vec4( tot, 1.0);
}