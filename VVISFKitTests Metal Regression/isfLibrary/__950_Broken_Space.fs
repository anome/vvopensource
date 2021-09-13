/*
{
  "IMPORTED" : [

  ],
  "CATEGORIES" : [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/WtfSDf by NuSan.  Shader coded live on twitch (https://www.twitch.tv/nusan_fx)\nThe shader was made using Bonzomatic.\nYou can find the original shader here: http://lezanu.fr/LiveCode/InsideBrokenSpace.glsl",
  "INPUTS" : [

  ]
}
*/


/*
Shader coded live on twitch (https://www.twitch.tv/nusan_fx)
The shader was made using Bonzomatic.
You can find the original shader here: http://lezanu.fr/LiveCode/InsideBrokenSpace.glsl
*/

float time = 0.0;

float box(vec3 p, vec3 s) {
  p=abs(p)-s;
  return max(p.x, max(p.y,p.z));
}

mat2 rot(float a) {
  float ca=cos(a);
  float sa=sin(a);
  return mat2(ca,sa,-sa,ca);  
}

#define rep(p,s) (fract(p/s+0.5)-0.5)*s

float rnd(float t) {
  return fract(sin(t*784.685)*827.542);
}

float curve(float t, float d, float p) {
  float g=t/d;
  return mix(rnd(floor(g)), rnd(floor(g)+1.0), pow(smoothstep(0.0,1.0,fract(g)), p));
}

vec3 kifs(vec3 p, float t, float t2) {
  
  float s=3.0;
  for(float i=0.0; i<5.0; ++i) {
    float t2 = t + i*3.0 + curve(t2, 0.8, 2.0) * 1.5;
    p.xy *= rot(t2);
    p.yz *= rot(t2*1.2);
    p=abs(p);
    p-=s;
    s*=0.6;
  }
    
  return p;
}

float smin(float a, float b, float h) {
  float k=clamp((a-b)/h*0.5+0.5,0.0,1.0);
  return mix(a,b,k) - k * (1.0-k ) * h;
}

float map(vec3 p) {
  
  p = rep(p, 80.0);
  
  float t=time*0.3 + 95.0;
  
  vec3 p1 = kifs(p, t * 0.1, t);
  vec3 p2 = kifs(p+vec3(2,0,0), t * 0.13+37.241, t);
  vec3 p3 = kifs(p+vec3(0,2,0), t * 0.17+27.74, t);
  
  float d1 = box(p1, vec3(5,3,7));
  float d2 = min(box(p2, vec3(5,10,2)), length(p2.xy)-1.0);
  float d3 = box(p3, vec3(5,2,3));
  
  
  d1 = abs(d1-9.0)-12.0;
  d1 = abs(d1)-1.5;
  
  d2 = abs(d2-8.0)-12.0;
  d2 = abs(d2)-2.0;
  
  
  //float d = length(vec2(d1,d2))-1.0;
  float d4 = max(abs(d1)-0.2,abs(d2)-0.7);
  float d5 = max(abs(d2)-0.2,abs(d3)-0.6);
  
  vec3 p4 = rep(p2, 1.0);
  float d6 = box(p4, vec3(0.4));
  
  float h=sin(p.x*0.1)*0.5 + sin(p.y*0.3)*0.7 + sin(p.z*0.7);
  
  /*
  d4 -= min(0,d6*h*0.7);
  d5 += d6*h*0.6;
  */
  float d7 = max(d4, d5)-0.1;
  d4 = smin(d4, d6*h*2.0, -0.5);
  d5 = smin(d5, -d6*h*1.0-0.5, -1.5);
  d5 = min(d5, d7);
    
  //d = min(d,d1+0.5);
  //d = min(d,d2+0.5);
  
  float d = min(d4, d5);
  //d *= 0.6;
  return d;
}

void cam(inout vec3 p) {
  float t=time*0.1 + curve(time, 3.7, 10.0)*5.0;
  float t2=time*0.17 + curve(time, 2.7, 10.0)*3.0;
  p.xz *= rot(t);
  p.xy *= rot(t2);
  
}

void main() {

   

  vec2 uv = vec2(gl_FragCoord.x / RENDERSIZE.x, gl_FragCoord.y / RENDERSIZE.y);
  uv -= 0.5;
  uv /= vec2(RENDERSIZE.y / RENDERSIZE.x, 1);
  time = mod(TIME * 0.6, 500.0) + 16.4;
    
  vec3 s=vec3((curve(time, 4.7, 10.0)-0.5)*20.0,(curve(time, 7.2, 10.0)-0.5)*10.0,-30.0);
  vec3 r=normalize(vec3(-uv, 0.8 + 0.7 * curve(time, 1.2, 10.0)));
  
  cam(s);
  cam(r);
  
  vec3 p=s;
  float at = 0.0;
  float dd = 0.0;
  float alpha = 1.0;
  for(int i=0; i<150; ++i) {
    float sd=map(p);
    float d = abs(sd);
    if(sd<0.001) alpha *= 0.92;
    
    if(d<0.001) {
      d = 0.01;
      //break;
    }
    if(dd>150.0) break;
    
    p += r * d;
    dd += d;
    
    at += (1.5/(1.8+d)) * 30.0 / (50.0+dd);
   
  }
    
  vec3 col=vec3(0);
  //col += pow(max(0,1-i/150.0),5);
  
  vec3 atmo = mix(vec3(1, 0.7, 0.3), vec3(0.2, 1.0, 0.6), pow(abs(r.x),3.0));
  atmo = mix(atmo, vec3(0.5, 0.7, 1.4)*3.0, pow(abs(r.y),7.0));
  
  col += pow(at * 0.03,1.3) * atmo;// * pow(alpha,0.2);
  col += alpha * atmo * 0.2;
  
  col *= 1.2-length(uv);
  
  //col = 1-exp(-col*2);
  //col = pow(col, vec3(1.3));
  
  #if 1
  float t3 = time*0.3 - length(uv)*0.2;
  col.xy *= rot(t3);
  col.xz *= rot(t3*0.7);
  col=abs(col);
  
  col+=max(vec3(0),col.yzx-1.0);
  col+=max(vec3(0),col.zxy-1.0);
  #endif
  
  
  gl_FragColor = vec4(col, 1);
}
