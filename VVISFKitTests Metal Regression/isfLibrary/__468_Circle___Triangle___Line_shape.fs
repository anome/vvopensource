/*{
    "CATEGORIES": [
        "XXX"
    ],
    "CREDIT": "CZ",
    "DESCRIPTION": "",
    "INPUTS": [
        {
            "DEFAULT": 0.003,
            "LABEL": "line_size",
            "MAX": 1,
            "MIN": 0,
            "NAME": "line_size",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "MAX": 6.283185307179586,
            "MIN": -6.283185307179586,
            "NAME": "angle_in",
            "TYPE": "float"
        },
        {
            "DEFAULT": [
                1,
                1,
                1,
                1
            ],
            "LABEL": "color_input",
            "NAME": "color_input",
            "TYPE": "color"
        },
        {
            "DEFAULT": 0.003,
            "LABEL": "smooth_shape",
            "MAX": 1,
            "MIN": 0.001,
            "NAME": "smooth_shape",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0,
            "LABEL": "originput",
            "MAX": 1,
            "MIN": -1,
            "NAME": "originput",
            "TYPE": "float"
        },
        {
            "DEFAULT": 1e-06,
            "LABEL": "ring_thickness",
            "MAX": 0.1,
            "MIN": 0,
            "NAME": "ring_thickness",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "LABEL": "radiux",
            "MAX": 1,
            "MIN": 0,
            "NAME": "radiux",
            "TYPE": "float"
        },
        {
            "DEFAULT": 0.5,
            "LABEL": "xradiux",
            "MAX": 1,
            "MIN": -1,
            "NAME": "xradiux",
            "TYPE": "float"
        }
    ],
    "ISFVSN": "2",
    "VSN": null
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
//	float xindex = x++;

//rotate
	mat2 rotate (float angle){
	return mat2(
			-cos(angle_in), sin(angle_in),
			sin(angle_in), cos(angle_in));
	}

//Resolution Screen
	float res = (RENDERSIZE.x/RENDERSIZE.y);

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

//lineV2
float plot(vec2 uv, float pct){
  return  smoothstep( pct-line_size, pct, uv.y) -
          smoothstep( pct, pct+line_size, uv.y);
} 



//triangle
float triangle(vec2 p, float size) {
    vec2 q = abs(p);
    return max(q.x * 0.866025 + p.y * 0.5, -p.y * 0.5) - size * 0.5;
    //return size * 0.5-min(q.x * 0.866025 + p.y * 0.5, -p.y * 0.5) ;
}


void main()	{
	vec2 uv = (gl_FragCoord.xy - .5 * RENDERSIZE.xy ) / RENDERSIZE.y;
	uv = (rotate((TIME)) * uv);

	vec2 origin = vec2(originput);
	
	float radiuxdiv = radiux * xradiux;
	
	
	
	float d = ring(uv - vec2(origin), radiux, ring_thickness);
	//d = min(d, ring(uv - vec2((origin.x), origin.y), radiux/2., ring_thickness));//small circle
	//d = min(d, ring(uv - vec2((origin.x-radiux/2.), origin.y), radiux/2., ring_thickness));
	//d = min(d, ring(uv - vec2((origin.x+radiux/2.), origin.y), radiux/2., ring_thickness));
	//d = min(d, ring(uv - vec2((origin.x), origin.y-radiux/2.), radiux/2., ring_thickness));
	//d = min(d, ring(uv - vec2(origin.x, origin.y+radiux/2.), radiux/2., ring_thickness));
	
	d = min(d, ring(uv - vec2(origin.x, origin.y+radiux), xradiux/2., ring_thickness));
	d = min(d, ring(uv - vec2(origin.x+radiux*cos(PI/6.), origin.y-radiux*sin(PI/6.)), xradiux/2., ring_thickness));
	d = min(d, ring(uv - vec2(origin.x-radiux*cos(PI/6.), origin.y-radiux*sin(PI/6.)), xradiux/2., ring_thickness));
	
	
	//abscisse ordonnée 
	d = min(d, line(uv - vec2(origin), vec2(1., line_size-0.003)));
	d = min(d, line(uv - vec2(origin), vec2(line_size-0.003, 1.)));
	//d = min(d, line(uv - vec2(uv.x), vec2(1., line_size-0.003))); // 45°
	//d = min(d, line(uv - vec2(-uv.y), vec2(line_size-0.003, 1.))); // 45°
	
	
	 //triangle(uv - vec2(0.0, 0.0), 0.11);
	//d = min(d triangle(uv - vec2(0.0, 0.0), 0.10));
	
	float simpleline = 
		
	 	// plot(uv,(uv.x)) //45°
	 	//+plot(uv,(-uv.x))
	 	//+plot(uv,((PI/4.)*uv.x))
	 	
	 	//+plot(uv,((uv.x/(sqrt(2.)/2.))+(radiux/2.)))
	 	//+plot(uv,((-uv.x/(sqrt(2.)/2.))+(radiux/2.)))
	 	//+plot(uv,((uv.x/(sqrt(2.)/2.))-(radiux/2.)))
	 	//+plot(uv,(-uv.x/0.9)-radiux)
	 	plot(uv+sin(PI/6.)*radiux,0.)
	 	+plot(uv-vec2(0.,radiux),-(tan(PI/3.)*uv.x))
	 	+plot(uv+vec2(0.,-radiux),(tan(PI/3.)*uv.x))
	 	
	 	/*+plot(uv,((uv.x)+radiux/2.))
	 	+plot(uv,((uv.x)-radiux/2.))
	 	+plot(uv,((-uv.x)+radiux/2.))
	 	+plot(uv,((-uv.x)-radiux/2.))*/
	 	
	 	
		;

    
    
	vec3 colorline = simpleline*vec3(color_input);
	vec3 colorcircle =vec3(smoothedge(d))*vec3(color_input);
   
    vec3 shaperadd = colorcircle+colorline;

   gl_FragColor = vec4(shaperadd,1.);	
}
