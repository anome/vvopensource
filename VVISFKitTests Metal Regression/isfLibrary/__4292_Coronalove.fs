
/*{
	"DESCRIPTION": "coronalove",
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
			"NAME": "proteines",
			"TYPE": "image"
		},
		{
            "NAME": "Nprotein",
            "TYPE": "float",
            "DEFAULT": 5,
            "MIN": 1,
            "MAX": 10
        },
        {
            "NAME": "sphere_radius",
            "TYPE": "float",
            "DEFAULT": 0.05,
            "MIN": 0.01,
            "MAX": 0.5
        },
        {
            "NAME": "protein_radius",
            "TYPE": "float",
            "DEFAULT": 0.015,
            "MIN": 0.01,
            "MAX": 0.05
        },
        {
            "NAME": "S2P_distance",
            "TYPE": "float",
            "DEFAULT": 5,
            "MIN": 0,
            "MAX": 1
        },
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		}
	],
	"PASSES": [
		{
			"TARGET": "oldBuffer",
			"PERSISTENT": true,
			"FLOAT": false
		}
	]
	
}*/

const float focal = 1.0;
const float pi = 3.14159265359;

const vec4 seed = vec4(12.9898,78.233, 45.666,   43758.5453123);

float rand2 (vec2 vec) {
    return fract(sin(dot(vec.xy, seed.xy)) * seed.w);
}

float rand3 (vec3 vec) {
    return fract(sin(dot(vec.xyz, seed.xyz)) * seed.w);
}

vec3 euler_rot_z(vec3 ori, vec3 vec) {
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

vec3 project(vec3 M, float radius) {
    return vec3(focal * M[0] / M[2], focal * M[1] / M[2], focal * radius / M[2]);
}

vec3 unproject(vec2 m, float Z) {
    return vec3(Z * m[0] / focal, Z * m[1] / focal, Z);
}

vec3 index2ori(float i) {
    float i_theta = mod(i, Nprotein);
    float i_phi = mod((i-i_theta) / Nprotein, Nprotein);
    float i_psi = (i-i_theta-i_phi*Nprotein) / (Nprotein * Nprotein);
    
    float theta = -pi + (2.*pi) * (i_theta / Nprotein);
    float phi = -pi + (2.*pi) * (i_phi / Nprotein);
    float psi = 0. + (pi) * (i_psi / Nprotein);
    
    return vec3(theta, phi, psi);
}

void main()	{
	vec4 inputPixelColor = IMG_NORM_PIXEL(inputImage, isf_FragNormCoord.xy);
	
	float sphere_Z = 0.5;
	vec3 protein;
	
	float theta = 2.*pi * TIME / 10.;
	vec2 sphere_pos = (pointInput + 0.25 * vec2(cos(theta), sin(theta)));
	
	vec3 sphere = unproject(sphere_pos, sphere_Z);
	vec3 sphere_ori = vec3(TIME / 10., TIME / 5., TIME / 20.);
	float dist_sphere = distance(sphere_pos.xy, isf_FragNormCoord.xy);
	float projected_sphere_radius = project(sphere, sphere_radius).z;
	
	vec4 bg_color = vec4(0.5, 0.5, 0.5, 0.0);
	vec4 sphere_color = inputPixelColor; //vec4(1.0, 0.0, 0.0, 1.0);
	vec4 protein_color = vec4(0.0, 1.0, 0.0, 1.0);
	float black_threshold = 0.1;
	vec4 color = bg_color;
	
	const float N = 1000.;
	float z = 100000.;
	
	for(float i = 0.; i < N; i++) {
	    if (i < (Nprotein*Nprotein*Nprotein)) {
	        vec3 ori = index2ori(i);
	        vec3 ori_noise = ori + rand3(ori) / 50.;
	        vec3 protein_ori = euler_rot_z(sphere_ori, euler_rot_z(ori_noise, vec3(0.0,0.0,1.0))).xyz;
	        protein = sphere.xyz + sphere_radius * (1. + S2P_distance) * protein_ori;
	        
	        vec3 m_protein = project(protein, protein_radius);
	        float dist_protein = distance(m_protein.xy, isf_FragNormCoord.xy);
	        float projected_protein_radius = m_protein.z;
	        
	        if (sphere.z < z) {
	            if (dist_sphere < projected_sphere_radius) {
	                z = sphere.z;
	                color.xyz = sphere_color.xyz;
	                color.w = 1.0;
	            }
	        }
	        
	        if (protein.z < z) {
	            if (dist_protein < m_protein.z) {
	                
	                vec2 tex= vec2(0.5, 0.5) - ( 1.0 * (m_protein.xy - isf_FragNormCoord.xy) / projected_protein_radius);
	                vec4 temp_color = IMG_NORM_PIXEL(proteines, tex);
	                float black = length(temp_color.xyz);
	                
	                if (black > black_threshold) {
	                    color = temp_color;
	                    color.w = black;
	                    z = protein.z;
	                }
	                
	                
	            }
	        }
	        
	    }
	}
	
	// motion blur
	vec2 center = vec2(0.5, 0.5);
	vec2 oldpos = isf_FragNormCoord.xy;
	
	float omega = 2.*pi * cos(2.*pi * TIME / 100.) / 100.;
	oldpos = oldpos - center;
	oldpos = (1.+ 10. * omega) * vec2(  dot(vec2(cos(omega), sin(omega)), oldpos),
	                dot(vec2(-sin(omega), cos(omega)), oldpos)) + center;
	
	vec4 oldPixel = IMG_NORM_PIXEL(oldBuffer, oldpos);
	oldPixel = 1.01 * oldPixel;
	
	gl_FragColor = mix(color, oldPixel, 0.50);
}
