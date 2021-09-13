/*
{
  "CATEGORIES" : [
    "df"
  ],
  "DESCRIPTION" : "Wave Glitch",
  "INPUTS" : [
    {
      "NAME" : "inputImage",
      "TYPE" : "image"
    },
    {
      "NAME" : "freeze",
      "TYPE" : "bool",
      "DEFAULT" : 0,
      "LABEL" : "Feedback"
    },
    {
      "NAME" : "distStrength",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.82,
      "MIN" : 0,
      "LABEL" : "Distort Strength"
    },
    {
      "NAME" : "waveScale",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 8,
      "LABEL" : "Wave Scale",
      "MIN" : 1
    },
    {
      "NAME" : "speedAmp",
      "TYPE" : "float",
      "MAX" : 30,
      "DEFAULT" : 1,
      "LABEL" : "Speed Amplitude",
      "MIN" : 0
    },
    {
      "NAME" : "seed",
      "TYPE" : "float",
      "MAX" : 10,
      "DEFAULT" : 0,
      "LABEL" : "seed",
      "MIN" : 0
    },
    {
      "NAME" : "rgbSplit",
      "TYPE" : "float",
      "MAX" : 100,
      "DEFAULT" : 10,
      "MIN" : 0
    }
  ],
  "PASSES": [
    {
      "TARGET": "bufferVariableNameA",
      "persistent": true,
      "float": true
    },
    {}
  ],
  "CREDIT" : "by Taiyo Yamamoto"
}
*/

const float PI = 3.1415926535;

vec2 rand(vec2 p) {
    return fract(sin(vec2(dot(p,vec2(127.1,311.7)),dot(p,vec2(269.5,183.3))))*43758.5453);
}

float rand(float v){
    return fract(sin(v) * 45464.6688);
}


vec3 mod289(vec3 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec2 mod289(vec2 x) {
  return x - floor(x * (1.0 / 289.0)) * 289.0;
}

vec3 permute(vec3 x) {
  return mod289(((x*34.0)+1.0)*x);
}

float snoise(vec2 v) {
  const vec4 C = vec4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                     -0.577350269189626,  // -1.0 + 2.0 * C.x
                      0.024390243902439); // 1.0 / 41.0
// First corner
  vec2 i  = floor(v + dot(v, C.yy) );
  vec2 x0 = v -   i + dot(i, C.xx);

// Other corners
  vec2 i1;
  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
  //i1.y = 1.0 - i1.x;
  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
  // x0 = x0 - 0.0 + 0.0 * C.xx ;
  // x1 = x0 - i1 + 1.0 * C.xx ;
  // x2 = x0 - 1.0 + 2.0 * C.xx ;
  vec4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;

// Permutations
  i = mod289(i); // Avoid truncation effects in permutation
  vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
		+ i.x + vec3(0.0, i1.x, 1.0 ));

  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;

// Gradients: 41 points uniformly over a line, mapped onto a diamond.
// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

  vec3 x = 2.0 * fract(p * C.www) - 1.0;
  vec3 h = abs(x) - 0.5;
  vec3 ox = floor(x + 0.5);
  vec3 a0 = x - ox;

// Normalise gradients implicitly by scaling m
// Approximation of: m *= inversesqrt( a0*a0 + h*h );
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

// Compute final noise value at P
  vec3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

void main()	{
	vec2 uv = isf_FragNormCoord.xy;
	vec2 vc = gl_FragCoord.xy;
	vec4 inputPixelColor = IMG_PIXEL(inputImage, vc);

	float sn = snoise(vec2(0.0 + seed, uv.y * waveScale) + vec2(0. + seed, TIME * speedAmp));
	float sn2 = snoise(vec2(0.0 + seed, uv.y * sn) + vec2(0. + seed, TIME * 0.5));

	float amp = pow((sn * 2. - 1.) * (distStrength), 1.5) * (pow(sn2, 5.) * 30.) * (rand(uv.y * 10.) * 0.1);
		  
	vc.x += sin(sn * PI * 2.) * amp * 100.;
	//vc.y += cos(sn * PI * 2.) * amp * 100.;
	
	float rgbAmp = rgbSplit * amp;
	vec2 vcR = vec2(vc.x + rgbAmp, vc.y);
	vec2 vcG = vec2(vc.x, vc.y);
	vec2 vcB = vec2(vc.x - rgbAmp, vc.y);
	
	if(freeze) {
		inputPixelColor.r = IMG_PIXEL(bufferVariableNameA, vcR).r;
		inputPixelColor.g = IMG_PIXEL(bufferVariableNameA, vcG).g;
		inputPixelColor.b = IMG_PIXEL(bufferVariableNameA, vcB).b;
	} else {
		inputPixelColor.r = IMG_PIXEL(inputImage, vcR).r;
		inputPixelColor.g = IMG_PIXEL(inputImage, vcG).g;
		inputPixelColor.b = IMG_PIXEL(inputImage, vcB).b;
	}
	
	gl_FragColor = inputPixelColor;
}
