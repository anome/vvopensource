/*{
    "PASSES": [{
        "TARGET": "renderBuffer",
        "WIDTH": 10,
        "HEIGHT": 10
    }]
}*/

float rnd(float x) {return fract(10000. * sin(x * 10000.));}


#define iTime time
#define iResolution resolution
#define iMouse mouse

uniform sampler2D u_tex_bg; // https://images-na.ssl-images-amazon.com/images/I/910PPWWqFuL.png

/////////////////////////////////////////////////
// shadertoy begins here
// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
/////////////////////////////////////////////////

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


// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
vec2 getDist(vec3 p) {
  float spheres = 1e10;
  for (int i = 0; i < 10; i++) {
    for(int j = 0; j < 10; j++) {
      vec3 ps = p - texture2D(renderBuffer, vec2(i, j)).rgb;
      spheres = min(spheres, length(ps) - .3);
    }
  }

  return vec2(spheres, BLUE);
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
  if (PASSINDEX == 0) {
    vec2 p = gl_FragCoord.xy;
    vec3 pos = texture2D(renderBuffer, p).xyz;
    // pos.x = 10. * rnd(1. + p.x + p.y * 1000. * time * 10.);
    // pos.y = 10. * rnd(2. + p.x + p.y * 1000. * time * 10.);
    // pos.z = 10. * rnd(3. + p.x + p.y * 1000. * time * 10.);
    pos.x = 101.521 * sin(time / 5. + 137. * p.x + 139. * p.y);
    pos.y = 30.12 * cos(time / 5. + 129. * p.x + 121. * p.y);
    pos = normalize(pos);
    fragColor = vec4(pos, 1.);
  }
  else {
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
    vec3 colorBg = vec3(0.185,0.176,0.26);
    vec3 color = vec3(0);
    vec3 light = vec3(50, 20, 50);
    //light.xz *= Rot(iTime);
    vec3 p = ro + rd * d;
    if (d < MAX_DIST) {
        vec3 n = getNormal(p);
        //n.zy *= Rot(iTime);
    	//color = vec3( n + 1.0 );
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
        float shadow = smoothstep(0.0, .1, rayMarchLight.z / PI);
        color_bw *= .5 + .5 * shadow;


        // tex *= color_bw;
        // color = tex;
    }
    color += 0.6 + vec3( color_bw );
    // coloring
    if (info == IVORY) {
        color *= vec3(0.332,0.400,0.349);
    }
    else if (info == BLUE) {
        color *= vec3(0.810,0.256,0.397);
    }
    else if (info == BLACK) {
        color *= vec3(0.130,0.130,0.130);
    }
    color = mix(color, colorBg, smoothstep(20., 28., d));



    fragColor = vec4(color,1.0);
    // fragColor = texture2D(renderBuffer, gl_FragCoord.xy / resolution);

  }

}

/////////////////////////////////////////////////
// ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
// shadertoy ends here
/////////////////////////////////////////////////

void main() {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}