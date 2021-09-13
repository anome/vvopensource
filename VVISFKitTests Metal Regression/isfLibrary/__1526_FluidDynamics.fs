/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#22675.7",
  "INPUTS": [
    
  ]
}
*/




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
	vec2 pos = vv_FragNormCoord*e;
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
	if(clr.x>0.001 && clr.y>0.001 && clr.z>0.001){
		clr = (sqrt(clr))/phi;
	}
	else
	{ 
		clr = (normalize(clr)*clr);
	}
		
	
	gl_FragColor = vec4((clr),1.0);
}