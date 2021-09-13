/*{
	"CREDIT": "by mojovideotech",
  "CATEGORIES" : [
    "2d",
    "fractal",
    "bump",
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION" : "Automatically converted from https://www.shadertoy.com/view/Xl3XWS by zackpudil.  Bump map 2.  Formula is an iterated box and sphere fold (like mandelbox).  Use the mouse to look around.\nUpdate: Added some edge lines for cheap AO",
  "INPUTS" : [
   	{
		"NAME" : 		"center",
		"TYPE" : 		"point2D",
		"DEFAULT" :		[ 0.0, 0.0 ],
		"MAX" : 		[ 3.14159, 3.14159 ],
     	"MIN" : 		[ -3.14159, -3.14159 ]
	},
    {
      	"NAME" : 		"c1",
      	"TYPE" : 		"color",
      	"DEFAULT" :		[ 0.3, 0.7, 0.2, 1.0 ]
    },
    {
      	"NAME" : 		"c2",
      	"TYPE" : 		"color",
      	"DEFAULT" :		[ 0.8, 0.2, 0.5, 1.0 ]
    },
    {
		"NAME" : 		"freq",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		0.1,
		"MAX" : 		2.0
	},
	{
		"NAME" : 		"zoom",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.5,
		"MIN" : 		0.5,
		"MAX" : 		5.0
	},
	{
		"NAME" : 		"rate",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.5,
		"MIN" : 		-1.0,
		"MAX" : 		1.0
	},
	 {
		"NAME" : 		"gamma",
		"TYPE" : 		"float",
		"DEFAULT" : 	1.5,
		"MIN" : 		0.5,
		"MAX" : 		3.0
	}
  ]
}
*/

float T = (TIME*rate)*0.1;

vec3 formula(vec2 p) {
	p*= zoom;
//	p += mouse;
	vec3 f = vec3(c1.rgb);
	for(int i = 0; i<5; i++) {
		p = 2.0*clamp(p, -0.5, 0.5) - p;
		p *= clamp(1.0/dot(p, p), 1.0, freq/0.01);
		float a = mix((cos(sin(-T) + freq)+T),pow(T,float(i)),T);
		p *= mat2(cos(a), -sin(a), -sin(a),cos(a));
		f = min(f, vec3(length(sin(p)), abs(p)));
	}
	return f;
}

vec3 bump(vec2 p, float e) {
	vec2 h = vec2(e, 0.0);
	mat3 m = mat3(
		formula(p + h) - formula(p - h),
		formula(p + h.yx) - formula(p - h.yx),
		-freq*c1.rgb);
	vec3 g = (c1.rgb*m)/e;
	
	return normalize(g);
}

float edge(vec2 p, float e) {
	vec2 h = vec2(e, 0.0);
	float d = dot(c2.rgb, formula(p));
	vec3 n1 = c2.rgb*mat3(formula(p + h.xy), formula(p + h.yx), vec3(0));
	vec3 n2 = c2.rgb*mat3(formula(p - h.xy), formula(p - h.yx), vec3(0));
	vec3 vv = abs(d - 0.5*(n1 + n2));
	float v = min(1.0, pow(vv.x+vv.y+vv.z, 0.45)*1.0);
	
	return v;
}

void main() {

//	vec2 p = (-RENDERSIZE.xy + gl_FragCoord.xy)/RENDERSIZE.y-0.5;
	vec2 p = vec2(gl_FragCoord.xy + RENDERSIZE.xy)/RENDERSIZE.y-0.5;
//	vec2 p = vec2(uv * 2.0 - 1.0);	
//	p.y -= 0.5;	
	
	float yy = radians(180.*p.y);
	float xz = radians(360.*p.x)+center.x;
    vec3 rd = vec3(sin(xz)*cos(yy), sin(yy), cos(xz)*cos(yy));	
//	vec3 rd = normalize(vec3(p, 1.0));
	rd = normalize(rd - vec3(0.0, center.y, 0.0));
	vec3 sn = bump(rd.yz, 0.2);
	vec3 re = reflect(rd, sn);
	vec3 col = c2.rgb * 0.5;
	col += formula(rd.xy)/vec3(fract(dot(-rd,sn)+T), length(rd.yz), 1.0);
//	col += pow(clamp(dot(-rd, re), 0.0, 1.0), 8.0)*(8.0*formula(rd.xy));
	col = pow(col, vec3(1.0/gamma));
    col *= edge(rd.xy, 0.5);
  // 	col -= 0.8*pow(clamp(1.0 + dot(rd, sn), 0.0, 1.0), 8.0);

	gl_FragColor = vec4(col, 1);
	
}