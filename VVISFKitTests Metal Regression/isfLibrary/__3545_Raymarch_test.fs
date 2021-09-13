/*{
	"CREDIT": "by julian",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/
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
		}
	]
}*/
float iTime = TIME;
#define MAX_STEPS 64
#define MAX_DIST 100.
#define SURF_DIST .001

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



float GetDist(vec3 p) {
   

    float r = 3.14159*sin(p.z*0.15)+(TIME*0.25*2.);
    mat2 R = mat2(cos(r), sin(r), -sin(r), cos(r));
    p.xy *= R ;    

    p = fract(p) * 2. - 1.;

    vec3 boxp = p;
    
    
    boxp.xz *= Rot(-TIME*0.5);
     boxp.yz *= Rot(-TIME*0.5);
  //  boxp.xy += sin(boxp.z*0.1+TIME*1.);
   float box = sdBox(boxp-vec3(0,0.0,0), vec3(0.35));
    
    
      float box2 = sdBox(boxp-vec3(0,0.0,0), vec3(0.15,.93,0.15));
      float box3 = sdBox(boxp-vec3(0,0.0,0), vec3(.93, 0.15,0.15));
       float box4 = sdBox(boxp-vec3(0,0.0,0), vec3(0.15, 0.15,.93));
    
  //  float tor = sdTorus(p-vec3(sin(TIME)*.3,0,cos(TIME)*0.3), vec2(0.2,0.2));
   
    
    float d =  smin(box3*0.6,smin(box2*0.6,smin(box,box4*0.6,0.2),0.2),0.2);
    return d*0.4;
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
    vec3 lightPos = vec3(0., 0.*0.5, TIME);
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
    
    
    
    
//	vec2 m = iMouse.xy/iResolution.xy;
    
    vec3 col = vec3(0);
    
    vec3 ro = vec3(0, 0., TIME);
    
      ro.yz *= Rot(TIME*0.00001);
   // ro.yz *= Rot(sin(TIME*0.000002)*3.14+1.);
   // ro.xz *= Rot(-m.x*6.2831);
   
    vec3 rd = R(uv, ro, vec3(0.,0.,0), .6);

    float d = RayMarch(ro, rd);
    
    float fog = 1. / (1. + d * d * 0.4);
    
    
    
    if(d<MAX_DIST) {
    	vec3 p = ro + rd * d;
    
    	float dif = GetLight(p);
    	col = vec3(dif);
    }
       vec3 sky = vec3(sin(TIME), .2, .2);//* mix(1., .75, mist);//*(rd.y*.25 + 1.);
   
  /*   
    vec3 LIGHT_COLOR = vec3(sin(iTime), 0.9, 0.9)*1.;
  
    float gres	= GlowMarch(ro, rd);
    col *= gres * LIGHT_COLOR*.4;
    */
  col *= vec3(fog);
  //  col = mix(sky, col, .7/(d*d/MAX_DIST/MAX_DIST*20. + 1.)); 
    gl_FragColor = vec4(col,1.0);
}