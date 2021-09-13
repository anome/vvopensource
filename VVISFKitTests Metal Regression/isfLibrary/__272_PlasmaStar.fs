/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "c1",
			"TYPE": "color",
			"DEFAULT": [
				0.7,
				0.1,
				0.2,
				1.0
			]
		},
		{
			"NAME": "c2",
			"TYPE": "color",
			"DEFAULT": [
				0.1,
				0.05,
				0.2,
				1.0
			]
		},
		{
			"NAME": 	"rate",
			"TYPE": 	"float",
			"DEFAULT": 	0.5,
			"MIN":		-3.0,
			"MAX": 		3.0
		},
		{
			"NAME": 	"density",
			"TYPE": 	"float",
			"DEFAULT": 	0.44,
			"MIN": 		0.005,
			"MAX": 		0.495
		},
		{
		"NAME" : 		"radius",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.73,
		"MIN" : 		0.01,
		"MAX" : 		2.0
		},
		{
		"NAME" : 		"rays",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.24,
		"MIN" : 		0.1,
		"MAX" : 		0.99
		},
		{
		"NAME" : 		"edge",
		"TYPE" : 		"float",
		"DEFAULT" : 	0.15,
		"MIN" : 		0.1,
		"MAX" : 		0.99
		},
		{
		"NAME" : 		"expand",
		"TYPE" : 		"float",
		"DEFAULT" : 	50.0,
		"MIN" : 		5.0,
		"MAX" : 		500.0
		},
		{
			"NAME": 	"offset",
			"TYPE": 	"point2D",
			"DEFAULT": 	[ 0.333, 0.1666 ],
			"MAX" : 	[ 0.335, 0.1668 ],
     		"MIN" : 	[ 0.331, 0.1665 ]
		}
	]
}*/

 
////////////////////////////////////////////////////////////
// PlasmaStar   by mojovideotech
//
// 3D noise from : 
// shadertoy.com/\XsX3zB  by Nikita Miropolskiy
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

vec3 random3(vec3 c) {
	float j = 4231.0*sin(dot(c,vec3(79.0, 919.0, 5.0)));
	vec3 r;
	r.z = fract(397.0*j);
   	j *= 0.5;
	r.x = fract(587.0*j);
	j *= 0.5;
	r.y = fract(773.0*j);
	return r-0.5;
}

float simplex3d(vec3 p) { 
	 vec3 s = floor(p + dot(p, vec3(offset.x)));
	 vec3 x = p - s + dot(s, vec3(offset.y));
	 vec3 e = step(vec3(0.0), x - x.yzx);
	 vec3 i1 = e*(1.0 - e.zxy);
	 vec3 i2 = 1.0 - e.zxy*(1.0 - e);
	 vec3 x1 = x - i1 + offset.y;
	 vec3 x2 = x - i2 + 2.0*offset.y;
	 vec3 x3 = x - 1.0 + 3.0*offset.y;
	 vec4 w, d;
	 w.x = dot(x, x);
	 w.y = dot(x1, x1);
	 w.z = dot(x2, x2);
	 w.w = dot(x3, x3);
	 w = max(0.6 - w, 0.0);
	 d.x = dot(random3(s), x);
	 d.y = dot(random3(s + i1), x1);
	 d.z = dot(random3(s + i2), x2);
	 d.w = dot(random3(s + 1.0), x3);
	 w *= w;
	 w *= w;
	 d *= w;
	 return dot(d, vec4(expand));
}


const mat3 rot1 = mat3(-0.37, 0.36, 0.85,-0.14,-0.93, 0.34,0.92, 0.01,0.4);
const mat3 rot2 = mat3(-0.55,-0.39, 0.74, 0.33,-0.91,-0.24,0.77, 0.12,0.63);
const mat3 rot3 = mat3(-0.71, 0.52,-0.47,-0.08,-0.72,-0.68,-0.7,-0.45,0.56);

float simplex3d_fractal(vec3 m) {
    return   0.5333333*simplex3d(m*rot1)
			+0.2666667*simplex3d(2.0*m*rot2)
			+0.1333333*simplex3d(4.0*m*rot3)
			+0.0666667*simplex3d(8.0*m);
}


#define 	pi 	3.141592653589793 // pi

void main() 
{
    float T = 33.3+rate*TIME;
    float bignessScale = 1.0/(0.5-density);
	vec2 p = gl_FragCoord.xy / RENDERSIZE.y;
    float aspect = RENDERSIZE.x/RENDERSIZE.y;
    vec2 positionFromCenter = p-vec2(0.5*aspect, 0.5);
    
    p = vec2(0.5*aspect, 0.5)+normalize(positionFromCenter)*min(length(positionFromCenter)+0.00, 0.05);
        
    // Noise:
    vec3 p3 = bignessScale*0.25*vec3(p.x, p.y, 0.0) + vec3(0.0, 0.0, T*0.025);
    float noise = simplex3d(p3*32.0);// simplex3d_fractal(p3*8.0+8.0);
	noise = 0.5 + 0.5*noise;
	
    float distanceFromCenter = clamp(length(positionFromCenter)/radius, 0.0, 1.0)*(noise);    
    
    float falloffMask = 2.0*distanceFromCenter-1.0;
    falloffMask = 1.0-pow(abs(falloffMask), 4.0);
    
    float thinnerMask = 2.0*distanceFromCenter-1.0;
    thinnerMask = pow(1.0-abs(thinnerMask), 4.0);
    float steppedValue = smoothstep(sin(rays),rays+0.1, noise*falloffMask);
    
    float finalValue = mix(steppedValue,falloffMask,1.0-edge);

    finalValue = smoothstep(sin(rays),rays+0.1, noise*finalValue);       
    
    gl_FragColor = c2;
    
    vec3 finalColor = fract(vec3(1.0-finalValue)*c1.rgb);    

	gl_FragColor += vec4(finalColor,1.0);
}