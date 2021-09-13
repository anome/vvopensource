// SaturdayShader Week 43 : Albers Squares
// by Joseph Fiola (http://www.joefiola.com)
// 2018-06-23

//Based on Patricio Gonzalez Vivo's Book of Shaders chapter on color https://thebookofshaders.com/06/
//Notably his "Interaction of color" examples - https://thebookofshaders.com/edit.php?log=160509131509


/*{
	"CREDIT": "",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS" : [
    {
      "NAME" : "rotateCanvas",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0,
      "MIN" : 0
    },
    {
      "NAME" : "pos",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0.5,
        0.5
      ],
      "MIN" : [
        0,
        0
      ]
    },
    {
    	"NAME" : "bars",
    	"TYPE" : "float",
    	"MAX" : 5.01,
    	"DEFAULT" : 4,
    	"LABEL" : "bars",
    	"MIN" : 0
    },
    {
      "NAME" : "width",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.2,
      "LABEL" : "width",
      "MIN" : 0
    },
    {
      "NAME" : "height",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.4,
      "LABEL" : "height",
      "MIN" : 0
    },
    {
      "NAME" : "offset_x",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.2,
      "LABEL" : "offset x",
      "MIN" : 0
    },
    {
      "NAME" : "offset_y",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.125,
      "LABEL" : "offset y",
      "MIN" : 0
    },
    {
      "NAME" : "checker",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.7,
      "LABEL" : "checker",
      "MIN" : 0
    },
    {
      "NAME" : "rotate_bars",
      "TYPE" : "float",
      "MAX" : 1,
      "DEFAULT" : 0.25,
      "LABEL" : "rotate bars",
      "MIN" : -1
    },
    {
      "NAME" : "color_0",
      "TYPE" : "color",
      "DEFAULT" : [
        1.0,
        1.0,
        1.0,
        1
      ],
      "LABEL" : "color 0"
    },
    {
      "NAME" : "color_1",
      "TYPE" : "color",
      "DEFAULT" : [
        0.5,
        0.5,
        0.5,
        0
      ],
      "LABEL" : "color 1"
    },
    {
      "NAME" : "color_2",
      "TYPE" : "color",
      "DEFAULT" : [
        0,
        0,
        0,
        1
      ],
      "LABEL" : "color 2"
    }
  ],
  "ISFVSN" : "2",
  "CREDIT" : ""
}
*/


#define PI 3.14159265359
#define TWO_PI 6.28318530718

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

float rect(in vec2 st, in vec2 size, in float checker){
	size = 0.25-size*0.125;
    vec2 uv = step(size,st*(1.0-st));
    uv = rotate2d(checker) * uv;
    
	return uv.x*uv.y;
}

void main()
{
	vec2 st = gl_FragCoord.xy / RENDERSIZE.xy;
	st -= vec2(pos);
	st.x *= RENDERSIZE.x/RENDERSIZE.y;
	st *= 2.0; // Scale the coordinate system
	st = rotate2d(rotateCanvas*-TWO_PI) * st;
	
	vec2 st1 = st;
	vec2 st2 = st;	
	
	vec4 influenced_color = color_0;
    vec4 influencing_color_A = color_1;
    vec4 influencing_color_B = color_2;
    
    vec4 color = vec4(0.);
    
    // Background Gradient
    color = mix( influencing_color_A,
                 influencing_color_B,
                 st.y);
                 
    // position of bars


    // Foreground rectangle
    vec2 size = vec2(width,height);
    vec2 offset = vec2(offset_x,offset_y);
    float checker = checker * PI;
    //float rotate_bars = 0.04;
    
    st = rotate2d(rotate_bars * -PI) * st + 0.5;
    color = mix(color,
               influenced_color,
               rect(st,size, checker));
               
    
    for (float i = 0.; i < 6.; i++){
    	if (i >= bars) break;
    	st = st1; 
    	st += offset * (i+1.0);
    	st = rotate2d(rotate_bars * -PI) * st + 0.5;
    	color = mix(color,
               influenced_color,
               rect(st,size, checker));
               
        st = st1; 
        st -= offset * (i+1.0);
    	st = rotate2d(rotate_bars * -PI) * st + 0.5;
    	color = mix(color,
               influenced_color,
               rect(st,size, checker));
    	
    }
               
    
		
	gl_FragColor = vec4(color);
}		

