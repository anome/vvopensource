/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "based on www.shadertoy.com/view/XdSSz1",
  "CATEGORIES": [
    "2d",
    "iterations",
    "fractal"
  ],
  "INPUTS": [
    {
      "NAME": "center",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MAX": [
        2,
        2
      ],
      "MIN": [
        -2,
        -2
      ]
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.75,
      "MIN": -3,
      "MAX": 3
    },
    {
      "NAME": "vectorX",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "vectorY",
      "TYPE": "float",
      "DEFAULT": 0.125,
      "MIN": -1,
      "MAX": 1
    },
    {
      "NAME": "width",
      "TYPE": "float",
      "DEFAULT": 0.075,
      "MIN": 0.025,
      "MAX": 0.125
    },
    {
      "NAME": "push",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 0.01,
      "MAX": 10
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": -16,
      "MIN": -20,
      "MAX": -1
    },
    {
      "NAME": "detail",
      "TYPE": "float",
      "DEFAULT": 16,
      "MIN": 0,
      "MAX": 19.9
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 31,
      "MIN": 7,
      "MAX": 36
    }
  ]
}*/


////////////////////////////////////////////////////////////
// LaceLikeLattice  by mojovideotech
//
// based on :
// String Theory by nimitz 
// www.shadertoy.com/view/XdSSz1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


mat2 mm2(in float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float f(vec2 p, float t)
{
	p.y = sin(p.y*1.+t*1.23)*cos(t+p.y*0.05);	
    p += sin(p.y*0.05)*.01;
    return smoothstep(-0.01,width,abs(p.x));
}

void main()
{
	float aspect = RENDERSIZE.x/RENDERSIZE.y;
	float time = TIME * rate;
	vec2 uv = vec2(gl_FragCoord.xy/RENDERSIZE.xy*1.5-center.xy);
	vec2 p = vec2(uv * 2.0 - center)*zoom;	
	p.x *= aspect;
	p.y = abs(p.y);
	vec3 col = vec3(0.0);
	float counter = depth;
	for(float i=0.;i<36.;i++)
	{
		vec3 col2 = (sin(vec3(34.,55.,89.)+i*0.125)*0.5+0.5+0.0)*(1.-f(p,time));
		col = max(col,col2);
        p.y -= 20.0 - detail;
        p.x -= sin(time*0.125+push)*1.5+1.5;
		p*= mm2(i*vectorY+vectorX);
        vec2 pa = vec2(abs(p.y-.5),abs(p.x));
        vec2 pb = vec2(p.x,abs(p.y));
        p = mix(pa,pb,smoothstep(-0.1,.1,sin(time*0.124)+.1));
        counter -= 1.0;
        if (counter<1.0) { break; 
        }
	}
	
	gl_FragColor = vec4(col,1.0);
}