/*{
	"DESCRIPTION": "Solaris",
	"CREDIT": "Silvia Fabiani",
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

//mat2 scale(vec2 _scale){
    //return mat2(_scale.x,0.0,
               // 0.0,_scale.y);
//}

float box(in vec2 _st, in vec2 _size){
    _size = vec2(0.5) - _size*0.5;
    vec2 uv = smoothstep(_size,
                        _size+vec2(0.001),
                        _st);
    uv *= smoothstep(_size,
                    _size+vec2(0.001),
                    vec2(1.0)-_st);
    return uv.x*uv.y;
}

//flower?

float circle(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-
	//lignes suivante: flou du contour (en augmentant les valeurs numériques, ça devient de plus en plus flou)
	(_radius*0.8),
                         _radius+(_radius*0.6),
 //ligne suivante: diamètre du cercle (en augmentant la valeur, le cercle rapetisse)
                         dot(dist,dist)* sin (TIME * 0.9));
}
float circle2(in vec2 _st, in float _radius){
    //ligne suivante: position du cercle dans l'espace
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-(_radius*0.4),
                         _radius+(_radius*0.2),
                         dot(dist,dist)* sin (TIME * 1.2));
}
float circle3(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-(_radius*0.01),
                         _radius+(_radius*sin (TIME * 0.5)),
                         dot(dist,dist)* sin (TIME * 1.9));
}
float circle4(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return smoothstep(_radius-(_radius*0.5),
                         _radius+(_radius*0.5),
                         dot(dist,dist)* cos (TIME * PI));}
float circle5(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return 1.-smoothstep(_radius-(_radius*0.01),
                         _radius+(_radius*0.01),
                         dot(dist,dist)* cos (TIME * 5.));
}
float circle6(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return smoothstep(_radius-(_radius*sin (TIME * 0.5)),
                         _radius+(_radius*0.01),
                          dot(dist,dist)* sin (TIME * 0.5));
}

void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE.y;
    vec3 color = vec3(0.0);

    st -= vec2(0.5,0.5);
    //st = scale( vec2(sin(TIME)*PI*2.) ) * st;
    st += vec2 ((sin (TIME) /5.), 0.5);

    
    // Add the shape on the foreground
    color += vec3(circle(st,0.07));
    color -= vec3(circle2(st,0.08));
    color += vec3(circle3(st,0.09));
    color -= vec3(circle4(st,0.08));
    color += vec3(circle5(st,0.1));
    color -= vec3(circle6(st,0.05));

    gl_FragColor = vec4(color,1.0);
   
	gl_FragColor.b *= 1. ;
	gl_FragColor.g *= 0.8 ;
	gl_FragColor.r *= 0. ;
}