/*{
  "CATEGORIES": [
    "INKA"
  ],
  "DESCRIPTION": "Automatically converted from http://glslsandbox.com/e#36876.1",
  "INPUTS": [
  	 { 
  	 	"NAME": "inputImage",
  	 	"TYPE": "image"
  	 },
  	 { 
  	 	"NAME": "zoom",
  	 	"TYPE": "float"
  	 },
  	 { 
  	 	"NAME": "layers",
  	 	"TYPE": "float"
  	 },
  	 { 
  	 	"NAME": "rotation",
  	 	"TYPE": "float"
  	 },
  	 { 
  	 	"NAME": "fade",
  	 	"TYPE": "float"
  	 },
  	 { 
  	 	"NAME": "seed",
  	 	"TYPE": "float"
  	 },
  	 { 
  	 	"NAME": "size",
  	 	"TYPE": "float"
  	 }
  ]
}*/

const vec4 bluegreenfilter1 	= vec4(1.0, 1.0, 1., 1.0);
const float PI = 3.141592654;
//--- FTL ---
// by Catzpaw 2016

vec4 star(vec2 uv, float zoom, float seed, float minorseed){
	uv *= zoom;

	vec2 s = floor(uv), f=fract(uv), p;
	float k = 3., d;
	
	p = .5 + .440 * (sin(s + minorseed * 3.14)) * sin(11. * fract(sin((s + seed) * mat2(7.5, 3.3, 6.2, 5.4)) * 55.)) - f;

	d = length(p) + fade * 0.01 * zoom;
	//k = min(d, k);
	k = d;
	//k = smoothstep(k*.8, k, 0.025 * size);
	k = smoothstep(k*.9, k, 0.025 * size);
	float shades = 2.0;
	
	vec4 color = vec4(
		(shades/(shades-1.0))*mod(floor(shades*uv.y)/shades, 1.0),
		(shades/(shades-1.0))*mod(floor(shades*uv.x)/shades, 1.0),
		(shades/(shades-1.0))*mod(floor(shades*uv.x)/shades, 1.0), 
		1.
	);
    color =  bluegreenfilter1;
    return vec4(k* color.r, k * color.g, k * color.b, k);
}

void main(void){
	float phase = rotation * 2. *PI;
	float b = 1.-((isf_FragNormCoord.x/2.) + (isf_FragNormCoord.y));
	float opacity = (isf_FragNormCoord.x + isf_FragNormCoord.y);
	vec2 uv = (gl_FragCoord.xy*2.-RENDERSIZE.xy) / min(RENDERSIZE.x,RENDERSIZE.y); 
	
	uv *= mat2(cos(phase), -sin(phase), sin(phase), cos(phase));
	
	vec4 c= vec4(0.,0., b, opacity);
	
	float _layers = layers * 5.;
	
	for(float i = 0.; i < 5.; i += 2.) {
	
		vec4 dust = star(uv, mod(_layers + i - zoom * _layers, _layers), i * 5.1, seed);
		c = mix(c, dust, dust.a*c.a);
		
		if(i > _layers)
			break;
	}
	gl_FragColor = vec4(c);
//	gl_FragColor = vec4(c);
}