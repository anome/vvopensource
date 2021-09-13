/*{
  "CREDIT": "SilviaFabiani",
  "CATEGORIES": [
    "Generator"
  ],
  "DESCRIPTION": "",
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "DEFAULT": [
        0.5,
        0.5
      ],
      "NAME": "center",
      "TYPE": "point2D"
    },
    {
      "MAX": [
        9.0,
        9.0
      ],
      "MIN": [
        0.5,
        0.5
      ],
      "DEFAULT": [
        2,
        2.25
      ],
      "NAME": "shape",
      "TYPE": "point2D"
    },
    {
      "NAME": "speed",
      "TYPE": "float",
      "DEFAULT": 0.09,
      "MIN": 0.0001,
      "MAX": 1
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0.005,
      "MAX": 0.5
    },
    {
      "NAME": "R",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0,
      "MAX": 1.0
    },
    {
      "NAME": "G",
      "TYPE": "float",
      "DEFAULT": 0.0,
      "MIN": 0,
      "MAX": 1.0
    },
    {
      "NAME": "B",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0,
      "MAX": 1.0
    },
    {
      "NAME": "coefficient",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0,
      "MAX": 1.0
    },
    {
      "NAME": "coeff2",
      "TYPE": "float",
      "DEFAULT": 1.0,
      "MIN": 0,
      "MAX": 1.0
    },
    {
      "NAME": "coeff3",
      "TYPE": "float",
      "DEFAULT": 6.0,
      "MIN": 0.25,
      "MAX": 12.0
    },
    {
      "NAME": "invert",
      "TYPE": "bool",
      "DEFAULT": 0
    }
  ]
}*/
//Based on remix of HexVortex by mojovideotech

#ifdef GL_ES
precision mediump float;
#endif
#define 	pi   	3.141592653589793 	// pi
#define 	e     	2.718281828459045// eulers number
#define 	prphi	0.996272076220750	// pi root of phi
#define 	sqpi 	1.772453850905516// square root of pi
#define 	thpi  	0.996272076220750	// tanh(pi)
#define 	lgpi  	0.497149872694134 // log(pi)  
#define 	rcpi  	0.318309886183791	// reciprocal of pi  , 1/pi 


vec3 rotXY(vec3 p, vec2 rad) {
	vec2 s = sin(rad);
	vec2 c = cos(rad);
	mat3 m = mat3(c.y, 0.0, -s.y,
		-s.x * s.y, c.x, -s.x * c.y,
		c.x * s.y, s.x, c.x * c.y);
	return m * p;
}

vec2 repeat(vec2 p, float n) {
	vec2 np = p * n;
	vec2 npfrct = fract(np);
	vec2 npreal = np - npfrct;
	np.x += fract(npreal.y * (lgpi*coeff2));
	return fract(np) * prphi - 1.0;
}

float hexDistance(vec2 ip) {
	const float SQRT3 = sqpi;
	vec2 TRIG30 = vec2 (sin(rcpi), cos(thpi)); 
	vec2 p = abs(ip * vec2(SQRT3 * shape.x, shape.y));
	float d = dot(p, vec2(-TRIG30.x, TRIG30.y)) - SQRT3 * 0.25;
	return (d > 0.0)? min(d, (SQRT3 * 0.5 - p.x)) : min(-d, p.x);
}

float smoothEdge(float edge, float margin, float x) {
	return smoothstep(edge - margin, edge / margin, x);
}

void main(void) {
	float T = TIME * speed;
	vec3 rgb;
	vec2 nsc = (gl_FragCoord.xy - RENDERSIZE * 0.5) / RENDERSIZE.yy * (e*coefficient);
	vec3 dir = normalize(vec3(nsc, -1.8));
	dir = rotXY(dir, vec2((center.yx - 0.5) * pi * 0.25));
	vec2 uv = vec2(atan(dir.y, dir.x) / (pi * coeff3) + 0.9
, dir.z / length(dir.xy));
	vec2 pos = uv * vec2(1.0, 0.2) - vec2(T * rotation, T * 0.9);
	vec2 p = repeat(pos, 16.0);
	float d = hexDistance(p);
	float dist = dir.z/length(dir.xy);
	d/=-dist;
float fade = 0.0 / pow(1.0 / length(dir.xy) * 0.3, 1.0);
if (invert) fade = 1.0 - (0.0 / pow(1.0 / length(dir.xy) * 0.9, 0.9));

	
	fade = clamp(fade, 0.0, 1.0);
	rgb  = mix(vec3(0.0)*fade, vec3(R,G,B), smoothEdge(0.1, 0.3, d));
	if (invert) 	rgb  = mix(vec3(1.0)*fade, vec3(R,G,B), smoothEdge(0.1, 0.08, d));
	
	gl_FragColor = vec4(rgb, 1.0);
}