
/*{
	"DESCRIPTION": "uará Lab, forked from https://www.youtube.com/watch?v=PGtv-dBi2wE",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inclination",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 250.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.3,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{
			"NAME": "colorCycle",
			"TYPE": "bool",
			"DEFAULT": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/
#define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST 0.01


float sdBoundingBox( vec3 p, vec3 b, float e )
{
       p = abs(p  )-b;
  vec3 q = abs(p+e)-e;
  return min(min(
      length(max(vec3(p.x,q.y,q.z),0.0))+min(max(p.x,max(q.y,q.z)),0.0),
      length(max(vec3(q.x,p.y,q.z),0.0))+min(max(q.x,max(p.y,q.z)),0.0)),
      length(max(vec3(q.x,q.y,p.z),0.0))+min(max(q.x,max(q.y,p.z)),0.0));
}
float sdTorus(vec3 p,vec2 r){
    float x = length(p.xz)-r.x;
    return length(vec2(x,p.y))-r.y;
}


float sdCapsule(vec3 p, vec3 a, vec3 b, float radius){
    vec3 ab = b-a;
    vec3 ap = p-a;
    float t = dot(ab,ap)/dot(ab,ab);
    t = clamp(t,0.0,1.0);
    vec3 c = a+t*ab;
    float d = length(p-c)-radius;
    return d;
}

float GetDist(vec3 p){
    vec4 s = vec4(0.0,1.0,6.0,1.0);
    
    float sphereDist = length(p-s.xyz)-s.w;
    float planeDist = p.y;
    float cd = sdCapsule(p,vec3(0.0,1.0,6.0),vec3(1.0,2.0,6.0),0.2);
    float td = sdTorus(p-vec3(0.0,0.5,6.0), vec2(1.5,0.3));
    float d = min(cd,planeDist);
    d=min(d,td);
    return d;
}

float RayMarch(vec3 ro, vec3 rd){
    float dO = 0.0;
    for(int i = 0; i< 50; i++){
        vec3 p = ro + rd *dO;
        float dS = GetDist(p);
        dO += dS;
        if(dO>MAX_DIST || dS<SURF_DIST) break;
        
    }
    return dO;
}

vec3 GetNormal(vec3 p){
    float d = GetDist(p);
    vec2 e = vec2(0.01, 0.0);
    vec3 n = d-vec3(
        GetDist(p-e.xyy),
        GetDist(p-e.yxy),
        GetDist(p-e.yyx));
        return normalize(n);
        
}

float GetLight(vec3 p){
    vec3 lightPos = vec3(0.0,5.0,6.0);
    lightPos.xz += vec2(1.0, 1.0);
    vec3 l = normalize(lightPos-p);
    vec3 n = GetNormal(p);

    float dif = clamp(dot(n,l), 0.0, 1.0);
    float d = RayMarch(p+n*SURF_DIST*2.0,l);
    if(d<length(lightPos-p)) dif*=0.1;
    return dif;
}


void main()	{

	vec2 uv = (gl_FragCoord.xy-pointInput*RENDERSIZE.xy)/RENDERSIZE.y;
	vec3 col = vec3(0.0);
	vec3 ro = vec3(0.0,2.0,0.0);
	vec3 rd = normalize(vec3(uv.x,uv.y-0.2,1.0));
    float d = RayMarch(ro,rd);
    vec3 p = ro+rd*d;
    float dif = GetLight(p);
    
    col = vec3(dif);
//	col = GetNormal(p);

	    gl_FragColor = vec4(vec3(col), 1.0);

}
