/*{
  "CATEGORIES": [
    "Automatically Converted",
    "GLSLSandbox"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#36876.1",
  "INPUTS": [

  	 { 
  	 	"NAME": "zoom",
  	 	"TYPE": "float",
  	 	"DEFAULT":1.0
  	 },
  	 { 
  	 	"NAME": "layers",
  	 	"TYPE": "float",
  	 	"DEFAULT":1.0
  	 },
  	 { 
  	 	"NAME": "rotation",
  	 	"TYPE": "float",
  	 	"DEFAULT":1.0
  	 },
  	 { 
  	 	"NAME": "fade",
  	 	"TYPE": "float",
  	 	"DEFAULT":1.0
  	 },
  	 { 
  	 	"NAME": "size",
  	 	"TYPE": "float",
  	 	"DEFAULT":1.0
  	 }
  ]
}*/


//--- FTL ---
// by Catzpaw 2016

vec3 star(vec2 uv, float scale, float seed){
	uv *= scale;

	vec2 s = floor(uv), f=fract(uv), p;
	float k = 3., d;
	
	p = .5 + .440 * sin(11. * fract(sin((s + seed) * mat2(7.5, 3.3, 6.2, 5.4)) * 55.)) - f;
	d = length(p) + fade * 0.01 * scale;
	k = min(d, k);
	
	k = smoothstep(0., k, 0.025 * size);
    return k * vec3(k, k, 1.);
}

void main(void){
	float phase = rotation * 6.283185307;
	float phase1 = zoom * 6.283185307;
	
	vec2 pos = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 uv = (gl_FragCoord.xy*2.-RENDERSIZE.xy) / min(RENDERSIZE.x,RENDERSIZE.y); 
	
	
	uv *= mat2(cos(phase), -sin(phase), sin(phase), cos(phase));
	
	vec3 c=vec3(0);
	
	float n = 1.0 / 10.;
	float _layers = layers * 20.;
	
	for(float i = 0.; i < 20.; i += 2.) {
		c += star(uv, mod(_layers + i - zoom * _layers, _layers), i * 5.1);
		
		if(i > _layers)
			break;
	}
	gl_FragColor = vec4(c, 88.5);
}