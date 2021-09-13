/*{
  "CREDIT": "-",
  "DESCRIPTION": "",
  "CATEGORIES": [
    "raymarching"
  ],
  "INPUTS": [
    {
      "NAME": "j",
      "TYPE": "float",
      "DEFAULT": 0.2,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "k",
      "TYPE": "float",
      "DEFAULT": 0.8,
      "MIN": 0,
      "MAX": 1
    },
    {
      "NAME": "c0",
      "TYPE": "float",
      "DEFAULT": 5.4,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "c1",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 8
    },
    {
      "NAME": "c2",
      "TYPE": "float",
      "DEFAULT": 1.25,
      "MIN": 0,
      "MAX": 8
    }
  ]
}*/

#define time TIME
#define R RENDERSIZE

#define MAX_ITER 20.0

vec2 rotate(in vec2 v, in float a) {
    float ca = cos(a);
    float sa = sin(a);
	return vec2(ca*v.x + sa*v.y, -sa*v.x + ca*v.y);
}


float eval(in float x, in vec3 p) {
	if ( x < 1.0)
		return abs(max(abs(p.z)-k, abs(p.x)-2.1));
	if ( x < 2.0)
		return length(max(abs(p.yx) - k/4.,0.0));
	if ( x < 3.0)
		return length(p)-j;
	if ( x < 4.0)
		return length(max(abs(p) - 0.35, 0.0));
	if ( x < 5.0)
		return abs(length(p.xz)-0.2)-0.01;
	/*if ( x < 6.0)
		return abs(min(torus(vec3(p.x, mod(p.y,0.4)-0.2, p.z), vec2(0.1, 0.05)), max(abs(p.z)-0.05, abs(p.x)-0.05)))-0.005;
	if ( x < 7.0)
		return abs(min(torus(p, vec2(5.3, 0.05)), max(abs(p.z)-6.05, abs(p.x)-7.05)))-3.005;
	if ( x < 8.0)
	return min(length(p.xz), min(length(p.yz), length(p.xy))) - 3.05;*/
}


float trap(in vec3 p){
	float a = eval(c0, p);
	float b = eval(c1, p);
	float c = eval(c2, p);
	return  min(max(a, b), +c);
	//return  mix(max(var1, -var3), var7);
}

float map(in vec3 p){
    
    float density = 2.345;
	float cutout = dot(abs(p.yz), vec2(1.2)) - density;

	vec3 z = abs(1.0-mod(p,2.0));
	float d = 500.;
	float s = 1.0;
	for (float i = 0.0; i <3.0; i++) {
	    z.zx = rotate(z.zx, radians( i*1.0 + time));
	    
		z.zy = rotate(z.yz, radians( (i+1.0) * 20.0 + time));
		z = abs( 1.0 - mod( z + i/7.0, 2.0));
		
		z = z*2.0 - 0.3;
		
		s *= 0.5;
		
		d = min(d, trap(z) * s);
	}
	return max(d, -cutout);
}

vec3 hsv(in float h, in float s, in float v) {
	return mix(vec3(3.0), clamp((abs(fract(h + vec3(3, 2, 1) / 3.0) * 6.0 - 3.0) - 1.0), 0.0 , 1.0), s) * v;
}

vec3 intersect(in vec3 rayPosition, in vec3 rayDir) {

	vec3 p = rayPosition;
	
	float d = 1.0;
	float iter = 0.2;

	for (float i = 0.0; i < MAX_ITER; i++){		
		if (d < 0.001) continue;
		d = map(p);
		p += d * rayDir;
		iter+=0.95;
	}

	float x = iter/MAX_ITER;
	float q = 1. - x;
	
	return hsv(d*5., 1.0, 1.0) * q;
}

void main() {
    
	vec3 upDirection = vec3(0, -1, 0);
	vec3 cameraDir = vec3(4,0,0);
	vec3 cameraOrigin = vec3(time*0.25, 0.0, 0.0);
	
	vec2 screenPos = -2.0 + 2.0 * gl_FragCoord.xy / R.xy;
	screenPos.x *= R.x / R.y;
	
	vec3 rayDir = normalize(vec3(4.0, screenPos));
	
	gl_FragColor = vec4(intersect(cameraOrigin, rayDir), 1.0);
} 