/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
		
      "MAX": [
        3.0,
        3.0
      ],
      "MIN": [
        -1.5,
        -1.5
      ],
      "DEFAULT":[0.5,0.5],
			"NAME": "XY",
			"TYPE": "point2D"
		},
		{
            "NAME": "Z",
            "TYPE": "float",
           "DEFAULT": 0.0,
            "MIN": -0.9,
            "MAX": 0.9
        },
        		{
            "NAME": "orbit",
            "TYPE": "float",
           "DEFAULT": 10.0,
            "MIN": 0.5,
            "MAX": 99.0
        },
        	{
            "NAME": "synrgy",
            "TYPE": "float",
           "DEFAULT": 0.7,
            "MIN": 0.5,
            "MAX": 1.25
        },   
        {
            "NAME": "burn",
            "TYPE": "float",
           "DEFAULT": 0.00001,
            "MIN": 0.000001,
            "MAX": 0.001
        },
        {
            "NAME": "blend",
            "TYPE": "float",
           "DEFAULT": 1.0,
            "MIN": 0.5,
            "MAX": 1.0
        }
	]
}*/

// MolecularWatercolor by mojovideotech
// based on: 
// http://glslsandbox.com/e#25631.3


#ifdef GL_ES
//precision highp float;
precision mediump float;
#endif

  
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

//float tt = (TIME*e+XY.y/(XY.x+length((vec3(vv_FragNormCoord,0.5+atan(TIME*rcpi)))))*pisq);
float tt = ((TIME));
float t = (rcpi*(pi+tt/pisq))+pepi;
float k = (lgpi*(pi+tt/chpi))+chpi;

vec3 qAxis = normalize(vec3(sin(t*(prpi)), cos(k*(cupi)), cos(k*(hfpi)) ));
vec3 wAxis = normalize(vec3(cos(k*(-trpi)/pi), sin(t*(rcpi)/pi), sin(k*(lgpi)/pi) ));
vec3 sAxis = normalize(vec3(cos(t*(trpi)), sin(t*(-rcpi)), sin(k*(lgpi)) ));
float axe = pow(qAxis.x+qAxis.y+qAxis.z+wAxis.x+wAxis.y+wAxis.z+sAxis.x+sAxis.y+sAxis.z,2.0);

vec3 camPos = (vec3(0.0, 1.0, -1.0));
vec3 camUp  = (vec3(0.0,-1.0,0.0));
float focus = pi+sin(t)*phi;
vec3 camTarget = vec3(Z);

vec3 rotate(vec3 vec, vec3 axis, float ang)
{
    return vec * cos(ang) + cross(axis, vec) * sin(ang) + axis * dot(axis, vec) * (1.0 - cos(ang));
}



vec3 pin(vec3 v)
{
    vec3 q = vec3(0.1);
   
    q.x = sin(v.x)*0.5+0.5;
    q.y = sin(v.y+0.66666667*pi)*0.5+0.5;
    q.z = sin(v.z+1.33333333*pi)*0.5+0.5;
   
    return normalize(q);
}

vec3 spin(vec3 v)
{
    for(int i = 1; i <7; i++)
    {
        v=pin((v.yzx*twpi)*(float(i)));
    }
    return v.zxy;
}
float len(vec3 v) {
    vec3 vt = v;
	vt = mod(v,0.5)-0.25;
    return max(max(-min(min(length(vt.xy)-0.125,length(vt.yz)-0.125),length(vt.zx)-0.125),length(vt)-0.2),max(0.0,length(v)-pi));
    // return max(0.0,length(v)-pi);
}

float map( vec3 p, float s )
{

	float scale = 1.0;

	vec4 orb = vec4(orbit); 
	
	for( int i=0; i<8;i++ )
	{
		p = -1.0 + 2.0*fract(0.5*p+0.5);

		float r2 = dot(p,p);
		
        orb = min( orb, vec4(abs(p),r2) );
		
		float k = max(s/r2,0.1);
		p     *= k;
		scale *= k;
	}
//	return max(0.0, length(z)-.5);
	return 0.25*abs(p.y)/scale;
}


void main ( void )
{
    vec2 pos = (vv_FragNormCoord);
    float ang = (sin(t*lnpi)*pi)+(distance(sAxis,wAxis)+distance(qAxis,sAxis)+distance(wAxis,qAxis));
    camPos = hfpi*(camPos * cos(ang) + cross(qAxis, camPos) * sin(ang) + qAxis * dot(qAxis, camPos) * (1.0 - cos(ang)));
   
    vec3 camDir = normalize(camTarget-camPos);
    camUp = rotate(camUp, camDir, sin(t*prpi)*pi);
    vec3 camSide = cross(camDir, camUp);
    vec3 sideNorm=normalize(cross(camUp, camDir));
    vec3 upNorm=cross(camDir, sideNorm);
    vec3 worldFacing=(camPos + camDir);
    vec3 rayDir = -normalize((worldFacing+sideNorm*pos.x + upNorm*pos.y - camDir*((focus))));
   
    vec3 clr = (rayDir);
   
    vec3 vx = camPos;
   
    float t = (synrgy);
    for(int i = 0 ; i < 33; i++) {
        float temp = map((camPos + rayDir * (t*pi)),t*hfpi)*.4;
        if(temp < burn) break;

        t += temp;
        camPos = rotate(camPos,((rayDir)),(temp/pi));
    }
    clr = (camPos + rayDir * (t*pi));
    clr = pin(twpi*cross(pin(clr)/t,camPos/t))/(t);
    gl_FragColor = vec4(clr*clr, blend);
}