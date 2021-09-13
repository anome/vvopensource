/*{
	"DESCRIPTION": "Your shader description",
	"CREDIT": "by you",
	"CATEGORIES": [
		"Your category"
	],
	"INPUTS": [
		{
			"NAME": "iChannel0",
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

vec3 iResolution = vec3(RENDERSIZE, 1.);
float iGlobalTime = TIME;



vec2 pa(in vec2 uv, in float per){
    vec2 result = (sin(uv.x*per)) * normalize(vec2(cos((uv.x)*per), .5));
    return result;
}

vec2 pb(in vec2 uv, in float per){
    vec2 result = (cos(uv.y*per)) * normalize(vec2(1., cos((uv.y)*per)));
    return result;
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    float t = iGlobalTime/4.;
    
	vec2 uv = (fragCoord.xy / iResolution.xy);
    uv.x *= iResolution.x/iResolution.y*.5+.5;
        
    vec2 vpert = pa(uv/.7, XParts*100.0);
    uv += vpert * sin(Amplitude) *.2;
    
    vec2 hpert = pb(uv/.7, YParts*100.0);
     uv += hpert * sin(Amplitude) * .108;
    
    vec4 col = texture2D(iChannel0, uv);
	fragColor = fragColor = mix(vec4(sin(uv.y*2.), 0., cos(uv.y*3.), 1.), col, .9);
}

void main(void) {
    mainImage(gl_FragColor, gl_FragCoord.xy);
}