/*
{
  "CATEGORIES": [
    "Automatically Converted"
  ],
  "INPUTS": [
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "DEFAULT":[0.5,0.5],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "DEFAULT":[0.5,0.5],
      "NAME": "phase",
      "TYPE": "point2D"
},
    {
      "MAX": 10,
      "MIN": 0.1,
      "DEFAULT":0.01,
      "NAME": "scale",
      "TYPE": "float"
},
     {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        0,
        0
      ],
      "DEFAULT":[0.5,0.5],
      "NAME": "brightness",
      "TYPE": "point2D"
    }
  ]
}
*/


#ifdef GL_ES
precision mediump float;
#endif

#define PI 3.14159265


void main()
{
	float timeAdjust = TIME*scale;
  vec2 adder;
	adder.x = mouse.x;
	adder.y = mouse.y;
	vec2 p=(2.0*gl_FragCoord.xy-RENDERSIZE)/max(RENDERSIZE.x,RENDERSIZE.y) + adder;
	
	vec2 newp=p;
	float d = length(newp - mouse);
	for(int i = 1; i < 35; i++){
  	newp.x+=.25/float(i)*sin((float(i)*p.y+timeAdjust/40.0+0.3*float(i)))+phase.x*d;		
  	newp.y+=.25/float(i)*sin(((float(i)*p.x+timeAdjust/1.0+0.3*float(i))))+phase.y*d;
	  p=newp;
	}


	vec3 col=vec3(0.5*sin(3.0*p.x)+0.5,0.5*sin(3.0*p.y)+0.5,sin(p.x+p.y));
	vec3 lum=vec3(0.299,0.587,0.114);

	vec3 c=vec3(dot(col*brightness.x,lum));
	gl_FragColor=vec4(c, brightness.y);
}