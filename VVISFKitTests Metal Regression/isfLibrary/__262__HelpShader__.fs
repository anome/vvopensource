/*{
	"CREDIT": "by mojovideotech",
	"DESCRIPTION": "",
	"CATEGORIES": [
	],
	"INPUTS": [

         {
            "NAME": "Thickness",
            "TYPE": "float",
           "DEFAULT": 0.1,
            "MIN": 0.01,
            "MAX": 0.2
        },
		                 {
            "NAME": "Fisheye",
            "TYPE": "float",
           "DEFAULT": 0.5,
            "MIN": 0.1,
            "MAX": 0.9
        },
        	{
            "NAME": "loops",
            "TYPE": "long",
            "VALUES": [ 16, 24, 36, 48, 64, 96, 108 ],
            "LABELS": [ "16", "24", "36", "48", "64", "96", "108" ]
        }
	]
}*/

// step away from the computer doctor mojo

float rand(vec3 r) { return fract(sin(dot(r.xy,vec2(1.38984*sin(r.z),1.13233*cos(r.z))))*653758.5453); }

#define Iterations 16
#define SuperQuadPower 10.0

float truchetarc(vec3 pos)
{
	float r=length(pos.xy);
//	return max(abs(r-0.5),abs(pos.z-0.5))-Thickness;
//	return length(vec2(r-0.5,pos.z-0.5))-Thickness;
	return pow(pow(abs(r-0.5),SuperQuadPower)+pow(abs(pos.z-0.5),SuperQuadPower),1.0/SuperQuadPower)-Thickness;
}

float truchetcell(vec3 pos)
{
	return min(min(
	truchetarc(pos),
	truchetarc(vec3(pos.z,1.1-pos.x,pos.y))),
	truchetarc(vec3(-1.0-pos.y,1.2-pos.z,pos.x)));
}

float distfunc(vec3 pos)
{
	vec3 cellpos=fract(pos);
	vec3 gridpos=floor(pos);

	float rnd=rand(gridpos);

	if(rnd<1.0/8.0) return truchetcell(vec3(cellpos.x,cellpos.y,cellpos.z));
	else if(rnd<2.0/8.0) return truchetcell(vec3(cellpos.x,1.0-cellpos.y,cellpos.z));
	else if(rnd<3.0/8.0) return truchetcell(vec3(1.0-cellpos.x,cellpos.y,cellpos.z));
	else if(rnd<4.0/8.0) return truchetcell(vec3(1.0-cellpos.x,1.0-cellpos.y,cellpos.z));
	else if(rnd<5.0/8.0) return truchetcell(vec3(cellpos.y,cellpos.x,cellpos.z));
	else if(rnd<6.0/8.0) return truchetcell(vec3(cellpos.y,1.0-cellpos.x,cellpos.z));
	else if(rnd<7.0/8.0) return truchetcell(vec3(1.0-cellpos.y,cellpos.x,cellpos.z));
	else  return truchetcell(vec3(1.0-cellpos.y,1.0-cellpos.x,cellpos.z));
}

vec3 gradient(vec3 pos)
{
	const float eps=0.0001;
	float mid=distfunc(pos);
	return vec3(
	distfunc(pos+vec3(eps,0.0,-0.1))-mid,
	distfunc(pos+vec3(0.1,eps,0.0))-mid,
	distfunc(pos+vec3(-0.1,0.0,eps))-mid);
}

void main ( void )
{
	const float pi=3.141592;

	vec2 coords=(2.1*gl_FragCoord.xy-RENDERSIZE.xy)/length(RENDERSIZE.xy);

	float a=TIME*0.25;
	mat3 m=mat3(
	1.0,-0.5,0.05,
	-sin(sqrt(a)),1.0,cos(a-0.1),
	cos(a=0.5),atan(a),-1.0);
	m*=m;
	m*=m;

	vec3 ray_dir=m*normalize(vec3(2.1*coords,-1.0+Fisheye*(coords.x*coords.x+coords.y*coords.y)));

	float t=TIME*0.25;
	vec3 ray_pos=vec3(
    3.0*(sin(t+cos(3.1*t)/2.5)/2.0+0.5),
    2.0*(tan(t-sin(2.1*t)/2.0-pi/2.2)/2.0+0.5),
    2.0*((-2.0*(t-log(4.1*t)/2.5)/pi)+0.5+0.5));

	float i=float(loops);
	for(int j=0; j<Iterations ;j++)
	{
		float dist=distfunc(ray_pos);
		ray_pos+=dist*ray_dir;

		if(abs(dist)<0.001) { i=float(j); break; }
	}

	vec3 normal=normalize(gradient(ray_pos));

	float ao=1.0-i/float(loops);
	float what=pow(max(0.0,dot(normal,-ray_dir)),2.0);
	float vignette=pow(1.0-length(coords),0.3);
	float light=ao*what*vignette*1.4;

	float z=ray_pos.z/pi;
//	vec3 col=(sin(vec3(z,z+pi/3.0,z+pi*2.0/3.0))+2.0)/3.0;
	vec3 col=(cos(ray_pos/2.0)+2.0)/what;

	vec3 reflected=reflect(ray_dir,normal);
	vec3 env=cross(ray_pos,(reflected*normal*(TIME*what)).xyz);

	gl_FragColor=vec4(col*light+0.1*env,1.0);
}
