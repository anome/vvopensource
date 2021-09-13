/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"lattice",
		"distance field"
	],
	"INPUTS": [
			{
		"NAME" : 		"center",
		"TYPE" : 		"point2D",
		"DEFAULT" : 	[ -0.5, 0.5 ],
		"MAX" : 		[ 1.0, 1.0 ],
     	"MIN" : 		[ -1.0, -1.0 ]
	},
	{
		"NAME" : 		"fov",
		"TYPE" : 		"float",
		"DEFAULT" : 	2.5,
		"MIN" : 		0.25,
		"MAX" : 		4.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.2,
		"MIN" : 		0.0,
		"MAX" : 		1.0
	},
	{
		"NAME" : 		"loops",
		"TYPE" : 		"float",
		"DEFAULT" :		96.0,
		"MIN" : 		10.0,
		"MAX" : 		100.0
	},
	{
		"NAME" : 		"depth",
		"TYPE" : 		"float",
		"DEFAULT" :		160.0,
		"MIN" : 		10.0,
		"MAX" : 		500.0
	},
	{
		"NAME" : 		"detail",
		"TYPE" : 		"float",
		"DEFAULT" :		0.01,
		"MIN" : 		0.0001,
		"MAX" : 		0.05
	}
	]
}*/



////////////////////////////////////////////////////////////
// MorphingLattice  by mojovideotech
//
// based on :
// shadertoy.com/\ltfBWn
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


float smootherstep(float e0, float e1, float x) {
    x = clamp((x - e0) / (e1 - e0), 0., 1.);
    return x * x * x * (x * (x * 6. - 15.) + 10.);
}

float smin(float a, float b, float k) {
    float h = smootherstep(0., 1., 0.5+0.5*(b-a)/k);
    return mix( b, a, h ) - k*h*(1.0-h);
}

float smax(float a, float b, float k) {
    float h = smootherstep(0., 1., 0.5+0.5*(a - b)/k);
    return mix( b, a, h ) - k*h*(1.0-h);
}

float sphere(vec3 p, float r) {
    return length(p) - r;
}

float box(vec3 p, vec3 s) {
    return length(max(abs(p) - s, 0.0));
}

float infcylinder(vec3 p, float r) {
    return length(p.xy) - r;
}

float infcross(vec3 p, float r) {
    return smin(infcylinder(p, r), smin(infcylinder(p.yzx, r), infcylinder(p.zxy, r), 1.15), 3.3);
}

float map(vec3 p, out float reduce) {
    vec3 porig = p;
    p += TIME * rate * 20.0;
    vec3 p2 = mod(p + 20., 40.) - 20.;
    float pabs = (porig.x + porig.y + porig.z) / 3.;
    float a = cos(pabs / 20. * 3.14 - 3.);
    float f = sign(a) * pow(abs(a), 0.5);
    reduce = 0.75 * (abs(f) * 0.9 + 0.1);
    return smax(
        f * sphere(p2 / abs(f), 4.),
        -f * infcross(p2 / abs(f), 2.7),
        2.
    );
}

void main() 
{
	vec2 screenpos = 2. * (gl_FragCoord.xy - RENDERSIZE.xy / 2. ) / RENDERSIZE.y;
    vec3 viewpoint_location = vec3(100.0*center.x, 100.0*center.y, 0.0);
    float b = TIME * rate;
  	vec3 viewpoint_direction = normalize(1. + 0.5 * vec3(cos(b), cos(b + 2.10), cos(b + 4.20)));
  	vec3 upwards = vec3(0., 0., 1.);
  	vec3 viewpoint_sideways = normalize(cross(viewpoint_direction, upwards));
  	vec3 viewpoint_upwards = cross(viewpoint_sideways, viewpoint_direction);
  	vec3 ray_relative_start = fov * viewpoint_direction +
                              screenpos.x     * viewpoint_sideways  +
                              screenpos.y     * viewpoint_upwards   ;
    vec3 o = viewpoint_location;
  	vec3 p = vec3(0., 0., 0.);
  	vec3 d = normalize(ray_relative_start);
    for (int i = 0; i < 100; i++) {
        float reduce,c;
        c = floor(loops)-float(i);
        float r = map(o + p, reduce);
        float e = (detail * length(p) / fov);
        if (c<0.0) break;
        if (r < e) {
            e = 0.1;
            float dr_dx = map(o + p + viewpoint_sideways * e, reduce) - map(o + p - viewpoint_sideways * e, reduce);
            float dr_dy = map(o + p + viewpoint_upwards * e, reduce) - map(o + p - viewpoint_upwards * e, reduce);
            float dr_dz = map(o + p + viewpoint_direction * e, reduce) - map(o + p - viewpoint_direction * e, reduce);
            vec3 normal = normalize(vec3(dr_dx, dr_dy, dr_dz));
            vec3 normal_world = normal.x * viewpoint_sideways + normal.y * viewpoint_upwards + normal.z * viewpoint_direction;
            vec3 color = vec3(0.3, 0.35, 0.4);
            for (int i = 0; i < 3; i++) {
                vec3 lightpos;
                vec3 lightcolor;
                if (i == 0) {
                    lightpos = vec3(40.,  30., 140.); lightcolor = vec3(0.6, 0.7, 0.8);
                } else if (i == 1) {
                    lightpos = vec3(40., 150.,  20.); lightcolor = vec3(0.6, 0.7, 0.8);
                } else {
                    lightpos = vec3(160., 30.,  20.); lightcolor = vec3(0.6, 0.7, 0.8);
                }
                vec3 dir = (o + p) - lightpos;
                color += lightcolor * max(0.0, -dot(normal_world, normalize(dir))) * pow(120. / length(dir), 1.) * 0.5;
            }
            color;
            gl_FragColor.xyzw = vec4(color * 60. / (length(p) + 25.) + 0.1, 1.);
            return;
        }
        p += d * r * reduce;
        if (length(p) > floor(depth)) {
            break;
        }
    }
    gl_FragColor.xyzw = vec4(0.1, 0.1, 0.1, 1.);
    return;
}
