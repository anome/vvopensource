/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
        {
			"NAME": "_shape",
			"TYPE": "point2D",
			"DEFAULT": [
				1.0,
				1.0
			],
			"MIN": [0.0, 0.0],
			"MAX": [1.0, 1.0]
		},
            {
			"NAME": "_colormap",
			"TYPE": "point2D",
			"DEFAULT": [
				1.0,
				0.0
			],
			"MIN": [0.0, 0.0],
			"MAX": [3.0, 3.0]
		},
		{
			"NAME": "_domain",
			"TYPE": "point2D",
			"DEFAULT": [
				1.0,
				1.0
			],
			"MIN": [0.0, 0.0],
			"MAX": [1.0, 1.0]
		},
		{
			"NAME" : "_stripe",
			"TYPE" : "bool"
		},
		{
			"NAME" : "_rotation",
			"TYPE" : "float",
			"DEFAULT" : 3.5,
			"MIN" : 0,
			"MAX" : "6.28"
		},
				{
			"NAME" : "_r_axis",
			"TYPE" : "color",
			"DEFAULT" : [1,0,1]
		}
	]
}*/

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

vec4 mod289(vec4 x)
{
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec4 permute(vec4 x)
{
  return mod289(((x*34.0)+1.0)*x);
}

vec4 taylorInvSqrt(vec4 r)
{
  return 1.79284291400159 - 0.85373472095314 * r;
}

vec2 fade(vec2 t) {
  return t*t*t*(t*(t*6.0-15.0)+10.0);
}

// Classic Perlin noise
float cnoise(vec2 P)
{
  vec4 Pi = floor(P.xyxy) + vec4(0.0, 0.0, 1.0, 1.0);
  vec4 Pf = fract(P.xyxy) - vec4(0.0, 0.0, 1.0, 1.0);
  Pi = mod289(Pi); // To avoid truncation effects in permutation
  vec4 ix = Pi.xzxz;
  vec4 iy = Pi.yyww;
  vec4 fx = Pf.xzxz;
  vec4 fy = Pf.yyww;

  vec4 i = permute(permute(ix) + iy);

  vec4 gx = fract(i / 42.) * 2.0 - 1.0 ;
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
  return 2.3 * n_xy;
}
///
//
// END NOISE SNIPPET
//
//

vec3   iResolution = vec3(RENDERSIZE, 1.0);
float  iGlobalTime = TIME;

const int MAX_ITER = 50;


float length8( vec2 p ) {
	return pow(pow(p.x, 8.) + pow(p.y, 8.), 1./8.);
}

float length2( vec2 p ) {
	return pow(pow(p.x, 2.) + pow(p.y, 2.), 1./2.);
}



mat3 RotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat3(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}

vec2 rotate(in vec2 v, in float a) {
	return vec2(cos(a)*v.x + sin(a)*v.y, -sin(a)*v.x + cos(a)*v.y);
}

float sdTorus( vec3 p, vec2 t )
{
  //vec2 q = vec2(length(p.xz)-t.x,p.y);
  //return length(q)-t.y;
  
	vec2 q = abs(vec2(max(abs(p.x), abs(p.z))-t.x, p.y));
	return max(q.x, q.y)-t.y;
  
}

vec3 pal( in float t, in vec3 a, in vec3 b, in vec3 c, in vec3 d )
{
	t = t * _colormap.x + _colormap.y;
    return a + b*cos( 6.28318*(c*t+d) );
}


vec3 palette(float f ) {
	return pal(f, vec3(0.8,0.5,0.4),vec3(0.2,0.4,0.2),vec3(2.0,1.0,1.0),vec3(0.0,0.25,0.25) );
	//return pal(f, vec3(0.5,0.5,0.5),vec3(0.5,0.5,0.5),vec3(1.0,1.0,1.0),vec3(0.0,0.10,0.20) );
}


float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));
}

