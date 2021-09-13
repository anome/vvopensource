/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
   
    {
      "NAME": "shape_line",
      "TYPE": "float",
      "DEFAULT": 0.98,
      "MIN": 0,
      "MAX": 1
    },
        {
      "MAX" : 6.2831853071795862,
      "NAME" : "angle_in",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "MIN" : -6.2831853071795862
    },
    {
      "MAX" : 10,
      "NAME" : "shape_count",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "MIN" : 1
    },
    {
      "MAX" : 10,
      "NAME" : "time_length",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "MIN" : 0.10000000000000001
    },
    {
      "NAME" : "color_input",
      "TYPE" : "color",
      "DEFAULT" : [
        1,
        1,
        1,
        1
      ],
      "LABEL" : "color_input"
    },
    {
      "MAX" : 10,
      "NAME" : "smooth_shape",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "LABEL" : "smooth_shape",
      "MIN" : 0.001
    },
    {
      "NAME" : "originput",
      "TYPE" : "point2D",
      "MAX" : [
        1,
        1
      ],
      "DEFAULT" : [
        0,
        0
      ],
      "LABEL" : "originput",
      "MIN" : [
        0,
        0
      ]
    },
    
    {
      "MAX" : 1,
      "NAME" : "slide",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "LABEL" : "slide",
      "MIN" : -1
    },
    {
      "MAX" : 1,
      "NAME" : "radiux",
      "TYPE" : "float",
      "DEFAULT" : 0.20000000298023224,
      "LABEL" : "radiux",
      "MIN" : 0
    }
  ],
  "ISFVSN" : "2",
  "VSN" : null,
  "CREDIT" : "CZ"
}
*/

//define value
#define PI 3.141592653589793
#define TAU PI * 2.
#define OR 1.61803398875

//smooth
	float smoothedge(float v) {
    return smoothstep(smooth_shape, 0. / RENDERSIZE.x, v);
	}

//iteration
	float x = 0.01 ;
	float xindex = x++;

//rotate
	mat2 rotate (float angle){
	return mat2(
			-cos(angle_in), sin(angle_in),
			sin(angle_in), cos(angle_in));
	}

//ring
	float ring(vec2 p, float radius, float width) {
  	return abs(length(p) - radius ) - width;
	}


//Shape
	vec4 shape(vec2 st, int N, float size){
	float a = atan(abs(st.x),mod(st.y,0.))+PI;
	float r = TAU/float(N);
	float dshape = cos(floor(0.5 + a/r) * r - a)*length(st*size);
	return vec4(vec3( step(radiux*shape_line,dshape) * step(dshape,radiux)),1.0);
	}
  
void main()	{

	vec2 screenspace = isf_FragNormCoord;
	screenspace -= .5 ;
	screenspace.x *= (RENDERSIZE.x/RENDERSIZE.y);
	screenspace = (rotate((TIME)) * screenspace);
	
	vec4 color = vec4(0.);
		
		//iteration loop
		for (float i=1.0; i<=shape_count; i++){
		float iteration = i;
		float index = i/shape_count;
		float osc = ((iteration+(mod(TIME, time_length)))*.5)+.5;	

		//Triangle
		vec4 t1 = shape (screenspace, 3,1.);
		
		//Square
		vec4 t2 = shape (screenspace, 4, 1.42);	
		
		//Ring Inside
		vec4 t3 = shape (screenspace, 50, 1.);
		
		//Ring Outside
		vec4 t4 = shape (screenspace, 50, .5);

		
		color += vec4(t1+t2+t3+t4)*vec4(color_input);
		//color += vec4(smoothedge(t3));

	}
	gl_FragColor = vec4(color);	
}
