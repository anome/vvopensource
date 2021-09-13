/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "generator",
    "retro",
    "equirectangular"
  ],
  "DESCRIPTION": "based on glslsandbox.com/e#12573.0",
  "INPUTS": [
    {
      "NAME": "seed1",
      "TYPE": "float",
      "DEFAULT": 0.77,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "seed2",
      "TYPE": "float",
      "DEFAULT": 0.53,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "seed3",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0.0001,
      "MAX": 9.999
    },
    {
      "NAME": "rotation",
      "TYPE": "float",
      "DEFAULT": 2,
      "MIN": 0,
      "MAX": 2
    },
    {
      "NAME": "rotozoom",
      "TYPE": "float",
      "DEFAULT": 34.95,
      "MIN": -50,
      "MAX": 50
    },
    {
      "NAME": "zoom",
      "TYPE": "float",
      "DEFAULT": 48.46,
      "MIN": -60,
      "MAX": 60
    },
    {
      "NAME": "density",
      "TYPE": "float",
      "DEFAULT": 7.43,
      "MIN": -50,
      "MAX": 50
    },
    {
      "NAME": "pulseRate",
      "TYPE": "float",
      "DEFAULT": 2.1,
      "MIN": 1,
      "MAX": 10
    },
    {
      "NAME": "glow",
      "TYPE": "float",
      "DEFAULT": 1.66,
      "MIN": 0,
      "MAX": 5
    },
    {
      "NAME": "baseColor",
      "TYPE": "color",
      "DEFAULT": [
        0.1,
        0.1,
        0.9,
        0.5
      ]
    },
    {
      "NAME": "glowColor",
      "TYPE": "color",
      "DEFAULT": [
        0.9,
        0.2,
        0.3,
        0.5
      ]
    }
  ]
}*/



////////////////////////////////////////////////////////////
// EquiRec_HAL10000  by mojovideotech
//
// based on :  
// glslsandbox.com/e#12573.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////



#ifdef GL_ES
precision mediump float;
#endif

#define pi 3.141592653589793 	// pi


vec2 rana(in vec2 p) {
	return fract(vec2(sin(p.x * 3889. + p.y * 3989.), cos(p.x * 983. + p.y * (17.*seed1))));
}

float ranb(vec2 p) {
    return fract(sin(dot(p.xy, vec2(139.*seed2, 1213.))) * 514229.);
}

vec2 ranc(float p) {
	return fract(vec2(sin(p * 5557.), cos(p * 4999.)*seed3));
}

vec3 voronoi(in vec2 x) {
	vec2 n = floor(x);
	vec2 f = fract(x);
	vec2 mg, mr;
	float md = 8.0, md2 = 8.0;
	for(int j = -1; j <= 1; j ++)
	{
		for(int i = -1; i <= 1; i ++)
		{
			vec2 g = vec2(float(i), float(j)); 
			vec2 o = rana(n + g); 
			vec2 r = g + o - f;
			float d = max(abs(r.x), abs(r.y)); 
			if(d < md)
				{md2 = md; md = d; mr = r; mg = g;}
			else if(d < md2)
				{md2 = d;}
		}
	}
	return vec3(n + mg, md2 - md);
}

mat2 rotate2d(float _angle) {
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

vec3 intersect(in vec3 o, in vec3 d, vec3 c, vec3 u, vec3 v) {
	vec3 q = o - c;
	return vec3(
		dot(cross(u, v), q),
		dot(cross(q, u), d),
		dot(cross(v, q), d)) / dot(cross(v, u), d);
}

void main( void ) {
	vec2 p = gl_FragCoord.xy / RENDERSIZE.xy;
	float th =  p.t * pi, ph = p.s * 2.0 * pi;
	vec3 s = vec3(sin(th) * cos(ph), sin(th) * sin(ph), cos(th));
    vec2 uv = vec2(rotate2d(rotation*pi)*s.xz);
	vec3 ro = vec3(10.0, 0.0, 0.0);
	vec3 ta = vec3(0.0, 1000.0, 0.0);
	vec3 ww = normalize(ro - ta);
	vec3 uu = normalize(cross(ww, normalize(vec3(0.0,1.0,0.0))));
	vec3 vv = normalize(cross(uu, ww));
	vec3 rd = normalize(uv.x * uu + uv.y * vv + 0.5 * ww);
	vec3 its;
	float v, g;
	float inten = 0.0;
	for(int i = 0; i < 9; i ++)
	{
		float layer = float(i);
		its = intersect(ro, rd, vec3(rotozoom, -10.0 - layer * (density*0.1), 0.0), vec3(1.0, 0.0, 0.0), vec3(0.0, 0.0, 1.0));
		if(its.x > 0.0)
		{
			vec3 vo = voronoi((its.xz) * (zoom*0.001) + 10.0 * ranc(float(i)));
			v = exp(-100.0 * (vo.z - 0.0367));
			float fx = 0.0;
			if(i == 5)
			{
				float T = TIME * pulseRate;
				float crd = fract(TIME * T) * 50.0 - 25.0;
				float fxi = cos(vo.x * 0.2 + -T * 1.5);  //abs(crd - vo.x);
				fx = clamp(smoothstep(0.7, 1.0, fxi), 0.0, 0.9) * 1.0 * ranb(vo.xy);
				fx *= exp(-2.0 * vo.z) * 3.0;
			}
			inten += v * 0.1 + fx;
		}
	}
	
	vec3 col = pow(mix(vec3(inten, (inten * 0.5), inten),baseColor.rgb,0.6), (5.5-glow) * glowColor.gbr);
	
	gl_FragColor = vec4(col, 1.0);
}
