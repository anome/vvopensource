/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "rayTexSpeed",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": -1
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"PERSISTENT": true
		},
		{
		
		}
	]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif

// glslsandbox uniforms
uniform float time;
uniform vec2 resolution;

// shadertoy emulation
#define iTime TIME
#define iResolution RENDERSIZE
#define iMouse pointInput


///////////////////////////////////////




#define MAX_STEPS 100
#define MAX_DIST 100.
#define EPSILON 0.0001
#define PI 3.14159265
#define RAY 1.
#define BLUE 2.
#define BLACK 3.
#define BG 4.


mat2 Rot(float a) {
    float s = sin(a), c = cos(a);
	return mat2(c, -s, s, c);
}

vec2 Rot2D (vec2 q, float a)
{
  return q * cos (a) + q.yx * sin (a) * vec2 (-1., 1.);
} // TODO replace with Rot

float sdSphere(vec3 p, float radius) { return length(p) - radius; }
float sdBox( vec3 p, vec3 b ) { vec3 q = abs(p) - b; return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0); }
float rnd( float x ) { return fract(10000. * sin(x * 1000.));}

float sdTorus(vec3 p, float smallRadius, float largeRadius) {
    return length(vec2(length(p.xz) - largeRadius, p.y)) - smallRadius;
}


vec3 IcosSym (vec3 p)
{
  float dihedIcos = 0.5 * acos (sqrt (5.) / 3.);
  float a, w;
  w = 2. * PI / 3.;
  p.z = abs (p.z);
  p.yz = Rot2D (p.yz, - dihedIcos);
  p.x = - abs (p.x);
  for (int k = 0; k < 4; k ++) {
    p.zy = Rot2D (p.zy, - dihedIcos);
    p.y = - abs (p.y);
    p.zy = Rot2D (p.zy, dihedIcos);
    if (k < 3) p.xy = Rot2D (p.xy, - w);
  }
  p.z = - p.z;
  a = mod (atan (p.x, p.y) + 0.5 * w, w) - 0.5 * w;
  p.yx = vec2 (cos (a), sin (a)) * length (p.xy);
  p.x -= 2. * p.x * step (0., p.x);
  return p;
}


// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
vec2 getDist(vec3 p) {
    
  vec3 pplan = p;
  pplan.x += iTime / 1.;
  pplan.xy /= 2.;
  pplan.z -=  pow((sin(pplan.x) + sin(pplan.y)) / 2., 1.) * 10.;
  pplan += 1.01 * IMG_NORM_PIXEL(bufferVariableNameA, (pplan.xy - 0.5)/ 100.).rgb;
    vec2 plane = vec2(pplan.z + 50., BLUE);
    plane.x *= 1.;
    return (plane);
}
// ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑






vec3 rayMarch(vec3 ro, vec3 rd) {
	float d = 0.;
    float info = 0.;
    //float glow = 0.;
    float minAngleToObstacle = 1e10;
    for (int i = 0; i < MAX_STEPS; i++) {
    	vec2 distToClosest = getDist(ro + rd * d);
        minAngleToObstacle = min(minAngleToObstacle, atan(distToClosest.x, d));
        d += distToClosest.x;
        info = distToClosest.y;
        if(abs(distToClosest.x) < EPSILON || d > MAX_DIST) {
        	break;
        }
    }
    return vec3(d, info, minAngleToObstacle);
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(EPSILON, 0.);
    vec3 n = getDist(p).x - vec3(getDist(p - e.xyy).x,
                               getDist(p - e.yxy).x,
                               getDist(p - e.yyx).x);
	return normalize(n);
}



vec3 getRayDirection (vec3 ro, vec2 uv, vec3 lookAt) {
    vec3 rd;
    rd = normalize(vec3(uv - vec2(0, 0.), 1.));
    vec3 lookTo = lookAt - ro;
    float horizAngle = acos(dot(lookTo.xz, rd.xz) / length(lookTo.xz) * length(rd.xz));
    rd.xz *= Rot(horizAngle);
    return rd;
}

vec3 getRayDir(vec2 uv, vec3 p, vec3 l, float z) {
    vec3 f = normalize(l-p),
        r = normalize(cross(vec3(0,1,0), f)),
        u = cross(f,r),
        c = f*z,
        i = c + uv.x*r + uv.y*u,
        d = normalize(i);
    return d;
}


void mainImage(out vec4 fragColor, in vec2 fragCoord )
{
    vec2 uv = (fragCoord-.5*iResolution.xy)/iResolution.y;

    // ray origin
    vec3 ro = vec3(EPSILON, 0.01 * sin(iTime), 6.0001);
    float zoom = 1.100;

    // ray direction
    vec3 rd = getRayDir(uv, ro, vec3(0), 1.);

    vec3 rm = rayMarch(ro, rd);
    float d = rm[0];
    float info = rm[1];

    float color_bw = 0.;
    vec3 colorBg = vec3(length(0.5 + 0.5 * sin(uv * 40. + vec2(-iTime * 50., 0)))) * 0.2 + 0.2;
    vec3 color = vec3(0);
    vec3 light = vec3(50, 20, 50);
    //light.xz *= Rot(iTime);
    vec3 p = ro + rd * d;
    if (d < MAX_DIST) {
        vec3 n = getNormal(p);
        //n.zy *= Rot(iTime);
    	color = vec3( n );
        //color *= info;
        // vec3 tex = boxmap(u_tex_bg, ro + rd * d, n, 32.0 ).xyz;//
        // self shadeing
        color_bw = .5 + .5 * dot(n, normalize(light - p));
        // drop shadeos
        // trying to raymarch to the light for MAX_DIST
        // and if we hit something, it's shadow
        vec3 dirToLight = normalize(light - p);
        vec3 rayMarchLight = rayMarch(p + dirToLight * .5, dirToLight);
        float distToObstable = rayMarchLight.x;
        float distToLight = length(light - p);
        // if (distToObstable < distToLight) {
        //     color_bw =  0.;
        // }

        // smooth shadows
        //float shadow = smoothstep(0.0, .1, rayMarchLight.z / PI);
        //color_bw *= .5 + .5 * shadow;


        // tex *= color_bw;
        // color = tex;
    }
    // color += 0.6 + vec3( color_bw );
    // // coloring
    // if (info == RAY) {
    //     color *= vec3(fract(length(p * 10.) - iTime * 10. * rayTexSpeed));
    // }
    // else if (info == BLUE) {
    //     color *= vec3(0.810,0.256,0.397);
    // }
    // else if (info == BLACK) {
    //     color *= vec3(0.130,0.130,0.130);
    // }
    // else if (info == BG) {
    //     color *= vec3(0.230,0.230,0.230);
    // }
    // color = mix(color, colorBg, smoothstep(150., 158., d));



    fragColor = vec4(color,1.0);
}
///////////////////////////////////////

void main(void)
{
    mainImage(gl_FragColor, gl_FragCoord.xy);
//  	vec4		stalePixel = IMG_THIS_PIXEL(bufferVariableNameA);
//  	gl_FragColor = mix(gl_FragColor,stalePixel,0.8);    

}