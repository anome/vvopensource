/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "MAX": [
        33,
        99
      ],
      "MIN": [
        1,
        1
      ],
      "DEFAULT": [
        3,
        13
      ],
      "NAME": "phase",
      "TYPE": "point2D"
    },
    {
      "MAX": 10,
      "MIN": 0.05,
      "DEFAULT": 2.125,
      "NAME": "loops",
      "TYPE": "float"
    }
  ]
}*/

////////////////////////////////////////////////////////////
// SpiralDistortionMap1   by mojovideotech
//
// based on:
// glslsandbox.com/e#26508.3
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define TWOPI 6.28318530718
#define PI 3.14159265359

float _periodX = floor(phase.x);
float _periodY = floor(phase.y);

vec2 cmul(const vec2 a, const vec2 b) { return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x); }

vec2 csq(const vec2 v) { return vec2(v.x*v.x - v.y*v.y, 2.*v.x*v.y); }

vec2 cinv(const vec2 v) { return vec2(v[0],-v[1]) / dot(v, v); }

vec2 cln(const vec2 z) { return vec2(log2(length(z*.667)), atan(z.y, z.x)); }

vec2 perturbedNewton(in vec2 z) {
  float a=1.333/(sin(TIME/sqrt(TIME)));
  mat2 rot=mat2(cos(a),-sin(a),sin(a),cos(a));  
  for(int i=2; i<6; ++i) {
    z = rot *(.667*radians(z + cinv(csq(z)))) / loops;
  }
  return z;
}

vec2 pentaplexify(const vec2 z) {
  vec2 y = z;
  for(float i=1.25; i<TWOPI-0.01; i+=TWOPI/1.5) 
  {
    y = cmul(y, z-vec2(cos(i+.1*TIME), sin(i+.1*TIME)));
  }
  return y;
}

vec2 infundibularize(in vec2 z) {
  vec2 lnz = cln(z) / TWOPI;
  return vec2(_periodX*(lnz.y) + _periodY*(lnz.x), _periodX*(lnz.x) - _periodY*(lnz.y));
}

vec3 hsv(float h, float s, float v) {
  return v * mix(
    vec3(0.5),
   clamp((abs(fract(h+vec3(3.25, 0.67, 2.25)/(0.333*cos(h/s)))*2.667)-sin(s+h)), 0.0, 1.0), s);
}

vec4 rainbowJam(in vec2 z) {
  vec2 uv = fract(vec2(z[0]/_periodX, z[1]/_periodY))*vec2(_periodX, _periodY);
  vec2 iz = floor(uv);
  vec2 wz = uv - iz;
  return vec4(hsv(pow(iz[0]/_periodX, 0.025),1.006,smoothstep(0.125,0.667,length(wz-vec2(0.333)))), 1.0);
}

void main( void ) {
  gl_FragColor = 
    rainbowJam(infundibularize(pentaplexify(perturbedNewton(PI*(gl_FragCoord.xy-(tan(RENDERSIZE.xy)))-(RENDERSIZE.x*1.75))))
  + TIME * -0.5);
}