/*
{
  "CATEGORIES" : [
    "XXX"
  ],
  "DESCRIPTION" : "",
  "INPUTS" : [
    {
      "MAX" : 6.2831853071795862,
      "NAME" : "angle_in",
      "TYPE" : "float",
      "DEFAULT" : 0,
      "MIN" : -6.2831853071795862
    },
    {
      "MAX" : 8,
      "NAME" : "grid_size",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "LABEL" : "grid_size",
      "MIN" : 0
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
        -1,
        -1
      ]
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
      "MAX" : 30,
      "NAME" : "shape_count",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "MIN" : 1
    },
    {
      "MAX" : 20,
      "NAME" : "zoom",
      "TYPE" : "float",
      "DEFAULT" : 1,
      "LABEL" : "zoom",
      "MIN" : 0
    },
    
    {
      "MAX" : 1,
      "NAME" : "radiux",
      "TYPE" : "float",
      "DEFAULT" : 0.5,
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

//rotate
	mat2 rotate (float angle){
	return mat2(
			-cos(angle_in), sin(angle_in),
			sin(angle_in), cos(angle_in));
}

//randomizer
float hash21(vec2 p) {
    p = fract(p*vec2(123.34, 456.21));
    p += dot(p, p+45.32);
    return fract(p.x*p.y);
}

float sdEquilateralTriangle( in vec2 p )
	{
	float size = .4;
    float k = sqrt(3.0);
    p.x = abs(p.x) - size;
    p.y = p.y + size/k;
    if( p.x+k*p.y>0.0 ) p=vec2(p.x-k*p.y,-k*p.x-p.y)/2.0;
    p.x -= clamp( p.x, -2.0, 0.0 );
    return -length(p)*sign(p.y);
	}
	
float udSegment( in vec2 p, in vec2 a, in vec2 b )
	{
    vec2 ba = b-a;
    vec2 pa = p-a;
    float h =clamp( dot(pa,ba)/dot(ba,ba), .0, 10.0 );//10. trait qui depasse
    return length(pa-h*ba);
	}

float sdPyramid( vec3 p, float h)
{
  float m2 = h*h + 0.25;
    
  p.xz = abs(p.xz);
  p.xz = (p.z>p.x) ? p.zx : p.xz;
  p.xz -= 0.5;

  vec3 q = vec3( p.z, h*p.y - 0.5*p.x, h*p.x + 0.5*p.y);
   
  float s = max(-q.x,0.0);
  float t = clamp( (q.y-0.5*p.z)/(m2+0.25), 0.0, 1.0 );
    
  float a = m2*(q.x+s)*(q.x+s) + q.y*q.y;
  float b = m2*(q.x+0.5*t)*(q.x+0.5*t) + (q.y-m2*t)*(q.y-m2*t);
    
  float d2 = min(q.y,-q.x*m2-q.y*0.5) > 0.0 ? 0.0 : min(a,b);
    
  return sqrt( (d2+q.z*q.z)/m2 ) * sign(max(q.z,-p.y));
}

float segment(vec2 p, vec2 p0, vec2 p1){
    vec2 a = p-p0; // the vector that we want to project on b
    vec2 b = p1-p0; // this line is our goal.
    
    vec2 proj = clamp((dot(a,b)/dot(b,b)),0.0,1.)*b; // vector projection of a onto b
    vec2 rejc = a-proj; // vector rejection or distance in other words
    
    return smoothstep(.0, 1., 1.0-dot(rejc, rejc)*5e4);
	}

float ring(vec2 p, float radius, float width) {
  	return abs(length(p) - radius ) - width;
	}
	
float ring2(vec2 uv, vec2 pos, float radius, float thick){
  return clamp((thick-abs(length(uv-pos) - radius))*100.0, 0.0, 1.0); 
	}

void main()	{
	vec2 uv = (gl_FragCoord.xy -.5 * RENDERSIZE.xy ) / RENDERSIZE.y ;
	uv *= zoom;
	
	//uv *= grid_size;
	//vec2 gv = fract(uv)-.5;
	//gv *= zoom;

	vec4 color = vec4(0.);
	vec4 colmod = vec4(vec3(0.5 + 0.5*cos(TIME+vec3(0,2,4))),1.);

		//iteration loop
		for (float y= -1.0; y<=1.; y++){
			for (float x=-1.0; x<=1.; x++){
			vec2 offs = vec2(x, y);
			uv = (rotate(TIME) * uv);
	

			
		float iteration = x+y;
		vec2 id = floor(uv);
		float ran = hash21(offs);
		
		float rantime = sin(TIME * .5 + ran * 6.2831)*.5+1.;
		vec2 origin = vec2(originput*ran);
		
		float rantimestep = step(0.2, rantime*TIME);
		
		float index = y+x/shape_count;

		//float funcTime = (tan(TIME));
		float funcTime = 1.;
		
		
		
		vec2 pointa = vec2(origin.x + cos(PI/2.)*rantime, origin.y + sin(PI/2.)*rantime)*radiux/index*rotate(TIME);
		vec2 pointb = vec2(origin.x - cos(5.*PI/6.)*rantime, origin.y - sin(5.*PI/6.)*rantime)*radiux/index*rotate(TIME);
		vec2 pointc = vec2(origin.x + cos(7.*PI/6.)*rantime, origin.y + sin(7.*PI/6.)*rantime)*radiux/index*rotate(TIME);
		
		color += vec4(segment(uv*funcTime, pointa, pointb));
		color += vec4(segment(uv*funcTime, pointb, pointc));
		color += vec4(segment(uv*funcTime, pointa, pointc));
		color += vec4(segment(uv*funcTime, origin, pointa));
		color += vec4(segment(uv*funcTime, origin, pointb));
		color += vec4(segment(uv*funcTime, origin, pointc));

  		//float intensity = ring2(uv*index, origin, radiux *funcTime, 0.01);
  		//if (intensity > 0.0)
    	//color = mix(color, color_input, intensity);
		

	}
	}

	gl_FragColor = vec4(color)*colmod;
}