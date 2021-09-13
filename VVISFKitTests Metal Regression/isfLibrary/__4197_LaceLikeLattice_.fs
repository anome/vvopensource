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
    		"MAX" : [
        		2.0,
        		2.0
      		],
      		"MIN" : [
        		-2.0,
        		-2.0
      		]
    	},
		{
			"NAME": "vectorX",
			"TYPE": "float",
			"DEFAULT": 5.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		{
			"NAME": "vectorY",
			"TYPE": "float",
			"DEFAULT": 0.125,
			"MIN": -1.0,
			"MAX": 1.0
		},
		{
			"NAME": "rate",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "push",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "width",
			"TYPE": "float",
			"DEFAULT": 0.075,
			"MIN": 0.025,
			"MAX": 10.0
		},
		{
			"NAME": "zoom",
			"TYPE": "float",
			"DEFAULT": 0.18,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "detail",
			"TYPE": "float",
			"DEFAULT": 0.75,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "depth",
			"TYPE": "float",
			"DEFAULT": 31.0,
			"MIN": 0.0,
			"MAX": 36.0
		},
		{
			"NAME": "GRAYSCALE",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "NEON",
			"TYPE": "bool",
			"DEFAULT": 0.0
		}
		
	]
}*/

// LaceLikeLattice by mojovideotech
// based on :
// String Theory by nimitz 
// www.shadertoy.com/view/XdSSz1


mat2 mm2(in float a){float c = cos(a), s = sin(a);return mat2(c,-s,s,c);}

float f(vec2 p, float t)
{
	p.y = sin(p.y*1.+t*1.23)*cos(t+p.y*0.05);	
    p += sin(p.y*0.05)*.01;
    return smoothstep(-0.0,width,abs(p.x));
}

void main()
{
	float aspect = RENDERSIZE.x/RENDERSIZE.y;
	float time = TIME * rate * 10.;
	vec2 uv = vec2(gl_FragCoord.xy/RENDERSIZE.xy*1.5-center.xy);
	vec2 p = vec2(uv * 2.0 - center)*((1.-zoom)*-100.-1.);	
	p.x *= aspect;
	p.y = abs(p.y);
	vec3 col = vec3(0.0);
	vec3 col2 = vec3(0.0);
	float counter = depth;
	for(float i=0.;i<36.;i++)
	{
		if (GRAYSCALE) { col2 = (fract(vec3(34.,55.,89.)+i*0.125)*0.5+0.5+0.0)*(1.-f(p,time)); }  // FUNCTION: original sin, tan is warmer, fract is grayscale }
		else { if (NEON) { col2 = ( tan(vec3(34.,55.,89.)+i*0.125)*0.5+0.5+0.0)*(1.-f(p,time)); }  // FUNCTION: original sin, tan is warmer, fract is grayscale )
				else { col2 = ( cos(vec3(34.,55.,89.)+i*0.125)*0.5+0.5+0.0)*(1.-f(p,time)); }  // FUNCTION: original sin, tan is warmer, fract is grayscale )
		}
		col = max(col,col2);
        p.y -= 20.0 - detail*20.0;
        p.x -= sin(time*0.125+(1.-push*2.)*3.1416)*1.5+1.5;
		p*= mm2(i*vectorY+vectorX);
        vec2 pa = vec2(abs(p.y-.5),abs(p.x));
        vec2 pb = vec2(p.x,abs(p.y));
        p = mix(pa,pb,smoothstep(-0.1,.1,sin(time*0.124)+.1));
        counter -= 1.0;
        if (counter<1.0) { break; }
	}
	
	gl_FragColor = vec4(col,1.0);
}