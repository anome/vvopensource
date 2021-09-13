/*{
	"CREDIT": "by You",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [
		{
			"NAME": "dscale",
			"TYPE": "float",
			"MIN": 0,
			"MAX":0.5
		},
		{
			"NAME": "dthresh",
			"TYPE": "float",
			"MIN": 5,
			"MAX":10
		},
		{
			"NAME": "drad",
			"TYPE": "float",
			"MIN": 0,
			"MAX":0.5
		},
		{
			"NAME": "dhue",
			"TYPE": "float",
			"MIN": 0,
			"MAX":3.14
		},
		{
			"NAME": "speed",
			"TYPE": "float",
			"MIN": 0,
			"MAX":100
		},
		{
			"NAME": "rotation",
			"TYPE": "float",
			"MIN": 0,
			"MAX":100
		}
	]
}*/

#define MAX_ITER 35

#define MINRAD2 .25
float SCALE = 2.8 + dscale;
float THRESH =  1. / exp(dthresh);
float minRad2 = clamp(MINRAD2, 1.0e-9, 1.0) + drad;
vec4 scale = vec4(SCALE, SCALE, SCALE, abs(SCALE)) / minRad2;


//----------------------------------------------------------------------------------------
float Map(vec3 pos) 
{
//	return (length(pos)-4.0);

	vec4 p = vec4(pos,1);
	vec4 p0 = p;  // p.w is the distance estimate

	for (int i = 0; i < 8; i++)
	{
		p.xyz = clamp(p.xyz, -1.0, 1.0) * 2.0 - p.xyz;

		// sphere folding: if (r2 < minRad2) p /= minRad2; else if (r2 < 1.0) p /= r2;
		float r2 = dot(p.xyz, p.xyz);
		p *= clamp(max(minRad2/r2, minRad2), 0.0, 1.0);

		// scale, translate
		p = p*scale + p0;
	}
	
	return ((length(p.xyz) - abs(SCALE - 1.0)) / p.w);
}

vec3 hsv(in float h, in float s, in float v) {
	return mix(vec3(1.0), clamp((abs(fract(h + vec3(3, 2, 1) / 3.0) * 6.0 - 3.0) - 1.0), 0.0 , 1.0), s) * v;
}

mat3 RotationMatrix(vec3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat3(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}

vec3 CameraPath( float t )
{
    vec3 p = vec3(-.81 + 3. * sin(2.14*t),.05+2.5 * sin(.942*t+1.3),.05 + 3.5 * cos(3.594*t) );
	return p;
} 

void main() {
	
	vec2 xy = gl_FragCoord.xy / RENDERSIZE - 0.5;
	float aspect = RENDERSIZE.x / RENDERSIZE.y;
	xy = vec2(aspect, 1.0) * xy * 2.; 


	float t = 0.;
	vec3 p;
	vec3 camPos = CameraPath(TIME/speed); 
	vec3 forward = CameraPath(TIME/speed + 0.1) - camPos; 

	vec3 ray = normalize(vec3(xy, 1.0)  );
	
	if(rotation < 4.)
    	ray = RotationMatrix(vec3(0.,0.,1.), rotation * length(xy)) * ray;
	else     	
    	ray = RotationMatrix(vec3(sin(TIME/speed/10. + 3.),sin(TIME/speed/20.),1.), rotation * TIME/speed ) * ray;

	
	//vec3(0, 0, -3.2);
	float iter = 0.;
	bool hit = false;
	
	for (int i = 0; i < MAX_ITER; i++) {
		p = t * ray + camPos;
		float d = Map(p);
		if (d < THRESH){
			break;
			hit = true;
		}			
		t += d;
		iter++;
	}
	
	
	float d = 1.0 - iter / float(MAX_ITER);
	
	vec3 color = hsv(d + dhue,1.,d);
	
	gl_FragColor = vec4(color, 1.0);
}