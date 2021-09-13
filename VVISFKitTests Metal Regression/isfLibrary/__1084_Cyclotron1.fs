/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
	"generator",  
    "2d",
    "fractal",
    "orbit trap"
  ],
  "DESCRIPTION": "Cyclotron1",
  "INPUTS": [
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 1.37,
      "MIN": 0.5,
      "MAX": 1.5
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 0.25,
      "MIN": 0.1,
      "MAX": 1.1
    },
    {
      "NAME": "seed3",
      "TYPE": "float",
      "DEFAULT": 8,
      "MIN": 0.5,
      "MAX": 60
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
      "DEFAULT": 0.125,
      "MIN": 0.05,
      "MAX": 1
    },
    {
      "NAME": "colorCycle",
      "TYPE": "float",
      "DEFAULT": 2,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "blend",
      "TYPE": "float",
      "DEFAULT": 2.5,
      "MIN": 0.1,
      "MAX": 6
    },
    {
      "NAME": "blendPoint",
      "TYPE": "float",
      "DEFAULT": -0.25,
      "MIN": -0.99,
      "MAX": 0.99
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 23,
      "MIN": 6,
      "MAX": 32
    },
    {
      "NAME": "pulseRate",
      "TYPE": "float",
      "DEFAULT": 12,
      "MIN": 1,
      "MAX": 60
    },
    {
      "NAME": "pulseDepth",
      "TYPE": "float",
      "DEFAULT": 0.075,
      "MIN": 0.001,
      "MAX": 0.1
    },
    {
      "NAME": "glow",
      "TYPE": "float",
      "DEFAULT": 1.0025,
      "MIN": 1,
      "MAX": 1.01
    },
    {
      "NAME": "c1",
      "TYPE": "color",
      "DEFAULT": [
        0.1,
        0.8,
        0.1,
        1
      ]
    },
    {
      "NAME": "c2",
      "TYPE": "color",
      "DEFAULT": [
        0.4,
        0.1,
        0.8,
        1
      ]
    },
    {
      "NAME": "c3",
      "TYPE": "color",
      "DEFAULT": [
        0.1,
        0.1,
        0.8,
        1
      ]
    },
    {
      "NAME": "c4",
      "TYPE": "color",
      "DEFAULT": [
        0.8,
        0.1,
        0.1,
        1
      ]
    },
    {
      "NAME": "c5",
      "TYPE": "color",
      "DEFAULT": [
        0.2,
        0.4,
        0.8,
        1
      ]
    },
    {
      "NAME": "c6",
      "TYPE": "color",
      "DEFAULT": [
        0.8,
        0.3,
        0.3,
        1
      ]
    }
  ]
}*/


///////////////////////////////////////////
// Cyclotron1  by mojovideotech
//
// based on:
// shadertoy.com/\4tBXRh  by Kali
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
///////////////////////////////////////////


vec3 palette[7];

vec3 gsmcolor(float c, float s) 
{
    s*=blend;
    c=mod(c-.1,6.);
    vec3 color1=vec3(0.0),color2=vec3(0.0);
    for(int i=0;i<7;i++) {
        if (float(i)-c<=.0) {
            color1 = palette[i];
            color2 = palette[(i+1>6)?0:i+1];
        }
    }
    return mix(color1,color2,smoothstep(blendPoint-s,blendPoint+s,fract(c)));
}

void main()
{
	palette[6]=vec3(c1.r,c1.g,c1.b);
	palette[5]=vec3(c2.r,c2.g,c2.b);
	palette[4]=vec3(c3.r,c3.g,c3.b);
	palette[3]=vec3(0.0);
	palette[2]=vec3(c4.r,c4.g,c4.b);
	palette[1]=vec3(c5.r,c5.g,c5.b);
	palette[0]=vec3(c6.r,c6.g,c6.b);
	
	vec3 color=vec3(0.0);
    vec2 uv = vec2(gl_FragCoord.xy/RENDERSIZE.xy);
    vec2 p = vec2(uv * 2.0 - 1.0)*zoom;	
         p.x*=RENDERSIZE.x/RENDERSIZE.y;
    float T = TIME * rate;
    float a=T*rotation/pulseRate;	
    float b=T*pulseRate;	
	float ot=100.;
	mat2 rot=mat2(cos(a),sin(a),-sin(a),cos(a));
	p.x+=sin(log(b))*pulseDepth;
	p.y+=sin(b)*pulseDepth;
    float l=length(p);
    for(int i=0;i<50;i++) {
        int bpc = int(floor(density));
        bpc -= i;
        if (bpc<1) break;
		p*=rot;
        p=abs(p)*seed1-1.;
        ot=min(ot,abs(dot(p,p)-sin(b+l*-seed3)*seed2-.5));
	}
	ot=max(-1.5,.1-ot)/.1; 
	color=gsmcolor(ot*4.+l*7.-T*colorCycle,1.)*(1.-.9*step(.999,1.-dot(p,p)));	
    color*=pow(glow,200.*l); 
    color*=1.-pow(l*1.1,5.); 
    color+=pow(max(0.,.1-l)/.2,3.)*1.2; 

    gl_FragColor = vec4(color,1.0);
}