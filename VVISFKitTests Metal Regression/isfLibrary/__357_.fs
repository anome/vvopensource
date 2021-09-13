/*{
	"CREDIT": "by nanaki14",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/

//	Simplex 3D Noise
//	by Ian McEwan, Ashima Arts
//
vec4 permute(vec4 x){return mod(((x*34.0)+1.0)*x, 289.0);}
vec4 taylorInvSqrt(vec4 r){return 1.79284291400159 - 0.85373472095314 * r;}

float snoise(vec3 v){
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

  //  x0 = x0 - 0. + 0.0 * C
  vec3 x1 = x0 - i1 + 1.0 * C.xxx;
  vec3 x2 = x0 - i2 + 2.0 * C.xxx;
  vec3 x3 = x0 - 1. + 3.0 * C.xxx;

// Permutations
  i = mod(i, 289.0 );
  vec4 p = permute( permute( permute(
             i.z + vec4(0.0, i1.z, i2.z, 1.0 ))
           + i.y + vec4(0.0, i1.y, i2.y, 1.0 ))
           + i.x + vec4(0.0, i1.x, i2.x, 1.0 ));

// Gradients
// ( N*N points uniformly over a square, mapped onto an octahedron.)
  float n_ = 1.0/7.0; // N=7
  vec3  ns = n_ * D.wyz - D.xzx;

  vec4 j = p - 49.0 * floor(p * ns.z *ns.z);  //  mod(p,N*N)

  vec4 x_ = floor(j * ns.z);
  vec4 y_ = floor(j - 7.0 * x_ );    // mod(j,N)

  vec4 x = x_ *ns.x + ns.yyyy;
  vec4 y = y_ *ns.x + ns.yyyy;
  vec4 h = 1.0 - abs(x) - abs(y);

  vec4 b0 = vec4( x.xy, y.xy );
  vec4 b1 = vec4( x.zw, y.zw );

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
  vec4 norm = taylorInvSqrt(vec4(dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3)));
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

float rnd(vec2 n){
  return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

vec2 hash(float p) {
  vec3 p3 = fract(vec3(p) * vec3(0.1031, 0.1030, 0.0973));
  p3 += dot(p3, p3.yzx + 19.19);

  return fract((p3.xx + p3.yz) * p3.zy);
}

float fill(vec2 p) {
  return 0.1 / smoothstep(0.0,  1.2, length( vec2(p.x,  p.y)));
}

vec3 wave(vec2 p) {
  vec3 c = vec3(0.0);

  float nx = p.x * 4.2;
  float ny = p.y * 50.5;
  float nz = TIME * 0.2;
  p.x += 2.0 * snoise(vec3(nx, ny, nz));
 
  c = vec3(fill(vec2(p.x * 0.5, snoise(vec3(0.8 * snoise(vec3(p.x * 1.5, p.y * 0.2, TIME *0.5)), p.y * TIME * 0.05 , nz * 0.01)) )));
  
  return c;
}

float blockNoise(in vec2 uv, float t) {
  float n = 0.0;
  float k = 0.8;
  float l = 1.7;
  uv += .3;
  for (int i = 0; i < 16; i++) {
    n += snoise(vec3(floor(uv * l), t / 10.0)) * k;
    k *= 1.9;
    l += 2.2;
  }
  return fract(n);
}

float whiteNoise(vec2 p, vec3 c) {
  float color = (c.r + c.g + c.b) / 3.0;
  float vignette = 1.0;
  color *= vignette;
  
  float noise = rnd(gl_FragCoord.st + mod(TIME, 10.0));
  color *= noise * 0.5 + 0.5;
  
  float scanLine = abs(sin(p.y * 150.0 + TIME * 5.0)) * 0.5 + 0.5;
  color *= scanLine;
  
  return color;
}

vec3 render(vec2 p) {
  vec3 c = vec3(0.0);
  
  c = wave(p);
  
  //c.gb += step(
  //  0.01,
  //  blockNoise(
  //    vec2(p.x * 0.1, p.y * 2.0), fract(TIME * 0.1)
  //    )
  //  );

  c = vec3( c * whiteNoise(p, c) * 0.2);

  //c = vec3(
  //  smoothstep(0.8, 1.0, c.x),
  //  c.y,
  //  0.1 + 0.8 * c.z
  //);

  return c;
}


void main() {
	vec2 uv = isf_FragNormCoord.xy * 2.0 - 1.0;
	vec4 c = vec4(render(uv), 1.0) * IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	
	gl_FragColor = c;
}