/*{
	"CREDIT": "by mojovideotech",
	"CATEGORIES": [
 "Generator"
	],
  "DESCRIPTION": "spherical lens which is magnifying a moving backdrop of Perlin noise.",
  "INPUTS": [
        {
            "NAME": "size",
            "TYPE": "float",
           "DEFAULT": 3.0,
            "MIN": -1.0,
            "MAX": 5.0
      },
      {
            "NAME": "layers",
            "TYPE": "float",
           "DEFAULT": 3.1,
            "MIN": 2.0,
            "MAX": 12.0
      },
      {
            "NAME": "seed",
            "TYPE": "float",
           "DEFAULT": 33.0,
            "MIN": 3.0,
            "MAX": 333.0
      },
      {
            "NAME": "refractionIn",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.0
        },
        {
            "NAME": "refractionOut",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 1.0
        },
        {
            "NAME": "colorWarp1",
            "TYPE": "float",
           "DEFAULT": 2.0,
            "MIN": -3.0,
            "MAX": 6.0
        },
        {
            "NAME": "colorWarp2",
            "TYPE": "float",
           "DEFAULT": 3.0,
            "MIN": -5.0,
            "MAX": 10.0
        },
        {
            "NAME": "rate",
            "TYPE": "float",
           "DEFAULT": 0.1,
            "MIN": -1.5,
            "MAX": 1.5
        }
  ]
}
*/


////////////////////////////////////////////////////////////
// MagnifyingMarble2   by mojovideotech
//
// based on : 
// shadertoy.com/ldfSDN
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////


#define M_PI 3.1415926535

float TT = TIME * rate;

vec3 light = vec3(50.0, 5.0, 20.0);

vec4 sph1 = vec4( 0.0, 0.0, 0.0, 1.0);

vec2 iSphere(in vec3 ro, in vec3 rd, in vec4 sph) {
	vec3 oc = ro - sph.xyz;
	float b = dot(oc, rd);
	float c = dot(oc, oc) - sph.w * sph.w; 
	float h = b*b - c; 
	vec2 t;
	if(h < 0.0) 
		t = vec2(-1.0);
	else  {
		float sqrtH = sqrt(h);
		t.x = (-b - sqrtH); 
		t.y = (-b + sqrtH);
	}
	return t;
}

vec3 nSphere(in vec3 pos, in vec4 sph ) { return (pos - sph.xyz)/sph.w; }

float intersect(in vec3 ro, in vec3 rd, out vec2 resT) {
	resT = vec2(1000.0);
	float id = -1.0;
	vec2 tsph = iSphere(ro, rd, sph1);
	if(tsph.x > 0.0 || tsph.y > 0.0) {
		id = 1.0;
		resT = tsph;
	}
	return id;
}


vec4 mod289(vec4 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }

vec4 permute(vec4 x) { return mod289(((x*34.0)+1.0)*x); }

vec4 taylorInvSqrt(vec4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

vec2 fade(vec2 t) { return t*t*t*(t*(t*6.0-15.0)+10.0); }

float cnoise(vec2 P) {
  vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
  vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
  Pi = mod289(Pi);
  vec4 ix = Pi.xzxz; 
  vec4 iy = Pi.yyww;
  vec4 fx = Pf.xzxz;
  vec4 fy = Pf.yyww;
  vec4 i = permute(permute(ix) + iy);
  vec4 gx = fract(i * (1.0 / seed)) * 2.0 - 1.0 ;
  vec4 gy = abs(gx) - 0.5 ;
  vec4 tx = floor(gx + 0.5);
  gx = gx - tx;
  vec2 g00 = vec2(gx.x,gy.x);
  vec2 g10 = vec2(gx.y,gy.y);
  vec2 g01 = vec2(gx.z,gy.z);
  vec2 g11 = vec2(gx.w,gy.w);
  vec4 norm = taylorInvSqrt(vec4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
  g00 *= norm.x;  
  g01 *= norm.y;  
  g10 *= norm.z;  
  g11 *= norm.w;  
  float n00 = dot(g00, vec2(fx.x, fy.x));
  float n10 = dot(g10, vec2(fx.y, fy.y));
  float n01 = dot(g01, vec2(fx.z, fy.z));
  float n11 = dot(g11, vec2(fx.w, fy.w));
  vec2 fade_xy = fade(Pf.xy);
  vec2 n_x = mix(vec2(n00, n01), vec2(n10, n11), fade_xy.x);
  float n_xy = mix(n_x.x, n_x.y, fade_xy.y);
  return layers * n_xy;
}

vec4 getFragColor(float noiseValue) {
	vec4 fragColor;
	fragColor.r = fract(noiseValue);
	fragColor.g = fract(colorWarp1 * fragColor.r);
	fragColor.b = fract(colorWarp2 * fragColor.g);
	fragColor.a = 1.0;
	return fragColor;
}

void main()
{
	float aspectRatio = RENDERSIZE.x/RENDERSIZE.y;
	vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
	vec4 ro = vec4(0.0, 0.0, 5.0-size, 1.0);
	vec3 rd = normalize(vec3( (-1.0+2.0*uv) * vec2(aspectRatio, 1.0), -1.0));
	vec2 t;
	float id = intersect(ro.xyz, rd, t);
	vec3 col;
	if(id > 0.5 && id < 1.5) {
		vec3 E = normalize(ro.xyz + t.x*rd);
		vec3 N = normalize(nSphere(E, sph1));
		vec3 L = normalize(light);
		vec3 reflectColor = vec3(0.0);
		float lambertTerm = dot(N, L);
		if (lambertTerm > 0.0) {
			float w = pow(1.0 - max(0.0, dot(normalize(L+E), E)), 5.0);
			reflectColor += (1.0-w)*pow(max(0.0, dot(reflect(-L, E), E)), 100.);
		}
		vec3 refractionVec = refract(rd, N, refractionIn);
		float id2 = intersect(E, refractionVec, t);
		if (id2 > 0.5 && id2 < 1.5) {
			E += refractionVec * t.y;
			E = normalize(E);
			N = normalize(nSphere(E, sph1));
			refractionVec = refract(refractionVec, N, refractionOut);
		}
		vec3 noiseColor = getFragColor(cnoise(vec2(TT + refractionVec.x + uv.x, refractionVec.y + uv.y))).rgb;
		col = mix(noiseColor, reflectColor, reflectColor);
	}
	else
		col = getFragColor(cnoise(vec2(TT + uv.x, uv.y))).rgb;
	
	gl_FragColor = vec4(col,1.0);
}

//
// GLSL textureless classic 2D noise "cnoise",
// with an RSL-style periodic variant "pnoise".
// Author:  Stefan Gustavson (stefan.gustavson@liu.se)
// Version: 2011-08-22
//
// Many thanks to Ian McEwan of Ashima Arts for the
// ideas for permutation and gradient selection.
//
// Copyright (c) 2011 Stefan Gustavson. All rights reserved.
// Distributed under the MIT license. See LICENSE file.
// https://github.com/ashima/webgl-noise
//
