/*{
	"CREDIT": "by julian",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "floatInput2",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "floatInput3",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "floatInput4",
			"TYPE": "float",
			"DEFAULT": 0.6,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "floatInput6",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "floatInput7",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "Mix",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		}
		
	]
}*/


float iTime = TIME;
#define MAX_STEPS 64
#define MAX_DIST 100.
#define SURF_DIST .001




#define PI 3.14159
#define TAU PI*2.
#define t iTime

vec3 lightPos;

float LineDist(vec3 p, vec3 a, vec3 b) {
	vec3 ba = a-b,
        pa = a-p;
    
    return length(cross(ba, pa))/length(ba);
}


mat2 Rot(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(c, -s, s, c);
}

float smin( float a, float b, float k ) {
    float h = clamp( 0.5+0.5*(b-a)/k, 0., 1. );
    return mix( b, a, h ) - k*h*(1.0-h);
}

float sdCapsule(vec3 p, vec3 a, vec3 b, float r) {
	vec3 ab = b-a;
    vec3 ap = p-a;
    
    float t = dot(ab, ap) / dot(ab, ab);
    t = clamp(t, 0., 1.);
    
    vec3 c = a + t*ab;
    
    return length(p-c)-r;
}

float sdCylinder(vec3 p, vec3 a, vec3 b, float r) {
	vec3 ab = b-a;
    vec3 ap = p-a;
    
    float t = dot(ab, ap) / dot(ab, ab);
    t = clamp(t, 0., 1.);
    
    vec3 c = a + t*ab;
    
    float x = length(p-c)-r;
    float y = (abs(t-.5)-.5)*length(ab);
    float e = length(max(vec2(x, y), 0.));
    float i = min(max(x, y), 0.);
    
    return e+i;
}

float sdTorus(vec3 p, vec2 r) {
	float x = length(p.xz)-r.x;
    return length(vec2(x, p.y))-r.y;
}

float sdBox(vec3 p, vec3 s) {
    p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, max(p.y, p.z)), 0.);
}



//mat2 R;
float T;

vec2 path(float z){
    float x = sin(z) + 3.0 * cos(z * 0.5) - 1.5 * sin(z * 1.12345);
    float y = cos(z) + 1.5 * sin(z * 0.3) + .2 * cos(z * 1.12345);
    return vec2(x,y)*floatInput2 ;
}





mat2 rz2 (float a) { float c=cos(a), s=sin(a); return mat2(c,s,-s,c); }
//float sphere (vec3 p, float r) { return length(p)-r; }
//float iso (vec3 p, float r) { return dot(p, normalize(sign(p)))-r; }
float cyl (vec2 p, float r) { return length(p)-r; }
float cube (vec3 p, vec3 r) { return length(max(abs(p)-r,0.)); }

vec2 modA (vec2 p, float count) {
    float an = TAU/count;
    float a = atan(p.y,p.x)+an*.5;
    a = mod(a, an)-an*.5;
    return vec2(cos(a),sin(a))*length(p);
}



vec2 path2(float z){
      float x = sin(z) + 1.0 * cos(z * 0.4) - .5 * sin(z * 0.12345);
    float y = cos(z) + 1. * sin(z * 0.4) + .2 * cos(z * 2.12345);
    return vec2(x,y);
}






