/*{
	"DESCRIPTION": "",
	"CREDIT": "SilviaFabiani",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		
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
    _size = vec2(0.834) - _size*0.2;
    
    vec2 uv = smoothstep(_size,
                        _size+vec2(0.01),
                        _st);
    uv *= smoothstep(_size,
                    _size+vec2(0.03),
                    vec2(1.0)-_st);
    return uv.x*uv.y;
}

//float cross(in vec2 _st, float _size){
    //return  box(_st, vec2(_size,_size/4.)) +
            //box(_st, vec2(_size/4.,_size));
//}
float SHADOW (in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(sin (TIME *0.05)/2.);
	return smoothstep(_radius-(_radius*  0.01),           _radius+(_radius*0.2),
                         dot(dist,dist)*0.2);
}
float tempo = (TIME, 0.2, 7.5);
float circle(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-(_radius*(cos (tempo*4.))),           _radius+(_radius*0.01),
                         dot(dist,dist)*4.0);
}

float circle2(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-(_radius* (cos (TIME *0.0005))),           _radius+(_radius*0.5),
                         dot(dist,dist)*1.5);
}

float circle3(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return smoothstep(_radius-(_radius*(cos (TIME* 0.0005))),           _radius+(_radius*1.5),
                         dot(dist,dist)*1.5);
}


float circle4(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return smoothstep(_radius-(_radius* (cos (TIME*0.1))), _radius+(_radius*(cos (TIME * 0.9)/2.5)),
    dot(dist,dist)*0.5);
}
float circle5(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(cos (TIME));
	return 1.- smoothstep(_radius-(_radius* 0.01),           _radius+(_radius*0.01),
                         dot(dist,dist)*12.);
}
float circle_ext(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 0.5-smoothstep(_radius-(_radius* 0.1),           _radius+(_radius*0.1),
                         dot(dist,dist)*0.18);
}
float circle6(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(cos (tempo/2.));
	return 1.-smoothstep(_radius-(_radius* 0.01),           _radius+(_radius*10.),
                         dot(dist,dist)*(cos (TIME*2.)));
}



void main(){
    vec2 st = (gl_FragCoord.xy/RENDERSIZE.y);
vec3 color = vec3(0.0, 0.0, 0.0);

    // move space from the center to the vec2(0.0)
    st -= vec2(1.,0.5);
    // rotate the space
    st = rotate2d( (TIME)*PI/2. ) * st;
        // move it back to the original place
    st += vec2(0.5);

    // Show the coordinates of the space on the background
   color = vec3(st.x,st.y,0.0);

    // Add the shape on the foreground
    //color += vec3(cross(st,0.1));
    //color -= vec3(SHADOW(st,0.02));
    color -= vec3(circle(st,0.05));
    color -= vec3(circle2(st,0.08));
    color -= vec3(circle3(st,0.08));
    color += vec3(circle4(st,0.1));
    color += vec3(circle5(st,0.003));
    color += vec3(circle6(st,0.6));
    color -= vec3(circle_ext(st,0.05));
   color -= vec3(circle_ext(st,0.09));

   gl_FragColor = vec4(color,1.0);
   
	gl_FragColor.b *= 0.9 ;
	gl_FragColor.g *= 0.9 ;
	gl_FragColor.r *= 0. ;
}