/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "distance field"
  ],
  "INPUTS": [
    {
      "NAME": "offset",
      "TYPE": "point2D",
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ]
    },
    {
      "NAME": "lightpos",
      "TYPE": "float",
      "DEFAULT": -3.14159,
      "MIN": -6.2831853,
      "MAX": 6.2831853
    },
    {
      "NAME": "spinwarp",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -6.2831853,
      "MAX": 6.2831853
    },
    {
      "NAME": "rotopan",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -6.2831853,
      "MAX": 6.2831853
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 8,
      "MIN": 0,
      "MAX": 12
    },
    {
      "NAME": "depth",
      "TYPE": "float",
      "DEFAULT": 8,
      "MIN": 3,
      "MAX": 16
    },
    {
      "NAME": "softedge",
      "TYPE": "float",
      "DEFAULT": 0.1,
      "MIN": 0,
      "MAX": 1
    }
  ]
}*/

///////////////////////////////////////////
// DistanceFunction  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////


#ifdef GL_ES
precision mediump float;
#endif


vec3 lightDir = vec3(cos(lightpos),1.0,sin(lightpos));

vec3 trans(vec3 p) {
  float dt = 16.0-density;	
  return mod(p, dt) - dt*0.5;
}

float distanceFunction(vec3 pos) {
  vec3 q = abs(trans(pos));
  return length(max(q - vec3(1.0, 1.0, 1.0), 0.0)) - softedge;
}
 
vec3 getNormal(vec3 p) {
  const float d = 0.0001;
  return normalize(vec3(
        distanceFunction(p+vec3(d,0.0,0.0))-distanceFunction(p+vec3(-d,0.0,0.0)),
        distanceFunction(p+vec3(0.0,d,0.0))-distanceFunction(p+vec3(0.0,-d,0.0)),
        distanceFunction(p+vec3(0.0,0.0,d))-distanceFunction(p+vec3(0.0,0.0,-d)))
    );
}
 
void main() {
  vec2 pos = (gl_FragCoord.xy*2.0 -RENDERSIZE) / RENDERSIZE.y;
  pos += offset;
  vec3 camPos = vec3(0.0,0.0,0.0) + vec3(offset.x, sin(TIME/6.0),cos(TIME/2.0))*10.0;
  vec3 camDir = vec3(cos(rotopan), 0.0, sin(rotopan));
  vec3 camUp = vec3(0.0, cos(spinwarp),sin(spinwarp));
  vec3 camSide = cross(camDir, camUp);
  float focus = 2.8;
  vec3 rayDir = normalize(camSide*pos.x + camUp*pos.y + camDir*focus);
  float t = 0.0, d;
  vec3 posOnRay = camPos;
  for(int i=0; i<64; ++i) {
    d = distanceFunction(posOnRay);
    t += d;
    if (d < 0.001 || t > depth*8.0) break;
    posOnRay = camPos + t*rayDir;
  }
  vec3 normal = getNormal(posOnRay);
  vec3 color;
  if(d < 0.05) {
    float diff = clamp(dot(lightDir, normal), 0.125, 0.95);
    color = vec3(diff);
  }
  else {
    color = vec3(0.0);
  }
  
  gl_FragColor = vec4(color, 1.0);
}