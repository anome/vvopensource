/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "3d",
    "raymarching"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "NAME": "offset1",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "offset2",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "offset3",
      "TYPE": "float",
      "DEFAULT": 7,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 128,
      "MIN": 16,
      "MAX": 256
    },
    {
      "NAME": "push",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.5,
      "MAX": 20
    },
    {
      "NAME": "pull",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.5,
      "MAX": 30
    },
    {
      "NAME": "radius",
      "TYPE": "float",
      "DEFAULT": 4.25,
      "MIN": 0.5,
      "MAX": 8
    },
    {
      "NAME": "bend",
      "TYPE": "float",
      "DEFAULT": 2,
      "MIN": 0.5,
      "MAX": 6
    },
    {
      "NAME": "edges",
      "TYPE": "float",
      "DEFAULT": 0.01,
      "MIN": 0.001,
      "MAX": 0.33
    },
    {
      "NAME": "multiplier",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.1,
      "MAX": 36
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": -2,
      "MAX": 2
    },
    {
      "NAME": "color1",
      "TYPE": "float",
      "DEFAULT": 21,
      "MIN": 2,
      "MAX": 89
    },
    {
      "NAME": "color2",
      "TYPE": "float",
      "DEFAULT": 34,
      "MIN": 3,
      "MAX": 233
    },
    {
      "NAME": "color3",
      "TYPE": "float",
      "DEFAULT": 55,
      "MIN": 5,
      "MAX": 337
    }
  ]
}*/

////////////////////////////////////////////////////////////
// HoleWorld1  by mojovideotech
//
// based on :
// https://www.shadertoy.com/view/XlBGRd by BrwnRyce
//
////////////////////////////////////////////////////////////



vec2 dSphere(vec3 p){
  	float d = length(p)-radius;
  	return vec2(d,floor(push));
}

vec2 oSub(vec2 a, vec2 b){
  	float d = max(a.x, -b.x);
  	return vec2(d,floor(pull));
}

vec2 infiniteSpheres(vec3 p, vec3 c){
    vec3 q = vec3(mod(p.x,c.x)-0.5*c.x, mod(p.y,c.y)-0.5*c.y, mod(p.z,c.z)-0.5*c.z);
	return dSphere(q);
}

vec2 dist(vec3 p) { return oSub(vec2(-1.0,2.0),infiniteSpheres(p,vec3(offset1, offset2, offset3))); }

void main()
{
	float T = TIME*rate;
	vec2 q = gl_FragCoord.xy/RENDERSIZE.xy;
    vec2 vPos = -1.0+bend*q;
    vec3 camUp = vec3(0, 2.0*cos(T*0.1), -2.0*sin(T*0.1)); 
    vec3 camPos = vec3(T*5.0+2.0*cos(T), 3.5, 3.5);
  	vec3 camLook = vec3(camPos.x-sin(T*0.5), camPos.y+0.2*cos(T*0.5), camPos.z+(pull-push));
    vec3 vpn = normalize(camLook-camPos);
  	vec3 u = normalize(cross(camUp,vpn));
  	vec3 v = cross(vpn,u);
	vec3 vcv = camPos+vpn;
  	vec3 scrCoord = vcv+vPos.x*u*RENDERSIZE.x/RENDERSIZE.y+vPos.y*v;
  	vec3 scp = normalize(scrCoord-camPos);
  	vec3 e = vec3(edges,0,0);
    float maxDepth = floor(depth);
  	vec2 d = vec2(0.02,0.1);
  	vec3 p;
  	float f = 1.0;
  	for(int i=0; i<256; i++) {
    	if ((abs(d.x) < .001) || (f > maxDepth)) break ;
    	f += d.x;
    	p = camPos+scp*f;
    	d = dist(p);
  	}
  	if (f < maxDepth){
    	vec3 color = vec3(0.25+0.5*cos(mod(p.x*multiplier,color1*fract(color2))),
                          0.25+0.5*cos(mod(p.x*multiplier,color2*fract(color3))),
                          0.25+0.5*cos(mod(p.x*multiplier,color3*fract(color1))));
  		vec3 n = vec3(d.x-dist(p-e.xyy).x, d.x-dist(p-e.yxy).x, d.x-dist(p-e.yyx).x);
    	vec3 N = normalize(n);
    	gl_FragColor=vec4(color*dot(N,normalize(camPos-p)),1.0);
    } 
    else
    	gl_FragColor=vec4(0.0,0.0,0.0,1.0);
    
}