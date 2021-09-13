/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "MAX" : 1,
      "NAME" : "line_size",
      "TYPE" : "float",
      "DEFAULT" : 0.0030000000000000001,
      "LABEL" : "line_size",
      "MIN" : 0
    },
    {
      "MAX" : 6.2831853071795862,
      "NAME" : "angle_in",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "MIN" : -6.2831853071795862
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
      "MAX" : 1,
      "NAME" : "smooth_shape",
      "TYPE" : "float",
      "DEFAULT" : 0.0030000000000000001,
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
      "LABEL" : "originput",
      "MIN" : [
        0,
        0
      ]
    },
    {
      "MAX" : 0.10000000000000001,
      "NAME" : "ring_thickness",
      "TYPE" : "float",
      "DEFAULT" : 9.9999999999999995e-07,
      "LABEL" : "ring_thickness",
      "MIN" : 0
    },
    {
      "MAX" : 1,
      "NAME" : "radiux",
      "TYPE" : "float",
      "DEFAULT" : 0.20000000000000001,
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

//rotate
	mat2 rotate (float angle){
	return mat2(
			-cos(angle_in), sin(angle_in),
			sin(angle_in), cos(angle_in));
	}

//Shapes
//ring
	float ring(vec2 p, float radius, float width) {
  	return abs(length(p) - radius ) - width;
	}
	
//line
float line(vec2 p, vec2 size) {  
  vec2 d = abs(p) - size;
  return min(max(d.x, d.y), 0.0) + length(max(d,.0));
}

void main()	{
	vec2 screenspace = isf_FragNormCoord;
	screenspace -= .5;
	
		if (RENDERSIZE.x < RENDERSIZE.y){
		screenspace.y *= (RENDERSIZE.y/RENDERSIZE.x);
		}
		else {
		screenspace.x *= (RENDERSIZE.x/RENDERSIZE.y);
		}
		
	screenspace = (rotate((TIME)) * screenspace);

	float d = 1.;
	
	//abscisse ordonnée 
	d = min(d, line(screenspace - vec2(originput), vec2(1., line_size-0.003)));
	d = min(d,  line(screenspace - vec2(originput), vec2(line_size-0.003, 1.)));
	
	//ring
	d = min(d, ring(screenspace - vec2(originput), 2.*radiux*cos(PI/6.), ring_thickness));
	vec4 colorcircle =vec4(smoothedge(d))*vec4(color_input);
   
    vec4 shaperadd = colorcircle;

   gl_FragColor = vec4(shaperadd);	
}