vec3 recurse( in vec3 p )
{
   float d = sdBox(p,vec3(1.0));
   vec3 res = vec3( d, 1.0, 0.0);

   float s = 1.;
   for( int m=0; m<4; m++ )
   {
      vec3 a = mod( p*s, 2.0 ) - 1.0;
      p = p * RotationMatrix(vec3(1.,1.,1.), TIME/10. * (mod(float(m), 2.0) == 1.0? 0.5 : 1.) );
      s *= 3.0;
      vec3 r = abs(1.0 - 3.0*abs(a));

      float da = max(r.x,r.y);
      float db = max(r.y,r.z);
      float dc = max(r.z,r.x);
      float c = (min(da,min(db,dc))-1.0)/s;

      if( c>d )
      {
          d = c;
          res = vec3( d, 0.2*da*db*dc, (1.0+float(m))/4.0);
       }
   }

   return res;
}

vec3 DE(in vec3 p)
{
	p = p * RotationMatrix(_r_axis.xyz, _rotation);
	return recurse(p);
}

vec3 gradient( in vec3 pos )
{
	vec3 eps = vec3( 0.001, 0.0, 0.0 );
	vec3 nor = vec3(
	    DE(pos+eps.xyy).x - DE(pos-eps.xyy).x,
	    DE(pos+eps.yxy).x - DE(pos-eps.yxy).x,
	    DE(pos+eps.yyx).x - DE(pos-eps.yyx).x );
	return normalize(nor);
}


float fbm(vec2 p) {
	const mat2 m = mat2( 0.80,  0.60, -0.60,  0.80 );
	p *= 3.;
    float f = 0.0;
    f += 0.500000*(0.5+0.5*cnoise( p )); p = m*p*2.02;
    f += 0.250000*(0.5+0.5*cnoise( p )); p = m*p*2.03;
    f += 0.125000*(0.5+0.5*cnoise( p )); p = m*p*2.01;
    f += 0.062500*(0.5+0.5*cnoise( p )); p = m*p*2.04;
    f += 0.031250*(0.5+0.5*cnoise( p )); p = m*p*2.01;
    f += 0.015625*(0.5+0.5*cnoise( p ));
    return f/0.96875;

}

vec3 intersect(in vec3 rayOrigin, in vec3 rayDir)
{
	float total_dist = 0.0;
	vec3 p = rayOrigin + rayDir/2.;
	vec3 d = vec3(1.0);
	float iter = 0.0;
	float mind = 3.14159;//+sin(iGlobalTime*0.1)*0.2;
	bool hit = false;
	
	for (int i = 0; i < MAX_ITER; i++)
	{		
		if (d.x < 0.001) {
			hit = true;
			continue;
		}
		
		d = DE(p);

		p += d.x * rayDir ;
		mind = min(mind, d.x);
		total_dist += d.x;
		iter++;
	}
	vec3 color = vec3(1.0) * d.y;
	float x = (iter/float(MAX_ITER));

	float shade = 1.0;
	if ( true) {
		vec3 n = gradient(total_dist * rayDir + rayOrigin);
	 	shade = abs(dot(n, -rayDir));

		color = palette(x + d.z) * shade * d.y;
		color = palette(d.z/10.) * (1. - x);

	} else {
		color = 1.0 - palette(5.);
		color /= 2.;
	}
	
	return color;
}



void main()
{
	vec3 cameraDir = vec3(1., 0., 0.);
	vec3 cameraOrigin = vec3(-2.5, 0., 0.);

	vec2 screenPos = -1.0 + 2.0 * gl_FragCoord.xy / iResolution.xy;
	screenPos.x *= iResolution.x / iResolution.y;
	vec3 rayDir = normalize(vec3(1.0, screenPos));
	
	vec3 color = intersect(cameraOrigin, rayDir);
	

	// vingette
	// color *= 1.0 - pow(length(screenPos), 2.);
	color *=1.2;
	
	gl_FragColor = vec4(color, 10.0);
} 