/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "GLSLSandbox"
    ],
    "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#67335.0",
    "INPUTS": [
    ]
}

*/



//++shat
#ifdef GL_ES
precision mediump float;
#endif




#define ITER 100

//----Some random raymarching

struct ray
{
	vec3 o,d;
	float l;
};

const vec3 colBg = vec3(1.8, 1.8, 1.8);
const vec3 colSh = vec3(0.6, 3.2, 3.0);

vec4 opu(vec4 a, vec4 b)
{
	if(a.w < b.w)return a; else return b;	
}
	
vec4 geo(vec3 p)
{
	p.x += sin(p.x*p.y)*4.0;
	vec3 pp = p;
	return opu(vec4(colSh, length(pp - vec3(sin(p.x*8.0+p.y*7.0+TIME * 1.)*0.1,0.,1.2))-0.18), 
		   vec4(colBg, p.y + 0.18));	
}
	
vec4 march(ray r)
{
	vec4 g = vec4(0.);
	for(int i = 0; i < ITER; i++)
	{
		g = geo(r.o + r.d * r.l);
		r.l += g.w;
		if (r.l > 8.) break;
	}
	g.w = r.l;
	return g;
}

vec3 normal(vec3 p)
{
	vec2 f = vec2(0.001,0.0);
	float c = geo(p).w;
	return normalize(c - vec3(geo(p - f.xyy).w, geo(p - f.yxy).w, geo(p - f.yyx).w));
}

float lighting(vec3 p)
{
	vec3 LP = vec3(1.,0.5,-0.5);
	vec3 lp = normalize(LP - p);
	vec3 n = normal(p);
	
	float l = clamp(dot(lp,n),0.1,1.);
	float s = pow(max(dot(lp,n),0.),50.);
	
	float t = 0.05;
	float r = 1.;
	for(int i = 0; i < 22; i++)
	{
		float g = geo(p+(lp+n)*t).w;
		r = min(r, 1.5 / t * g);
		t+=g;
		if(g < 0.001)
			break;
	}
	l *= clamp(r,0.02,1.);
	
	l += s;
	
	return l;
}

void main( void ) 
{
	vec2 uv = ( gl_FragCoord.xy - 0.5 * RENDERSIZE.xy ) / RENDERSIZE.x;

	ray r = ray(vec3(0.),vec3(uv, 1.),0.);
	vec4 m = march(r);
	vec3 p = r.o + r.d * m.w;
	vec3 c = vec3(0.);
	c = m.rgb * lighting(p);

	gl_FragColor = vec4( c, 1.0 );
}