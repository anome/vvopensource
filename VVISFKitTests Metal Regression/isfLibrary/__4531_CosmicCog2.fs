/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "http://glslsandbox.com/e#27494.4",
  "INPUTS": [
       {
            "NAME":"mixer1",
            "TYPE": "float",
           "DEFAULT": 0.6,
            "MIN": 0.15,
            "MAX": 0.85
        },
            {
            "NAME":"mixer2",
            "TYPE": "float",
           "DEFAULT": 0.2,
            "MIN": 0.1,
            "MAX": 0.9
        },
         {
            "NAME":"mixer3",
            "TYPE": "float",
           "DEFAULT": 0.4,
            "MIN": 0.05,
            "MAX": 0.95
        },
         {
            "NAME": "speed",
            "TYPE": "float",
           "DEFAULT": 0.6,
            "MIN": 0.1,
            "MAX": 1.5
        },
         {
            "NAME": "zoom",
            "TYPE": "float",
           "DEFAULT": 3.8,
            "MIN": 0.25,
            "MAX": 5.0
        },
         {
            "NAME": "sides",
            "TYPE": "float",
           "DEFAULT": 6.0,
            "MIN": 2.0,
            "MAX": 20.0
        },
         {
            "NAME": "shifth",
            "TYPE": "float",
           "DEFAULT": 0.525,
            "MIN": 0.0,
            "MAX": 1.0
        },
         {
            "NAME":"shiftv",
            "TYPE": "float",
           "DEFAULT": 0.125,
            "MIN": 0.0,
            "MAX": 1.0
        }
  ]
}
*/

// CosmicCog2 by mojovideotech
// based on :
// http://glslsandbox.com/e#27494.4

#ifdef GL_ES
//precision highp float;
precision mediump float;
#endif 

   
#define ptpi 1385.45573136 //powten(pi)
#define pipi  36.46215960 //pi pied, pi^pi
#define picu  31.00627668 //pi cubed, pi^3
#define pepi  23.14069263 //powe(pi);
#define chpi  11.59195327  //cosh(pi)
#define shpi  11.54873935 //sinh(pi)
#define pisq  9.869604401 //pi squared, pi^2
#define twpi  6.283185307  //two =pi, 2*pi 
#define pi    3.141592653 //pi
#define e     2.718281828 //eulers number
#define sqpi  1.772453850 //square root of pi 
#define phi   1.618033988 //golden ratio
#define hfpi  1.570796326 //half pi, 1/pi
#define cupi  1.464591887 //cube root of pi
#define prpi  1.439619495 //pi root of pi
#define lnpi  1.144729885 //logn(pi); 
#define trpi  1.047197551 //one third of pi, pi/3 
#define thpi  0.996272076 //tanh(pi)
#define lgpi  0.497149872 //log(pi)       
#define rcpi  0.318309886 // reciprocal of pi  , 1/pi  
#define rcpipi  0.02742569 // reciprocal of pipi  , 1/pipi 

float tt = TIME;
float t = (rcpi*(pi+tt/pisq))+pepi;
float k = (rcpi*(pi+tt/chpi))+chpi;
vec3 qAxis = normalize(vec3(sin(t*(prpi)), cos(k*(cupi)), cos(k*(hfpi)) ));
vec3 wAxis = normalize(vec3(cos(k*(-trpi)/pi), sin(t*(rcpi)/pi), sin(k*(lgpi)/pi) ));
vec3 sAxis = normalize(vec3(cos(t*(trpi)), sin(t*(-rcpi)), sin(k*(lgpi)) ));
float axe = pow(qAxis.x+qAxis.y+qAxis.z+wAxis.x+wAxis.y+wAxis.z+sAxis.x+sAxis.y+sAxis.z,2.0);
vec3 camPos = (vec3(0.0, 0.0, 1.0))/(pi+twpi+sin(t)*pi);
vec3 camUp  = (vec3(0.0,1.0,0.0));
float focus = pi+sin(t)*phi;
vec3 camTarget = normalize(vec3(cos(t*(-thpi)), sin(t*(trpi)), (sin(t*(lnpi))+cos(t*(lnpi)))*0.5 ));
vec3 rotate(vec3 vec, vec3 axis, float ang)
{
	return vec * cos(ang) + cross(axis, vec) * sin(ang) + axis * dot(axis, vec) * (1.0 - cos(ang));
}

