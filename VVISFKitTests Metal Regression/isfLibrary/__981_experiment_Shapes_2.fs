/*{
	"CREDIT": "Silvia",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
	{
		"NAME" : 		"value1",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.65,
		"MIN" : 		0.5,
		"MAX" : 		0.8
	},
	{
		"NAME" : 		"value2",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.7,
		"MIN" : 		0.4,
		"MAX" : 		0.8
	},	
	{
		"NAME" : 		"offset",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.1,
		"MAX" : 		0.75
	},
			{
			"NAME": "ColorA",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.5,
				1.0,
				1.0
			]
		},
		{
			"NAME": "ColorB",
			"TYPE": "color",
			"DEFAULT": [
				1.0,
				1.0,
				1.0,
				1.0
			]
		}		
	]
}
*/



#ifdef GL_ES
precision mediump float;
#endif
float rect(in vec2 st, in vec2 size){
	size = 0.25-size*0.25;
    vec2 uv = step(size,st *(1.0-st));
    vec2 offset = vec2 (.3,0.);
	return uv.x*uv.y;
	
}

void main() {
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    

    vec3 influenced_color = vec3(0.9,0.8,0.9);
    vec3 ColorA = vec3(ColorA);
    vec3 ColorB = vec3(ColorB);
    
    vec3 color = vec3(1.0);
    
    // Background Gradient
    color = mix( ColorA,
                 ColorB,
                 st.y);
    
    // Foreground rectangle
    color = mix(color,
               (1.0 - influenced_color),
               rect(st,vec2((cos(offset -value1)),(cos ( TIME -value2*2.5)))));
               color = mix((color),
               ColorA,
               rect((cos(offset /st - offset)) ,vec2((TIME+value1),(sin (TIME+value2*1.5)))));
               color = mix((1.0-color),
               (1.0 - ColorB),
               rect((cos(offset /st - offset)) ,vec2((cos(TIME*value1)),(sin(TIME*value2)))));
    gl_FragColor = vec4(color,1.0);
}