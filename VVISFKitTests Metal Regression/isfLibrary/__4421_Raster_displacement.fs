/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "image",
			"TYPE": "image"
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
#define IVORY 1.
#define BLUE 2.
#define BLACK 3.

#define PHI (sqrt(5.)*0.5 + 0.5)

mat2 Rot(float a) {
    float s = sin(a), c = cos(a);
	return mat2(c, -s, s, c);
}

float sdBox( vec3 p, vec3 b )
{
  vec3 q = abs(p) - b;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
vec2 getDist(vec3 p) {
  vec3 boxSize = vec3(1, .1, 1);
  vec3 pb = p;
  pb.xz *= Rot(iTime / 10.);
  pb.xz /= 10.;
  pb.y += 1. * texture2D(image, (pb.xz + 1.) / 2.).x;
  float box = sdBox(pb, boxSize) * 0.1;

  p.xz /= 10.;
  // p.y += 1.5;
  boxSize.y += 0.1;
  float boxCutter = sdBox(p, boxSize) * 0.1;
  box = max(box, boxCutter);
  return vec2(box, BLUE);
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
    vec3 ro = vec3(-3, 5, 3);
    float zoom = 1.100;

    // ray direction
    vec3 rd = getRayDir(uv, ro, vec3(0), 1.);

    vec3 rm = rayMarch(ro, rd);
    float d = rm[0];
    float info = rm[1];

    float color_bw = 0.;
    vec3 colorBg = vec3(.0);
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
        // drop shadows
        // trying to raymarch to the light for MAX_DIST
        // and if we hit something, it's shadow
        vec3 dirToLight = normalize(light - p);
        vec3 rayMarchLight = rayMarch(p + dirToLight * .06, dirToLight);
        float distToObstable = rayMarchLight.x;
        float distToLight = length(light - p);
        // if (distToObstable < distToLight) {
        //     color_bw =  0.;
        // }

        // smooth shadows
        float shadow = smoothstep(0.0, .15, rayMarchLight.z / PI);
        color_bw *= .5 + .5 * shadow;


        // tex *= color_bw;
        // color = tex;
    }
    // color += 0.6 + vec3( color_bw );
    // // coloring
    // if (info == IVORY) {
    //     color *= vec3(0.332,0.400,0.349);
    // }
    // else if (info == BLUE) {
    //     color *= vec3(0.810,0.256,0.397);
    // }
    // else if (info == BLACK) {
    //     color *= vec3(0.130,0.130,0.130);
    // }
    color = mix(color, colorBg, smoothstep(20., 28., d));

    fragColor = vec4(color, 1);
}


///////////////////////////////////////

void main(void)
{
    mainImage(gl_FragColor, gl_FragCoord.xy);
//  	vec4		stalePixel = IMG_THIS_PIXEL(bufferVariableNameA);
//  	gl_FragColor = mix(gl_FragColor,stalePixel,0.8);    

}