/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	{
      "NAME" : "Draw",
      "TYPE" : "float",
      "MAX" : 1.8,
      "DEFAULT" : 1.5,
      "MIN" :0.8
    },
    {
      "NAME" : "Diameter1",
      "TYPE" : "float",
      "MAX" : 5.0,
      "DEFAULT" : 3.0,
      "MIN" :2.0
    },
    {
      "NAME" : "RED",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" :0.0
    },
{
      "NAME" : "GREEN",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" :0.0
    },
    {
      "NAME" : "BLUE",
      "TYPE" : "float",
      "MAX" : 1.0,
      "DEFAULT" : 0.5,
      "MIN" :0.0
    }

	]
	
}*/

#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265359
#define TWO_PI 6.28318530718
  
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

float circle(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.000,0.000);
	return 1.-smoothstep(_radius-(_radius*0.034),
                         _radius+(_radius*0.01),
                         dot(dist,dist)*
                         4.656);
}
float circle2(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2(0.000,0.000);
	return smoothstep(_radius-(_radius*-0.646),
                         _radius+(_radius*0.066),
                         dot(dist,dist)*(sin (TIME*PI)*Diameter1)*2.);
}
void main(){
  vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
  st.x *= RENDERSIZE.x/RENDERSIZE.y;
  vec3 color = vec3(0.0);
    st -= vec2(1.0,0.5);
    st = rotate2d( (TIME)/PI/2. ) * st;
    st += vec2(0.5);
  float d = 0.0;
  st = st *2.-1.;
  int N = 12;
  float a = atan(st.x,st.y)+PI;
  float r = TWO_PI/float(N);
  d = cos(floor(1.5+a/r)*r-a)*length(st/Draw);
  color = vec3(smoothstep(0.5,0.5,d));
  color = vec3(d);
        color -= vec3(circle(st,4.));
        color += vec3(circle2(st,3.));
    color -= vec3(circle(st,2.368));
    color += vec3(circle2(st,1.576));
    color -= vec3(circle2(st,1.224));
    color += vec3(circle2(st,0.904)); 
    color -= vec3(circle(st,0.496));
    color += vec3(circle(st,0.376));
    color -= vec3(circle2(st,0.256));
    color += vec3(circle2(st,0.160));
    color += vec3(circle2(st,0.05));
    color -= vec3(circle(st,0.055));
  gl_FragColor =  (vec4(color,1.0));
  color = vec3(1.0-smoothstep(.45,.45,d));
 gl_FragColor.b *= BLUE ;
	gl_FragColor.g *= GREEN ;
	gl_FragColor.r *= RED ;   
}