float GetDist(vec3 p) {
	
	
	

	float lightDist = length(p-lightPos);
	
	
	
   vec3 p2 = p;
 vec2 o = path(p.z) / 5.0;
 //  o = vec3(a / 5.0,iTime)
 p = vec3(p.x,p.y,p.z)-vec3(o.x,o.y,0.);  
 
 
 
 
 
    float r = 3.14159*sin(p.z*0.15)+(TIME*0.25*floatInput6);
    mat2 R = mat2(cos(r), sin(r), -sin(r), cos(r));
    p.xy *= R ;    


	       float selector = fract(sin(dot(floor(p) + 13.37, vec3(7., 157., 113.)))*43758.5453);

float boxsize;
    if (selector > .75) {
       boxsize = 0.4;
    }  else if (selector > .15) {
    boxsize = 0.3;
    } else if (selector > .25) {
	boxsize = 0.5;
    }
    

    p = fract(p) * 2. - 1.;
  
  //   p.xyz = mod(p.xyz , 2.) - 1.;

    
    
    
    
	//vec3 mirrorp=abs(p)-vec3(sin(iTime*0.2),1.0,.0);
    vec3 boxp = p;
    
    
   // boxp.xz *= Rot(-TIME*0.5);
  //   boxp.yz *= Rot(-TIME*0.5);
  //  boxp.xy += sin(boxp.z*0.1+TIME*1.);
   float box = sdBox(boxp-vec3(0,0.0,0), vec3(boxsize));
    
    
      float box2 = sdBox(boxp-vec3(0,0.0,0), vec3(0.15,1.1,0.15));
      float box3 = sdBox(boxp-vec3(0,0.0,0), vec3(1.1, 0.15,0.15));
       float box4 = sdBox(boxp-vec3(0,0.0,0), vec3(0.15, 0.15,1.1));
    
  //  float tor = sdTorus(p-vec3(sin(TIME)*.3,0,cos(TIME)*0.3), vec2(0.2,0.2));
 // float x=sin(p.z*.345)*.1-.1;
      float s = 30.2;
    float third = (abs(dot(sin(p2*s), cos(p2.zxz*s)))-.4)/s;
    box4 = mix(box4, third, 0.4);
   
    
    float d =  smin(box3*0.3,smin(box2*0.6,smin(box,box4*0.6,0.2),0.2),0.2);
    
    
    //---------------
    
        vec2 o2 = path2(p2.z) / 4.0;
    p = vec3(p2.x,p2.y,p2.z)-vec3(o2.x,o2.y,0.);
    
    
    
    
    
    
    
           p.xy *= rz2(p.z*sin(t*0.002+250.));

    
    float cyl2wave = .4+1.5*(sin(p.z+t*2.5)*.1);
    float cylfade = 2.-smoothstep(.1,8.,abs(p.z));
    float cyl2r = 0.1*cyl2wave*cylfade;
    float cylT = 2.;
    float cylC = 2.;
   // vec2 cyl2p = modA(p.xy*rz2(p.z*cylT), cylC)-vec2(cyl2wave, 0)*cylfade;
    vec2 cyl2p = modA(p.xy, (abs(sin(t*0.5)+5.)))-vec2(cyl2wave, 0)*cylfade;

    
    float cyl2 = cyl(cyl2p, cyl2r);
    cyl2p = modA(p.xy*rz2(-p.z*cylT), cylC)-vec2(cyl2wave, 0)*cylfade;
   // cyl2 = smin(cyl2, cyl(cyl2p, cyl2r),.2);
    //cyl2p = modA(p.xy*rz2(-p.z*cylT), cylC*.5)-vec2(cyl2wave, 0)*cylfade;
   
   
   
   
   
    
   // cyl2 = cyl2;
    
    
     // float x=sin(p.z*.345)*.1-.1;
     
     s = 30.2;
     third = (abs(dot(sin(p2*s), cos(p2.zxz*s)))-.4)/s;
    cyl2 = mix(cyl2, third, 0.1);
    
  /*   
    vec3 cubP = p;
    float cubC = 0.1;
   // float cubI = floor(cubP.x / cubC);
    cubP.z = mod(cubP.z, cubC)-cubC*1.;
    cubP.xy *= rz2(t*3.);
    //cubP.yz *= rz2(t*3.+cubI*8.);
    
    
    cyl2 = smin(cyl2, cube(cubP,vec3(.1*cyl2wave*cylfade)),.5);
    
    */
   
    
    /*
    // m.y=sin(p.z*.345)*.5-.2;
    float x = sin(p2.z*.345)*.5-.2;
    float s = 1.;
    float third = (abs(dot(sin(p*s), cos(p.zxy*s)))-.1)/s;
   d = mix(d, third, x);
*/
    
     
    
       float displacement = sin(.7*p.x+ TIME)*sin(.5*p.y+ TIME)*cos(.4*p.x+ TIME);
       d = d + displacement *floatInput;
    
     d = mix(d,cyl2,Mix);
     
     

 //  d = mix(d,smin(d,cyl2,0.1),Mix);
 
    
    return d;
    //smin(d,cyl2,0.1);
}

