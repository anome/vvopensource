/*{
    "CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "generator",
    "cubes"
  ],
  "DESCRIPTION": "",
  "ISFVSN" : "2.0",
  "VSN" : "2",
  "INPUTS" : [
    {
        "NAME" :        "rate",
        "TYPE" :        "float",
        "DEFAULT" :     0.25,
        "MIN" :         -1.5,
        "MAX" :         1.5
    },
    {
        "NAME" :        "zoom",
        "TYPE" :        "float",
        "DEFAULT" :     0.1,
        "MIN" :         0.1,
        "MAX" :         30.0
    },
    {
        "NAME" :        "fov",
        "TYPE" :        "float",
        "DEFAULT" :     2.0,
        "MIN" :         1.0,
        "MAX" :         5.0
    },
    {
        "NAME" :        "cubesX",
        "TYPE" :        "float",
        "DEFAULT" :     3.0,
        "MIN" :         1.0,
        "MAX":          5.0
    },
    {
        "NAME" :        "cubesY",
        "TYPE" :        "float",
        "DEFAULT" :     3.0,
        "MIN" :         1.0,
        "MAX":          5.0
    },
    {
        "NAME" :        "cubesZ",
        "TYPE" :        "float",
        "DEFAULT" :     3.0,
        "MIN" :         1.0,
        "MAX":          5.0
    },    
    {
        "NAME" :        "scale",
        "TYPE" :        "float",
        "DEFAULT" :     1.15,
        "MIN" :         0.15,
        "MAX" :         2.0
    },
    {
        "NAME" :        "density",
        "TYPE" :        "float",
        "DEFAULT" :     6.125,
        "MIN" :         1.0,
        "MAX" :         10.0
    },
    {
        "NAME" :        "rot",
        "TYPE" :        "float",
        "DEFAULT" :     -0.33,
        "MIN" :         -1.0,
        "MAX" :         1.0
    },
    {
        "NAME" :        "edge",
        "TYPE" :        "float",
        "DEFAULT" :     0.567,
        "MIN" :         0.1,
        "MAX" :         1.0
    },
    {
        "NAME" :        "cycle",
        "TYPE" :        "float",
        "DEFAULT" :     9.33,
        "MIN" :         0.1,
        "MAX" :         12.0
    },
    {
        "NAME" :        "fog",
        "TYPE" :        "float",
        "DEFAULT" :     0.5,
        "MIN" :         0.0,
        "MAX" :         1.0
    }
  ]
}
*/



////////////////////////////////////////////////////////////////////
// CubeClusterMatrix  by mojovideotech
// v2.0 ESSL fix 4/2020
//
// based on :
// glslsandbox.com/e#422890.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////////////


#ifdef GL_ES
precision mediump float;
#endif

#define     frpi    12.56637061435917   // four pi, 4*pi
#define     twpi    6.283185307179586   // two pi, 2*pi
#define     pi      3.141592653589793   // pi


bool intersects(vec3 ro, vec3 rd, vec3 box_center, float box_size, out float N) {
    vec3 t1 = (box_center - vec3(box_size) - ro)/rd;
    vec3 t2 = (box_center + vec3(box_size) - ro)/rd;
    vec3 t_min = min(t1, t2);
    vec3 t_max = max(t1, t2);
    float t_near = max(t_min.x, max(t_min.y, t_min.z));
    float t_far = min(t_max.x, min(t_max.y, t_max.z));
    if (t_near > t_far || t_far < 0.0) {return false; }
    N = t_near;
    return true;
}

mat3 camera(vec3 e, vec3 la) {
    vec3 roll = vec3(0, 1, 0);
    vec3 f = normalize(la - e);
    vec3 r = normalize(cross(roll, f));
    vec3 u = normalize(cross(f, r));
    return mat3(r, u, f);
}

void main()
{
    vec4  col = vec4(0.0, 0.0, 0.0, 1.0);
    vec2 uv = (2.0*gl_FragCoord.xy - RENDERSIZE)/min(RENDERSIZE.x, RENDERSIZE.y);
    float a = rate*TIME*twpi;
    float a2 = rot*pi;
    vec3 ro = (30.1-zoom)*normalize(vec3(cos(a), sin(a2), -sin(a)+cos(a2)));
    vec3 rd = camera(ro, vec3(0))*normalize(vec3(uv, fov));
    const float I = 1e6;
    float N = I;
    vec3 cluster = floor(vec3(cubesX, cubesY, cubesZ));
    float inside = 0.0;
    vec3 box_color = vec3(0.0);
    for (float i = 0.0; i < 6.0; i++) {
        if (i > cluster.x) break;
        for (float j = 0.0; j < 6.0; j++) {
            if (j > cluster.y) break;
            for (float k = 0.0; k < 6.0; k++) {
                if (k > cluster.z) break;
                vec3 p = density*(vec3(i, j, k) - 0.5*vec3(cluster - 1.0));
                float l = length(p);
                float s = 0.125 + scale*(0.5 + 0.5*sin(0.5*TIME*frpi - cycle*l));
                float t = 0.0;
                if (intersects(ro, rd, p, s, t) && t < N) {
                    N = t;
                    vec3 n = ro + rd*N - p;
                    float E = 0.25*edge;
                    vec3 normal = smoothstep(vec3(s - E), vec3(s), n) + smoothstep(vec3(s - E), vec3(s), -n);
                    box_color = vec3(i,j,k)*fract(cluster*5.4);
                    inside = smoothstep(1.5, 1.0, normal.x + normal.y + normal.z);
                }
            }
        }
    }

    if (N == I)
        col = mix(col, mix(vec4(0.5, 0.5, 0.5, 1.0), col, 0.75*length(uv)), fog);
    else
        col = vec4(box_color*inside+(1.0-inside)*mix(box_color,vec3(1.0),0.3), 1.0);

    gl_FragColor = col;
}