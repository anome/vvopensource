/*
{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "based on SearchingForSomething",
  "CATEGORIES": [
    "Generator"
  ],
  "INPUTS": [
    {
      "MAX": [
        10,
        10
      ],
      "MIN": [
        -10,
        -10
      ],
      "DEFAULT":[-5.0,8.0],
      "NAME": "mouse",
      "TYPE": "point2D"
    },
    {
      "MAX": [
        1,
        1
      ],
      "MIN": [
        -1,
        -1
      ],
      "DEFAULT":[-0.1,1.0],
      "NAME": "phase",
      "TYPE": "point2D"
},
    {
      "MAX": 0.75,
      "MIN": 0.01,
      "DEFAULT":0.50,
      "NAME": "scale",
      "TYPE": "float"
},
    {
      "MAX": 0.5,
      "MIN": 0.001,
      "DEFAULT":0.1,
      "NAME": "zoom",
      "TYPE": "float"
},
    
     {
      "MAX": 1,
      "MIN": 0.25,
      "DEFAULT":0.5,
      "NAME": "brightness",
      "TYPE": "float"
    },
         {
      "MAX": 10,
      "MIN": 1,
      "DEFAULT":5,
      "NAME": "loops",
      "TYPE": "float"
    }
  ]
}
*/

// Something by mojovideotech


#ifdef GL_ES
precision highp float;
#endif

#define PI 3.14159265


void main()
{

    vec2 adder;
	adder.x = mouse.x;
	adder.y = mouse.y;
	vec2 p=(3.0*gl_FragCoord.yx-RENDERSIZE)/(RENDERSIZE.x,RENDERSIZE.y)+(adder*scale);
	
	vec2 newp=p;
	float d = length(newp - mouse);
	for(int i = 10; i > 5; i--){
	  float d = length(newp- mouse);
  	newp.x+=1.0/float(loops)*sin((1./zoom)/d+TIME*scale*float(i)*PI)-phase.x;		
  	newp.y+=1.0/float(i)*log2((1./zoom)/d+TIME*zoom*float(i)*PI)+phase.y;
	  p=newp;
	}


	vec3 col=vec3(0.75*sin(PI*p.x)+0.5,0.5*sin(PI*(p.x*p.y))+0.3,tan((p.x*p.y)/PI));
	vec3 lum=vec3(0.9,0.9,0.9);

	vec3 c=vec3(dot(col/brightness,col));
	gl_FragColor=vec4(col, brightness);
}