float RayMarch(vec3 ro, vec3 rd) {
 float dO=0.;
    
    for(int i=0; i<MAX_STEPS; i++) {
    	vec3 p = ro + rd*dO;
        float dS = GetDist(p);
        dO += dS;
        if(dO>MAX_DIST || abs(dS)<SURF_DIST) break;
    }
    
    return dO;
}

vec3 GetNormal(vec3 p) {
	float d = GetDist(p);
    vec2 e = vec2(.001, 0);
    
    vec3 n = d - vec3(
        GetDist(p-e.xyy),
        GetDist(p-e.yxy),
        GetDist(p-e.yyx));
    
    return normalize(n);
}

float GetLight(vec3 p) {
	    vec2 a = path(iTime * 1.0)*1.0;
     vec3 o = vec3(a / 5.0,iTime);
    vec3 lightPos =  o;
    vec3 l = normalize(lightPos-p);
    vec3 n = GetNormal(p);
    
    float dif = clamp(dot(n, l)*.5+.5, 0., 1.);
    float d = RayMarch(p+n*SURF_DIST*5., l);
   if(p.y<.01 && d<length(lightPos-p)) dif *= .5;
    
    return dif;
}



//f
vec3 R(vec2 uv, vec3 p, vec3 l, float z) {
    vec3 f = normalize(l-p),
        r = normalize(cross(vec3(0,1,0), f)),
        u = cross(f,r),
        c = p+f*z,
        i = c + uv.x*r + uv.y*u,
        d = normalize(i-p);
    return d;
}



void main()
{
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
    
    
    uv = uv * 2.0 - 1.0;
	
    uv.x *= RENDERSIZE.x / RENDERSIZE.y;
    
    vec2 a = path(iTime * 1.0)*1.0;
     vec3 o = vec3(a / 5.0,iTime);
    
    
//	vec2 m = iMouse.xy/iResolution.xy;
    
    vec3 col = vec3(0);
    
    vec3 ro = vec3(0, 0., TIME);
  // ro *= vec3(a);
    
   
   // o.yz *= Rot(sin(TIME*0.000002)*3.14+1.);
    //o.xz *= Rot(-m.x*6.2831);
   
    vec3 rd = R(uv, o, vec3(0.,0.,0), floatInput4);
   
   lightPos = vec3(o.x,o.y,o.z+0.2) ;//- vec3(0.,0.,-.3);
   
   // vec3 L = lightPos-p;	// vec from surface to light
  //  float ld = length(L);	// distance from surface to light

   
   
   
   
  if (floatInput7 == 0.){ 
    rd.yz *= Rot(TIME*0.5)*1.1;
  }
    float d = RayMarch(o, rd);
    
    float fog = 1. / (1. + d * d * 0.4);
    
vec3 p;

    if(d<MAX_DIST) {
    	 p = o + rd * d;
    
    	float dif = GetLight(p);
    	col = vec3(dif);
    	
    	   float dO = length(ro-p);
 
 
 vec3 glowCol = sin(vec3(.123, .234, .345)*TIME)*.5+.85;

   
   	   if(length(lightPos-o)<dO) {
        float minLd = LineDist(lightPos, o,o+rd); 
        vec3 light = glowCol*.25/minLd;
    //   vec3 light = vec3(1.1,1.1,1.1);
       col *= light;
    }
   	

    	
    }
     
 
     
     
     
     
     
     vec3 sky = vec3(abs(sin(TIME))-.5, .1, .1);//* mix(1., .75, mist);//*(rd.y*.25 + 1.);
   
   
    
   
   
   
    col *= vec3(fog);
   if (floatInput3 == 0.){
   
    col = mix(sky, col, 1.7/(d+d/MAX_DIST/MAX_DIST*floatInput3 +.5)); 
   
     
   	
   }
    gl_FragColor = vec4(col,1.0);
}