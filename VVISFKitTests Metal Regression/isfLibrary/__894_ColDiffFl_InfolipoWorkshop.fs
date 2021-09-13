/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "from http://glslsandbox.com/e#35553.0",
  "CATEGORIES": [
    "fluid",
    "liquid"
  ],
  "INPUTS": [
  
	{
			"NAME" :	"color1",
			"TYPE" :	"float",
			"DEFAULT" :	0.0,
			"MIN" :	-2.5,
			"MAX" :	2.5
	},
	{
			"NAME" :	"color2",
			"TYPE" :	"float",
			"DEFAULT" :	0.0,
			"MIN" :	-1.25,
			"MAX" :	1.125
	},
	{
			"NAME" :	"cycle1",
			"TYPE" :	"float",
			"DEFAULT" :	0.0,
			"MIN" :	0.00,
			"MAX" :	3.1459
	},
	{
			"NAME" :	"cycle2",
			"TYPE" :	"float",
			"DEFAULT" :	0.0,
			"MIN" :	-0.497,
			"MAX" :	0.497
	},
	
    	{
      			"NAME" :	"direction",
      			"TYPE" :	"float",
      			"DEFAULT" :	0.0,
      			"MIN" : 	-0.1,
      			"MAX" :		0.1
    	}
  ]
}*/

///////////////////////////////////////////
// Based on ColorDiffusionFlow  by mojovideotech
//simplified version with cos algorithm by Silvia
// based on :
// glslsandbox.com/\e#35553.0
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
///////////////////////////////////////////


#ifdef GL_ES
precision mediump float;
#endif
 
#define 	pi   	3.141592653589793 	// pi

void main() {
	
		
	//IN FOLLOWING LINE REPLACING "-" BY "+" CHANGES DIRECTION OF ANIMATION. INCREASING NUMERIC VALUE SPEEDS UP ANIMATION
	//float T = TIME * -1.0;
	//	float T = TIME * 1.0;
		float T = TIME * direction;
		//float T = TIME * -0.1;
	vec2 p=(2.*isf_FragNormCoord);
	for(int i=1;i<11;i++) {
    	vec2 newp=p;
		float ii = float(i); 
		
	
		
			//FOLLOWING LINE CAN BE ACTIVATED ALONE.  
			//IT MUST BE ACTIVATED TOGETHER WITH THE FOLLOWING TO OBTAIN THE FLOW EFFECT
    newp.x+=(cos(TIME*-0.05))/ii*sin(ii*pi*p.y+T*0.9+cos((T/(5.0*ii))*ii));
    
    	
    	//FOLLOWING LINE CAN BE ACTIVATED ALONE 
    newp.y+=(cos(TIME*0.01))/ii*cos(ii*pi*p.x+T+0.5+sin((T/(5.0*ii))*ii));
    	
    	//FOLLOWING LINE MUST BE ACTIVATED TOGETHER WITH PREVIOUS 
    //  newp.y+=depthY/ii*cos(ii*pi*p.x+T+0.05+sin((T/(5.0*ii))*ii));
  

//THE 4 FOLLOWING LINES MAKE COLOR STRIPES MIX. ONE OF THESE LINES MUST BE ACTIVATED TO OBTAIN FLOW EFFECT

       p=newp+log(DATE.w)/40.0;
    	//p=newp+sqrt(DATE.w)/40.0;
    	
    	//FOLLOWING LINE GENERATES THE MOST PLEASANT COMBINATIONS
    	p=newp+fract(DATE.w)/40.0;
    	
    	//p=newp+sin(DATE.w)/40.0;
  }
  
  //vec3= Set OF 3 INFOS SEPARATED BY A COMMA. USE COLOR1-COLOR2 SLIDERS TO SWITCH BETWEEN COLORS
  
  //SET 1: 
  
  //vec3 col=vec3(cos(p.x+p.y+3.0
  //)*(cos(TIME*1.1)),
 //),
 //*0.5+0.5),
//sin(p.x+p.y+6.0),
  // (sin(p.x+p.y+9.0*color2)));
   
   
//SET 2:

   vec3 col=vec3
   (cos(p.x+p.y+3.0*color1)*0.5+0.5
   //,
  
  *(cos(TIME*0.05)),
sin(p.x+p.y+6.0),
(sin(p.x+p.y+9.0*color2)))*(0.5+0.5);
  //sin(p.x+p.y+6.0*cycle1)*0.5+0.5,
  //(sin(p.x+p.y+9.0));
  //*(cos(TIME*0.09))
  //)

 // +cos(p.x+p.y+12.0
 // *(cos(TIME*2.5))
 // ) 
 //)
 //*0.25+.5
 // )
  //;
//+cos(p.x+p.y+12.0*cycle2))*0.25+.5);


	//FOLLOWING LINE GENERATES BLURRED COLOR STRIPES.
  gl_FragColor=vec4(col*col, 1.0);
}


