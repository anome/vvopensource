/*{
  "CREDIT": "by Paulofalcao, remixed by Tim Gerritsen for EboStudio/EboSuite",
  "CATEGORIES": [
  ],
  "DESCRIPTION": "Cubes and Spheres (http://glslsandbox.com/e#897.0)",
  "INPUTS": [
    {
        "NAME":     "inputImage",
        "TYPE":     "image"
    },
  {
    "LABEL": "Fov",
    "NAME": "uFov",
    "TYPE": "float",
    "DEFAULT": 27.8,
    "MIN": 0.0,
    "MAX": 180.0
  },
  {
    "LABEL": "Cam direct X",
    "NAME": "uCameraDirectionX",
    "TYPE": "float",
    "DEFAULT": 100.0,
    "MIN": 20.0,
    "MAX": 180.0
  },  
  {
    "LABEL": "Cam direct Y",
    "NAME": "uCameraDirectionY",
    "TYPE": "float",
    "DEFAULT": 18.0,
    "MIN": -180.0,
    "MAX": 180.0
  },  
  {
    "LABEL": "Box/Sphere",
    "NAME": "uSphere",
    "TYPE": "float",
    "DEFAULT": 0.307,
    "MIN": 0.0,
    "MAX": 1.0
  },  
  {
    "LABEL": "Frequency X",
    "NAME": "uFrequencyX",
    "TYPE": "float",
    "DEFAULT": 0.3,
    "MIN": 0.0,
    "MAX": 10.0
  },  
  {
    "LABEL": "Frequency Y",
    "NAME": "uFrequencyY",
    "TYPE": "float",
    "DEFAULT": 2.619,
    "MIN": 0.0,
    "MAX": 10.0
  },  
  {
    "LABEL": "Amplitude X",
    "NAME": "uAmplitudeX",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 1.0
  },  
  {
    "LABEL": "Amplitude Y",
    "NAME": "uAmplitudeY",
    "TYPE": "float",
    "DEFAULT": 1.0,
    "MIN": 0.0,
    "MAX": 1.0
  },  
  {
    "LABEL": "Speed forward",
    "NAME": "uSpeedForward",
    "TYPE": "float",
    "DEFAULT": 0.409,
    "MIN": -10.0,
    "MAX": 10.0
  },  
  {
    "LABEL": "Speed wave X",
    "NAME": "uSpeedX",
    "TYPE": "float",
    "DEFAULT": 5.61,
    "MIN": -10.0,
    "MAX": 10.0
  },  
  {
    "LABEL": "Speed wave Y",
    "NAME": "uSpeedY",
    "TYPE": "float",
    "DEFAULT": -0.777,
    "MIN": -10.0,
    "MAX": 10.0
  },
  {
    "LABEL": "Size frequency",
    "NAME": "uSizeFrequency",
    "TYPE": "float",
    "DEFAULT": 16.142,
    "MIN": 0.0,
    "MAX": 50.0
  },  
  {
    "LABEL": "Size amplitude",
    "NAME": "uSizeAmplitude",
    "TYPE": "float",
    "DEFAULT": 0.441,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "LABEL": "Color A",
    "NAME": "uColorA",
    "TYPE": "color",
    "DEFAULT": [0.0, 0.0, 0.0, 1.0]
  },
  {
    "LABEL": "Color B",
    "NAME": "uColorB",
    "TYPE": "color",
    "DEFAULT": [0.0, 0.0, 0.0, 1.0]
  },
  {
    "LABEL": "Ambient light",
    "NAME": "uAmbient",
    "TYPE": "float",
    "DEFAULT": 0.75,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "LABEL": "Use alpha",
    "NAME": "uAlpha",
    "TYPE": "bool",
    "DEFAULT": 0
  }
  ]
}
*/


// Cubes and Spheres
//
// by @paulofalcao
//

#ifdef GL_ES
precision highp float;
#endif


//Util Start
float PI=3.14159265;

vec2 ObjUnion(
  in vec2 obj0,
  in vec2 obj1)
{
  if (obj0.x<obj1.x)
    return obj0;
  else
    return obj1;
}

vec2 sim2d(
  in vec2 p,
  in float s)
{
   vec2 ret=p;
   ret=p+s/2.0;
   ret=fract(ret/s)*s-s/2.0;
   return ret;
}

vec3 stepspace(
  in vec3 p,
  in float s)
{
  return p-mod(p-s/2.0,s);
}

vec3 phong(
  in vec3 pt,
  in vec3 prp,
  in vec3 normal,
  in vec3 light,
  in vec3 color,
  in float spec,
  in vec3 ambLight)
{
   vec3 lightv=normalize(light-pt);
   float diffuse=dot(normal,lightv);
   vec3 refl=-reflect(lightv,normal);
   vec3 viewv=normalize(prp-pt);
   float specular=pow(max(dot(refl,viewv),0.0),spec);
   return (max(diffuse,0.0)+ambLight)*color+specular;
}

//Util End
vec2 BoxUV(vec3 boxPos, vec3 boxSize)
{
  vec3 uvw = boxPos / boxSize;
  vec3 ap = abs(uvw)+0.5;
  vec2 uv = (mix(mix(uvw.xy, uvw.xz, 1.-step(1., ap.z)),uvw.zy,step(1., ap.x))+0.5);

  vec2 res = RENDERSIZE;
  float aspect = res.x/res.y;
  uv.x = (uv.x-0.5)/aspect+0.5;
  return uv;
}

