/*{
	"DESCRIPTION": "",
	"CREDIT": "SilviaFabiani",
	"ISFVSN": "2",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		{
			"NAME": "Magnitudo",
			"TYPE": "float",
			"MAX ": 0.35,
			"MIN ": 0.15,
			"DEFAULT": 0.260	
		},
		{
			"NAME": "OFFSET",
			"TYPE": "float",
			"MAX ": 0.09,
			"MIN ": 0.01,
			"DEFAULT": 0.06
		},
		{
			"NAME": "startColor",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.5,
				1.0,
				0.5
			]
		},
		{
			"NAME": "endColor",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				0.5
			]
		}
		
	]
		
}*/
#ifdef GL_ES
precision mediump float;
#endif
float rect( vec2 st,  vec2 size){
	size = 0.25-size*0.15;
    vec2 uv = step(size,st*(1.0-st));
	return uv.x*uv.y;
	
}

void main(){
	
	float tempo = clamp (TIME, 0.0,120.0);
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
   vec3 colorA = vec3 (sin (startColor+tempo));
    vec3 colorB = vec3 (sin (endColor+tempo));
    vec3 color = vec3(0.);
    color = mix( colorA,
                 colorB,
                 st.x);
    
    vec2 size =  vec2 (0.08, sin (tempo+Magnitudo));
    vec2 offset = vec2 (0.06, cos (tempo+OFFSET));
    color = mix(color,
               colorA+0.1,
               rect(st - 0.2-offset,size/2.5));
   
    
    color = mix(color,
               colorB,
               rect(st+offset+0.1,size));
               color = mix(color,
               colorA+0.3,
               rect((st-(offset-0.1)/32.0),size/0.5));
    
    color = mix(color,
               colorA,
               rect(st-(offset+0.05),size));        
    


    gl_FragColor = vec4(color,1.0);
}