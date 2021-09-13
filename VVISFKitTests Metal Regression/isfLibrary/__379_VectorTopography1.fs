/*{
  "CREDIT": "by mojovideotech",
  "DESCRIPTION": "",
  "CATEGORIES": [ 
  		"generator"
  	],
  "INPUTS": [
    {
	    "NAME": "offset",
      	"TYPE": "point2D",   
      	"MAX": [ 2, 4 ],
      	"MIN": [ -2, -4 ],
      	"DEFAULT": [ 0.85, -1.05 ]
    },
    {	"NAME": "shift",
      	"TYPE": "point2D",
      	"MAX": [ 3, 2 ],
      	"MIN": [ -0.5, 0 ],
      	"DEFAULT": [ -0.2, 0.2 ]
    },
    {
      	"NAME": "rate",
      	"TYPE": "float",
      	"DEFAULT": 0.78,
      	"MIN": 0.001,
      	"MAX": 0.999
    },
    {
      	"NAME": "depth",
      	"TYPE": "float",
      	"DEFAULT": 2.07,
      	"MIN": 0.001,
      	"MAX": 2.999
    },
    {
      	"NAME": "loops",
      	"TYPE": "float",
      	"DEFAULT": 0.59,
      	"MIN": 0.0005,
      	"MAX": 1.499
    },
    {
      	"NAME": "stepsize",
      	"TYPE": "float",
      	"DEFAULT": 0.31,
      	"MIN": 0.001,
      	"MAX": 1.999
    },
    {
      	"NAME": "scale",
      	"TYPE": "float",
      	"DEFAULT": 0.92,
      	"MIN": 0.5,
      	"MAX": 3.5
    },
    {
      	"NAME": "hue",
      	"TYPE": "float",
      	"DEFAULT": 0.33,
      	"MIN": 0.05,
      	"MAX": 1
    },
    {
      	"NAME": "baseColor",
      	"TYPE": "color",
      	"DEFAULT": [ 0.4, 0.4, 0.4, 1 ]
    }
  ]
}*/

////////////////////////////////////////////////////////////
// VectorTopography1  by mojovideotech
//
// Creative Commons Attribution-NonCommercial-ShareAlike 3.0
////////////////////////////////////////////////////////////

#define 	pi   	3.141592653589793 	// pi
#define 	phi   	1.618033988749895 	// golden ratio

float speed=TIME*rate;
float ground_x=abs(phi+speed);
float ground_y=abs((phi*pi)-speed);
float ground_z=abs(pi+speed);

vec2 rotate(vec2 k,float t) {
	return vec2(cos(t)*k.x-sin(t)*k.y,sin(t)*k.x+cos(t)*k.y);
	}

float scene(vec3 p) {
	float bar_p=scale;
	float bar_w=bar_p/dot(speed,phi);
	float bar_x=length(mod(abs(max(p.yz,bar_p)-bar_p*0.125)-bar_w,0.0));
	float bar_y=length(mod(abs(max(p.xz,bar_p)-bar_p*0.125)-bar_w,0.0));
	float bar_z=length(max(abs(mod(p.xy,bar_p)-bar_p*0.125)-bar_w,0.0));
	float tube_p=inversesqrt(bar_w);
	float tube_w=mod(tube_p,phi);
	float tube_x=length(max(p.xz,tube_p)+tube_p*5.25)-tube_w;
	float tube_y=length(max(p.xy,tube_p)+tube_p*5.25)-tube_w;
	float tube_z=length(max(p.zy,tube_p)+tube_p*5.25)-tube_w;
	return -min(min(max(max(-bar_x,-bar_y),-bar_z),tube_y),tube_z);
	}

void main() {
	vec2 position=(gl_FragCoord.xy/(RENDERSIZE.xy+vec2(RENDERSIZE.x*shift.x,RENDERSIZE.y*shift.y)));
	vec2 p=offset.x-offset.y*position;
	vec3 dir=normalize(vec3(position*vec2((RENDERSIZE.y*RENDERSIZE.x)/position),0.95));	
	dir.xz=rotate(dir.xz+(ground_x,phi),pow(p.x,pi));	
	dir.yx=rotate(dir.yx+(ground_y,pi),pow(p.y,p.x));			
  	dir.zy=rotate(dir.zy+(ground_z,phi),pow(pi,p.y));
	vec3 ray=vec3(ground_x,ground_y,ground_z);
	float t=depth;
	const int ray_n=360;
	for(int i=0;i<ray_n;i++) {
		float k=scene(ray+dir*t);
        if(abs(k)<loops) break;
		t+=k*stepsize;
		}
	vec3 hit=ray+dir*t;
	vec2 h=vec2(0.005,-0.005);
	vec3 n=normalize(vec3(scene(hit+h.xyx),scene(hit+h.yxy),scene(hit+h.yyy)));
	float c=(n.x*2.0+n.y+n.z)*0.667-t*0.5;
	vec3 color=vec3(c*t*0.625-p.x*0.125,c*t*0.25+t*0.03125,c*0.375+t*0.0625+p.y*0.125);
	color=smoothstep(0.5,0.5,c)-color+vec3(p.x,p.y,-c);
	color=vec3(vec3(baseColor+(baseColor/(hue*10.0)))*vec3((color.x*hue)-0.33,color.y*0.25+(hue/-5.0),color.z+0.67));
	gl_FragColor=vec4(color,1.0);
}