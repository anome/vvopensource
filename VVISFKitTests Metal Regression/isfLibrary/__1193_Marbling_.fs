/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "Amplitude",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "XParts",
			"TYPE": "float",
			"DEFAULT": 0.2,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "YParts",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
      	]
}*/

// Original shader "Marbling" by lainga: https://www.shadertoy.com/view/MdySzR

float iGlobalTime = TIME;



vec2 pa(in vec2 uv, in float per){
    vec2 result = (sin(uv.x*per)) * normalize(vec2(cos((uv.x)*per), .5));
    return result;
}

vec2 pb(in vec2 uv, in float per){
    vec2 result = (cos(uv.y*per)) * normalize(vec2(1., cos((uv.y)*per)));
    return result;
}


void main(void) {
    float t = iGlobalTime/4.;
    
	vec2 uv = (gl_FragCoord.xy / RENDERSIZE.xy);
        
    vec2 vpert = pa(uv/.7, XParts*100.0);
    uv += vpert * sin(Amplitude) *.2;
    
    vec2 hpert = pb(uv/.7, YParts*100.0);
     uv += hpert * sin(Amplitude) * .108;
    
    vec4 col = IMG_NORM_PIXEL(inputImage, uv);
	gl_FragColor =  mix(vec4(sin(uv.y*2.), 0., cos(uv.y*3.), 1.), col, .9);
	
}