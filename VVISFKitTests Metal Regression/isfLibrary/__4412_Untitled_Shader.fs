
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "isfBoxSize",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
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

/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/tllfzf by illus0r.  Experimenting with this code https://www.shadertoy.com/view/4tcGDr",
  "INPUTS" : [
    {
      "NAME" : "iMouse",
      "TYPE" : "point2D"
    }
  ]
}
*/
/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/tllfzf by illus0r.  Experimenting with this code https://www.shadertoy.com/view/4tcGDr",
  "INPUTS" : [
    {
      "NAME" : "iMouse",
      "TYPE" : "point2D"
    }
  ]
}
*/


#define iTime TIME
#define iResolution RENDERSIZE
#define iMouse pointInput
//#define fragCoord gl_FragCoord.xy

////////////////////////////////////////////
// SHADERTOY BEGINS
////////////////////////////////////////////


// "RayMarching starting point" 
// by Martijn Steinrucken aka BigWings/CountFrolic - 2020
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// 
// You can use this shader as a template for ray marching shaders

#define MAX_STEPS 200
#define MAX_DIST 200.
#define SURF_DIST .0001

#define S smoothstep

mat2 Rot(float a) {
    float s=sin(a), c=cos(a);
    return mat2(c, -s, s, c);
}

float Hash21(vec2 p) {
    p = fract(p*vec2(123.34,233.53));
    p += dot(p, p+23.234);
    return fract(p.x*p.y);
}

float hash(vec3 p)  // replace this by something better
{
    p  = 50.0*fract( p*0.3183099 + vec3(0.71,0.113,0.419));
    return -1.0+2.0*fract( p.x*p.y*p.z*(p.x+p.y+p.z) );
}


float sdBox(vec3 p, vec3 s) {
    p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, max(p.y, p.z)), 0.);
}


float random (float x) {
    return fract(12345.67 * sin(x * 654.24));
}

// return value noise (in x) and its derivatives (in yzw)
vec4 noised( in vec3 x )
{
    vec3 i = floor(x);
    vec3 w = fract(x);
    
#if 1
    // quintic interpolation
    vec3 u = w*w*w*(w*(w*6.0-15.0)+10.0);
    vec3 du = 30.0*w*w*(w*(w-2.0)+1.0);
#else
    // cubic interpolation
    vec3 u = w*w*(3.0-2.0*w);
    vec3 du = 6.0*w*(1.0-w);
#endif    
    
    
    float a = hash(i+vec3(0.0,0.0,0.0));
    float b = hash(i+vec3(1.0,0.0,0.0));
    float c = hash(i+vec3(0.0,1.0,0.0));
    float d = hash(i+vec3(1.0,1.0,0.0));
    float e = hash(i+vec3(0.0,0.0,1.0));
	float f = hash(i+vec3(1.0,0.0,1.0));
    float g = hash(i+vec3(0.0,1.0,1.0));
    float h = hash(i+vec3(1.0,1.0,1.0));
	
    float k0 =   a;
    float k1 =   b - a;
    float k2 =   c - a;
    float k3 =   e - a;
    float k4 =   a - b - c + d;
    float k5 =   a - c - e + g;
    float k6 =   a - b - e + f;
    float k7 = - a + b + c - d + e - f - g + h;

    return vec4( k0 + k1*u.x + k2*u.y + k3*u.z + k4*u.x*u.y + k5*u.y*u.z + k6*u.z*u.x + k7*u.x*u.y*u.z, 
                 du * vec3( k1 + k4*u.y + k6*u.z + k7*u.y*u.z,
                            k2 + k5*u.z + k4*u.x + k7*u.z*u.x,
                            k3 + k6*u.x + k5*u.y + k7*u.x*u.y ) );
}



float GetDist(vec3 p) {
    
    float len = length(p);
    float boxTooClose = sdBox(p, vec3(1.0));
    p.x += iTime / 1.;

    float scale = 0.3;
    float dist = 5.;
    vec3 size = vec3(isfBoxSize);
	
    float idx = floor((p.x + size.x) / dist);
    float idy = floor((p.y + size.y) / dist);
    float idz = floor((p.z + size.z) / dist);
    float id = idx + idy * 123.1 + idz * 931.7;
	
    
    
    
    //p *= scale;
	
    
    p = mod(p + dist * 0.5, dist) - dist * 0.5;
	
    //p.x += 0.2 * random(id + 100.5);
    //p.y += 0.2 * random(id + 213.1);
    //p.z += 0.2 * random(id);
    //p.yz *= Rot(id);
    //p.xz *= Rot(id / 4.);
    size *= 0.5 + 0.5 * sin(len / 10. - iTime * 4.);
    
    float box = sdBox(p, size);
    //box *= 0.5;
    return max(box, -boxTooClose);
    
    //float n = noised(p).x + 0.8 + 0.5 * sin((len + 10.) / 5. - iTime);
    
    //return max(n, -boxTooClose);
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

vec3 GetRayDir(vec2 uv, vec3 p, vec3 l, float z) {
    vec3 f = normalize(l-p),
        r = normalize(cross(vec3(0,1,0), f)),
        u = cross(f,r),
        c = f*z,
        i = c + uv.x*r + uv.y*u,
        d = normalize(i);
    return d;
}



//float sdBox( in vec2 p, in vec2 b )
//{
//    vec2 d = abs(p)-b;
//    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
//}


float sdBox2d( in vec2 p, in vec2 b )
{
    vec2 d = abs(p)-b;
    return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
}

//void mainImage( out vec4 fragColor, in vec2 fragCoord )
void main() {
    vec2 uv = (gl_FragCoord.xy-.5*iResolution.xy)/iResolution.y;

    vec2 m = iMouse.xy;//iResolution.xy;
    
    vec3 col = vec3(0);
    
    //vec3 ro = vec3(-0.0, -2.0, -0.0);
    vec3 ro = vec3(.0, 3., .0);
    vec3 camDir = vec3(0.001, 1., 0);
    
    camDir.yz *= Rot(-m.y*3.14+1.);
    camDir.xz *= Rot(-m.x*6.2831);
    
    vec3 rd = GetRayDir(uv, ro, ro + camDir, 1.);

    float d = RayMarch(ro, rd);
    
    if(d<MAX_DIST) {
    	vec3 p = ro + rd * d;
    	vec3 n = GetNormal(p);
        
    	float dif = dot(n, normalize(vec3(1,2,3)))*.5+.5;
    	col += dif;
	    col = pow(col, vec3(.4545));	// gamma correction
        //col = n / 2. + 0.5;
        //col *= n.x;
        col = vec3(1.5 / pow(d, 0.5));
    }
  
    
    gl_FragColor = vec4(vec3(col), 1.0);
}


////////////////////////////////////////////
// SHADERTOY ENDS
////////////////////////////////////////////


// void main() {
//     //vec3 fragColor
//     //mainImage(fragColor, gl_fragCoord);
//     gl_FragColor = vec4(fragColor, 1.0);
// }

