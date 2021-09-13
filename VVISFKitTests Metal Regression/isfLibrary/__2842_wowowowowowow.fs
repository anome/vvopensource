/*{
	"CREDIT": "by lennyjpg",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	
		{
			"NAME": "cover",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "blend",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 10.0
		},
		{
			"NAME": "waveA",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "waveB",
			"TYPE": "float",
			"DEFAULT": 0.15,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "waveC",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.0,
			"MAX": 1.0
		},
		
		{
			"NAME": "push",
			"TYPE": "float",
			"DEFAULT": 2.0,
			"MIN": 1.0,
			"MAX": 5.0
		},
		{
			"NAME": "stretch",
			"TYPE": "float",
			"DEFAULT": 0.25,
			"MIN": 0.0,
			"MAX": 2.0
		},
		{
			"NAME": "level",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "blur",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	]
}*/


	vec3 permute(vec3 x) { return mod(((x*34.0)+1.0)*x, 289.0); }
	float snoise(vec2 v){
	  const vec4 C = vec4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
	  vec2 i  = floor(v + dot(v, C.yy) );
	  vec2 x0 = v -   i + dot(i, C.xx);
	  vec2 i1;
	  i1 = (x0.x > x0.y) ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
	  vec4 x12 = x0.xyxy + C.xxzz;
	  x12.xy -= i1;
	  i = mod(i, 289.0);
	  vec3 p = permute( permute( i.y + vec3(0.0, i1.y, 1.0 ))
	  + i.x + vec3(0.0, i1.x, 1.0 ));
	  vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
	  m = m*m;
	  m = m*m;
	  vec3 x = 2.0 * fract(p * C.www) - 1.0;
	  vec3 h = abs(x) - 0.5;
	  vec3 ox = floor(x + 0.5);
	  vec3 a0 = x - ox;
	  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
	  vec3 g;
	  g.x  = a0.x  * x0.x  + h.x  * x0.y;
	  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	  return 130.0 * dot(m, g);
	}

void main(){
   vec2 uv = isf_FragNormCoord.xy - .5;
   float t = TIME * speed * 2.0, x = uv.x * stretch;
   vec3 s = vec3(.4,.7,-1.3) * t;
   vec3 f = vec3(waveA,waveB,waveC) * t;
   vec3 q = vec3(1,1.5,0.5) * x;    

   vec3 d = vec3( snoise(vec2(q.x+s.x, f.x)), snoise(vec2(q.y+s.y, f.y)), snoise(vec2(q.z+s.z, f.z)));
   vec3 w = sin(d)*0.5;
   vec3 ww = smoothstep(w - blur, w + blur,vec3(uv.y+level));
    ww *= push;
    
    vec3 gradient = vec3(4. * uv.y + .5),
    overlay = mix(cover.rgb,gradient,.5),
    final = mix(ww,overlay, blend);
    gl_FragColor = vec4(final,1.0);
}

