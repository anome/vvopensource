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


const vec4 redfilter1 		= vec4(1.0, 0.01, 0.0, 1.0);
const vec4 bluegreenfilter1 	= vec4(0.0, 1.0, 0.7, 1.0);
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
	
	vec4 redrecord = color * redfilter1;
	vec4 bluegreenrecord = color * bluegreenfilter1;
	vec4 rednegative = vec4(redrecord.r);
	vec4 bluegreennegative = vec4((bluegreenrecord.g + bluegreenrecord.b)/2.0);

	vec4 redoutput = rednegative * redfilter1;
	vec4 bluegreenoutput = bluegreennegative * bluegreenfilter1;

	// additive 'projection"
	color = redoutput + bluegreenoutput;

	//k = smoothstep(0., k, 0.025 * size);
    //return (k - d) * vec3(k, k, 1.);
	//return (pow(1.0 - (zoom/4.), 2.)) * vec3(k);
    //return max(1., 1.2 * pow(1.0 - (zoom/4.), .3)) * vec3(k* color.r, k * color.g, k * color.b) - k * 0.3;
    return vec4(k* color.r, k * color.g, k * color.b, k);
}

void main(void){
	float phase = rotation * 6.283185307;
	float phase1 = zoom * 6.283185307;

	vec2 pos = gl_FragCoord.xy / RENDERSIZE.xy;
	vec2 uv = (gl_FragCoord.xy*2.-RENDERSIZE.xy) / min(RENDERSIZE.x,RENDERSIZE.y); 
	
	uv *= mat2(cos(phase), -sin(phase), sin(phase), cos(phase));
	
	vec4 c=IMG_PIXEL(inputImage,gl_FragCoord.xy);
	
	float n = 1.0 / 10.;
	float _layers = layers * 5.;
	
	
	
	for(float i = 0.; i < 5.; i += 2.) {
		
		
		vec4 dust = star(uv, mod(_layers + i - zoom * _layers, _layers), i * 5.1, seed);
		c = mix(c, dust, dust.a*c.a);
		
		if(i > _layers)
			break;
	}
	gl_FragColor = vec4(c);
}