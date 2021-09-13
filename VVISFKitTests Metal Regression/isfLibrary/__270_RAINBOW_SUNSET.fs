/*{
	"DESCRIPTION": "Calming mood colorful sunset/sunrise generator. Range of sunset colors picked by user. Speed of sunset picked by user.",
	"CREDIT": "Rotation algorithm used from Book of Shader Tutorial 8",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.1,
			"MAX": 2.0
		},
	
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif


float box(in vec2 _st, in vec2 _size){
    _size = vec2(0.5) - _size*0.2;
    vec2 uv = smoothstep(_size,
                        _size+vec2(2.0),
                        _st);
    uv *= smoothstep(_size,
                    _size+vec2(0.1),
                    vec2(4.0)-_st);
    return uv.x*uv.y;
}

float swipe(in vec2 _st, float _size){
    return  box(_st, vec2(_size,_size/10.)) + 
            box(_st, vec2(_size/10.,_size));
}

void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    vec3 color = vec3(colorInput*abs(sin(TIME*speed)));
        
    
    vec2 translate = vec2(cos(TIME*speed),sin(TIME*speed));
    st += translate*1.3;

  
    

    // Add the shape on the foreground
    color += vec3(swipe(st,0.4));
    
    
    //extra color layer
     color += vec3(st.x, 0.4, 0.5);

    gl_FragColor = vec4(color,0.4);
}