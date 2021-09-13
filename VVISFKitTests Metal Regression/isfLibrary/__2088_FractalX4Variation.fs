/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
	{
            "NAME": "seed",
            "TYPE": "float",
           "DEFAULT": 0.976,
            "MIN": 0.855,
            "MAX": 1.000
        },
        {
            "NAME": "offset",
            "TYPE": "float",
           "DEFAULT": 3.5,
            "MIN": 1.0,
            "MAX": 20.0
        },
        {
            "NAME": "tone",
            "TYPE": "float",
           "DEFAULT": 3.15,
            "MIN": 3.11,
            "MAX": 3.19
        },
         {
            "NAME": "depth",
            "TYPE": "float",
           "DEFAULT": 0.7,
            "MIN": 0.1,
            "MAX": 0.8
        },
         {
            "NAME": "detail",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0.4,
            "MAX": 1.0
         }
	]
}*/

// FractalX4Variation by mojovideotech

// based on :
// https://www.shadertoy.com/view/4tSGWd
// Created by Stephane Cuillerdier - Aiekick/2015
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

void main ( void )
{
	float t = sin(TIME*.1)*depth+(detail-(depth*0.5));
    
    vec2 s = RENDERSIZE.xy;
    vec2 g = gl_FragCoord.xy;
    vec2 uv = (offset*g-s)/s.y;
    
    vec2 mo = s / 2. * vec2(seed, t);
    mo = (2.*mo-s)/s.y;
        
    float 
    	x=uv.x,
        y=uv.y,
        m=0.;
        
        
	for (int i=0;i<36;i++)
    {
        // kali formula 
        // from http://www.fractalforums.com/new-theories-and-research/very-simple-formula-for-fractal-patterns
     	x=abs(x);
        y=abs(y);
        m=x*x+y*y;
        x=x/m+mo.x;
        y=y/m+mo.y;
    }
         
    vec2 res = abs(vec2(x,y))/(x*y)+uv;
        
    float d = dot(res,res.yx);
        
    float tt = sin(tone);
        
    float rr = mix(1./d, d, abs(tt));
    float gg = mix(rr, d, abs(tt));
    float bb = mix(gg, d, abs(tt));
    vec3 c = vec3(rr,gg,bb);
    
    gl_FragColor.brg = c;
}