vec3 swr(vec3 p){
	vec3 col = vec3((sin(p))*0.5+0.5);
	for(int i=1; i<4; i++)	{
		float ii = float(i);
		col.xyz=(sin((col.zxy+col.yzx)*ii)*0.5+0.5)+(sin((col.zxy*col.yzx)*ii)*0.5+0.5);
		col *= (col+(mix(cos(p*ii+col*3.14)*0.5+0.5,1./(1.+col),sin(p.z)*0.49+0.5)))/hfpi;
	}
	return (normalize(col)*normalize(col.zxy)*normalize(col.yzx))*col;
}

vec3 axr(vec3 p){
	vec3 col = vec3((sin(p)));
	vec3 lol = col;
	for(int i=1; i<4; i++)	{
		float ii = float(i);
		lol.xyz = rotate(col.xyz, qAxis, ii*pi*distance(sAxis,wAxis));
		lol.yzx = rotate(col.yzx, wAxis, ii*pi*distance(qAxis,sAxis));
		lol.zyx = rotate(col.zxy, sAxis, ii*pi*distance(wAxis,qAxis));
		col += lol/pi;
	}
	
	return ((col)*0.5+0.5);
}



void main( void )
{
	vec2 pos = vec2(gl_FragCoord.x-shifth*RENDERSIZE.x,gl_FragCoord.y-shiftv*RENDERSIZE.y)/ RENDERSIZE.y*e*(5.25-zoom);
	float ang = (sin(t*lnpi)*pi)+(distance(sAxis,wAxis)+distance(qAxis,sAxis)+distance(wAxis,qAxis));
	camPos = (camPos * cos(ang) + cross(qAxis, camPos) * sin(ang) + wAxis * dot(sAxis, camPos) * (1.0 - cos(ang)))*e;
	
	vec3 camDir = normalize(camTarget-camPos);
	camUp = rotate(camUp, camDir, sin(t*prpi)*pi);
    	vec3 camSide = cross(camDir, camUp);
	vec3 sideNorm=normalize(cross(camUp, camDir));
	vec3 upNorm=cross(camDir, sideNorm);
	vec3 worldFacing=(camPos + camDir);
    	vec3 rayDir = normalize((worldFacing+sideNorm*pos.x + upNorm*pos.y - camDir*((focus))))/pi;
	vec3 tv=rayDir;
	vec3 rdt=rayDir;
	vec3 clr = (axr(rayDir*pipi));
	for(int i=1;i<16;i++) 
	{
		float ii = pow(float(i),e/(float(i)));
		rdt = rayDir;
		rayDir = rotate(rayDir, qAxis, pow((rdt.z*ii),(pi*cos(qAxis.x*pi)+pi+rcpipi)/(1.+ii)));
		rayDir = rotate(rayDir, wAxis, pow((rdt.x*ii),(pi*cos(wAxis.y*pi)+pi+rcpipi)/(1.+ii)));
		rayDir = rotate(rayDir, sAxis, pow((rdt.y*ii),(pi*cos(sAxis.z*pi)+pi+rcpipi)/(1.+ii)));
		tv  += (rayDir*ii)*(pi*axe);
		clr = (clr+(rayDir))/pi;

	}
	clr =  (axr(tv*clr)*swr(tv+clr));
	
	    float TT = tt*speed;
	 
         float sd = floor(sides);
         float aa=atan(pos.x,pos.y)+TT;
         float b=6.28319/float(sd);
         float ff = (smoothstep(2.9,2.5, cos(floor(.5+aa/b)*b-aa)*length(pos.xy)));
	
	  float ss = 1.5-length(min(pos.xy,vec2(2.1)));   
          float clr2 = (smoothstep(0.25,0.1,ss));
	
	   float r = length(pos)*2.0;
           float a = atan(pos.x,pos.y)-TT;
           float f = smoothstep(.25,-1.0, cos(a*(sd*2.0))-0.9)*1.+3.5;
           float color = ( 1.-smoothstep(f,f+0.3,r) );
	   float color2 = ( 1.1-smoothstep(f-0.333,f-0.3,r) );
	
	if(clr.x>0.001 && clr.y>0.001 && clr.z>0.001){
		clr = (sqrt(clr))/pi;
	}
	else
	{ 
		clr = (normalize(clr)/ff)-clr;
	}
	
      vec3 G1 = vec3(mod(mix(vec3(ff+color2)/vec3(clr-ff/(ff+clr2)),vec3(color,ff-f,color+ff),mixer2),vec3(ff/f,color2-ff,color2-ss)));
      vec3 G2 = vec3(mod(vec3(color2,mix(clr2,ff,mixer3)-0.5,0.5),vec3(ff-clr)));
      
     gl_FragColor = vec4(vec3(mix(G1,G2,mixer1)),1.0);
}