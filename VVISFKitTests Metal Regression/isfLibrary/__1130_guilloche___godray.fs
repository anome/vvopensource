/*
{
  "CATEGORIES": [
        "Patterns",
        "Halftone Effects",
        "Color Effect",
        "multipass",
        "blur",
        "godray"
    ],
   "CREDIT": "splenooname - Andrea Bovo <spleen666@gmail.com>",
  "DESCRIPTION": "guilloche + godray",
  "INPUTS":[
    {
      "NAME":"inputImage",
      "TYPE":"image"
    },
    {
      "LABEL":"uSpacing",
      "NAME":"uSpacing",
      "TYPE":"float",
      "DEFAULT":0.01,
      "MIN":0,
      "MAX":0.05
    },
    {
      "LABEL":"uFrequency",
      "NAME":"uFrequency",
      "TYPE":"float",
      "DEFAULT":30,
      "MIN":5,
      "MAX":50
    },
    {
      "LABEL":"uWeight",
      "NAME":"uWeight",
      "TYPE":"float",
      "DEFAULT":0.65,
      "MIN":0.1,
      "MAX":1.0
    },
    {
      "LABEL":"uHeight",
      "NAME":"uHeight",
      "TYPE":"float",
      "DEFAULT":0.003,
      "MIN":0,
      "MAX":0.015
    },
    {
      "LABEL":"uWidth",
      "NAME":"uWidth",
      "TYPE":"float",
      "DEFAULT":0.07,
      "MIN":0.01,
      "MAX":0.1
    },
    {
      "LABEL":"uAlias",
      "NAME":"uAlias",
      "TYPE":"float",
      "DEFAULT":0.002,
      "MIN":0.0001,
      "MAX":0.01
    },
    {
      "LABEL":"uBright",
      "NAME":"uBright",
      "TYPE":"float",
      "DEFAULT": 0.7,
      "MIN":0.1,
      "MAX":1.0
    },
    {
      "LABEL":"uDist",
      "NAME":"uDist",
      "TYPE":"float",
      "DEFAULT":0.18,
      "MIN":0.01,
      "MAX":0.35
    }
  ],
  "ISFVSN": "2",
  "PASSES": [
    {
      "PERSISTENT": true,
      "TARGET": "bufferA",
      "WIDTH": "$WIDTH/1.0",
	  	"HEIGHT": "$HEIGHT/1.0"
    },
    {

    }
  ]
}
*/

#define R RENDERSIZE
#define t TIME

#define PI 3.14159265359
#define uLevels 10.0

#define NUM_SAMPLES 8.0

mat2 rotate2d(float angle) {
  return mat2(cos(angle), -sin(angle), sin(angle), cos(angle));
}

float luma(vec4 color) {
  return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

// guilloche
const float levels = 6.0;
const float angle = PI/levels;
const float height = 0.003;

float rand(vec2 uv){
  return fract( sin( dot(uv.xy + fract(t), vec2(12.9898, 78.233)))* 43758.5453 );
}

vec3 guilloche( vec2 uv, float t){
  float result = 0.0;
  float tex = luma( IMG_NORM_PIXEL(inputImage, uv) );
  tex *= tex;
  // diagonal waves
  for (float i = 0.0; i<levels; i+=1.0) {
    // new uv coordinate
    vec2 nuv = rotate2d(angle + angle*i) * uv;
    // calculate wave
    float fq = (uFrequency/ 2.0) * (1.5+ sin(t *0.7) );
    float wave = sin(nuv.x * fq) * height;
    float x = ( uSpacing/2.0) + wave;
    float y = mod(nuv.y, uSpacing);
    // wave line
    float line = uWidth * (1.0 - (tex*uBright) - (i*uDist) );
    float waves = smoothstep(line, line+uAlias, abs(x-y) );
    // save the result for the next wave
    result += waves;
  }
  result /= levels;
  return vec3(result);
}

// godray
const float uDensity = 0.9;
const float uDecay = 0.75;
vec3 godray(vec2 uv, vec2 pos, float t) {
  vec2 tc = uv.xy;
  vec2 deltaUv = tc - pos.xy;
  deltaUv *= (1.0 / NUM_SAMPLES * uDensity);
  float illuminationDecay = 0.25;
  vec3 color = IMG_NORM_PIXEL(bufferA, tc.xy).rgb;
  tc += deltaUv * 1.5 * rand(uv);
  for (float i = 0.0; i < NUM_SAMPLES; i+=1.0){
    tc -= deltaUv;
    vec3 sampleTex = IMG_NORM_PIXEL(bufferA, tc.xy).rgb;
    sampleTex *= illuminationDecay * uWeight;
    color += sampleTex;
    illuminationDecay *= uDecay;
  }
  return color;
}

void main() {
  vec2 uv = gl_FragCoord.xy/R.xy;
  vec3 rgb = vec3(0);
  if (PASSINDEX == 0)	{
    rgb = sqrt(guilloche(uv, t));
    gl_FragColor= vec4( rgb, 1.0);
  }
  else if (PASSINDEX == 1){
    rgb = godray( uv, vec2(0.5, 0.5), t);
    gl_FragColor = vec4(rgb, 1.0);
  }
}