//Scene Start
vec2 obj(in vec3 p, out vec2 uv)
{ 
  vec2 np = floor((p.xz-1.0)/2.0);
  vec3 fp=stepspace(p,2.0);
  float d=sin(fp.x*uFrequencyX+TIME*uFrequencyX*uSpeedX)*uAmplitudeX+cos(fp.z*uFrequencyY+TIME*uFrequencyY*uSpeedY)*uAmplitudeY;
  p.y=p.y+d;

  float r = 1.0-(sin(np.x*uSizeFrequency)*0.5+0.5)*uSizeAmplitude;
  p.xz=sim2d(p.xz,2.0);
  float c1=length(max(abs(p)-vec3(0.6,0.6,0.6),0.0))-0.35*r;
  float c2=length(p)-1.0*r;
  float cf=uSphere;sin(TIME)*0.5+0.5;
  uv = BoxUV(p*0.7,vec3(r));
  return vec2(mix(c1,c2,cf),1.0);
}

vec3 obj_c(vec3 p){
  vec2 fp=sim2d(p.xz-1.0,4.0);
  if (fp.y>0.0) fp.x=-fp.x;
  if (fp.x>0.0) return uColorA.rgb;
  else return uColorB.rgb;
}

//Scene End


float raymarching(
  in vec3 prp,
  in vec3 scp,
  in int maxite,
  in float precis,
  in float startf,
  in float maxd,
  out float objid,
  out vec2 objuv)
{ 
  const vec3 e=vec3(0.1,0,0.0);
  vec2 s=vec2(startf,0.0);
  vec3 c,p,n;
  float f=startf;
  for(int i=0;i<64;i++){
    if (abs(s.x)<precis||f>maxd||i>maxite) break;
    f+=s.x;
    p=prp+scp*f;
    s=obj(p,objuv);
    objid=s.y;
  }
  if (f>maxd) objid=-1.0;
  return f;
}

vec3 camera(
  in vec3 prp,
  in vec3 vrp,
  in vec3 vuv,
  in float vpd)
{
  vec2 vPos=-1.0+2.0*gl_FragCoord.xy/RENDERSIZE.xy;
  vec3 vpn=normalize(vrp-prp);
  vec3 u=normalize(cross(vuv,vpn));
  vec3 v=cross(vpn,u);
  if (RENDERSIZE.x > RENDERSIZE.y) {
    u *= RENDERSIZE.x/RENDERSIZE.y;
  } else {
    v *= RENDERSIZE.y/RENDERSIZE.x;
  }
  vec3 scrCoord=prp+vpn*vpd+vPos.x*u+vPos.y*v;
  return normalize(scrCoord-prp);
}

vec3 normal(in vec3 p)
{
  //tetrahedron normal
  const float n_er=0.01;
  vec2 uv;
  float v1=obj(vec3(p.x+n_er,p.y-n_er,p.z-n_er),uv).x;
  float v2=obj(vec3(p.x-n_er,p.y-n_er,p.z+n_er),uv).x;
  float v3=obj(vec3(p.x-n_er,p.y+n_er,p.z-n_er),uv).x;
  float v4=obj(vec3(p.x+n_er,p.y+n_er,p.z+n_er),uv).x;
  return normalize(vec3(v4+v1-v3-v2,v3+v4-v1-v2,v2+v4-v3-v1));
}

vec4 render(
  in vec3 prp,
  in vec3 scp,
  in int maxite,
  in float precis,
  in float startf,
  in float maxd,
  in vec3 background,
  in vec3 light,
  in float spec,
  in vec3 ambLight,
  out vec3 n,
  out vec3 p,
  out float f,
  out float objid,
  out vec2 objuv)
{ 
  objid=-1.0;
  f=raymarching(prp,scp,maxite,precis,startf,maxd,objid,objuv);
  if (objid>-0.5){
    p=prp+scp*f;
    vec3 c=obj_c(p);
    n=normal(p);
    vec4 color = IMG_NORM_PIXEL(inputImage,objuv);
    c += color.rgb;
    vec3 cf=phong(p,prp,n,light,c,spec,vec3(uAmbient));
    float alpha = float(uAlpha);
    return vec4(cf, alpha*color.a+(1.0-alpha));
  }
  f=maxd;


  return vec4(background, 0.0); //background color
}

void main(void){
 
  //Camera animation
  vec3 vuv=vec3(0,1,0);
  vec3 vrp=vec3(TIME*uSpeedForward,0.0,0.0);
  float mx=radians(180.0-uCameraDirectionX);
  float my=radians(uCameraDirectionY);//mouse.y*PI/2.01; 
  vec3 prp=vrp+vec3(cos(my)*cos(mx),sin(my),cos(my)*sin(mx))*12.0; //Trackball style camera pos
  float vpd=1.0/radians(uFov);
  vec3 light=prp+vec3(5.0,0,5.0);
  
  vec3 scp=camera(prp,vrp,vuv,vpd);
  vec3 n,p;
  float f,o;
  const float maxe=0.01;
  const float startf=0.1;
  const vec4 backc=vec4(0.0,0.0,0.0,0.0);
  const float spec=8.0;
  const vec3 ambi=vec3(0.1,0.1,0.1);
  
  vec2 ouv;
  vec4 c1=render(prp,scp,256,maxe,startf,60.0,backc.rgb,light,spec,ambi,n,p,f,o,ouv);
  float a = c1.a;
  c1.rgb=c1.rgb*max(1.0-f*.015,0.0);
  vec4 c2=backc;
  if (o>0.5){
    scp=reflect(scp,n);
    c2=render(p+scp*0.05,scp,32,maxe,startf,10.0,backc.rgb,light,spec,ambi,n,p,f,o,ouv);
  }
  c2=c2*max(1.0-f*.1,0.0);
  vec3 c = c1.rgb*0.75+c2.rgb*0.25;
  gl_FragColor=vec4(c,c1.a);
}