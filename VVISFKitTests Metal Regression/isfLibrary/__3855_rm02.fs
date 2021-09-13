#define GLSLIFY 1
/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "by gosub7777777",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "DEFAULT": 5.0,
            "MAX": 10.0,
            "MIN": 0,
            "NAME": "glowSize",
            "TYPE": "float"
        },
        {
            "DEFAULT": 5.0,
            "MAX": 100.0,
            "MIN": 0,
            "NAME": "lineFreq",
            "TYPE": "float"
        },
        {
            "DEFAULT": 2.0,
            "MAX": 10.0,
            "MIN": 0,
            "NAME": "lineSpeed",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 1.0,
            "MIN": 0,
            "NAME": "lineWidthA",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "MAX": 1.0,
            "MIN": 0,
            "NAME": "lineWidthB",
            "TYPE": "float"
        }
        
    ],
    "ISFVSN": "2",
    "PASSES": [
        {
            "FLOAT": true,
            "PERSISTENT": true,
            "TARGET": "first",
            "WIDTH": "$WIDTH",
			"HEIGHT": "$HEIGHT"
        },
        {
            "TARGET": "final",
            "WIDTH": "$WIDTH",
			"HEIGHT": "$HEIGHT"
        }
    ]
}
*/

// RAYTRACING
vec2 doModel(vec3 p);
// Originally sourced from https://www.shadertoy.com/view/ldfSWs
// Thank you Iñigo :)

vec2 calcRayIntersection(vec3 rayOrigin, vec3 rayDir, float maxd, float precis) {
  float latest = precis * 2.0;
  float dist   = +0.0;
  float type   = -1.0;
  vec2  res    = vec2(-1.0, -1.0);

  for (int i = 0; i < 90; i++) {
    if (latest < precis || dist > maxd) break;

    vec2 result = doModel(rayOrigin + rayDir * dist);

    latest = result.x;
    type   = result.y;
    dist  += latest;
  }

  if (dist < maxd) {
    res = vec2(dist, type);
  }

  return res;
}

vec2 calcRayIntersection(vec3 rayOrigin, vec3 rayDir) {
  return calcRayIntersection(rayOrigin, rayDir, 20.0, 0.001);
}

// Originally sourced from https://www.shadertoy.com/view/ldfSWs
// Thank you Iñigo :)

vec3 calcNormal(vec3 pos, float eps) {
  const vec3 v1 = vec3( 1.0,-1.0,-1.0);
  const vec3 v2 = vec3(-1.0,-1.0, 1.0);
  const vec3 v3 = vec3(-1.0, 1.0,-1.0);
  const vec3 v4 = vec3( 1.0, 1.0, 1.0);

  return normalize( v1 * doModel( pos + v1*eps ).x +
                    v2 * doModel( pos + v2*eps ).x +
                    v3 * doModel( pos + v3*eps ).x +
                    v4 * doModel( pos + v4*eps ).x );
}

vec3 calcNormal(vec3 pos) {
  return calcNormal(pos, 0.002);
}

vec2 squareFrame(vec2 screenSize) {
  vec2 position = 2.0 * (gl_FragCoord.xy / screenSize.xy) - 1.0;
  position.x *= screenSize.x / screenSize.y;
  return position;
}

vec2 squareFrame(vec2 screenSize, vec2 coord) {
  vec2 position = 2.0 * (coord.xy / screenSize.xy) - 1.0;
  position.x *= screenSize.x / screenSize.y;
  return position;
}

mat3 calcLookAtMatrix(vec3 origin, vec3 target, float roll) {
  vec3 rr = vec3(sin(roll), cos(roll), 0.0);
  vec3 ww = normalize(target - origin);
  vec3 uu = normalize(cross(ww, rr));
  vec3 vv = normalize(cross(uu, ww));

  return mat3(uu, vv, ww);
}

vec3 getRay(mat3 camMat, vec2 screenPos, float lensLength) {
  return normalize(camMat * vec3(screenPos, lensLength));
}

vec3 getRay(vec3 origin, vec3 target, vec2 screenPos, float lensLength) {
  mat3 camMat = calcLookAtMatrix(origin, target, 0.0);
  return getRay(camMat, screenPos, lensLength);
}

