/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "XXX"
  ],
  "INPUTS": [
    {
      "NAME": "rotation",
      "TYPE": "point2D",
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ]
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0.05,
      "MAX": 2
    },
    {
      "NAME": "form",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "flip",
      "TYPE": "bool",
      "DEFAULT": 0
    },
    {
      "NAME": "alpha",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "reform",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": -10,
      "MAX": 10
    },
    {
      "NAME": "deform",
      "TYPE": "float",
      "DEFAULT": 3,
      "MIN": -15,
      "MAX": 15
    }
  ]
}*/


// AlphaRaySurface by mojovideotech
// based on:
// glslsandbox.com/e#29499.0
// AlgebraicSurface_Caley by Aaron Montag
// Raytracer for algebraic surfaces up to deg 4

#ifdef GL_ES
precision highp float;
#endif

const float R = 19.;
const float oo = 1000.;
const int IT = 12;

float F(vec3 v) 
{
  float x = v.x; float y = v.y; float z = v.z;
  float s = x+y+z+1.;
  if (form) return reform*(x*x*x+y*y*y+z*z*z+1.)-alpha*s*s*s;
  else  return x*x+y*y+z*z+(abs(alpha/2.)+.35*sin(TIME))*x*y*z-1.;
}

vec3 dF(vec3 v) 
{
  float x = v.x; float y = v.y; float z = v.z;
  float s = x+y+z+1.;
  float da = deform; float db = da/6.0; float dc = da/4.0;
  if (flip) return vec3(da*x*x-dc*alpha*s*s,da*y*y-dc*alpha*s*s,da*z*z-dc*alpha*s*s);
  else return vec3(db*x+alpha*y*z,db*y+alpha*x*z,db*z+alpha*x*y);
}

float SR(vec3 v)
{
  return dot(v,v)-R*R;  
}

vec3 ray(vec2 pos, float t)
{
  float th = TIME*rate-4.*rotation.x;
  float phi = TIME*rate+3.*rotation.y;
  return mat3(vec3(cos(th),0.,sin(th)),vec3(0.,1.,0.),vec3(-sin(th),0.,cos(th)))*
    		(mat3(vec3(1.,0,0.),vec3(0,cos(phi),sin(phi)),vec3(0,-sin(phi),cos(phi)))*  
      		(vec3(pos,1.)*t+vec3(rotation-vec2(.5),-18.)));
}

float eval(vec4 poly, float t) 
{
  return (((poly[3])*t+poly[2])*t+poly[1])*t+poly[0];
}

vec4 d(vec4 p) 
{
  vec4 r = vec4(0.);
  for (int i=0; i<3; i++) 
  {
    r[i] = p[i+1]*float(i+1);  
  }
  return r;
}

float bisect(vec4 p, float l, float u, float def) 
{
  if (l==u) return def;
  float lv = eval(p, l);
  float uv = eval(p, u);
  if (lv*uv>=0.) return def;
  float m, mv;
  for (int i=0; i<IT; i++)
  {
    m = (l+u)/2.;
    mv = eval(p, m);
    if (lv*mv>0.) { l = m; } 
    else { u = m; }
  }
  return m;
}

float firstroot(vec4 poly, float l, float u)
{ 
  vec4 p[4];
  p[3] = poly;
  for(int i=3; i>=1; i--) 
  {
    p[i] = d(p[i+1]);  
  }
  vec4 roots = vec4(u);
  vec4 oroots = vec4(u);
  for(int i=1; i<4; i++)
  { 
    roots[0] = bisect(p[i], l, oroots[0], l);
    for (int j=1; j<4; j++) 
    { 
    	if (j<i) roots[j] = bisect(p[i], oroots[j-1], oroots[j],roots[j-1]);
    }
    oroots = roots;
  }
  for(int i=0; i<4; i++) 
  {
    if(roots[i]!=l && roots[i]!=u) return roots[i];  //if(abs(eval(poly,roots[i]))<.01) return roots[i];
  }
  return oo;
}

mat4 A = mat4(
vec4(  1.000000000000000, -0.366666666666667,  0.040000000000000, -0.001333333333333),
vec4( -0.000000000000000,  0.600000000000000, -0.100000000000000,  0.004000000000000),
vec4(  0.000000000000000, -0.300000000000000,  0.080000000000000, -0.004000000000000),
vec4( -0.000000000000000,  0.066666666666667, -0.020000000000000,  0.001333333333333));

void main( void )
{
  vec2 pos = ( gl_FragCoord.xy / RENDERSIZE.xy ) -vec2(.5);
  pos.y *= RENDERSIZE.y/RENDERSIZE.x;
  vec4 vals;
  vec4 rvals;
  for(int i=0; i<5; i++)
  {
    vec3 p = ray(pos, 5.*float(i));
    vals[i] = F(p);
    rvals[i] = SR(p);
  }
  vec4 poly = A*vals;
  vec4 rpoly = A*rvals; 
  float D = (rpoly[1]*rpoly[1])-4.*rpoly[2]*rpoly[0]; 
  float froot = oo;
  if (D>=0.) froot = firstroot(poly, max(0.,(-rpoly[1]-sqrt(D))/(2.*rpoly[2])), max(0.,(-rpoly[1]+sqrt(D))/(2.*rpoly[2])));
  gl_FragColor = vec4(0.);
  if (froot != oo) 
  {
    vec3 n = normalize(dF(ray(pos,froot)));
    vec3 l[5]; vec3 c[5]; 
    l[0] = vec3(-1.,1.2,0.5);
    l[1] = vec3(0.5,-1.2,1.);
    l[2] = vec3(1.,0.5,-1.);
    l[3] = -ray(vec2(.0,3.0),-5.);
    l[4] = ray(vec2(5.0,.0),-10.);
    c[0] = vec3(.5,.1,.1);
    c[1] = vec3(.3,.1,.5);
    c[2] = vec3(.1,.5,.1);
    c[3] = vec3(.1,.1,.5);
    c[4] = vec3(.5,.1,.1);
    gl_FragColor = vec4(0.,0.,0.,1.0);
    float illumination; 
    for (int i=0; i<5; i++)
    {
      	illumination = smoothstep (max(0.,dot(normalize(l[i]),n)),min(.0,dot(normalize(c[i]),n)),min(0.,dot(normalize(l[i]),c[i])));
      	gl_FragColor.rgb += illumination*illumination*c[i];
    }
  } 
}