/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES" : [
    "generator"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
  ],
  "ISFVSN" : 2.0
}
*/


////////////////////////////////////////////////////////////////////
// SpaceFleetFormation  by mojovideotech
//
// based on :
// shadertoy.com/view/llBSRm
//
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////

#ifdef GL_ES
precision highp float;
#endif


#define 	twpi  	6.283185307179586  	// two pi, 2*pi
#define 	trpi  	1.047197551196598 	// one third of pi, pi/3

#define 	EPSN 	0.001
#define 	ITERS 	60
#define 	MAXD 	12.0
#define		T       TIME * 0.125


mat2 rot(in float a) { float c = cos(a), s = sin(a); return mat2(c,-s,s,c); }

float hexagon(vec3 p, vec2 h) {
	vec3 q = abs(p);
	return max(q.y - h.y, max(dot(vec2(cos(trpi), sin(trpi)), q.zx), q.z) - h.x);
}

float sphere(vec3 pos, float radius) { return length(pos) - radius; }

float opRep( vec3 p, vec3 c ) { return sphere(mod(p,c)-0.5*c, 0.05); }

float opRep2( vec3 p, vec3 c ) { return hexagon(mod(p,c)-0.5*c, vec2(0.095,0.0125)); }

float distFunct(vec3 pos) { return min(opRep2(pos,vec3(0.5)), opRep(pos,vec3(0.5))); }

vec3 getNormal(vec3 pos) { 
    vec2 eps = vec2(0.0, EPSN);
	vec3 normal = normalize(vec3(
    distFunct(pos + eps.yxx) - distFunct(pos - eps.yxx),
    distFunct(pos + eps.xyx) - distFunct(pos - eps.xyx),
    distFunct(pos + eps.xxy) - distFunct(pos - eps.xxy)));
    return normal;
}

vec3 render(vec2 st) {
    vec3 cp = vec3(1.0, 1.0, 1.0);
    cp *= vec3((smoothstep(-3.0, 3.0, sin(T*0.2+cos(T)))-0.5)*2.0, 1.0, T * 3.0);
    vec3 ct = vec3((smoothstep(5.0, -5.0, cos(T*0.2+sin(T)))-0.5), T*0.2, 1.0); 
    vec3 cu = normalize(vec3(0.0, 1.0, 0.0));
	vec3 cd = normalize(ct + cp);
	cu.zy *= rot(sin(T)-cos(T)-T);
	cu.xz *= rot(sin(T)-cos(T)+T);
	vec3 cr = normalize(cross(cu,cd));
	cu = normalize(cross(cd,cr));
    vec3 rd = normalize(cd+st.x*cr+st.y*cu);
	float dist = distFunct(cp);
	float total = dist; 
		for(int i = 0; i<ITERS; i++) {
		dist = distFunct(cp+rd*total);
		total += dist;						   
		if(dist<EPSN || dist>MAXD) continue;  
	}
	vec3 fd = cp+rd*total; 
	float fog = 1.0 / (1.0 + total * total * 0.4);
	vec3 col = vec3(fog);					
	if(dist<EPSN) { col.gbr += getNormal(fd); } 
	else { col = vec3(0.0); }
    return vec3(col*fog);
}

void main( ) 
{
    vec2 uv = (2.0 * (gl_FragCoord.xy / RENDERSIZE.xy) - 1.0 );
    uv.y *= RENDERSIZE.y/RENDERSIZE.x;	
    vec3 c = render(uv); 	
	gl_FragColor = vec4(c,1.0); 
}