void orbitCamera(
  in float camAngle,
  in float camHeight,
  in float camDistance,
  in vec2 screenResolution,
  out vec3 rayOrigin,
  out vec3 rayDirection
) {
  vec2 screenPos = squareFrame(screenResolution);
  vec3 rayTarget = vec3(0.0);

  rayOrigin = vec3(
    camDistance * sin(camAngle),
    camHeight,
    camDistance * cos(camAngle)
  );

  rayDirection = getRay(rayOrigin, rayTarget, screenPos, 2.0);
}

// PRIMITIVES
float sdPlane( vec3 p, vec4 n )
{
  // n must be normalized
  return dot(p,n.xyz) + n.w;
}

float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));
}

float udRoundBox( vec3 p, vec3 b, float r )
{
  return length(max(abs(p)-b,0.0))-r;
}

float sdTorus( vec3 p, vec2 t )
{
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

float sdCapsule( vec3 p, vec3 a, vec3 b, float r )
{
    vec3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdTriPrism( vec3 p, vec2 h )
{
    vec3 q = abs(p);
    return max(q.z-h.y,max(q.x*0.866025+p.y*0.5,-p.y)-h.x*0.5);
}

float sdHexPrism( vec3 p, vec2 h )
{
    vec3 q = abs(p);
    return max(q.z-h.y,max((q.x*0.866025+q.y*0.5),q.y)-h.x);
}

float sdSphere( vec3 p, float s )
{
  return length( p ) - s;
}

float dot2( in vec3 v ) { return dot(v,v); }
float udTriangle( vec3 p, vec3 a, vec3 b, vec3 c )
{
    vec3 ba = b - a; vec3 pa = p - a;
    vec3 cb = c - b; vec3 pb = p - b;
    vec3 ac = a - c; vec3 pc = p - c;
    vec3 nor = cross( ba, ac );

    return sqrt(
    (sign(dot(cross(ba,nor),pa)) +
     sign(dot(cross(cb,nor),pb)) +
     sign(dot(cross(ac,nor),pc))<2.0)
     ?
     min( min(
     dot2(ba*clamp(dot(ba,pa)/dot2(ba),0.0,1.0)-pa),
     dot2(cb*clamp(dot(cb,pb)/dot2(cb),0.0,1.0)-pb) ),
     dot2(ac*clamp(dot(ac,pc)/dot2(ac),0.0,1.0)-pc) )
     :
     dot(nor,pa)*dot(nor,pa)/dot2(nor) );
}

float sdCone( in vec3 p, in vec3 c )
{
    vec2 q = vec2( length(p.xz), p.y );
    float d1 = -p.y-c.z;
    float d2 = max( dot(q,c.xy), p.y);
    return length(max(vec2(d1,d2),0.0)) + min(max(d1,d2), 0.);
}

float sdCappedCylinder( vec3 p, vec2 h )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - h;
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

// OPS
// #pragma glslify: opU = require('glsl-sdf-ops/union' )
float ao( in vec3 pos, in vec3 nor )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + 0.12 * float( i ) / 4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = doModel ( aopos ).x;
        occ += -(dd-hr)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}

float softshadow( in vec3 ro, in vec3 rd, in float mint, in float tmax )
{
	float res = 1.0;
    float t = mint;
    for( int i=0; i<16; i++ )
    {
		float h = doModel ( ro + rd*t ).x;
        res = min( res, 8.0*h/t );
        t += clamp( h, 0.02, 0.10 );
        if( h<0.001 || t>tmax ) break;
    }
    return clamp( res, 0.0, 1.0 );
}

float smin(float a, float b, float k) {
  float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
  return mix(b, a, h) - k * h * (1.0 - h);
}

// #pragma glslify: opUSPow = require(glsl-smooth-min/pow) 
// #pragma glslify: opUSExp = require(glsl-smooth-min/exp) 

//
// GLSL textureless classic 3D noise "cnoise",
// with an RSL-style periodic variant "pnoise".
// Author:  Stefan Gustavson (stefan.gustavson@liu.se)
// Version: 2011-10-11
//
// Many thanks to Ian McEwan of Ashima Arts for the
// ideas for permutation and gradient selection.
//
// Copyright (c) 2011 Stefan Gustavson. All rights reserved.
// Distributed under the MIT license. See LICENSE file.
// https://github.com/ashima/webgl-noise
//

vec3 mod289_0(vec3 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 mod289_0(vec4 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 permute_0(vec4 x)
{
  return mod289_0(((x*34.0)+1.0)*x);
}

vec4 taylorInvSqrt_0(vec4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}

vec3 fade(vec3 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}

// Classic Perlin noise, periodic variant
float pnoise(vec3 P, vec3 rep)
{
  vec3 Pi0 = mod(floor(P), rep); // Integer part, modulo period
  vec3 Pi1 = mod(Pi0 + vec3(1.0), rep); // Integer part + 1, mod period
  Pi0 = mod289_0(Pi0);
  Pi1 = mod289_0(Pi1);
  vec3 Pf0 = fract(P); // Fractional part for interpolation
  vec3 Pf1 = Pf0 - vec3(1.0); // Fractional part - 1.0
  vec4 ix = vec4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
  vec4 iy = vec4(Pi0.yy, Pi1.yy);
  vec4 iz0 = Pi0.zzzz;
  vec4 iz1 = Pi1.zzzz;

  vec4 ixy = permute_0(permute_0(ix) + iy);
  vec4 ixy0 = permute_0(ixy + iz0);
  vec4 ixy1 = permute_0(ixy + iz1);

  vec4 gx0 = ixy0 * (1.0 / 7.0);
  vec4 gy0 = fract(floor(gx0) * (1.0 / 7.0)) - 0.5;
  gx0 = fract(gx0);
  vec4 gz0 = vec4(0.5) - abs(gx0) - abs(gy0);
  vec4 sz0 = step(gz0, vec4(0.0));
  gx0 -= sz0 * (step(0.0, gx0) - 0.5);
  gy0 -= sz0 * (step(0.0, gy0) - 0.5);

  vec4 gx1 = ixy1 * (1.0 / 7.0);
  vec4 gy1 = fract(floor(gx1) * (1.0 / 7.0)) - 0.5;
  gx1 = fract(gx1);
  vec4 gz1 = vec4(0.5) - abs(gx1) - abs(gy1);
  vec4 sz1 = step(gz1, vec4(0.0));
  gx1 -= sz1 * (step(0.0, gx1) - 0.5);
  gy1 -= sz1 * (step(0.0, gy1) - 0.5);

  vec3 g000 = vec3(gx0.x,gy0.x,gz0.x);
  vec3 g100 = vec3(gx0.y,gy0.y,gz0.y);
  vec3 g010 = vec3(gx0.z,gy0.z,gz0.z);
  vec3 g110 = vec3(gx0.w,gy0.w,gz0.w);
  vec3 g001 = vec3(gx1.x,gy1.x,gz1.x);
  vec3 g101 = vec3(gx1.y,gy1.y,gz1.y);
  vec3 g011 = vec3(gx1.z,gy1.z,gz1.z);
  vec3 g111 = vec3(gx1.w,gy1.w,gz1.w);

  vec4 norm0 = taylorInvSqrt_0(vec4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
  g000 *= norm0.x;
  g010 *= norm0.y;
  g100 *= norm0.z;
  g110 *= norm0.w;
  vec4 norm1 = taylorInvSqrt_0(vec4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
  g001 *= norm1.x;
  g011 *= norm1.y;
  g101 *= norm1.z;
  g111 *= norm1.w;

  float n000 = dot(g000, Pf0);
  float n100 = dot(g100, vec3(Pf1.x, Pf0.yz));
  float n010 = dot(g010, vec3(Pf0.x, Pf1.y, Pf0.z));
  float n110 = dot(g110, vec3(Pf1.xy, Pf0.z));
  float n001 = dot(g001, vec3(Pf0.xy, Pf1.z));
  float n101 = dot(g101, vec3(Pf1.x, Pf0.y, Pf1.z));
  float n011 = dot(g011, vec3(Pf0.x, Pf1.yz));
  float n111 = dot(g111, Pf1);

  vec3 fade_xyz = fade(Pf0);
  vec4 n_z = mix(vec4(n000, n100, n010, n110), vec4(n001, n101, n011, n111), fade_xyz.z);
  vec2 n_yz = mix(n_z.xy, n_z.zw, fade_xyz.y);
  float n_xyz = mix(n_yz.x, n_yz.y, fade_xyz.x);
  return 2.2 * n_xyz;
}

//
// Description : Array and textureless GLSL 2D/3D/4D simplex
//               noise functions.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//

vec3 mod289_1(vec3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 mod289_1(vec4 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 permute_1(vec4 x) {
     return mod289_1(((x*34.0)+1.0)*x);
}

vec4 taylorInvSqrt_1(vec4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}

float snoise(vec3 v)
  {
  const vec2  C = vec2(1.0/6.0, 1.0/3.0) ;
  const vec4  D = vec4(0.0, 0.5, 1.0, 2.0);

// First corner
  vec3 i  = floor(v + dot(v, C.yyy) );
  vec3 x0 =   v - i + dot(i, C.xxx) ;

// Other corners
  vec3 g = step(x0.yzx, x0.xyz);
  vec3 l = 1.0 - g;
  vec3 i1 = min( g.xyz, l.zxy );
  vec3 i2 = max( g.xyz, l.zxy );

  //   x0 = x0 - 0.0 + 0.0 * C.xxx;
  //   x1 = x0 - i1  + 1.0 * C.xxx;
  //   x2 = x0 - i2  + 2.0 * C.xxx;
  //   x3 = x0 - 1.0 + 3.0 * C.xxx;
  vec3 x1 = x0 - i1 + C.xxx;
  vec3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
  vec3 x3 = x0 - D.yyy;      // -1.0+3.0*C.x = -0.5 = -D.y

// Permutations
  i = mod289_1(i);
  vec4 p = permute_1( permute_1( permute_1(
             i.z + vec4(0.0, i1.z, i2.z, 1.0 ))
           + i.y + vec4(0.0, i1.y, i2.y, 1.0 ))
           + i.x + vec4(0.0, i1.x, i2.x, 1.0 ));

// Gradients: 7x7 points over a square, mapped onto an octahedron.
// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
  float n_ = 0.142857142857; // 1.0/7.0
  vec3  ns = n_ * D.wyz - D.xzx;

  vec4 j = p - 49.0 * floor(p * ns.z * ns.z);  //  mod(p,7*7)

  vec4 x_ = floor(j * ns.z);
  vec4 y_ = floor(j - 7.0 * x_ );    // mod(j,N)

  vec4 x = x_ *ns.x + ns.yyyy;
  vec4 y = y_ *ns.x + ns.yyyy;
  vec4 h = 1.0 - abs(x) - abs(y);

  vec4 b0 = vec4( x.xy, y.xy );
  vec4 b1 = vec4( x.zw, y.zw );

  //vec4 s0 = vec4(lessThan(b0,0.0))*2.0 - 1.0;
  //vec4 s1 = vec4(lessThan(b1,0.0))*2.0 - 1.0;
  vec4 s0 = floor(b0)*2.0 + 1.0;
  vec4 s1 = floor(b1)*2.0 + 1.0;
  vec4 sh = -step(h, vec4(0.0));

  vec4 a0 = b0.xzyw + s0.xzyw*sh.xxyy ;
  vec4 a1 = b1.xzyw + s1.xzyw*sh.zzww ;

  vec3 p0 = vec3(a0.xy,h.x);
  vec3 p1 = vec3(a0.zw,h.y);
  vec3 p2 = vec3(a1.xy,h.z);
  vec3 p3 = vec3(a1.zw,h.w);

//Normalise gradients
  vec4 norm = taylorInvSqrt_1(vec4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
  p0 *= norm.x;
  p1 *= norm.y;
  p2 *= norm.z;
  p3 *= norm.w;

// Mix final noise value
  vec4 m = max(0.6 - vec4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3)), 0.0);
  m = m * m;
  return 42.0 * dot( m*m, vec4( dot(p0,x0), dot(p1,x1),
                                dot(p2,x2), dot(p3,x3) ) );
  }

float grain(vec2 texCoord, vec2 resolution, float frame, float multiplier) {
    vec2 mult = texCoord * resolution;
    float offset = snoise(vec3(mult / multiplier, frame));
    float n1 = pnoise(vec3(mult, offset), vec3(1.0/texCoord * resolution, 1.0));
    return n1 / 2.0 + 0.5;
}

float grain(vec2 texCoord, vec2 resolution, float frame) {
    return grain(texCoord, resolution, frame, 2.5);
}

float grain(vec2 texCoord, vec2 resolution) {
    return grain(texCoord, resolution, 0.0);
}

/**
Basic FXAA implementation based on the code on geeks3d.com with the
modification that the IMG_NORM_PIXELLod stuff was removed since it's
unsupported by WebGL.

--

From:
https://github.com/mitsuhiko/webgl-meincraft

Copyright (c) 2011 by Armin Ronacher.

Some rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.

    * The names of the contributors may not be used to endorse or
      promote products derived from this software without specific
      prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#ifndef FXAA_REDUCE_MIN
    #define FXAA_REDUCE_MIN   (1.0/ 128.0)
#endif
#ifndef FXAA_REDUCE_MUL
    #define FXAA_REDUCE_MUL   (1.0 / 8.0)
#endif
#ifndef FXAA_SPAN_MAX
    #define FXAA_SPAN_MAX     8.0
#endif

//optimized version for mobile, where dependent 
//texture reads can be a bottleneck
vec4 fxaa(sampler2D tex, vec2 fragCoord, vec2 resolution,
            vec2 v_rgbNW, vec2 v_rgbNE, 
            vec2 v_rgbSW, vec2 v_rgbSE, 
            vec2 v_rgbM) {
    vec4 color;
    vec2 inverseVP = vec2(1.0 / resolution.x, 1.0 / resolution.y);
    vec3 rgbNW = IMG_NORM_PIXEL(tex, v_rgbNW).xyz;
    vec3 rgbNE = IMG_NORM_PIXEL(tex, v_rgbNE).xyz;
    vec3 rgbSW = IMG_NORM_PIXEL(tex, v_rgbSW).xyz;
    vec3 rgbSE = IMG_NORM_PIXEL(tex, v_rgbSE).xyz;
    vec4 texColor = IMG_NORM_PIXEL(tex, v_rgbM);
    vec3 rgbM  = texColor.xyz;
    vec3 luma = vec3(0.299, 0.587, 0.114);
    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM  = dot(rgbM,  luma);
    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
    
    vec2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));
    
    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) *
                          (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);
    
    float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
    dir = min(vec2(FXAA_SPAN_MAX, FXAA_SPAN_MAX),
              max(vec2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX),
              dir * rcpDirMin)) * inverseVP;
    
    vec3 rgbA = 0.5 * (
        IMG_NORM_PIXEL(tex, fragCoord * inverseVP + dir * (1.0 / 3.0 - 0.5)).xyz +
        IMG_NORM_PIXEL(tex, fragCoord * inverseVP + dir * (2.0 / 3.0 - 0.5)).xyz);
    vec3 rgbB = rgbA * 0.5 + 0.25 * (
        IMG_NORM_PIXEL(tex, fragCoord * inverseVP + dir * -0.5).xyz +
        IMG_NORM_PIXEL(tex, fragCoord * inverseVP + dir * 0.5).xyz);

    float lumaB = dot(rgbB, luma);
    if ((lumaB < lumaMin) || (lumaB > lumaMax))
        color = vec4(rgbA, texColor.a);
    else
        color = vec4(rgbB, texColor.a);
    return color;
}

//To save 9 dependent texture reads, you can compute
//these in the vertex shader and use the optimized
//frag.glsl function in your frag shader. 

//This is best suited for mobile devices, like iOS.

void texcoords(vec2 fragCoord, vec2 resolution,
			out vec2 v_rgbNW, out vec2 v_rgbNE,
			out vec2 v_rgbSW, out vec2 v_rgbSE,
			out vec2 v_rgbM) {
	vec2 inverseVP = 1.0 / resolution.xy;
	v_rgbNW = (fragCoord + vec2(-1.0, -1.0)) * inverseVP;
	v_rgbNE = (fragCoord + vec2(1.0, -1.0)) * inverseVP;
	v_rgbSW = (fragCoord + vec2(-1.0, 1.0)) * inverseVP;
	v_rgbSE = (fragCoord + vec2(1.0, 1.0)) * inverseVP;
	v_rgbM = vec2(fragCoord * inverseVP);
}

vec4 apply(sampler2D tex, vec2 fragCoord, vec2 resolution) {
	vec2 v_rgbNW;
	vec2 v_rgbNE;
	vec2 v_rgbSW;
	vec2 v_rgbSE;
	vec2 v_rgbM;

	//compute the texture coords
	texcoords(fragCoord, resolution, v_rgbNW, v_rgbNE, v_rgbSW, v_rgbSE, v_rgbM);
	
	//compute FXAA
	return fxaa(tex, fragCoord, resolution, v_rgbNW, v_rgbNE, v_rgbSW, v_rgbSE, v_rgbM);
}

 

vec2 smin(vec2 a, vec2 b, float k){
    // return vec2(opUS(a.x, b.x, k), opUS(a.y, b.y, k));
    return vec2(smin(a.x, b.x, k), smin(a.y, b.y, k));
}

vec2 opU(vec2 a, vec2 b){
    return smin(a,b,0.8);
}

// DEFINE RENDER MODEL
vec2 doModel(vec3 p) {
    
    float height = sin(TIME * 0.4)*0.3 + .5;

    float mID1 = 1.;
    float mID2 = 2.;
    float mID3 = 33.;

    float plane = sdPlane(    p - vec3(  .0, 0.,  .0 ), vec4( 0.0, 1.0, 0.0, 0.0 ));

	vec2 res = vec2( sdSphere(   p - vec3(  .0, sin(TIME * 0.34)*0.5 + .5,  .0 ), 0.25 )                                , mID1 );
	res = opU( res, vec2( sdBox(      p - vec3( 1.0, sin(TIME * 0.14)*0.7 + .5,   .0 ), vec3( .25 ))                     , mID1 ));
	res = opU( res, vec2( udRoundBox( p - vec3( 1.0, sin(TIME * 0.64)*0.4 + .5,  1.0 ), vec3( .15 ), 0.1 )               , mID1 ));
	res = opU( res, vec2( sdTorus(    p - vec3(  .0, sin(TIME * 0.344)*0.4 + .5, 1.0 ), vec2( 0.20, 0.05 ))              , mID2 ));
	res = opU( res, vec2( sdCapsule(  p,  vec3(-1.3, sin(TIME * 0.754)*0.5 + .5,  -.1 ), vec3( -1.0, 0.20, 0.2), 0.1 )    , mID1 ));
	res = opU( res, vec2( sdTriPrism( p - vec3(-1.0, sin(TIME * 0.234)*0.4 + .5, -1.0 ), vec2(0.25,0.05) )                , mID1 ));
	res = opU( res, vec2( sdCappedCylinder( p - vec3( 1.0, sin(TIME * 0.54324)*0.5 + .5, -1.0 ), vec2(0.1,0.2) )                  , mID3 ));
	res = opU( res, vec2( sdCone(     p - vec3( 0.0, sin(TIME * 0.2344)*0.4 + .5, -1.0 ), vec3( 0.8, 0.6, 0.3 ))           , mID3 ));
	res = opU( res, vec2( sdHexPrism( p - vec3(-1.0, sin(TIME * 0.534)*0.43 + .5,  1.0 ), vec2( 0.25, 0.05 ))              , mID1 ));

    float kappa = 1.0;

    res = smin(res, vec2( plane , mID3 ), kappa);
    res = smin(res, vec2( plane , mID3 ), kappa);

  	return res;
}

// COMPUTE LIGHTING
vec3 lighting( vec3 pos, vec3 nor, vec3 ro, vec3 rd) {
  
  	// vec3  ref = reflect( rd, nor );
	// vec3  lig = normalize( vec3(-0.6, 0.7, -0.5) );
	// float amb = clamp( 0.5+0.5*nor.y, 0.0, 1.0 );
	// float dif = clamp( dot( nor, lig ), 0.0, 1.0 );
	// float bac = clamp( dot( nor, normalize(vec3(-lig.x,0.0,-lig.z))), 0.0, 1.0 )*clamp( 1.0-pos.y,0.0,1.0);
	// float dom = smoothstep( -0.1, 0.1, ref.y );
	// float fre = pow( clamp(1.0+dot(nor,rd),0.0,1.0), 2.0 );
	// float spe = pow(clamp( dot( ref, lig ), 0.0, 1.0 ),16.0);

	// float sw = softshadow( pos, lig, 0.02, 2.5 );
	// dom *= softshadow( pos, ref, 0.02, 2.5 );

	// brdf += 1.20 * dif * vec3(1.00,0.90,0.60);
	// brdf += 1.20 * spe * vec3(1.00,0.90,0.60) * dif;
	// brdf += 0.30 * amb * vec3(0.50,0.70,1.00) * occ;
	// brdf += 0.40 * dom * vec3(0.50,0.70,1.00) * occ;
	// brdf += 0.30 * bac * vec3(0.25,0.25,0.25) * occ;
	// brdf += 0.40 * fre * vec3(1.00,1.00,1.00) * occ;
	// brdf += 0.02;

	float outBuff = 1.0;

	float occ = ao( pos, nor );
    outBuff = exp( occ * 3.);

	return vec3(outBuff);
}

// COMPUTE MATERIAL COLOR
vec3 material( float materialID, vec3 pos, vec3 nor, vec3 ro, vec3 rd  ){
	// return sin( vec3(1.0,0.2,0.5) * ( materialID * 10.0 ) ) * 0.5 + 0.5;
    // if(materialID >= 0.55){
    //     return vec3(0.0,1.0,0.0);
    // }
    vec3 color = vec3(0.9,0.002,0.001);

    float line = fract(pos.x * 10.) + fract(pos.z * 10.);
    line = distance(vec2(0), pos.xz );
    line = sin(line * lineFreq - TIME * lineSpeed)  * 0.5 + 0.5;
    // line *= sin( pos.y * 100.0) * 0.5 + 0.5;
    
    line =  smoothstep(line,  lineWidthA, lineWidthB );

    return color * line;

    // return vec3( fract(materialID*100.0));
}

vec3 renderRays(){
    vec3 color  = vec3(0.0);

    // if(PASSINDEX == 1){}	
	vec3 ro, rd;

    float rotation = TIME;
    float height   = 1.5;
    float dist     = 3.;

    orbitCamera(rotation * 0.1, height, dist, RENDERSIZE.xy, ro, rd);
	
    vec2 res = calcRayIntersection( ro, rd );

	if ( res.y > -0.5 ) {

		vec3 pos = ro + rd * res.x;
		vec3 nor = calcNormal( pos );
		color = material( res.y, pos, nor, ro, rd ) * lighting( pos, nor, ro, rd );
		// color = material( res.y );
		
        // color = lighting( pos, nor, ro, rd );
		color = mix( color, vec3(0.0), 1.0 - exp( -0.6 * res.x * res.x ));
	}
	
    // gamma correction
    color = pow( color, vec3( 0.23 ));

    return color;
}

// Can go down to 10 or so, and still be usable, probably...
#define ITERATIONS 20

#define TAU  6.28318530718

//-------------------------------------------------------------------------------------------
// Use last part of hash function to generate new random radius and angle...
vec2 Sample(inout vec2 r)
{
    r = fract(r * vec2(33.3983, 43.4427));
    return r-.5;
    //return sqrt(r.x+.001) * vec2(sin(r.y * TAU), cos(r.y * TAU))*.5; // <<=== circular sampling.
}

//-------------------------------------------------------------------------------------------
#define HASHSCALE 443.8975
vec2 Hash22(vec2 p)
{
	vec3 p3 = fract(vec3(p.xyx) * HASHSCALE);
    p3 += dot(p3, p3.yzx+19.19);
    return fract(vec2((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y));
}

//-------------------------------------------------------------------------------------------
vec3 blur(vec2 uv, float radius)
{
	radius = radius * .04;
    
    vec2 circle = vec2(radius) * vec2((RENDERSIZE.y / RENDERSIZE.x), 1.0);
    
	// Remove the time reference to prevent random jittering if you don't like it.
	vec2 random = Hash22(uv+ fract(TIME));

    // Do the blur here...
	vec3 acc = vec3(0.0);
	for (int i = 0; i < ITERATIONS; i++)
    {
        acc += IMG_NORM_PIXEL( first, uv + circle * Sample(random), radius * 10.0 ).xyz;
    }
	return acc / float(ITERATIONS);
}

void main() {
    vec2 uv = gl_FragCoord.xy/RENDERSIZE.xy;
    
    if(PASSINDEX == 0){
        vec3 rays = renderRays(); 
	    gl_FragColor.rgb = rays;
    }

    if(PASSINDEX == 1){
        // vec4 pass0 = IMG_PIXEL(first, gl_FragCoord.xy);
        

        // gl_FragColor.rgb = pass0.rgb;
        // gl_FragColor.rgb = fxaa(blurBuffer, gl_FragCoord.xy, RENDERSIZE).rgb;
    
	    // gl_FragColor.rgb = blur(uv, glowSize);

	    // gl_FragColor.rgb = pass0.rgb + blur(uv, glowSize);
        
        vec3 outImg = apply(first, gl_FragCoord.xy, RENDERSIZE).rgb + blur(uv, glowSize);
        	    
        gl_FragColor.rgb = outImg;
    }

	gl_FragColor.a = 1.;
}