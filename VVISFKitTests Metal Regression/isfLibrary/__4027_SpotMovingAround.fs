/*{
	"CREDIT": "by sf",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
	  
   {
      "NAME": "X",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
		{
      "NAME": "Y",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.0,
      "MAX": 1.0
    },
     
   {
      "NAME": "sizeX",
      "TYPE": "float",
      "DEFAULT": -1.75,
      "MIN": -2.0,
      "MAX": -1.4
    },
		{
      "NAME": "sizeY",
      "TYPE": "float",
      "DEFAULT": -0.98,
      "MIN": -1.22,
      "MAX": -0.78
    },
    {
      "NAME": "Blur",
      "TYPE": "float",
      "DEFAULT": 2.0,
      "MIN": 1.19,
      "MAX": 2.0
    }
    ]
}*/
// Author @patriciogv - 2015
// http://patriciogonzalezvivo.com

#ifdef GL_ES
precision mediump float;
#endif
//float tempo = clamp (TIME, 0.0, 6.0);
float tempo = sin (TIME*2.0); //fréquence du mouvement pérodique

mat2 scale(vec2 _scale){
    return mat2(_scale.x,0.0,
                0.0,_scale.y);
}



float circle(in vec2 _st, in float _radius){
    vec2 dist = _st-vec2((cos (tempo/X/2.0)),Y); //amplitude du mouvement
   
return 1.-smoothstep(_radius-(_radius*(Blur+tempo-0.2)),

    
	//return 1.-smoothstep(_radius-(_radius*(sin (Blur*TIME))),
//	return 1.-smoothstep(_radius-(_radius*Blur),
                         _radius+(_radius*0.01),
                         dot(dist,dist)*4.0);
}

void main(){
	vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;


st -= vec2(0.5);
    st = scale( vec2(sizeX,sizeY)) * st;
    st += vec2(0.5);
    vec3 color = vec3(circle(st,0.5));



	gl_FragColor = vec4( color, 1.0 );
}

