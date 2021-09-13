/*{
	"CREDIT": "by Igor Molochevski, based on mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
        {
            "NAME": "rotadj",
            "TYPE": "float",
           "DEFAULT": 0.31,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "BUILDADJ",
            "TYPE": "float",
           "DEFAULT": 0.31,
            "MIN": 0.0,
            "MAX": 1.0
        },
        
         {
            "NAME": "BUILDADJ1",
            "TYPE": "float",
           "DEFAULT": 0.31,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "NAME": "sysadj",
            "TYPE": "float",
            "DEFAULT": 0.99,
            "MIN": 0.3,
            "MAX": 30.0
        },
        {
            "NAME": "sysadj2",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 0.3,
            "MAX": 100.0
        }, 
        {
            "NAME": "sysadj3",
            "TYPE": "float",
            "DEFAULT": 50.0,
            "MIN": 0.3,
            "MAX": 100.0
        }
        
        
	]
}*/

//copied from :
//https://www.shadertoy.com/view/lslXR8

//#define NO_BAIL
const vec3 grid = vec3(4.0);
const vec3 griddiv = vec3(2.0);
vec3 aabb  = vec3(BUILDADJ, 0.35, 0.35);
vec3 aabb2 = vec3(0.15, BUILDADJ, 0.15);
vec3 aabb3 = vec3(BUILDADJ1, 0.35, BUILDADJ);

float pyramid( vec3 p, float h) {
	vec3 q=abs(p);
	return max(-p.y, (q.x+q.y+q.z-h)/BUILDADJ1 );
}

float box(vec3 p) {
	return min(  pyramid( vec3(p.x,-p.y,p.z)-vec3(0.0,0.0,0.0),1.0),
		min( pyramid(p-vec3(0.0, 0.0, 0.0), 0.90),
		min(min(length(max(abs(p)-aabb, 0.0)),
			length(max(abs(p)-aabb2, 0.0))),
			length(max(abs(p)-aabb3, 0.0)))));
}

vec3 rot(vec3 p, float f) {
	float s = sin(f);
	float c = cos(f);
	p.xy *= mat2(c, -s, s, c);
	p.yz *= mat2(c, -s, s, c);
	return p;
}

vec3 trans(vec3 p, out float rotout) {
	vec3 rep = floor(p/grid);
	p = mod(p,grid)-griddiv;
	rotout = TIME*0.28 + (rep.x+rep.z+rep.y)*rotadj;
	p = rot(p, rotout);
	return p;	
}
vec3 normal(vec3 p) {
	vec3 e = vec3(0.02,0.0,0.0);
	return normalize(vec3(
		box(p+e.xyy)-box(p-e.xyy),
		box(p+e.yxy)-box(p-e.yxy),
		box(p+e.yyx)-box(p-e.yyx)));
}

float scene(vec3 p) {
	float dummy;
	return box(trans(p,dummy));
}

vec3 normal(vec3 p, float d) {
	vec3 e = vec3(0.04,0.0,0.0);
	return normalize(vec3(
		scene(p+e.xyy)-d,
		scene(p+e.yxy)-d,
		scene(p+e.yyx)-d));
}

bool raystep(inout float d, inout vec3 p,vec3 ray){
	const float eps=0.016;
	d=scene(p);
	p+=max(eps,d*sysadj)*ray;
	return (d < eps);
}
	
void main( void )
{
	vec2 xy = gl_FragCoord.xy / RENDERSIZE.xy - vec2(0.5,0.5);
	xy.y *= -RENDERSIZE.y / RENDERSIZE.x;

	vec3 ro = 2.0*normalize(vec3(cos(TIME/4.0),cos(TIME/4.0),sin(TIME/4.0)));
    vec3 eyed = normalize(vec3(0.0) - ro);
    vec3 ud = normalize(cross(vec3(0.0,1.0,0.0), eyed));
    vec3 vd = normalize(cross(eyed,ud));

	const float fov = 3.14 * 9.7;
	float f = fov * length(xy);
	vec3 rd = normalize(normalize(xy.x*ud + xy.y*vd) + (1.0/tan(f))*eyed);

	vec3 p = ro;
	float d=0.017;
	float dummy;
	bool hit=false;
	
	for(int i = 0; i < 64; i++) 
	{
		#ifdef NO_BAIL
		hit = ( hit == false ) ? raystep(d,p,rd):true;
		#else
		hit = ( hit == false ) ? raystep(d,p,rd):true;
		if (hit) continue;
		#endif
	}
	
	vec3 bg = normalize(p).zzz + 0.21;
	
	if(d < 0.016) {
		vec3 n = normal(p,d); 
		vec3 col = vec3(dot(vec3(0.0,0.0,1.0), n));
		float objrot;
		vec3 objp = trans(p,objrot);
		vec3 objn = abs(rot(n,objrot));
		
		vec2 uv = 
			(objn.y > 0.707) ? vec2(objp.zx) : 
			(objn.x > 0.707) ? vec2(objp.zy) :
							   vec2(objp.xy) ;
		vec3 tex = IMG_PIXEL(inputImage, uv).rgb;
		vec3 hl = smoothstep(0.6, 1.0, col);
		col *= clamp(tex.xyz+0.13, 0.0, 1.0);

		col = col + hl*.4;
		float dall = length(p-ro)*1.20;
		//FOG VARIBLES
		float fog = clamp(dall/mix(sysadj2,sysadj3,((rd.z+1.0)*0.5)), 0.5, 1.0);

		gl_FragColor = vec4(mix(col, bg, fog),1.0);
	}
	else {
		gl_FragColor = vec4(bg, 1.0);
	}

}