/*{
	"CREDIT": "by glawapple",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"generator"
	],
	"INPUTS": [
		    {
      "NAME": "blur",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0.0,
      "MAX": 1.5
    },
    {
      "NAME": "count",
      "TYPE": "float",
      "DEFAULT": 1.5,
      "MIN": 0,
      "MAX": 36.0
    },
    {
      "NAME": "angle",
      "TYPE": "float",
      "DEFAULT": 0,
      "MIN": 0,
      "MAX": 360
    },
        {
      "NAME": "pos",
      "TYPE": "point2D",
      "DEFAULT": [
        0.5,
        0.5
      ],
      "MIN": [
        0,
        0
      ],
      "MAX": [
        1,
        1
      ]
    }
	]
}*/

#define PI 3.14159265359
#define TWO_PI 6.28318530718

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}



void main(){
    vec2 st = gl_FragCoord.xy/RENDERSIZE.xy;
     st -= vec2(pos);
    vec3 color = vec3(0.0);
    st = rotate2d(angle*0.01745) * st;

   vec2 pos = vec2(0.0)-st;

    float r = length(pos);
    float a = atan(pos.y,pos.x);

    float f = cos(a*count);
    // f = abs(cos(a*3.));
    // f = abs(cos(a*2.5))*.5+.3;
    // f = abs(cos(a*12.)*sin(a*3.))*.8+.1;
    // f = smoothstep(-.5,1., cos(a*10.))*0.2+0.5;

   // color = vec3( 1.-smoothstep(f,f+0.02,r) );

  gl_FragColor = vec4(vec3(tan(f*blur)),1.0);
}