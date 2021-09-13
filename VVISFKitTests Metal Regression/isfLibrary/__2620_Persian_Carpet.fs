/*{
	"CREDIT": "by joshpbatty",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "scale",
			"TYPE": "float",
			"DEFAULT": 1.3,
			"MIN": 1.299,
			"MAX": 1.3
		},
		{
			"NAME": "fold",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				-0.5
			]
		}
	]
}*/

// https://www.shadertoy.com/view/MdlXzM

 //vec2 fold = vec2(0.5, -0.5);
 vec2 translate = vec2(1.5);
 //float scale = 1.3;


vec3 hsv(float h,float s,float v) {
	return mix(vec3(1.),clamp((abs(fract(h+vec3(3.,2.,1.)/3.)*6.-3.)-1.),0.,1.),s)*v;
}

vec2 rotate(vec2 p, float a){
	return vec2(p.x*cos(a)-p.y*sin(a), p.x*sin(a)+p.y*cos(a));
}

void main() {	

//	vec2 p = -1.0 + 2.0*fragCoord.xy/iResolution.xy;
	vec2 p = -1.0 + 2.0*isf_FragNormCoord.xy;// * RENDERSIZE.xy;


	p.x *= RENDERSIZE.x/RENDERSIZE.y;
	p *= 0.003;
    float x = p.y;
	p = abs(mod(p, 8.0) - 4.0);
	for(int i = 0; i < 36; i++){
	  p = abs(p - fold) + fold;
	  p = p*scale - vec2(translate);
	  p = rotate(p, 3.14159/(8.0+sin(TIME*0.001+float(i)*0.1)*0.5+0.5));
	}
	float i = x*10.0 + atan(p.y, p.x) + TIME*0.5;
	float h = floor(i*6.0)/5.0 + 0.07;
	h += smoothstep(0.0, 0.4, mod(i*6.0/5.0, 1.0/5.0)*5.0)/5.0 - 0.5;
	gl_FragColor=vec4(hsv(h, 1.0, smoothstep(-1.0, 3.0, length(p))), 1.0);
	
	//gl_FragColor = vec4(1.0,abs(sin(TIME*10.0)),0.0,1.0);
}