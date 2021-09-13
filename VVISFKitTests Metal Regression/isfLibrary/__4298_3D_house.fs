
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "EDGE_WIDTH",
			"TYPE": "float",
			"DEFAULT": 0.05,
			"MIN": 0.001,
			"MAX": 0.5
		},
		{
		    "NAME": "BOOM",
			"TYPE": "float",
			"DEFAULT": 0.0,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

const float pi = 3.14159265359;

mat4 Q1 = mat4( 0.0, 0.0, 3.0, 0.0,  // 1. column, point 1: XYZ0
                1.0, 0.0, 3.0, 0.0,
                1.0, 1.0, 3.0, 0.0,
                0.0, 1.0, 3.0, 0.0);
mat4 Q2 = mat4( 0.0, 0.0, 3.0, 0.0,  // 1. column, point 1: XYZ0
                0.0, 0.0, 4.0, 0.0,
                0.0, 1.0, 4.0, 0.0,
                0.0, 1.0, 3.0, 0.0);
mat4 Q3 = mat4( 1.0, 0.0, 3.0, 0.0,  // 1. column, point 1: XYZ0
                1.0, 0.0, 4.0, 0.0,
                1.0, 1.0, 4.0, 0.0,
                1.0, 1.0, 3.0, 0.0);
mat4 Q4 = mat4( 0.0, 0.0, 4.0, 0.0,  // 1. column, point 1: XYZ0
                1.0, 0.0, 4.0, 0.0,
                1.0, 1.0, 4.0, 0.0,
                0.0, 1.0, 4.0, 0.0);

float focal;
float u0 = 0.5;
float v0 = 0.5;

vec3 euler_rot(vec3 ori, vec3 vec) {
    /*
    R =  [  cos(psi)*cos(theta)         -sin(psi)*cos(phi)+cos(psi)*sin(theta)*sin(phi)     sin(psi)*sin(phi)+cos(psi)*sin(theta)*cos(phi)  ;
            sin(psi)*cos(theta)         cos(psi)*cos(phi)+sin(psi)*sin(theta)*sin(phi)      -cos(psi)*sin(phi)+sin(psi)*sin(theta)*cos(phi) ;
            -sin(theta)                 cos(theta)*sin(phi)                                 cos(theta)*cos(phi)                             ];
    // Here, we compute R*[0;0;1], that's the third column
    */
    float theta = ori[0];
    float phi = ori[1];
    float psi = ori[2];
    float m11 = cos(psi)*cos(theta);
    float m21 = sin(psi)*cos(theta);
    float m31 = -sin(theta);
    float m12 = -sin(psi)*cos(phi)+cos(psi)*sin(theta)*sin(phi);
    float m22 = cos(psi)*cos(phi)+sin(psi)*sin(theta)*sin(phi);
    float m32 = cos(theta)*sin(phi);
    float m13 = sin(psi)*sin(phi)+cos(psi)*sin(theta)*cos(phi);
    float m23 = -cos(psi)*sin(phi)+sin(psi)*sin(theta)*cos(phi);
    float m33 = cos(theta)*cos(phi);
    
    float x = dot(vec3(m11,m12,m13), vec);
    float y = dot(vec3(m21,m22,m23), vec);
    float z = dot(vec3(m31,m32,m33), vec);
    
    return vec3(x, y, z);
}
vec2 project(vec3 p, float foc, vec2 c) {
    float fx = foc;
    float fy = foc * RENDERSIZE.x / RENDERSIZE.y;
    vec2 m = vec2(fx*p.x/p.z+c.x, fy*p.y/p.z+c.y);
    return m;
}

vec4 drawLine(vec2 p, vec2 p1, vec2 p2, float width_mod) {
    float l = length(p2-p1);
    vec2 u = (p2-p1) / l;
    vec2 v = vec2(-u.y, u.x);
    float x = dot(p-p1, u);
    float y = dot(p-p1, v);
    
    float width = EDGE_WIDTH * width_mod;
    float vx = smoothstep(-width, 0., x) * smoothstep(-width, 0., length(p2-p1)-x);
    float vy = smoothstep(-width, 0., -abs(y));
    float vn = length(vec2(vx,vy));
    float f = 1.0;//abs(cos(5. * 2.*pi * vn + 2.*pi*TIME));
    vec4 color = vec4(vec3(f*vx*vy),1.0);
    
    float wx = exp((pow(cos(2.*pi* x / l),2.)-1.) / 0.01);
    float wy = exp((pow(cos(2.*pi* y / l),2.)-1.) / 0.01);
    
    color += 0.1 * vec4(wx+wy,wx+wy,wx+wy,0.0);
    
    return color;
}

vec4 draw3DLine(vec2 pos, vec3 p1, vec3 p2, float width) {
    vec3 u = p2-p1;
    vec3 v = vec3(-u.y,u.x,0.);
    vec3 w = vec3(-u.x*u.z,-u.y*u.z,u.x*u.x+u.y*u.y);
    u = u / length(u);
    v = v / length(v);
    w = w / length(w);
    
    vec3 p1uvw = vec3(dot(p1,u),dot(p1,v),dot(p1,w));
    
    float fx = focal;
    float fy = focal * RENDERSIZE.x / RENDERSIZE.y;
    vec3 m = vec3((pos.x-u0) / fx, (pos.y-v0) / fy, 1.);
    vec3 muvw = vec3(dot(m,u),dot(m,v),dot(m,w));
    
    float Z = (muvw.y * p1uvw.y + muvw.z * p1uvw.z) / (muvw.y*muvw.y+muvw.z*muvw.z);
    float alpha = Z * muvw.x - p1uvw.x;
    
    float hu = dot(Z * m - p1, u);
    float hv = dot(Z * m - p1, v);
    float hw = dot(Z * m - p1, w);
    float hn = sqrt(hv*hv+hw*hw);
    
    float vx = smoothstep(-width, 0., hu) * smoothstep(-width, 0., length(p2-p1)-hu);
    float vy = smoothstep(-width, 0., -hn);
    float vn = length(vec2(vx,vy));
    float f = abs(cos(5. * 2.*pi * vn + 2.*pi*TIME));
    //vec4 color = vec4(vec3(f*vx*vy),1.0);
    
    float wu = exp(-(1. - pow(cos(hu),2.)) / 0.1);
    float wv = exp(-(1. - pow(cos(hv),2.)) / 0.1);
    float ww = exp(-(1. - pow(cos(hw),2.)) / 0.1);
    vec4 color = vec4(wu,wv,ww,1.0);
    
    return color;
}

void renderFace(vec2 pos, mat4 Q, inout vec4 color,
                vec3 campos, vec3 camori,
                mat4 effect
                ) {
    vec4 boom = effect[0];
    vec3 boom_center = boom.xyz;
    float boom_str = 1. + boom.w;
    
    // effects
    // boom effect
    vec3 p1 = euler_rot(camori, Q[0].xyz - campos);
    vec3 p2 = euler_rot(camori, Q[1].xyz - campos);
    vec3 p3 = euler_rot(camori, Q[2].xyz - campos);
    vec3 p4 = euler_rot(camori, Q[3].xyz - campos);
    
    p1 = boom_str * (p1- boom_center) + boom_center;
    p2 = boom_str * (p2- boom_center) + boom_center;
    p3 = boom_str * (p3- boom_center) + boom_center;
    p4 = boom_str * (p4- boom_center) + boom_center;
    
    vec4 edge_color;
    float width = EDGE_WIDTH;
    /*
    edge_color += draw3DLine(pos, p1, p2, width);
    edge_color += draw3DLine(pos, p2, p3, width);
    edge_color += draw3DLine(pos, p3, p4, width);
    edge_color += draw3DLine(pos, p4, p1, width);
    */
    vec2 uv0 = vec2(u0,v0);
    vec2 m1 = project(p1,focal,uv0);
    vec2 m2 = project(p2,focal,uv0);
    vec2 m3 = project(p3,focal,uv0);
    vec2 m4 = project(p4,focal,uv0);
    edge_color += drawLine(pos, m1, m2, width);
    edge_color += drawLine(pos, m2, m3, width);
    edge_color += drawLine(pos, m3, m4, width);
    edge_color += drawLine(pos, m4, m1, width);
    
    color = max(color, edge_color);
}

void main()	{
	vec4 inputPixelColor;
	vec2 pos = isf_FragNormCoord.xy;
	inputPixelColor = IMG_NORM_PIXEL(inputImage, pos);
    // set up the camera
    float fov = radians(90.);
    focal = 1. / tan(fov/2.);
	vec4 color = vec4(0.,0.,0., 1.);
	
	vec3 campos, camori;
	vec3 target = vec3(0.5, 0.5, 3.5);
	float ang = 0.1*TIME;
	campos = vec3(target.x - 3.5 * cos(ang), target.y, target.z - 3.5 * sin(ang));
	camori = vec3(ang + pi/2., 0.0, 0.0);
	mat4 effect;
	effect[0] = vec4(0.0, 0.0, 3.5, 1.*smoothstep(0., 0.1, mod(TIME,1.)));
	
	renderFace(pos, Q1, color, campos, camori, effect);
	renderFace(pos, Q2, color, campos, camori, effect);
    renderFace(pos, Q3, color, campos, camori, effect);
	renderFace(pos, Q4, color, campos, camori, effect);
	
	gl_FragColor = color;
}
