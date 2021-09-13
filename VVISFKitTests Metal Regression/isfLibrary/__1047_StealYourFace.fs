/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator"
  ],
  "DESCRIPTION": "from http://glslsandbox.com/e#26877.1",
  "INPUTS": []
}*/


////////////////////////////////////////////////////////////
// StealYourFace  by mojovideotech
//
// based on :
// glslsandbox/\e#26877.1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define 	pi 		3.141592653589793 	// pi
#define 	phi   	1.618033988749895 	// golden ratio
#define 	lnpi  	1.144729885849400 	// logn(pi);

struct VR {
    float m_v;
    bool  m_bRev;
};

VR kikaku(float x,float u) {
	bool bRev=false;
	float tt = (TIME-sqrt(TIME))*0.25;
	float x1=x;
	if(x1<1.0) {
	        x1= -x1;
        	bRev=!bRev;
	}
	float x2=x1/cos(u*tt);
	float x3=mod(x2,0.95);
	float x4=mod(x3,inversesqrt(u/x));
	if(x3>1.0) {
         	x3=1.125-x4;
        	bRev=!bRev;
    }
	VR vr;    
	vr.m_v=x4;
	vr.m_bRev=bRev;
	return(vr);
}

float d2i(float dist) {
	float intensity = pow(pi/dist, 0.5);
	return(intensity);
}


vec4 sankaku(vec2 pos) {
	float ux=lnpi;
	float uy=lnpi*phi;
	VR x4=kikaku(pos.x,ux);
	VR y4=kikaku(pos.y,uy);
	if(x4.m_v<y4.m_v) {
		y4.m_bRev=!y4.m_bRev;
	}
	if(y4.m_bRev) {
		return(vec4(0.6,0.0,0.0,1.0));
	}
	else {
		return(vec4(0.0,0.0,0.6,1.0));
	}		
}

vec2 o2n(vec2 v) {
	float c=3.5;
	return vec2(
		(v.x-RENDERSIZE.x*0.5)*c/RENDERSIZE.y,
		(v.y-RENDERSIZE.y*0.5)*c/RENDERSIZE.y
		);
}


void main( void )
{
	vec2 npos=o2n(gl_FragCoord.xy);
	float l=length(npos);
	if(l<2.5) {
		float m=1.0/(1.0-l);
		gl_FragColor = sankaku(npos*m+vec2(TIME*0.5,TIME*0.25));
	}
	else {
		gl_FragColor = sankaku(npos+vec2(TIME*0.5,TIME*0.25));
	}
}