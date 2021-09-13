/*
{
  "CATEGORIES" : [
    "Generator"
  ],
  "DESCRIPTION" : "Creates a blood splatter",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "colorInput",
      "TYPE" : "color",
      "DEFAULT" : [
        0.95969659090042114,
        0,
        0.021815943371908609,
        1
      ]
    },
    {
      "NAME" : "seedVal",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.23400000000000001,
      "MIN" : 0
    }
  ],
  "CREDIT" : "keijiro, adapted by David Lublin"
}
*/

//	This is adapted from https://github.com/keijiro/BloodFx

//#extension GL_EXT_gpu_shader4 : enable

//
// Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
//
// Original work (webgl-noise) Copyright (C) 2011 Ashima Arts.
// Translation and modification was made by Keijiro Takahashi.
//
// This shader is based on the webgl-noise GLSL shader. For further details
// of the original shader, please see the following description from the
// original source code.
//

//
// Description : Array and textureless GLSL 2D simplex noise function.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//

vec3 mod289(vec3 x)
{
    return x - floor(x / 289.0) * 289.0;
}

vec2 mod289(vec2 x)
{
    return x - floor(x / 289.0) * 289.0;
}

vec3 permute(vec3 x)
{
    return mod289((x * 34.0 + 1.0) * x);
}

vec3 taylorInvSqrt(vec3 r)
{
    return 1.79284291400159 - 0.85373472095314 * r;
}

float snoise(vec2 v)
{
    const vec4 C = vec4( 0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                             0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                            -0.577350269189626,  // -1.0 + 2.0 * C.x
                             0.024390243902439); // 1.0 / 41.0
    // First corner
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v -   i + dot(i, C.xx);

    // Other corners
    vec2 i1;
    i1.x = step(x0.y, x0.x);
    i1.y = 1.0 - i1.x;

    // x1 = x0 - i1  + 1.0 * C.xx;
    // x2 = x0 - 1.0 + 2.0 * C.xx;
    vec2 x1 = x0 + C.xx - i1;
    vec2 x2 = x0 + C.zz;

    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    vec3 p =
      permute(permute(i.y + vec3(0.0, i1.y, 1.0))
                    + i.x + vec3(0.0, i1.x, 1.0));

    vec3 m = max(0.5 - vec3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
    m = m * m;
    m = m * m;

    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;

    // Normalise gradients implicitly by scaling m
    m *= taylorInvSqrt(a0 * a0 + h * h);

    // Compute final noise value at P
    vec3 g;
    g.x = a0.x * x0.x + h.x * x0.y;
    g.y = a0.y * x1.x + h.y * x1.y;
    g.z = a0.z * x2.x + h.z * x2.y;
    return 130.0 * dot(m, g);
}

vec3 snoise_grad(vec2 v)
{
    const vec4 C = vec4( 0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                             0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                            -0.577350269189626,  // -1.0 + 2.0 * C.x
                             0.024390243902439); // 1.0 / 41.0
    // First corner
    vec2 i  = floor(v + dot(v, C.yy));
    vec2 x0 = v -   i + dot(i, C.xx);

    // Other corners
    vec2 i1;
    i1.x = step(x0.y, x0.x);
    i1.y = 1.0 - i1.x;

    // x1 = x0 - i1  + 1.0 * C.xx;
    // x2 = x0 - 1.0 + 2.0 * C.xx;
    vec2 x1 = x0 + C.xx - i1;
    vec2 x2 = x0 + C.zz;

    // Permutations
    i = mod289(i); // Avoid truncation effects in permutation
    vec3 p =
      permute(permute(i.y + vec3(0.0, i1.y, 1.0))
                    + i.x + vec3(0.0, i1.x, 1.0));

    vec3 m = max(0.5 - vec3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
    vec3 m2 = m * m;
    vec3 m3 = m2 * m;
    vec3 m4 = m2 * m2;

    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    vec3 x = 2.0 * fract(p * C.www) - 1.0;
    vec3 h = abs(x) - 0.5;
    vec3 ox = floor(x + 0.5);
    vec3 a0 = x - ox;

    // Normalise gradients
    vec3 norm = taylorInvSqrt(a0 * a0 + h * h);
    vec2 g0 = vec2(a0.x, h.x) * norm.x;
    vec2 g1 = vec2(a0.y, h.y) * norm.y;
    vec2 g2 = vec2(a0.z, h.z) * norm.z;

    // Compute noise and gradient at P
    vec2 grad =
      -6.0 * m3.x * x0 * dot(x0, g0) + m4.x * g0 +
      -6.0 * m3.y * x1 * dot(x1, g1) + m4.y * g1 +
      -6.0 * m3.z * x2 * dot(x2, g2) + m4.z * g2;
    vec3 px = vec3(dot(x0, g0), dot(x1, g1), dot(x2, g2));
    return 130.0 * vec3(grad, dot(m4, px));
}

// Hash function from H. Schechter & R. Bridson, goo.gl/RXiKaH
float Hash(float s)
{
	s = pow(s,2747636419.0);
	s *= 2654435769.0;
	//s = pow(s,float(int(s) >> 16));
	s *= 2654435769.0;
	//s = pow(s,float(int(s) >> 16));
	s *= 2654435769.0;
	return s;
}

float Random(float seed)
{
	return float(Hash(seed)) / 4294967295.0; // 2^32-1
	//return rand(vec2(seed,0.1234184));
}

void main()	{
	// Parameters from the particle system
	vec2 texcoord = isf_FragNormCoord;
	float seed = seedVal + floor(TIME);

	// Animated radius parameter
	float tp = 1.0 - mod(TIME,1.0);
	float radius = 1.0 - tp * tp * tp * tp;

	// Zero centered UV
	vec2 uv = texcoord.xy - 0.5;

	// Noise 1 - Radial curve
	float freq = mix(1.2, 2.7, Random((seed*48923.23*floor(TIME))));
	float n1 = snoise(atan(vec2(uv.y, uv.x)) * freq + seed * 764.2174 * floor(TIME));

	// I prefer steep curves, so use sixth power.
	float n1p = n1 * n1;
	n1p = n1p * n1p * n1p;

	// Noise 2 - Small dot
	float n2 = snoise(uv * 8.0 / radius + seed * 1481.28943);

	// Potential = radius + noise * radius ^ 3;
	float p = radius * (0.23 + radius * radius * (n1p * 0.9 + n2 * 0.07));

	// Antialiased thresholding
	float l = length(uv);
	float a = smoothstep(l - 0.01, l, p);
	gl_FragColor = vec4(colorInput.rgb, colorInput.a * a);
}
