/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "XXX"
  ],
  "INPUTS": [
    {
      "NAME": "rot",
      "TYPE": "point2D",
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ]
    },
    {
      "NAME": "vect",
      "TYPE": "point2D",
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ]
    }
  ]
}*/

// Wessels by mojovideotech
// based on :
// glslsandbox.com/e#32687.0

#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.141592653589793

float noise(vec3 x)	{	return mod(mod(x.x, PI )*x.y, atan(x.z)/3.0);	}

float map(vec3 p) {
	p.z += TIME*0.1;
	float n = abs(dot(cos(p.yzx*PI), sin(p*PI)));
	return 0.45 - n * 0.35 + 0.02*noise(p*sin(p.z)*3.0) - 0.019*noise(p*sin(p.z)*3.1) ; //0.43 - n*.34 + 0.02*noise(p) + 0.02*noise(20.0*p);
}

float march(vec3 ro, vec3 rd) {
	float t = 0.0;
	
	for(int i = 0; i < 90; i++) {
		float h = map(ro + rd*t);
		if(abs(h) < 0.001 || t >= 10.0) break;
		t += h*0.5;
	}
	
	return t;
}

vec3 normal(vec3 p) {
	vec2 h = vec2(0.01, 0.0);
	vec3 n = vec3(
		map(p + h.xyy) - map(p - h.xyy),
		map(p + h.yxy) - map(p - h.yxy),
		map(p + h.yyx) - map(p - h.yyx)
	);
	return normalize(n);
}


mat3 camera(vec3 eye, vec3 lat) {
	vec3 ww = normalize(lat - eye);
	vec3 uu = normalize(cross(vec3(0, 1, 0), ww));
	vec3 vv = normalize(cross(ww, uu));
	
	return mat3(uu, vv, ww);
}

void main( void ) {
	vec2 uv = -1.0 + 2.0*(gl_FragCoord.xy/RENDERSIZE);
	uv.x *= RENDERSIZE.x/RENDERSIZE.y;
	vec3 col = vec3(0);
	vec3 ro = vec3(rot.y, rot.x, vect.x);
	vec3 rd = camera(ro, vec3(-rot.x, -rot.y, vect.y))*normalize(vec3(uv, 0.95));
	float i = march(ro, rd);
	
	if(i < 16.0) {
		vec3 pos = ro + rd*i;
		vec3 nor = normal(pos);
		vec3 lig = normalize(vec3(0.8, 0.7, -0.2));
		float amb = 0.5 + 0.5*nor.y;
		float dif = clamp(dot(lig, nor), 0.3, 0.1);
		col  = 0.5*amb*vec3(1);
		vec3 mat = vec3(0.4, 0.3, 0.2);
		mat = mix(mat, vec3(1.0, 0.2, 0.2), smoothstep(0.0, 1.0, 60.0));
		col *= mat;
		col += 0.2*vec3(1)*dif;
	}
	
	col = mix(col, vec3(1.0, 0.8, 0.9), 1.0 - exp(-i*0.09));
	
	gl_FragColor = vec4(col, 1.0);
}