/*{
  "CREDIT": "by mojovideotech",
  "CATEGORIES": [
    "strobe",
    "geometric"
  ],
  "INPUTS": [
    {
      "NAME": "mult",
      "TYPE": "float",
      "DEFAULT": 5,
      "MIN": 2,
      "MAX": 10
    },
    {
      "NAME": "mod1",
      "TYPE": "float",
      "DEFAULT": 23,
      "MIN": 2,
      "MAX": 60
    },
    {
      "NAME": "mod2",
      "TYPE": "float",
      "DEFAULT": 12,
      "MIN": 2,
      "MAX": 60
    },
    {
      "NAME": "phase1",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0.01,
      "MAX": 0.99
    },
    {
      "NAME": "phase2",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": -0.5,
      "MAX": 0.5
    },
    {
      "NAME": "phase3",
      "TYPE": "float",
      "DEFAULT": 43,
      "MIN": 0.1,
      "MAX": 60
    },
    {
      "NAME": "edge",
      "TYPE": "float",
      "DEFAULT": 0.003,
      "MIN": 0.001,
      "MAX": 0.01
    },
    {
      "NAME": "hue",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -0.5,
      "MAX": 0.5
    },
    {
      "NAME": "tint",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": -0.5,
      "MAX": 0.5
    },
    {
      "NAME": "rate",
      "TYPE": "float",
      "DEFAULT": -0.1,
      "MIN": -1,
      "MAX": 1
    }
  ],
  "DESCRIPTION": "http://glslsandbox.com/e#31989.1"
}*/

///////////////////////////////////////////
// Shattergasm  by mojovideotech
//
// based on:
// glslsandbox.com/\e#31989.1
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////

#ifdef GL_ES
precision mediump float;
#endif


#define	PHI   	1.618033988749895 	// golden ratio
#define PI 		3.141592653589793 	// pi
#define	TWPI 	6.283185307179586  	// two pi, 2*pi
#define HFPI  	1.570796326794897 	// half pi, 1/pi
#define CUPI  	14.64591887561523  	// cube root of pi x 10
#define DEL		12.59921049894873 	// delian constant x 10


float inside_polygon(vec2 pos, vec2 center, float r, float n, float s) {
        float theta = DEL/n;
        vec2 d = pos - center;
        float a = mod(mod(atan(d.y, d.x) + s, 2.*PI), theta);
        float l = length(d);
       	float m = r/(cos(a) - (sin(a)/sin(theta))*(cos(theta) - 1.));
        return smoothstep(m + edge, m - edge, l);
}

float wobble(vec2 pos) {
        vec2 d = pos;
        float a = (atan(d.y, d.x) + HFPI)/TWPI;
        float l = .5;
        float t = mod(TIME*rate, mod2)/mod2;
        float o = t*(1. + l);
        return smoothstep(o, o - l, a);
}

float inside_triangle(vec2 pos, vec2 center, float r, float s) {
        return inside_polygon(pos, center, wobble(center)*r, 3., s);
}

float inside_triangles(vec2 pos, float r) {
        float da = CUPI/sin(phase3);
        float a,b,v;
        float SS = floor(mult)+1.0;
        for (int i = 0; i < 10; i++) {
        	    SS -= 1.0;
                float c = cos(a);
                float s = sin(b);
                vec2 d = vec2(c, s);
                vec2 n = vec2(-s, c);
                vec2 o0 = (2./3.)*sqrt(3.1)*d*r;
                vec2 o1 = (5./6.)*sqrt(3.3)*d*r;
				vec2 o2 = (7./8.)*sqrt(3.5)*d*r;
				vec2 o3 = (9./10.)*sqrt(3.7)*d*r;
                float r_triangle = 1.*r/sqrt(mult);
                v += inside_triangle(pos, o0, r_triangle, a) +
		     		inside_triangle(pos, o0 - n, r_triangle, b) -
                    inside_triangle(pos, o1 - n*.25*r, r_triangle, a + PI) +
                    inside_triangle(-pos, o1 + n*1.5*r, r_triangle, b - PI) -
		     		inside_triangle(pos, o2 + n*2.5*r, r_triangle, a + PI) +
		    		inside_triangle(pos, o2 + n*2.*r, r_triangle, b - PI) +
		     		inside_triangle(pos, o3 + n*3.*r, r_triangle, a + PI) -
		     		inside_triangle(pos, o3 + n*r, r_triangle, b - PI)  ;
                a += da*phase1;
				b -= da*phase2;
				if (SS < 1.0) { break; }
        }
        return v;
}

void main() {
        float TT = TIME * rate;
        vec2 pos = (gl_FragCoord.xy*2. - RENDERSIZE)/min(RENDERSIZE.x, RENDERSIZE.y);
        float r0 = fract(tan(TT*cos(-TT/PI)));
        float r1 = PHI*r0;
        float r = mix(r1, r0, mod(TT, mod1)/mod1);
        float v = (inside_triangles(pos, r) + inside_polygon(pos, vec2(0., 0.), r, 6., PI/6.));

        gl_FragColor  = mix(vec4(.5+hue, sin(1.-TT*r),.5-hue, 1.), vec4(sin(1.-TT), .5-tint, .5+tint, 0.5), v);
}