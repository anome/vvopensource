/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "ISFVSN" : "2",
  "INPUTS" : [
    {
      "NAME" : "Linewidth",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.835,
      "MIN" : 0.001
    },
    {
      "NAME" : "Edge",
      "TYPE" : "float",
      "MAX" : 2,
      "DEFAULT" : 1,
      "MIN" : 0.001
    },
    {
      "NAME" : "Speed",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 1.2,
      "MIN" : 0.3
    },
    {
      "NAME" : "Radiant1",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.2,
      "MIN" : 0.001
    },
    {
      "NAME" : "Radiant2",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 0.7,
      "MIN" : 0.05
    },
    {
      "NAME" : "Radiant3",
      "TYPE" : "float",
      "MAX" : 4,
      "DEFAULT" : 0.08,
      "MIN" : 0.05
    },
    {
      "NAME" : "Radiant4",
      "TYPE" : "float",
      "MAX" : 0.05,
      "DEFAULT" : 0.002,
      "MIN" : 0.001
    },
    {
      "NAME" : "r",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.5,
      "MIN" : 0
    },
    {
      "NAME" : "g",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.3,
      "MIN" : 0
    },
    {
      "NAME" : "b",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.2,
      "MIN" : 0
    },
    {
      "NAME" : "invert",
      "TYPE" : "bool"
    }
  ],
  "CREDIT" : ""
}
*/

#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265359

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

float box(in vec2 _st, in vec2 _size){
    _size = vec2(Linewidth) - _size*0.9;
    
    vec2 uv = smoothstep(_size,
                        _size+vec2(0.001),
                        _st);
    uv *= smoothstep(_size,
                    _size+vec2(0.0003),
                    vec2(1.0)-_st);
    return uv.x*uv.y;
}


float cross(in vec2 _st, float _size){
    return  box(_st, vec2(_size,_size/4.)) +
            box(_st, vec2(_size/4.,_size));
}
float circle(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5);
	return (sin (TIME / 0.9 ))+ smoothstep(_radius-(_radius*-Edge),
                         _radius+(_radius*(sin (TIME / Radiant1))),
                         dot(dist,dist)*3.920);
                         }
float circle2(in vec2 _st, in float _radius){
   vec2 dist = _st-vec2(0.5,0.5);
	return (cos (TIME))-smoothstep(_radius-(_radius-Radiant2),
                        _radius+(_radius+Radiant2),
                         dot(dist,dist)*3.664);
}
float circle3(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.5,0.5);
	return 1.-smoothstep(_radius-(_radius*Radiant3),
                         _radius+(_radius+Radiant3),
                        dot(dist,dist)*3.2);
}
float circle4(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(sin (TIME * Speed));
	return  1.-smoothstep(_radius-(_radius-Radiant4),
                         _radius+(_radius-Radiant4),
                         dot(dist,dist)*3.920);
                         }
                         float circle5(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(sin (TIME * Speed));
	return  smoothstep(_radius-(_radius-Radiant4),
                         _radius+(_radius-Radiant4),
                         dot(dist,dist)*(cos (TIME *8.)));
                         }






void main(){
    vec2 st = (gl_FragCoord.xy/RENDERSIZE.y);
    vec3 color = vec3(r);

    // move space from the center to the vec2(0.0)
    st -= vec2(1., 0.5);
    // rotate the space
    st = rotate2d( ((TIME)*PI)/4. ) * st;
    // move it back to the original place
    st += vec2(0.5);

    // Add the shape on the foreground
    color -= vec3(cross(st,1.5));
    color += vec3(circle(st,Radiant1));
    color += vec3(circle2(st,Radiant2));
    color -= vec3(circle3(st,Radiant3));
    color += vec3(circle4(st,Radiant4));
    color -= vec3(circle5(st,Radiant4));

    
    gl_FragColor = vec4(color,1.0);
   
	gl_FragColor.b *= b ;
	gl_FragColor.g *= g ;
	gl_FragColor.r *= r ;
}