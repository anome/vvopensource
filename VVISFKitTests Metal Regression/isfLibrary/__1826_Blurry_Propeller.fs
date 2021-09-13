/*{
	"CREDIT": "by isaacwellishvj",
	"DESCRIPTION": "",
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
			"NAME": "spinRate",
			"TYPE": "float",
			"DEFAULT": 10.0,
			"MIN": -10.0,
			"MAX": 10.0
		},
		
			{
			"NAME": "backgroundNoise",
			"TYPE": "float",
			"DEFAULT": 1.0,
			"MIN": 0.0,
			"MAX": 10.0
		},
		
			{
			"NAME": "blur",
			"TYPE": "float",
			"DEFAULT": 0.01,
			"MIN": 0.0001,
			"MAX": 0.1
		},
		
	
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	]
}*/
	
	
	
	#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265359

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

float box(in vec2 _st, in vec2 _size){
    _size = vec2(0.5) - _size*0.5;
    vec2 uv = smoothstep(_size,
                        _size+vec2(blur),
                        _st);
    uv *= smoothstep(_size,
                    _size+vec2(blur),
                    vec2(1.0)-_st);
    return uv.x*uv.y;
}

float cross(in vec2 _st, float _size){
    return  box(_st, vec2(_size,_size/4.)) + 
            box(_st, vec2(_size/4.,_size));
}

void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
    vec3 color = vec3(0.5);
    
    // move space from the center to the vec2(0.0)
    st -= vec2(0.5);
    // rotate the space
    st = rotate2d( (TIME)*spinRate ) * st;
    // move it back to the original place
    st += vec2(0.5);

    // Show the coordinates of the space on the background
     color = vec3(-colorInput*(st.x*st.y));
     color += vec3(abs(cos(TIME*100.0)*spinRate*100.0)*sin(TIME),abs(sin(st.x*TIME*backgroundNoise)), st.y);
    
     //color += vec3(colorInput*abs(sin(TIME/10.0)));

    // Add the shape on the foreground
    color += vec3(cross(st,0.5));

    gl_FragColor = vec4(color,1.0);

}